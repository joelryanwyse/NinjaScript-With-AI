//
// Copyright (C) 2026, Affordable Indicators, Inc.
// aiSignalStudio - Phase 2: WPF node canvas, toolbox, config panel, template management.
//
// This file contains everything WPF-related for Signal Studio:
//   - SignalStudioWindow      (top-level Window)
//   - NodeCanvas              (zoom/pan, wire creation, drag-to-move, grid snap)
//   - NodeVisual              (single node box with ports, labels, value preview)
//   - WireVisual              (Bezier-curve connection)
//   - NodeToolbox             (categorized sidebar of node factories)
//   - NodeConfigPanel         (property editor for selected node)
//   - TemplateBrowserDialog   (save/load UI)
//
// The indicator's launch hook lives at the bottom as a `partial aiSignalStudio`.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using NinjaTrader.NinjaScript;
using WpfPath = System.Windows.Shapes.Path;
using IOPath  = System.IO.Path;

namespace NinjaTrader.NinjaScript.Indicators
{
	// ============================================================================================
	// Window
	// ============================================================================================

	public class SignalStudioWindow : Window
	{
		private readonly aiSignalStudio     _host;
		private          SignalStudioEngine _engine;

		private NodeCanvas       _canvas;
		private NodeToolbox      _toolbox;
		private NodeConfigPanel  _configPanel;
		private ComboBox         _templateCombo;
		private TextBlock        _statusLeft;
		private TextBlock        _statusRight;
		private TextBox          _nameBox;

		private string _currentTemplatePath;

		private static readonly string TemplatesRoot = IOPath.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
			"NinjaTrader 8", "templates", "SignalStudio");

		public SignalStudioWindow(aiSignalStudio host, SignalStudioEngine engine)
		{
			_host   = host;
			_engine = engine;

			Title                 = "Signal Studio";
			Width                 = 1280;
			Height                = 780;
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
			Background            = new SolidColorBrush(Color.FromRgb(30, 30, 34));
			Foreground            = Brushes.White;

			BuildLayout();
			EnsureTemplatesDirExists();
			RepopulateTemplateCombo();
			RebuildCanvasFromEngine();
			UpdateStatus();

			Closing += OnClosing;
			KeyDown += OnKeyDown;
		}

		// ------------------------------------------------------------
		// Layout
		// ------------------------------------------------------------

