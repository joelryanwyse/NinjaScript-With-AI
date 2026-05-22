using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using NinjaTrader.Cbi;
using NinjaTrader.Core;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Tools;
using NinjaTrader.NinjaScript;

namespace NinjaTrader.NinjaScript.AddOns
{
	/// <summary>
	/// Adds "New > Affordable Indicators > Accounts Dashboard" to the Control Center menu.
	/// Self-contained and coexists with other Affordable Indicators launcher AddOns — each
	/// product ships its own launcher file, and all independently find-or-create the shared
	/// "Affordable Indicators" submenu.
	/// </summary>
	public class aiAccountsDashboardLauncher : AddOnBase
	{
		private const string AffordableMenuHeader	= "Affordable Indicators";
		private const string MenuItemLabel			= "Accounts Dashboard";

		private NTMenuItem affordableMenu;
		private NTMenuItem dashboardMenuItem;
		private NTMenuItem parentMenu;
		private System.Windows.Threading.DispatcherTimer _menuUpdateTimer;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Name		= "aiAccountsDashboardLauncher";
				Description	= "Adds Accounts Dashboard launcher to Control Center New menu";
			}
		}

		protected override void OnWindowCreated(Window window)
		{
			ControlCenter cc = window as ControlCenter;
			if (cc == null)
				return;

			parentMenu = cc.FindFirst("ControlCenterMenuItemNew") as NTMenuItem;
			if (parentMenu == null)
				return;

			// Find or create the shared "Affordable Indicators" submenu
			affordableMenu = FindAffordableMenu(parentMenu);
			if (affordableMenu == null)
			{
				affordableMenu = new NTMenuItem
				{
					Header	= AffordableMenuHeader,
					Style	= Application.Current.TryFindResource("MainMenuItem") as Style
				};
				parentMenu.Items.Add(affordableMenu);
			}

			// Don't duplicate our own item if another instance of this AddOn already added it
			foreach (object item in affordableMenu.Items)
			{
				NTMenuItem existing = item as NTMenuItem;
				if (existing == null) continue;
				string header = ExtractHeaderText(existing);
				if (header != null && header.StartsWith(MenuItemLabel))
					return;
			}

			dashboardMenuItem = new NTMenuItem
			{
				Header	= MenuItemLabel,
				Style	= Application.Current.TryFindResource("MainMenuItem") as Style
			};
			dashboardMenuItem.Click += OnLaunchDashboard;
			affordableMenu.Items.Add(dashboardMenuItem);

			_menuUpdateTimer = new System.Windows.Threading.DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(3),
				IsEnabled = true
			};
			_menuUpdateTimer.Tick += OnMenuUpdateTick;
		}

		private NTMenuItem FindAffordableMenu(NTMenuItem parent)
		{
			foreach (object item in parent.Items)
			{
				NTMenuItem mi = item as NTMenuItem;
				if (mi != null && mi.Header != null && mi.Header.ToString() == AffordableMenuHeader)
					return mi;
			}
			return null;
		}

		private string ExtractHeaderText(NTMenuItem item)
		{
			if (item.Header is string) return (string)item.Header;
			StackPanel sp = item.Header as StackPanel;
			if (sp != null && sp.Children.Count > 0)
			{
				TextBlock tb = sp.Children[0] as TextBlock;
				if (tb != null) return tb.Text;
			}
			return null;
		}

		private void OnMenuUpdateTick(object sender, EventArgs e)
		{
			if (dashboardMenuItem == null)
				return;
			try
			{
				int count = CountIndicatorTabs(typeof(NinjaTrader.NinjaScript.Indicators.aiDuplicateAccountActions));
				if (count > 0)
				{
					var sp = new StackPanel { Orientation = Orientation.Horizontal };
					sp.Children.Add(new TextBlock { Text = MenuItemLabel + " " });
					sp.Children.Add(new TextBlock { Text = "(" + count + " open)", Opacity = 0.5 });
					dashboardMenuItem.Header = sp;
				}
				else
					dashboardMenuItem.Header = MenuItemLabel;
			}
			catch
			{
				dashboardMenuItem.Header = MenuItemLabel;
			}
		}

		private int CountIndicatorTabs(Type indicatorType)
		{
			if (!Application.Current.Dispatcher.CheckAccess())
				return (int)Application.Current.Dispatcher.Invoke(new Func<int>(() => CountIndicatorTabs(indicatorType)));

			int total = 0;
			foreach (Window w in Application.Current.Windows)
			{
				Chart chart = w as Chart;
				if (chart == null || chart.MainTabControl == null)
					continue;
				foreach (TabItem tab in chart.MainTabControl.Items)
				{
					ChartTab ct = tab.Content as ChartTab;
					if (ct == null || ct.ChartControl == null)
						continue;
					foreach (var ind in ct.ChartControl.Indicators)
					{
						if (indicatorType.IsInstanceOfType(ind))
						{
							total++;
							break;
						}
					}
				}
			}
			return total;
		}

		protected override void OnWindowDestroyed(Window window)
		{
			if (!(window is ControlCenter))
				return;

			if (_menuUpdateTimer != null)
			{
				_menuUpdateTimer.IsEnabled = false;
				_menuUpdateTimer.Tick -= OnMenuUpdateTick;
				_menuUpdateTimer = null;
			}

			if (dashboardMenuItem != null)
			{
				dashboardMenuItem.Click -= OnLaunchDashboard;
				if (affordableMenu != null)
					affordableMenu.Items.Remove(dashboardMenuItem);
				dashboardMenuItem = null;
			}

			// Only remove the shared submenu if no other products are using it
			if (affordableMenu != null && parentMenu != null && affordableMenu.Items.Count == 0)
				parentMenu.Items.Remove(affordableMenu);

			affordableMenu = null;
			parentMenu = null;
		}

		// ────────────────────────────────────────────────────────────────
		//  Launch handler
		// ────────────────────────────────────────────────────────────────

		private void OnLaunchDashboard(object sender, RoutedEventArgs e)
		{
			try
			{
				string instrumentName = ResolveFrontMonthNQ();
				string templatePath = GenerateDashboardTemplate(instrumentName);
				Chart newChart = RestoreChartFromTemplate(templatePath);

				if (newChart != null)
				{
					// Set the caption before Show so the window never displays "Chart".
					// Bars are hidden via CandleStyle.IsVisible=false in the template.
					string version = "";
					try { version = NinjaTrader.NinjaScript.Indicators.aiDuplicateAccountActions.DashboardVersion; } catch { }
					newChart.Caption = "Accounts Dashboard - " + version;
					newChart.Show();

					// Hide the time-axis scrollbar as soon as the chart's visual tree is loaded.
					// Dispatcher.BeginInvoke at Loaded priority runs after layout but before the
					// first user-visible render, so the scrollbar never appears.
					newChart.Dispatcher.BeginInvoke(
						System.Windows.Threading.DispatcherPriority.Loaded,
						new Action(() => HideChartScrollBar(newChart)));

					// After the polling loop finishes, nudge the window by 1px to force WPF
					// to run a full top-level layout pass. This reclaims the empty scrollbar
					// space without touching NT8's internal Grid structure.
					var nudgeTimer = new System.Windows.Threading.DispatcherTimer
					{
						Interval = TimeSpan.FromMilliseconds(2100)
					};
					nudgeTimer.Tick += (s, args) =>
					{
						nudgeTimer.Stop();
						try
						{
							newChart.Width = newChart.ActualWidth + 1;
							newChart.Dispatcher.BeginInvoke(
								System.Windows.Threading.DispatcherPriority.Render,
								new Action(() =>
								{
									try { newChart.Width = newChart.ActualWidth - 1; } catch { }
								}));
						}
						catch { }
					};
					nudgeTimer.Start();
				}
				else
				{
					string savedName = "Accounts Dashboard";
					string destPath = Path.Combine(Globals.UserDataDir, "templates", "Chart", savedName + ".xml");
					File.Copy(templatePath, destPath, true);
					MessageBox.Show(
						"A chart template named '" + savedName + "' has been saved.\n\n"
						+ "To launch:\n"
						+ "1. Open a new Chart (New > Chart)\n"
						+ "2. Set instrument to " + instrumentName + "\n"
						+ "3. Set period to Daily, 20 days\n"
						+ "4. Right-click the chart > Templates > 'Accounts Dashboard'",
						"Accounts Dashboard",
						MessageBoxButton.OK, MessageBoxImage.Information);
				}

				try { if (File.Exists(templatePath)) File.Delete(templatePath); } catch { }
			}
			catch (Exception ex)
			{
				Print("aiAccountsDashboardLauncher error: " + ex.Message);
				Print(ex.StackTrace);
			}
		}


		// ────────────────────────────────────────────────────────────────
		//  Helpers
		// ────────────────────────────────────────────────────────────────

		private string ResolveFrontMonthNQ()
		{
			try
			{
				Instrument bestNQ = null;
				DateTime now = DateTime.Now;

				lock (Instrument.All)
				{
					foreach (Instrument instr in Instrument.All)
					{
						if (instr.MasterInstrument == null) continue;
						if (instr.MasterInstrument.Name != "NQ") continue;
						if (instr.Expiry == DateTime.MinValue) continue;
						if (instr.Expiry < now) continue;
						if (bestNQ == null || instr.Expiry < bestNQ.Expiry)
							bestNQ = instr;
					}
				}

				if (bestNQ != null)
					return bestNQ.FullName;

				int year = now.Year;
				int month = now.Month;
				string[] codes = { "03", "06", "09", "12" };
				foreach (string m in codes)
				{
					int cm = int.Parse(m);
					if (cm >= month)
					{
						string tryName = "NQ " + m + "-" + (year % 100).ToString("D2");
						Instrument test = Instrument.GetInstrument(tryName);
						if (test != null) return test.FullName;
					}
				}
				string nextName = "NQ 03-" + ((year + 1) % 100).ToString("D2");
				Instrument nextTest = Instrument.GetInstrument(nextName);
				if (nextTest != null) return nextTest.FullName;
			}
			catch { }

			return "NQ 06-26";
		}

		/// <summary>
		/// Tries to hide the chart's time-axis scrollbar via ChartControl.Properties.ShowScrollBar
		/// AND disables input on any ScrollBar in the visual tree. Re-applies periodically for
		/// a short window to beat the indicator's own init.
		/// </summary>
		private void HideChartScrollBar(Chart chart)
		{
			int ticks = 0;
			var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
			timer.Tick += (s, args) =>
			{
				ticks++;
				try
				{
					ApplyHideScrollBar(chart);
					DisableScrollBarsInVisualTree(chart);
				}
				catch { }
				if (ticks >= 40) // 2 seconds of re-applying
					timer.Stop();
			};
			timer.Start();
			ApplyHideScrollBar(chart);
			DisableScrollBarsInVisualTree(chart);
		}

		private void ApplyHideScrollBar(Chart chart)
		{
			if (chart == null || chart.MainTabControl == null) return;
			foreach (TabItem tab in chart.MainTabControl.Items)
			{
				ChartTab ct = tab.Content as ChartTab;
				if (ct == null || ct.ChartControl == null) continue;
				var props = ct.ChartControl.Properties;
				if (props == null) continue;

				// Toggle trick: set true → false so NT8's property-change notification fires
				// even if the value was already false. This matches what OK/Apply on the
				// chart Properties dialog does internally.
				props.ShowScrollBar = true;
				props.ShowScrollBar = false;

				// Hide axis text, grid lines, and crosshair labels so the empty chart area
				// renders blank while the indicator is still loading. The indicator itself
				// sets these the same way — doing it pre-emptively eliminates the flash.
				try
				{
					var transparent = System.Windows.Media.Brushes.Transparent;
					props.ChartText = transparent;
					if (props.AxisPen != null)
						props.AxisPen.Brush = transparent;
					props.AreHGridLinesVisible = false;
					props.AreVGridLinesVisible = false;
					props.CrosshairLabelBackground = transparent;
					props.CrosshairLabelForeground = transparent;
				}
				catch { }

				// Force the tab's Header to "Accounts Dashboard" — don't depend on the
				// indicator setting Properties.TabName since that isn't picked up by the UI
				// without the user applying the Properties dialog.
				const string desiredTabName = "Accounts Dashboard";
				if (!object.Equals(tab.Header, desiredTabName))
					tab.Header = desiredTabName;
				if (props.TabName != desiredTabName)
					props.TabName = desiredTabName;

				// Force the chart to re-layout with the new property value. Without this,
				// the setter takes effect only when the user opens + applies the Properties
				// dialog (or another event triggers a chart refresh).
				ct.ChartControl.InvalidateVisual();
				ct.ChartControl.UpdateLayout();

				// Also try ChartControl.RefreshProperties() via reflection — NT8 has this
				// internally as the method the Properties dialog calls on Apply.
				TryInvokeChartRefresh(ct.ChartControl);
			}
		}

		private void TryInvokeChartRefresh(object chartControl)
		{
			if (chartControl == null) return;
			string[] candidates = { "RefreshProperties", "ApplyProperties", "OnPropertiesChanged", "Refresh" };
			Type t = chartControl.GetType();
			foreach (string name in candidates)
			{
				try
				{
					MethodInfo m = t.GetMethod(name,
						BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
						null, Type.EmptyTypes, null);
					if (m != null)
					{
						m.Invoke(chartControl, null);
						return;
					}
				}
				catch { }
			}
		}

		/// <summary>
		/// Walks the chart's visual tree and disables input on every ScrollBar it finds.
		/// This does not hide the scrollbar but prevents the user from interacting with it.
		/// </summary>
		private void DisableScrollBarsInVisualTree(System.Windows.DependencyObject root)
		{
			if (root == null) return;
			int count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(root);
			for (int i = 0; i < count; i++)
			{
				var child = System.Windows.Media.VisualTreeHelper.GetChild(root, i);
				if (child is System.Windows.Controls.Primitives.ScrollBar)
				{
					var sb = (System.Windows.Controls.Primitives.ScrollBar)child;
					sb.Visibility = Visibility.Collapsed;
					sb.IsHitTestVisible = false;
					sb.IsEnabled = false;
					sb.Focusable = false;
				}
				DisableScrollBarsInVisualTree(child);
			}
		}

		private Chart RestoreChartFromTemplate(string templatePath)
		{
			try
			{
				Chart newChart = new Chart();

				System.Xml.Linq.XDocument xdoc = System.Xml.Linq.XDocument.Load(templatePath);
				System.Xml.Linq.XElement root = xdoc.Root;

				Type chartType = typeof(Chart);
				MethodInfo restoreMethod = chartType.GetMethod("Restore",
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
					null,
					new Type[] { typeof(System.Xml.Linq.XDocument), typeof(System.Xml.Linq.XElement), typeof(bool) },
					null);

				if (restoreMethod != null)
				{
					restoreMethod.Invoke(newChart, new object[] { xdoc, root, true });
				}
				else
				{
					restoreMethod = chartType.GetMethod("Restore",
						BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
						null,
						new Type[] { typeof(System.Xml.Linq.XDocument), typeof(System.Xml.Linq.XElement) },
						null);

					if (restoreMethod != null)
						restoreMethod.Invoke(newChart, new object[] { xdoc, root });
					else
						return null;
				}

				return newChart;
			}
			catch (Exception ex)
			{
				Print("aiAccountsDashboardLauncher RestoreChart error: " + ex.Message);
				if (ex.InnerException != null)
					Print("  Inner: " + ex.InnerException.Message);
				return null;
			}
		}

		// ────────────────────────────────────────────────────────────────
		//  Template
		// ────────────────────────────────────────────────────────────────

		private string GenerateDashboardTemplate(string instrumentName)
		{
			string id1 = Guid.NewGuid().ToString("N");
			string id2 = Guid.NewGuid().ToString("N");
			string globexName = instrumentName.Contains(" ")
				? instrumentName.Split(' ')[0] + " " + instrumentName.Split(' ')[1] + " Globex"
				: instrumentName + " Globex";

			// BarsPeriodTypeSerialize: 0=Tick, 1=Volume, 2=Range, 3=Second, 4=Minute, 5=Day
			// Dashboard uses Daily / 20 days so the chart isn't expensive to load.
			string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<NinjaTrader>
  <NTTabPage>
    <InstrumentLink>0</InstrumentLink>
    <IntervalLink>0</IntervalLink>
    <ChartTraderVisibility>Collapsed</ChartTraderVisibility>
    <SeriesCount>1</SeriesCount>
    <DataSeries>
      <BarsProperties>
        <BarsProperties xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
          <BarsPeriod>
            <BarsPeriodTypeSerialize>5</BarsPeriodTypeSerialize>
            <BaseBarsPeriodType>Minute</BaseBarsPeriodType>
            <BaseBarsPeriodValue>1</BaseBarsPeriodValue>
            <VolumetricDeltaType>BidAsk</VolumetricDeltaType>
            <MarketDataType>Last</MarketDataType>
            <PointAndFigurePriceType>Close</PointAndFigurePriceType>
            <ReversalType>Tick</ReversalType>
            <Value>1</Value>
            <Value2>1</Value2>
          </BarsPeriod>
          <RangeType>Days</RangeType>
          <BarsBack>20</BarsBack>
          <DaysBack>20</DaysBack>
          <From>2020-01-01T00:00:00</From>
          <IsStableSession>true</IsStableSession>
          <To>2099-12-01T00:00:00</To>
          <TradingHoursSerializable />
          <AutoScale>true</AutoScale>
          <CenterPriceOnScale>false</CenterPriceOnScale>
          <DisplayInDataBox>true</DisplayInDataBox>
          <Label>" + instrumentName + @"</Label>
          <MaxSerialized>0</MaxSerialized>
          <MinSerialized>0</MinSerialized>
          <Panel>0</Panel>
          <PriceMarker>
            <BackgroundSerialize>DEFAULT</BackgroundSerialize>
            <IsVisible>false</IsVisible>
          </PriceMarker>
          <ShowGlobalDrawObjects>false</ShowGlobalDrawObjects>
          <ScaleJustification>Right</ScaleJustification>
          <TradingHoursVisibility>Off</TradingHoursVisibility>
          <PlotExecutions>DoNotPlot</PlotExecutions>
          <BarsSeriesId>" + id1 + @"</BarsSeriesId>
          <Id>" + id2 + @"</Id>
          <Instrument>" + instrumentName + @"</Instrument>
          <IsLinked>true</IsLinked>
          <IsPrimarySeries>true</IsPrimarySeries>
          <ZOrder>1</ZOrder>
        </BarsProperties>
        <ChartStyles>
          <ChartStyle>
            <CandleStyle>
              <CandleStyle xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                <IsVisible>false</IsVisible>
                <BarWidth>4</BarWidth>
                <ChartStyleTypeSerialize>1</ChartStyleTypeSerialize>
                <DownBrushSerialize>#00000000</DownBrushSerialize>
                <UpBrushSerialize>#00000000</UpBrushSerialize>
                <StrokeSerialize>#00000000</StrokeSerialize>
                <Stroke2Serialize>#00000000</Stroke2Serialize>
              </CandleStyle>
            </CandleStyle>
          </ChartStyle>
        </ChartStyles>
      </BarsProperties>
    </DataSeries>
    <Indicators>
      <Indicator BarsIndex=""0"" Instrument=""" + globexName + @""" Name=""NinjaTrader.NinjaScript.Indicators.aiDuplicateAccountActions"" Panel=""-1"" DisplayName=""aiDuplicateAccountActions"">
        <aiDuplicateAccountActions xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
          <IsVisible>true</IsVisible>
          <AreLinesConfigurable>true</AreLinesConfigurable>
          <ArePlotsConfigurable>false</ArePlotsConfigurable>
          <BarsPeriodSerializable>
            <BarsPeriodTypeSerialize>5</BarsPeriodTypeSerialize>
            <BaseBarsPeriodType>Minute</BaseBarsPeriodType>
            <BaseBarsPeriodValue>1</BaseBarsPeriodValue>
            <VolumetricDeltaType>BidAsk</VolumetricDeltaType>
            <MarketDataType>Last</MarketDataType>
            <PointAndFigurePriceType>Close</PointAndFigurePriceType>
            <ReversalType>Tick</ReversalType>
            <Value>1</Value>
            <Value2>1</Value2>
          </BarsPeriodSerializable>
          <BarsToLoad>0</BarsToLoad>
          <DisplayInDataBox>false</DisplayInDataBox>
          <Panel>-1</Panel>
          <ScaleJustification>Right</ScaleJustification>
          <ShowTransparentPlotsInDataBox>false</ShowTransparentPlotsInDataBox>
          <Calculate>OnBarClose</Calculate>
          <Displacement>0</Displacement>
          <IsAutoScale>false</IsAutoScale>
          <IsDataSeriesRequired>true</IsDataSeriesRequired>
          <IsOverlay>true</IsOverlay>
          <Lines />
          <MaximumBarsLookBack>TwoHundredFiftySix</MaximumBarsLookBack>
          <Name>aiDuplicateAccountActions</Name>
          <Plots />
          <SelectedValueSeries>0</SelectedValueSeries>
          <InputPlot>0</InputPlot>
          <IsTradingHoursBreakLineVisible>true</IsTradingHoursBreakLineVisible>
          <DrawHorizontalGridLines>true</DrawHorizontalGridLines>
          <DrawVerticalGridLines>true</DrawVerticalGridLines>
          <DrawOnPricePanel>true</DrawOnPricePanel>
          <PaintPriceMarkers>true</PaintPriceMarkers>
          <ChartHashCodeDeserialized>0</ChartHashCodeDeserialized>
          <IndicatorId>0</IndicatorId>
          <MaxSerialized>0</MaxSerialized>
          <MinSerialized>0</MinSerialized>
          <ZOrder>0</ZOrder>
          <AllInstruments>true</AllInstruments>
          <IsCrossEnabled>true</IsCrossEnabled>
          <IsXEnabled>true</IsXEnabled>
          <IsFadeEnabled>false</IsFadeEnabled>
          <IsBuildMode>true</IsBuildMode>
          <IsCopyBasicFunctionsChecked>true</IsCopyBasicFunctionsChecked>
          <IsRiskFunctionsChecked>true</IsRiskFunctionsChecked>
          <TheCopierMode>Selection</TheCopierMode>
          <ShowStatusMessages>true</ShowStatusMessages>
          <ShowConnectedStatus>true</ShowConnectedStatus>
          <QtyColumn>true</QtyColumn>
          <UnrealizedColumn>true</UnrealizedColumn>
          <RealizedColumn>true</RealizedColumn>
          <CommissionsColumn>true</CommissionsColumn>
          <TotalPNLColumn>true</TotalPNLColumn>
          <NetLiquidationColumn>true</NetLiquidationColumn>
          <IsXColumnEnabled>true</IsXColumnEnabled>
          <IsCrossColumnEnabled>true</IsCrossColumnEnabled>
          <ShowFlattenEverything>true</ShowFlattenEverything>
          <ColumnAutoLiquidate>true</ColumnAutoLiquidate>
          <ColumnRemaining>true</ColumnRemaining>
          <ColumnDailyGoal>true</ColumnDailyGoal>
          <ColumnDailyLoss>true</ColumnDailyLoss>
          <AlertLogEnabled>true</AlertLogEnabled>
          <ShowTotalRows>true</ShowTotalRows>
        </aiDuplicateAccountActions>
      </Indicator>
    </Indicators>
    <Strategies />
  </NTTabPage>
</NinjaTrader>";

			string tempPath = Path.Combine(Path.GetTempPath(), "ai_dashboard_template_" + Guid.NewGuid().ToString("N") + ".xml");
			File.WriteAllText(tempPath, xml);
			return tempPath;
		}
	}
}