		private void BuildLayout()
		{
			var root = new DockPanel { LastChildFill = true };

			// Toolbar - wrap in a horizontal scroller so no buttons get clipped on narrow windows.
			var tb = BuildToolbar();
			var tbScroll = new ScrollViewer
			{
				Content                       = tb,
				HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
				VerticalScrollBarVisibility   = ScrollBarVisibility.Disabled,
				Background                    = new SolidColorBrush(Color.FromRgb(22, 22, 26))
			};
			DockPanel.SetDock(tbScroll, Dock.Top);
			root.Children.Add(tbScroll);

			// Status bar
			var sb = BuildStatusBar();
			DockPanel.SetDock(sb, Dock.Bottom);
			root.Children.Add(sb);

			// Main grid: 3 columns (toolbox | canvas | config panel)
			var grid = new Grid();
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(220), MinWidth = 180 });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(6)   });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star), MinWidth = 300 });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(6)   });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(300), MinWidth = 240 });

			_toolbox = new NodeToolbox();
			_toolbox.NodeFactoryActivated += (typeKey) => OnToolboxAdd(typeKey);
			Grid.SetColumn(_toolbox, 0);
			grid.Children.Add(_toolbox);

			grid.Children.Add(BuildSplitter(1));

			_canvas = new NodeCanvas();
			_canvas.GraphChanged    += OnGraphChanged;
			_canvas.SelectionChanged += OnSelectionChanged;
			Grid.SetColumn(_canvas, 2);
			grid.Children.Add(_canvas);

			grid.Children.Add(BuildSplitter(3));

			_configPanel = new NodeConfigPanel();
			_configPanel.NodeChanged += () => OnGraphChanged();
			Grid.SetColumn(_configPanel, 4);
			grid.Children.Add(_configPanel);

			root.Children.Add(grid);
			Content = root;
		}

		private static GridSplitter BuildSplitter(int col)
		{
			var s = new GridSplitter
			{
				Width                         = 6,
				HorizontalAlignment           = HorizontalAlignment.Stretch,
				VerticalAlignment             = VerticalAlignment.Stretch,
				Background                    = new SolidColorBrush(Color.FromRgb(50, 50, 55)),
				ShowsPreview                  = true,
				ResizeBehavior                = GridResizeBehavior.PreviousAndNext
			};
			Grid.SetColumn(s, col);
			return s;
		}

		private UIElement BuildToolbar()
		{
			var panel = new DockPanel { LastChildFill = false, Background = new SolidColorBrush(Color.FromRgb(22, 22, 26)) };
			panel.Margin = new Thickness(0);

			var lbl = MakeLabel("Template:", 64);
			DockPanel.SetDock(lbl, Dock.Left);
			panel.Children.Add(lbl);

			_templateCombo = new ComboBox { Width = 220, Margin = new Thickness(4) };
			_templateCombo.SelectionChanged += OnTemplateComboChanged;
			panel.Children.Add(_templateCombo);

			_nameBox = new TextBox { Width = 240, Margin = new Thickness(4), ToolTip = "Current template name (edit, then Save)" };
			panel.Children.Add(_nameBox);

			panel.Children.Add(MakeButton("Starters",   90, (s, e) => ShowStartersPopup(s as Button)));
			panel.Children.Add(MakeButton("New",        60, (s, e) => DoNewEmpty()));
			panel.Children.Add(MakeButton("Save",       60, (s, e) => DoSave(false)));
			panel.Children.Add(MakeButton("Save as",    70, (s, e) => DoSave(true)));
			panel.Children.Add(MakeButton("Reload",     60, (s, e) => DoReloadFromDisk()));
			panel.Children.Add(MakeButton("Re-eval",    70, (s, e) => RequestReevaluate()));
			panel.Children.Add(MakeButton("Validate",   70, (s, e) => DoValidate()));

			return panel;
		}

		private UIElement BuildStatusBar()
		{
			var sbar = new DockPanel
			{
				Background    = new SolidColorBrush(Color.FromRgb(22, 22, 26)),
				LastChildFill = false
			};
			_statusLeft  = new TextBlock { Foreground = Brushes.LightGray, Margin = new Thickness(8, 3, 8, 3) };
			_statusRight = new TextBlock { Foreground = Brushes.LightGray, Margin = new Thickness(8, 3, 8, 3) };

			DockPanel.SetDock(_statusLeft,  Dock.Left);
			DockPanel.SetDock(_statusRight, Dock.Right);

			sbar.Children.Add(_statusLeft);
			sbar.Children.Add(_statusRight);
			return sbar;
		}

		private static TextBlock MakeLabel(string text, double width)
		{
			return new TextBlock
			{
				Text                = text,
				Foreground          = Brushes.LightGray,
				VerticalAlignment   = VerticalAlignment.Center,
				Margin              = new Thickness(8, 0, 4, 0),
				Width               = width
			};
		}

		private static Button MakeButton(string text, double width, RoutedEventHandler handler)
		{
			var b = new Button
			{
				Content    = text,
				Width      = width,
				Margin     = new Thickness(4),
				Height     = 24,
				Background = new SolidColorBrush(Color.FromRgb(55, 55, 60)),
				Foreground = Brushes.White,
				BorderBrush = new SolidColorBrush(Color.FromRgb(85, 85, 90))
			};
			b.Click += handler;
			return b;
		}

		// ------------------------------------------------------------
		// Toolbar actions
		// ------------------------------------------------------------

		private void ShowStartersPopup(Button anchor)
		{
			var ctx = new ContextMenu();
			foreach (var e in SignalStudioStarterTemplates.All)
			{
				var entry = e;
				var mi = new MenuItem { Header = entry.DisplayName, ToolTip = entry.Description };
				mi.Click += (s, ev) =>
				{
					try
					{
						var t = entry.Factory();
						if (!string.IsNullOrEmpty(_nameBox.Text) && _nameBox.Text != t.Name) t.Name = _nameBox.Text;
						SignalStudioTemplateBinder.Apply(t, _engine, _host);
						_currentTemplatePath = null;
						_nameBox.Text        = t.Name ?? "";
						RebuildCanvasFromEngine();
						RequestReevaluate();
					}
					catch (Exception ex) { MessageBox.Show(this, "Template load failed: " + ex.Message, "Signal Studio"); }
				};
				ctx.Items.Add(mi);
			}
			if (anchor != null) { ctx.PlacementTarget = anchor; ctx.Placement = PlacementMode.Bottom; ctx.IsOpen = true; }
		}

		private void DoNewEmpty()
		{
			var t = new SignalStudioTemplate { Name = "Untitled", Created = DateTime.Now };
			SignalStudioTemplateBinder.Apply(t, _engine, _host);
			_currentTemplatePath = null;
			_nameBox.Text        = t.Name;
			RebuildCanvasFromEngine();
			RequestReevaluate();
		}

		private void DoSave(bool saveAs)
		{
			try
			{
				EnsureTemplatesDirExists();
				string name = string.IsNullOrWhiteSpace(_nameBox.Text) ? (_engine.TemplateName ?? "Untitled") : _nameBox.Text.Trim();
				_engine.TemplateName = name;

				string path;
				if (saveAs || string.IsNullOrEmpty(_currentTemplatePath))
				{
					string safe = SanitizeFileName(name);
					path = IOPath.Combine(TemplatesRoot, safe + ".json");
					int n = 2;
					while (File.Exists(path) && !string.Equals(path, _currentTemplatePath, StringComparison.OrdinalIgnoreCase))
						path = IOPath.Combine(TemplatesRoot, safe + "_" + (n++) + ".json");
				}
				else path = _currentTemplatePath;

				var tmpl = _canvas.ExportTemplate(name);
				File.WriteAllText(path, SignalStudioTemplateIO.Serialize(tmpl));
				_currentTemplatePath = path;
				SetStatusLeft("Saved: " + IOPath.GetFileName(path));
				RepopulateTemplateCombo();
			}
			catch (Exception ex) { MessageBox.Show(this, "Save failed: " + ex.Message, "Signal Studio"); }
		}

		private void DoReloadFromDisk()
		{
			if (string.IsNullOrEmpty(_currentTemplatePath) || !File.Exists(_currentTemplatePath))
			{
				SetStatusLeft("Nothing to reload");
				return;
			}
			try
			{
				var json = File.ReadAllText(_currentTemplatePath);
				var t = SignalStudioTemplateIO.Deserialize(json);
				SignalStudioTemplateBinder.Apply(t, _engine, _host);
				_nameBox.Text = t != null ? (t.Name ?? "") : "";
				RebuildCanvasFromEngine();
				RequestReevaluate();
			}
			catch (Exception ex) { MessageBox.Show(this, "Reload failed: " + ex.Message, "Signal Studio"); }
		}

		private void OnTemplateComboChanged(object s, SelectionChangedEventArgs e)
		{
			if (_templateCombo.SelectedItem == null) return;
			var path = _templateCombo.SelectedItem as string;
			if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;
			try
			{
				var json = File.ReadAllText(path);
				var t = SignalStudioTemplateIO.Deserialize(json);
				SignalStudioTemplateBinder.Apply(t, _engine, _host);
				_currentTemplatePath = path;
				_nameBox.Text = t != null ? (t.Name ?? IOPath.GetFileNameWithoutExtension(path)) : "";
				RebuildCanvasFromEngine();
				RequestReevaluate();
			}
			catch (Exception ex) { MessageBox.Show(this, "Load failed: " + ex.Message, "Signal Studio"); }
		}

		private void DoValidate()
		{
			var issues = new List<string>();
			int longOuts = 0, shortOuts = 0;
			foreach (var n in _engine.Nodes)
			{
				if (n is LongOutputNode)  longOuts++;
				if (n is ShortOutputNode) shortOuts++;
				foreach (var c in n.Inputs) if (c != null && c.FromNode == null) issues.Add("Dangling input on " + (n.DisplayName ?? n.TypeKey));
			}
			if (longOuts == 0)  issues.Add("No LongOutput  node.");
			if (shortOuts == 0) issues.Add("No ShortOutput node.");
			if (longOuts > 1)   issues.Add("Multiple LongOutput nodes - only the most recent will be used.");
			if (shortOuts > 1)  issues.Add("Multiple ShortOutput nodes - only the most recent will be used.");

			string msg = issues.Count == 0 ? "No issues found." : string.Join(Environment.NewLine, issues);
			MessageBox.Show(this, msg, "Signal Studio - Validation");
		}

		private void RepopulateTemplateCombo()
		{
			try
			{
				_templateCombo.Items.Clear();
				if (!Directory.Exists(TemplatesRoot)) return;
				var files = Directory.GetFiles(TemplatesRoot, "*.json");
				Array.Sort(files);
				foreach (var f in files) _templateCombo.Items.Add(f);
				if (!string.IsNullOrEmpty(_currentTemplatePath))
				{
					foreach (var item in _templateCombo.Items)
					{
						if (string.Equals((string)item, _currentTemplatePath, StringComparison.OrdinalIgnoreCase))
						{
							_templateCombo.SelectedItem = item;
							break;
						}
					}
				}
			}
			catch { }
		}

		private static void EnsureTemplatesDirExists()
		{
			try { if (!Directory.Exists(TemplatesRoot)) Directory.CreateDirectory(TemplatesRoot); } catch { }
		}

		private static string SanitizeFileName(string s)
		{
			if (string.IsNullOrEmpty(s)) return "Untitled";
			var invalid = IOPath.GetInvalidFileNameChars();
			var sb = new System.Text.StringBuilder();
			foreach (var ch in s) sb.Append(Array.IndexOf(invalid, ch) >= 0 ? '_' : ch);
			return sb.ToString();
		}

		// ------------------------------------------------------------
		// Canvas <-> Engine sync
		// ------------------------------------------------------------

		private void RebuildCanvasFromEngine()
		{
			if (_canvas == null || _engine == null) return;
			_canvas.LoadFromEngine(_engine, _host);
			if (!string.IsNullOrEmpty(_engine.TemplateName)) _nameBox.Text = _engine.TemplateName;
			UpdateStatus();
		}

		private void OnToolboxAdd(string typeKey)
		{
			var node = SignalStudioNodeFactory.Create(typeKey);
			if (node == null) return;
			node.DisplayName = DefaultDisplayName(typeKey);
			// Drop roughly in the center of the visible canvas
			var center = _canvas.ViewportCenterInContent();
			node.X = center.X;
			node.Y = center.Y;
			_canvas.AddNode(node);
			OnGraphChanged();
		}

		private static string DefaultDisplayName(string typeKey)
		{
			switch (typeKey)
			{
				case "PriceSource":     return "Price";
				case "Constant":        return "Const";
				case "IndicatorPlot":   return "IndicatorPlot";
				case "Builtin":         return "Builtin";
				case "Crossover":       return "Crossover";
				case "Comparison":      return "Compare";
				case "Slope":           return "Slope";
				case "Range":           return "Range";
				case "Pattern":         return "Pattern";
				case "Swing":           return "Swing";
				case "Expression":      return "Expr";
				case "ChartObject":     return "ChartObj";
				case "And":             return "AND";
				case "Or":              return "OR";
				case "Not":             return "NOT";
				case "AtLeastNOfM":     return "At least N";
				case "WeightedAverage": return "Weighted avg";
				case "TimeFilter":      return "Time";
				case "DayFilter":       return "Day";
				case "CooldownFilter":  return "Cooldown";
				case "BarsSinceFilter": return "Bars since";
				case "LongOutput":      return "Long";
				case "ShortOutput":     return "Short";
				default:                 return typeKey;
			}
		}

		private void OnSelectionChanged(SignalStudioNodeBase node)
		{
			_configPanel.Bind(node);
			UpdateStatus();
		}

		private void OnGraphChanged()
		{
			// Canvas and engine share node references; just re-topo and re-evaluate.
			try { _engine.BuildGraph(_host); } catch { }
			_engine.TemplateName = string.IsNullOrWhiteSpace(_nameBox.Text) ? (_engine.TemplateName ?? "Untitled") : _nameBox.Text;
			_canvas.RefreshSummaries();
			RequestReevaluate();
			UpdateStatus();
		}

		private void RequestReevaluate()
		{
			try { _host.RequestHistoryReevaluation(); } catch { }
			SetStatusRight("Eval queued: " + DateTime.Now.ToString("HH:mm:ss"));
		}

		private void UpdateStatus()
		{
			int nodes = _engine != null ? _engine.Nodes.Count : 0;
			int wires = _canvas != null ? _canvas.WireCount   : 0;
			string n = nodes == 1 ? "1 node"  : nodes + " nodes";
			string w = wires == 1 ? "1 wire"  : wires + " wires";
			SetStatusLeft(string.Format("{0}  |  {1}  |  {2}", n, w, _currentTemplatePath != null ? IOPath.GetFileName(_currentTemplatePath) : "(unsaved)"));
		}

		private void SetStatusLeft (string s) { if (_statusLeft  != null) _statusLeft .Text = s ?? ""; }
		private void SetStatusRight(string s) { if (_statusRight != null) _statusRight.Text = s ?? ""; }

		private void OnClosing(object s, CancelEventArgs e)
		{
			try { _host.OnStudioWindowClosed(); } catch { }
		}

		private void OnKeyDown(object s, KeyEventArgs e)
		{
			if (e.Key == Key.Delete) _canvas.DeleteSelected();
			else if (e.Key == Key.F5) RequestReevaluate();
			else if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control) DoSave(false);
		}
	}

	// ============================================================================================
	// NodeCanvas
	// ============================================================================================

	public class NodeCanvas : Grid
	{
		// Transforms applied to the content layer
		private readonly ScaleTransform     _scale = new ScaleTransform(1, 1);
		private readonly TranslateTransform _pan   = new TranslateTransform(0, 0);

		private readonly Canvas     _contentLayer;
		private readonly Canvas     _wireLayer;
		private readonly Canvas     _nodeLayer;
		private readonly Rectangle  _background;

		private SignalStudioEngine _engine;
		private Indicator _host;

		private readonly Dictionary<string, NodeVisual> _nodes = new Dictionary<string, NodeVisual>(StringComparer.OrdinalIgnoreCase);
		private readonly List<WireVisual>               _wires = new List<WireVisual>();

		private NodeVisual  _selected;
		private Point       _lastMousePos;
		private bool        _panning;
		private bool        _draggingNode;
		private bool        _creatingWire;
		private NodeVisual  _wireStartNode;
		private int         _wireStartPort;
		private WpfPath     _wireGhost;
		private const double GridStep = 10.0;
		private const double MinScale = 0.2;
		private const double MaxScale = 4.0;

		public event Action                                 GraphChanged;
		public event Action<SignalStudioNodeBase>           SelectionChanged;

		public int WireCount { get { return _wires.Count; } }

		public NodeCanvas()
		{
			Background          = new SolidColorBrush(Color.FromRgb(18, 18, 22));
			ClipToBounds        = true;
			Focusable           = true;
			SnapsToDevicePixels = true;

			_contentLayer = new Canvas { IsHitTestVisible = true };
			var tg = new TransformGroup();
			tg.Children.Add(_scale);
			tg.Children.Add(_pan);
			_contentLayer.RenderTransform = tg;

			_background = new Rectangle
			{
				Width  = 20000, Height = 20000,
				Fill   = BuildGridBrush(),
				IsHitTestVisible = false
			};
			Canvas.SetLeft(_background, -10000);
			Canvas.SetTop (_background, -10000);
			_contentLayer.Children.Add(_background);

			_wireLayer = new Canvas { IsHitTestVisible = false };
			_nodeLayer = new Canvas { IsHitTestVisible = true };
			_contentLayer.Children.Add(_wireLayer);
			_contentLayer.Children.Add(_nodeLayer);

			Children.Add(_contentLayer);

			MouseWheel       += OnMouseWheel;
			MouseDown        += OnMouseDown;
			MouseMove        += OnMouseMove;
			MouseUp          += OnMouseUp;
			MouseLeave       += (s, e) => { _panning = false; _draggingNode = false; CancelWireDrag(); };
		}

		private Brush BuildGridBrush()
		{
			var vb = new VisualBrush
			{
				TileMode  = TileMode.Tile,
				Viewport  = new Rect(0, 0, GridStep, GridStep),
				ViewportUnits = BrushMappingMode.Absolute,
				Viewbox       = new Rect(0, 0, GridStep, GridStep),
				ViewboxUnits  = BrushMappingMode.Absolute
			};
			var dot = new Ellipse
			{
				Width  = 1.0, Height = 1.0,
				Fill   = new SolidColorBrush(Color.FromRgb(45, 45, 50))
			};
			var c = new Canvas { Width = GridStep, Height = GridStep };
			Canvas.SetLeft(dot, 0); Canvas.SetTop(dot, 0);
			c.Children.Add(dot);
			vb.Visual = c;
			return vb;
		}

		// ------------------------------------------------------------
		// Template <-> Canvas
		// ------------------------------------------------------------

		public void LoadFromEngine(SignalStudioEngine engine, Indicator host)
		{
			_engine = engine;
			_host   = host;
			_nodeLayer.Children.Clear();
			_wireLayer.Children.Clear();
			_nodes.Clear();
			_wires.Clear();
			_selected = null;
			SelectionChanged?.Invoke(null);

			if (engine == null) return;

			// Create visuals for every node
			foreach (var node in engine.Nodes)
			{
				var nv = new NodeVisual(node);
				HookNode(nv);
				_nodes[node.Id] = nv;
				_nodeLayer.Children.Add(nv);
				Canvas.SetLeft(nv, node.X);
				Canvas.SetTop (nv, node.Y);
			}

			// Create wire visuals
			foreach (var node in engine.Nodes)
			{
				var target = _nodes.ContainsKey(node.Id) ? _nodes[node.Id] : null;
				if (target == null) continue;
				for (int i = 0; i < node.Inputs.Count; i++)
				{
					var conn = node.Inputs[i];
					if (conn == null || conn.FromNode == null) continue;
					NodeVisual srcVis;
					if (!_nodes.TryGetValue(conn.FromNode.Id, out srcVis)) continue;
					AddWireVisual(srcVis, conn.FromPort, target, conn.ToPort);
				}
			}

			UpdateAllWires();
		}

		// After engine rebuild (e.g., from external template load), reconcile visuals with model IDs.
		public void ReconcileWithEngine(SignalStudioEngine engine, Indicator host) { LoadFromEngine(engine, host); }

		public SignalStudioTemplate ExportTemplate(string name)
		{
			var t = new SignalStudioTemplate { Name = name, Created = DateTime.Now, Version = 1 };
			foreach (var nv in _nodes.Values)
			{
				var n = nv.Model;
				t.Nodes.Add(new SSNodeData
				{
					Id          = n.Id,
					Type        = n.TypeKey,
					DisplayName = n.DisplayName,
					X           = Canvas.GetLeft(nv),
					Y           = Canvas.GetTop (nv),
					Config      = n.GetConfig() ?? new Dictionary<string, object>()
				});
			}
			foreach (var w in _wires)
			{
				t.Connections.Add(new SSConnectionData
				{
					From     = w.From.Model.Id,
					FromPort = w.FromPort,
					To       = w.To.Model.Id,
					ToPort   = w.ToPort
				});
			}
			return t;
		}

		public void AddNode(SignalStudioNodeBase node)
		{
			if (node == null) return;
			var nv = new NodeVisual(node);
			HookNode(nv);
			_nodes[node.Id] = nv;
			_nodeLayer.Children.Add(nv);
			Canvas.SetLeft(nv, node.X);
			Canvas.SetTop (nv, node.Y);
			if (_engine != null) _engine.AddNode(node);
			SetSelection(nv);
			RaiseChanged();
		}

		public void DeleteSelected()
		{
			if (_selected == null) return;
			var nv = _selected;
			// Remove incident wires
			for (int i = _wires.Count - 1; i >= 0; i--)
			{
				var w = _wires[i];
				if (w.From == nv || w.To == nv)
				{
					_wireLayer.Children.Remove(w.Visual);
					_wires.RemoveAt(i);
					if (w.To != null) w.To.Model.Inputs.RemoveAll(c => c.ToPort == w.ToPort && c.FromNodeId == w.From.Model.Id);
				}
			}
			_nodeLayer.Children.Remove(nv);
			_nodes.Remove(nv.Model.Id);
			if (_engine != null) _engine.RemoveNode(nv.Model);
			_selected = null;
			SelectionChanged?.Invoke(null);
			RaiseChanged();
		}

		private void HookNode(NodeVisual nv)
		{
			nv.MouseDown     += (s, e) => OnNodeMouseDown(nv, e);
			nv.PortMouseDown += (port, isInput, e) => OnPortMouseDown(nv, port, isInput, e);
		}

		private void AddWireVisual(NodeVisual src, int srcPort, NodeVisual dst, int dstPort)
		{
			var w = new WireVisual(src, srcPort, dst, dstPort);
			_wires.Add(w);
			_wireLayer.Children.Add(w.Visual);
		}

		public void UpdateAllWires()
		{
			foreach (var w in _wires) w.UpdateGeometry();
		}

		public void RefreshSummaries()
		{
			foreach (var nv in _nodes.Values) nv.RefreshSummary();
		}

		public void RefreshValues()
		{
			foreach (var nv in _nodes.Values) nv.RefreshValue();
		}

		public Point ViewportCenterInContent()
		{
			// Guard against the canvas not yet being measured. Without this the node would
			// drop at (0,0) screen coords which may fall outside the viewport after panning.
			double vw = ActualWidth  > 10 ? ActualWidth  : 800;
			double vh = ActualHeight > 10 ? ActualHeight : 500;
			double cx = vw / 2.0;
			double cy = vh / 2.0;
			var content = ScreenToContent(new Point(cx, cy));
			// Offset by roughly half a node so the new node is visually centered, not anchored
			return new Point(Math.Round(content.X - 85), Math.Round(content.Y - 25));
		}

		private Point ScreenToContent(Point p)
		{
			double x = (p.X - _pan.X) / _scale.ScaleX;
			double y = (p.Y - _pan.Y) / _scale.ScaleY;
			return new Point(x, y);
		}

		// ------------------------------------------------------------
		// Zoom / Pan
		// ------------------------------------------------------------

		private void OnMouseWheel(object sender, MouseWheelEventArgs e)
		{
			double oldScale = _scale.ScaleX;
			double factor   = e.Delta > 0 ? 1.1 : 1.0 / 1.1;
			double newScale = Math.Max(MinScale, Math.Min(MaxScale, oldScale * factor));
			if (Math.Abs(newScale - oldScale) < 1e-6) return;

			// Zoom to cursor: keep the content point under the cursor stationary
			var mouse = e.GetPosition(this);
			double dx = (mouse.X - _pan.X) * (1 - newScale / oldScale);
			double dy = (mouse.Y - _pan.Y) * (1 - newScale / oldScale);
			_pan.X += dx;
			_pan.Y += dy;
			_scale.ScaleX = newScale;
			_scale.ScaleY = newScale;

			UpdateAllWires();
			e.Handled = true;
		}

		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			Focus();
			_lastMousePos = e.GetPosition(this);

			if (e.ChangedButton == MouseButton.Middle || (e.ChangedButton == MouseButton.Left && Keyboard.Modifiers == ModifierKeys.Alt))
			{
				_panning = true;
				CaptureMouse();
				Cursor = Cursors.Hand;
				e.Handled = true;
				return;
			}

			// Click on canvas background = clear selection
			if (e.ChangedButton == MouseButton.Left)
			{
				SetSelection(null);
			}
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			var pos = e.GetPosition(this);
			var delta = new Vector(pos.X - _lastMousePos.X, pos.Y - _lastMousePos.Y);
			_lastMousePos = pos;

			if (_panning)
			{
				_pan.X += delta.X;
				_pan.Y += delta.Y;
				UpdateAllWires();
			}
			else if (_draggingNode && _selected != null)
			{
				double nx = Canvas.GetLeft(_selected) + delta.X / _scale.ScaleX;
				double ny = Canvas.GetTop (_selected) + delta.Y / _scale.ScaleY;
				// Snap to grid
				nx = Math.Round(nx / GridStep) * GridStep;
				ny = Math.Round(ny / GridStep) * GridStep;
				Canvas.SetLeft(_selected, nx);
				Canvas.SetTop (_selected, ny);
				_selected.Model.X = nx;
				_selected.Model.Y = ny;
				UpdateWiresFor(_selected);
			}
			else if (_creatingWire && _wireStartNode != null)
			{
				UpdateWireGhost(pos);
			}
		}

		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (_panning)
			{
				_panning = false;
				Cursor   = Cursors.Arrow;
				ReleaseMouseCapture();
				e.Handled = true;
				return;
			}
			if (_draggingNode)
			{
				_draggingNode = false;
				ReleaseMouseCapture();
				RaiseChanged();   // node moved - record in model only (no topology change)
				return;
			}
			if (_creatingWire)
			{
				TryFinishWire(e.GetPosition(this));
			}
		}

		private void OnNodeMouseDown(NodeVisual nv, MouseButtonEventArgs e)
		{
			if (e.ChangedButton != MouseButton.Left) return;
			SetSelection(nv);
			_draggingNode = true;
			CaptureMouse();
			e.Handled = true;
		}

		private void OnPortMouseDown(NodeVisual nv, int port, bool isInput, MouseButtonEventArgs e)
		{
			if (e.ChangedButton != MouseButton.Left) return;

			if (isInput)
			{
				// Clicking an input port disconnects any wire ending there
				DisconnectInput(nv, port);
				RaiseChanged();
				e.Handled = true;
				return;
			}

			// Start wire drag from an output port
			_creatingWire   = true;
			_wireStartNode  = nv;
			_wireStartPort  = port;
			_draggingNode   = false;

			_wireGhost = new WpfPath
			{
				Stroke            = new SolidColorBrush(Color.FromRgb(160, 200, 255)),
				StrokeThickness   = 2,
				IsHitTestVisible  = false,
				Opacity           = 0.9
			};
			_wireLayer.Children.Add(_wireGhost);
			UpdateWireGhost(e.GetPosition(this));
			CaptureMouse();
			e.Handled = true;
		}

		private void UpdateWireGhost(Point screenPos)
		{
			if (_wireGhost == null || _wireStartNode == null) return;
			var start = _wireStartNode.GetOutputPortContentPoint(_wireStartPort);
			var end   = ScreenToContent(screenPos);
			_wireGhost.Data = WireVisual.BuildBezier(start, end);
		}

		private void TryFinishWire(Point screenPos)
		{
			if (_wireGhost != null) { _wireLayer.Children.Remove(_wireGhost); _wireGhost = null; }
			_creatingWire = false;
			ReleaseMouseCapture();

			// Hit test to find an input port under the cursor
			NodeVisual target = null;
			int targetPort = -1;
			foreach (var nv in _nodes.Values)
			{
				int p;
				if (nv.HitTestInputPort(ScreenToContent(screenPos), out p))
				{
					target = nv;
					targetPort = p;
					break;
				}
			}

			if (target == null || target == _wireStartNode) { _wireStartNode = null; return; }

			// Remove any existing wire at that target port, then add the new wire
			DisconnectInput(target, targetPort);
			AddWireVisual(_wireStartNode, _wireStartPort, target, targetPort);
			target.Model.Inputs.Add(new NodeConnection
			{
				FromNodeId = _wireStartNode.Model.Id,
				FromNode   = _wireStartNode.Model,
				FromPort   = _wireStartPort,
				ToPort     = targetPort
			});

			_wireStartNode = null;
			UpdateAllWires();
			RaiseChanged();
		}

		private void DisconnectInput(NodeVisual target, int port)
		{
			for (int i = _wires.Count - 1; i >= 0; i--)
			{
				var w = _wires[i];
				if (w.To == target && w.ToPort == port)
				{
					_wireLayer.Children.Remove(w.Visual);
					_wires.RemoveAt(i);
					target.Model.Inputs.RemoveAll(c => c.ToPort == port);
				}
			}
		}

		private void CancelWireDrag()
		{
			if (_wireGhost != null) { _wireLayer.Children.Remove(_wireGhost); _wireGhost = null; }
			_creatingWire = false;
			_wireStartNode = null;
		}

		private void UpdateWiresFor(NodeVisual nv)
		{
			foreach (var w in _wires)
			{
				if (w.From == nv || w.To == nv) w.UpdateGeometry();
			}
		}

		private void SetSelection(NodeVisual nv)
		{
			if (_selected != null && _selected != nv) _selected.IsSelected = false;
			_selected = nv;
			if (_selected != null) _selected.IsSelected = true;
			SelectionChanged?.Invoke(nv != null ? nv.Model : null);
		}

		private void RaiseChanged() { var h = GraphChanged; if (h != null) h(); }
	}

	// ============================================================================================
	// NodeVisual
	// ============================================================================================

	public class NodeVisual : Border
	{
		public SignalStudioNodeBase Model { get; private set; }
		public bool IsSelected
		{
			get { return _selected; }
			set { _selected = value; BorderBrush = value ? Brushes.Yellow : _normalBorder; }
		}
		public event Action<int, bool, MouseButtonEventArgs> PortMouseDown;

		private bool _selected;
		private readonly Brush _normalBorder = new SolidColorBrush(Color.FromRgb(90, 90, 100));

		private readonly TextBlock  _title;
		private readonly TextBlock  _summary;
		private readonly TextBlock  _valueBlock;
		private readonly Canvas     _portsLeft;
		private readonly Ellipse    _portRight;

		private readonly int _inputCount;

		private const double NodeWidth   = 170;
		private const double PortRadius  = 5;

		public NodeVisual(SignalStudioNodeBase model)
		{
			Model = model;

			_normalBorder = new SolidColorBrush(Color.FromRgb(90, 90, 100));
			BorderBrush   = _normalBorder;
			BorderThickness = new Thickness(1);
			CornerRadius    = new CornerRadius(4);
			Background      = new SolidColorBrush(CategoryColor(model.Category));
			Width           = NodeWidth;
			Padding         = new Thickness(6, 4, 6, 4);
			SnapsToDevicePixels = true;

			var stack = new StackPanel { Orientation = Orientation.Vertical };

			_title = new TextBlock
			{
				Foreground          = Brushes.White,
				FontWeight          = FontWeights.SemiBold,
				TextTrimming        = TextTrimming.CharacterEllipsis,
				Text                = string.IsNullOrEmpty(model.DisplayName) ? model.TypeKey : model.DisplayName
			};
			stack.Children.Add(_title);

			_summary = new TextBlock
			{
				Foreground          = new SolidColorBrush(Color.FromRgb(210, 210, 220)),
				FontSize            = 10,
				TextWrapping        = TextWrapping.Wrap,
				Margin              = new Thickness(0, 2, 0, 0),
				Text                = BuildSummary(model)
			};
			stack.Children.Add(_summary);

			_valueBlock = new TextBlock
			{
				Foreground          = Brushes.LightGreen,
				FontSize            = 10,
				Margin              = new Thickness(0, 2, 0, 0),
				Text                = "— "
			};
			stack.Children.Add(_valueBlock);

			// Ports. Inputs on the left, output on the right.
			_inputCount = DetermineInputCount(model);

			_portsLeft = new Canvas();
			_portRight = new Ellipse { Width = 2 * PortRadius, Height = 2 * PortRadius, Fill = Brushes.Gold, Stroke = Brushes.Black, StrokeThickness = 1 };

			// Overlay grid so the ports render outside the Border's content area.
			var grid = new Grid();
			grid.Children.Add(stack);
			grid.Children.Add(_portsLeft);
			grid.Children.Add(_portRight);

			Child = grid;

			_portRight.HorizontalAlignment = HorizontalAlignment.Right;
			_portRight.VerticalAlignment   = VerticalAlignment.Center;
			_portRight.Margin              = new Thickness(0, 0, -PortRadius, 0);
			_portRight.Cursor              = Cursors.Cross;
			_portRight.MouseDown          += (s, e) => { var h = PortMouseDown; if (h != null) h(0, false, e); };
			_portRight.Tag                  = "out";

			BuildInputPorts();

			Loaded += (s, e) => LayoutPorts();
			SizeChanged += (s, e) => LayoutPorts();
		}

		private static int DetermineInputCount(SignalStudioNodeBase model)
		{
			// Most nodes use 0/1/2 inputs. Logic/weighted nodes use up to 6 variable inputs.
			switch (model.TypeKey)
			{
				case "PriceSource":
				case "Constant":
				case "IndicatorPlot":
				case "Builtin":
					return 0;
				case "Slope":
				case "Not":
				case "TimeFilter":
				case "DayFilter":
				case "CooldownFilter":
				case "BarsSinceFilter":
				case "ChartObject":
					return 1;
				case "LongOutput":
				case "ShortOutput":
					return 2; // trigger + optional confidence
				case "Crossover":
				case "Comparison":
				case "Expression":
				case "Range":
				case "Pattern":
				case "Swing":
					return 2;
				case "And":
				case "Or":
				case "AtLeastNOfM":
				case "WeightedAverage":
					return 6; // variable; allocate 6 slots visually
				default:
					return 2;
			}
		}

		private void BuildInputPorts()
		{
			_portsLeft.Children.Clear();
			_portsLeft.Width  = 2 * PortRadius;
			_portsLeft.HorizontalAlignment = HorizontalAlignment.Left;
			_portsLeft.Margin = new Thickness(-PortRadius, 0, 0, 0);

			for (int i = 0; i < _inputCount; i++)
			{
				int portIdx = i;
				var dot = new Ellipse
				{
					Width  = 2 * PortRadius, Height = 2 * PortRadius,
					Fill   = new SolidColorBrush(Color.FromRgb(140, 180, 255)),
					Stroke = Brushes.Black, StrokeThickness = 1,
					Cursor = Cursors.Cross,
					Tag    = portIdx
				};
				dot.MouseDown += (s, e) => { var h = PortMouseDown; if (h != null) h(portIdx, true, e); };
				_portsLeft.Children.Add(dot);
			}
		}

		private void LayoutPorts()
		{
			if (_inputCount == 0) return;
			double h = ActualHeight > 0 ? ActualHeight : 46;
			double spacing = _inputCount > 1 ? (h - 2 * PortRadius) / (_inputCount - 1) : 0;
			double y = PortRadius;
			for (int i = 0; i < _portsLeft.Children.Count; i++)
			{
				var el = _portsLeft.Children[i] as Ellipse;
				if (el == null) continue;
				Canvas.SetLeft(el, 0);
				Canvas.SetTop (el, y - PortRadius);
				y += spacing;
			}
			_portsLeft.Height = h;
		}

		// World-space coordinates of the output port anchor (for wire geometry)
		public Point GetOutputPortContentPoint(int port)
		{
			double left = Canvas.GetLeft(this);
			double top  = Canvas.GetTop (this);
			double w    = ActualWidth > 0 ? ActualWidth : NodeWidth;
			double h    = ActualHeight > 0 ? ActualHeight : 46;
			return new Point(left + w, top + h / 2.0);
		}

		public Point GetInputPortContentPoint(int portIndex)
		{
			double left = Canvas.GetLeft(this);
			double top  = Canvas.GetTop (this);
			double h    = ActualHeight > 0 ? ActualHeight : 46;
			if (_inputCount <= 1) return new Point(left, top + h / 2.0);
			double spacing = (h - 2 * PortRadius) / (_inputCount - 1);
			double y = top + PortRadius + spacing * portIndex;
			return new Point(left, y);
		}

		public bool HitTestInputPort(Point contentPoint, out int port)
		{
			port = -1;
			for (int i = 0; i < _inputCount; i++)
			{
				var p = GetInputPortContentPoint(i);
				if ((contentPoint - p).Length <= PortRadius + 3)
				{
					port = i;
					return true;
				}
			}
			return false;
		}

		public void RefreshSummary() { _summary.Text = BuildSummary(Model); _title.Text = string.IsNullOrEmpty(Model.DisplayName) ? Model.TypeKey : Model.DisplayName; }
		public void RefreshValue  () { _valueBlock.Text = Model.LastOutput.ToString("F3", CultureInfo.InvariantCulture); }

		private static string BuildSummary(SignalStudioNodeBase model)
		{
			var cfg = model.GetConfig();
			if (cfg == null || cfg.Count == 0) return model.TypeKey;
			var parts = new List<string>();
			foreach (var kv in cfg)
			{
				if (kv.Value == null) continue;
				string v = kv.Value.ToString();
				if (v.Length > 14) v = v.Substring(0, 14) + "…";
				parts.Add(kv.Key + "=" + v);
				if (parts.Count >= 3) break;
			}
			return string.Join("  ", parts);
		}

		private static Color CategoryColor(SSNodeCategory cat)
		{
			switch (cat)
			{
				case SSNodeCategory.DataSource: return Color.FromRgb(50, 70, 110);
				case SSNodeCategory.Solver:     return Color.FromRgb(60, 100, 70);
				case SSNodeCategory.Logic:      return Color.FromRgb(110, 80,  60);
				case SSNodeCategory.Filter:     return Color.FromRgb(90, 60, 110);
				case SSNodeCategory.Output:     return Color.FromRgb(120, 40,  60);
				default:                         return Color.FromRgb(60, 60, 70);
			}
		}
	}

	// ============================================================================================
	// WireVisual
	// ============================================================================================

	// Composition wrapper - System.Windows.Shapes.Path is sealed, so we hold one instead of
	// inheriting. The canvas adds wire.Visual as a UIElement.
	public class WireVisual
	{
		public NodeVisual From     { get; private set; }
		public int        FromPort { get; private set; }
		public NodeVisual To       { get; private set; }
		public int        ToPort   { get; private set; }
		public WpfPath    Visual   { get; private set; }

		public WireVisual(NodeVisual from, int fromPort, NodeVisual to, int toPort)
		{
			From     = from;
			FromPort = fromPort;
			To       = to;
			ToPort   = toPort;
			Visual   = new WpfPath
			{
				Stroke            = PickStroke(to),
				StrokeThickness   = 2,
				IsHitTestVisible  = false
			};
			UpdateGeometry();
		}

		private static Brush PickStroke(NodeVisual dest)
		{
			if (dest != null && dest.Model is LongOutputNode)  return new SolidColorBrush(Color.FromRgb(120, 200, 120));
			if (dest != null && dest.Model is ShortOutputNode) return new SolidColorBrush(Color.FromRgb(220, 110, 110));
			return new SolidColorBrush(Color.FromRgb(140, 160, 190));
		}

		public void UpdateGeometry()
		{
			if (From == null || To == null || Visual == null) return;
			var a = From.GetOutputPortContentPoint(FromPort);
			var b = To  .GetInputPortContentPoint (ToPort);
			Visual.Data = BuildBezier(a, b);
		}

		public static Geometry BuildBezier(Point a, Point b)
		{
			double dx = Math.Max(40, Math.Abs(b.X - a.X) * 0.5);
			var c1 = new Point(a.X + dx, a.Y);
			var c2 = new Point(b.X - dx, b.Y);

			var fig = new PathFigure { StartPoint = a, IsClosed = false };
			fig.Segments.Add(new BezierSegment(c1, c2, b, true));
			var g = new PathGeometry();
			g.Figures.Add(fig);
			g.Freeze();
			return g;
		}
	}

	// ============================================================================================
	// NodeToolbox
	// ============================================================================================

	public class NodeToolbox : ScrollViewer
	{
		public event Action<string> NodeFactoryActivated;

		public NodeToolbox()
		{
			HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
			VerticalScrollBarVisibility   = ScrollBarVisibility.Auto;
			Background                    = new SolidColorBrush(Color.FromRgb(26, 26, 30));

			var stack = new StackPanel { Margin = new Thickness(4) };
			Content = stack;

			stack.Children.Add(BuildHeader("Toolbox"));
			stack.Children.Add(BuildCategory("Data sources", new[]
			{
				T("PriceSource",  "Price"),
				T("Constant",     "Constant"),
				T("Builtin",      "Built-in"),
				T("IndicatorPlot","External plot")
			}));
			stack.Children.Add(BuildCategory("Solvers", new[]
			{
				T("Crossover",    "Crossover"),
				T("Comparison",   "Comparison"),
				T("Slope",        "Slope"),
				T("Range",        "Range"),
				T("Pattern",      "Pattern"),
				T("Swing",        "Swing"),
				T("Expression",   "Expression"),
				T("ChartObject",  "Chart object")
			}));
			stack.Children.Add(BuildCategory("Logic", new[]
			{
				T("And",             "AND"),
				T("Or",              "OR"),
				T("Not",             "NOT"),
				T("AtLeastNOfM",     "At least N"),
				T("WeightedAverage", "Weighted avg")
			}));
			stack.Children.Add(BuildCategory("Filters", new[]
			{
				T("TimeFilter",       "Time window"),
				T("DayFilter",        "Day of week"),
				T("CooldownFilter",   "Cooldown"),
				T("BarsSinceFilter",  "Bars-since")
			}));
			stack.Children.Add(BuildCategory("Output", new[]
			{
				T("LongOutput",  "Long"),
				T("ShortOutput", "Short")
			}));
		}

		private UIElement BuildHeader(string text)
		{
			return new TextBlock
			{
				Text       = text,
				Foreground = Brushes.White,
				FontWeight = FontWeights.Bold,
				Margin     = new Thickness(4, 4, 4, 8)
			};
		}

		private UIElement BuildCategory(string title, Tuple<string, string>[] entries)
		{
			var expander = new Expander
			{
				Header     = title,
				Foreground = Brushes.White,
				IsExpanded = true,
				Margin     = new Thickness(0, 0, 0, 6)
			};
			var stack = new StackPanel();
			foreach (var e in entries)
			{
				var key = e.Item1; var label = e.Item2;
				var btn = new Button
				{
					Content    = label,
					ToolTip    = label + "  (adds a " + key + " node)",
					HorizontalContentAlignment = HorizontalAlignment.Left,
					Padding    = new Thickness(6, 0, 6, 0),
					Margin     = new Thickness(2),
					Background = new SolidColorBrush(Color.FromRgb(50, 50, 60)),
					Foreground = Brushes.White,
					BorderBrush = new SolidColorBrush(Color.FromRgb(80, 80, 90)),
					Height = 24
				};
				btn.Click += (s, ev) => { var h = NodeFactoryActivated; if (h != null) h(key); };
				stack.Children.Add(btn);
			}
			expander.Content = stack;
			return expander;
		}

		private static Tuple<string, string> T(string key, string label) { return Tuple.Create(key, label); }
	}

	// ============================================================================================
	// NodeConfigPanel
	// ============================================================================================

	public class NodeConfigPanel : ScrollViewer
	{
		public event Action NodeChanged;

		private SignalStudioNodeBase _node;
		private readonly StackPanel _stack;

		public NodeConfigPanel()
		{
			Background                  = new SolidColorBrush(Color.FromRgb(26, 26, 30));
			HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
			VerticalScrollBarVisibility   = ScrollBarVisibility.Auto;
			_stack                        = new StackPanel { Margin = new Thickness(8) };
			Content                       = _stack;
		}

		public void Bind(SignalStudioNodeBase node)
		{
			_node = node;
			_stack.Children.Clear();
			if (node == null)
			{
				_stack.Children.Add(new TextBlock { Text = "No node selected", Foreground = Brushes.LightGray, FontStyle = FontStyles.Italic });
				return;
			}

			_stack.Children.Add(new TextBlock { Text = node.TypeKey, Foreground = Brushes.White, FontWeight = FontWeights.Bold, FontSize = 14, Margin = new Thickness(0, 0, 0, 8) });

			AddStringEditor("Display name", node.DisplayName, v => { node.DisplayName = v; RaiseChanged(); });

			var props = node.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(p => p.CanRead && p.CanWrite && IsSerializableType(p.PropertyType))
				.ToList();

			foreach (var p in props)
			{
				if (p.Name == "Id" || p.Name == "DisplayName" || p.Name == "X" || p.Name == "Y" || p.Name == "Inputs" || p.Name == "LastOutput") continue;

				var captured = p;
				var val = captured.GetValue(node, null);

				if (captured.PropertyType == typeof(bool))
				{
					AddBoolEditor(captured.Name, (bool)(val ?? false), v => { captured.SetValue(node, v, null); RaiseChanged(); });
				}
				else if (captured.PropertyType == typeof(int))
				{
					AddIntEditor(captured.Name, (int)(val ?? 0), v => { captured.SetValue(node, v, null); RaiseChanged(); });
				}
				else if (captured.PropertyType == typeof(double))
				{
					AddDoubleEditor(captured.Name, (double)(val ?? 0.0), v => { captured.SetValue(node, v, null); RaiseChanged(); });
				}
				else if (captured.PropertyType == typeof(string))
				{
					AddStringEditor(captured.Name, (string)val ?? "", v => { captured.SetValue(node, v, null); RaiseChanged(); });
				}
				else if (captured.PropertyType.IsEnum)
				{
					AddEnumEditor(captured.Name, captured.PropertyType, val, v => { captured.SetValue(node, v, null); RaiseChanged(); });
				}
			}
		}

		private static bool IsSerializableType(Type t)
		{
			return t == typeof(string) || t == typeof(bool) || t == typeof(int) || t == typeof(double) || t.IsEnum;
		}

		private void AddLabeledRow(string label, UIElement editor)
		{
			var row = new DockPanel { Margin = new Thickness(0, 2, 0, 2) };
			var lb = new TextBlock { Text = label, Foreground = Brushes.LightGray, Width = 120, VerticalAlignment = VerticalAlignment.Center };
			DockPanel.SetDock(lb, Dock.Left);
			row.Children.Add(lb);
			row.Children.Add(editor);
			_stack.Children.Add(row);
		}

		private void AddStringEditor(string label, string value, Action<string> onChanged)
		{
			var tb = new TextBox { Text = value ?? "", Margin = new Thickness(2), Background = new SolidColorBrush(Color.FromRgb(40, 40, 48)), Foreground = Brushes.White, BorderBrush = new SolidColorBrush(Color.FromRgb(70, 70, 80)) };
			tb.LostFocus += (s, e) => onChanged(tb.Text);
			AddLabeledRow(label, tb);
		}

		private void AddBoolEditor(string label, bool value, Action<bool> onChanged)
		{
			var cb = new CheckBox { IsChecked = value, Foreground = Brushes.White, Margin = new Thickness(2), VerticalAlignment = VerticalAlignment.Center };
			cb.Checked   += (s, e) => onChanged(true);
			cb.Unchecked += (s, e) => onChanged(false);
			AddLabeledRow(label, cb);
		}

		private void AddIntEditor(string label, int value, Action<int> onChanged)
		{
			var tb = new TextBox { Text = value.ToString(CultureInfo.InvariantCulture), Margin = new Thickness(2), Background = new SolidColorBrush(Color.FromRgb(40, 40, 48)), Foreground = Brushes.White, BorderBrush = new SolidColorBrush(Color.FromRgb(70, 70, 80)) };
			tb.LostFocus += (s, e) =>
			{
				int v; if (int.TryParse(tb.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out v)) onChanged(v);
				else tb.Text = value.ToString(CultureInfo.InvariantCulture);
			};
			AddLabeledRow(label, tb);
		}

		private void AddDoubleEditor(string label, double value, Action<double> onChanged)
		{
			var tb = new TextBox { Text = value.ToString("R", CultureInfo.InvariantCulture), Margin = new Thickness(2), Background = new SolidColorBrush(Color.FromRgb(40, 40, 48)), Foreground = Brushes.White, BorderBrush = new SolidColorBrush(Color.FromRgb(70, 70, 80)) };
			tb.LostFocus += (s, e) =>
			{
				double v; if (double.TryParse(tb.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out v)) onChanged(v);
				else tb.Text = value.ToString("R", CultureInfo.InvariantCulture);
			};
			AddLabeledRow(label, tb);
		}

		private void AddEnumEditor(string label, Type enumType, object value, Action<object> onChanged)
		{
			var cb = new ComboBox { Margin = new Thickness(2), Background = new SolidColorBrush(Color.FromRgb(40, 40, 48)), Foreground = Brushes.White, BorderBrush = new SolidColorBrush(Color.FromRgb(70, 70, 80)) };
			foreach (var v in Enum.GetValues(enumType)) cb.Items.Add(v);
			cb.SelectedItem = value;
			cb.SelectionChanged += (s, e) => { if (cb.SelectedItem != null) onChanged(cb.SelectedItem); };
			AddLabeledRow(label, cb);
		}

		private void RaiseChanged() { var h = NodeChanged; if (h != null) h(); }
	}

	// ============================================================================================
	// Indicator partial: studio window launch + history re-evaluation
	// ============================================================================================

	public partial class aiSignalStudio
	{
		private SignalStudioWindow _studioWindow;

		// Called from property changed-handlers (via pOpenStudio). Opens the window on the UI dispatcher.
		public void OpenStudioWindow()
		{
			try
			{
				if (ChartControl == null) { Print("[aiSignalStudio] Chart not ready; cannot open Studio window."); return; }
				ChartControl.Dispatcher.InvokeAsync(() =>
				{
					if (_studioWindow != null)
					{
						try { _studioWindow.Activate(); return; } catch { _studioWindow = null; }
					}
					var engine = _engine ?? (_engine = new SignalStudioEngine());
					_studioWindow = new SignalStudioWindow(this, engine);
					try { _studioWindow.Owner = Window.GetWindow(ChartControl); } catch { }
					_studioWindow.Show();
				});
			}
			catch (Exception ex) { Print("[aiSignalStudio] OpenStudioWindow failed: " + ex.Message); }
		}

		internal void OnStudioWindowClosed()
		{
			_studioWindow = null;
		}

		// Triggers a repaint so the indicator picks up the new graph on next bar close.
		// NT's Series<double> doesn't allow setting historical bars by index, so true
		// history backfill requires a full indicator reload. For live preview, the engine
		// has already been rebuilt - the new logic applies from the next bar forward, and
		// ForceRefresh() redraws any plots we've updated on the current bar.
		public void RequestHistoryReevaluation()
		{
			try
			{
				if (ChartControl == null) return;
				ChartControl.Dispatcher.InvokeAsync(() =>
				{
					try
					{
						if (CurrentBar >= 0)
						{
							// Re-evaluate current bar so the preview is accurate immediately
							var r = _engine.Evaluate(this, CurrentBar);
							Values[0][0] = r.LongValue;
							Values[1][0] = r.ShortValue;
							Values[2][0] = r.Direction;
							Values[3][0] = r.Confidence;
							Values[4][0] = r.HasSignal ? 1 : 0;
						}
						ForceRefresh();
					}
					catch (Exception ex) { Print("[aiSignalStudio] reeval failed: " + ex.Message); }
				});
			}
			catch { }
		}
	}
}
