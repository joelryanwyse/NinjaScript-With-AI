//
// Copyright (C) 2026, Affordable Indicators, Inc. <www.affordableindicators.com>.
// aiSignalStudio - Phase 1: Core Engine (no WPF canvas yet)
//
// This file contains:
//   1. Support types:  BarContext, SignalResult, NodeConnection, enums
//   2. Base node:      SignalStudioNodeBase
//   3. Data sources:   PriceSourceNode, ConstantNode, IndicatorPlotNode, BuiltinIndicatorNode
//   4. Solvers:        CrossoverSolverNode, ComparisonSolverNode, SlopeSolverNode
//   5. Logic:          AndNode, OrNode, NotNode
//   6. Outputs:        LongOutputNode, ShortOutputNode
//   7. Engine:         SignalStudioEngine (topological sort + per-bar evaluate)
//   8. Templates:      SignalStudioTemplate + tiny JSON reader/writer
//   9. Indicator:      aiSignalStudio (5 public plots for ECT consumption)
//

#region Using declarations
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Xml.Serialization;
using System.Windows.Media;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using SharpDX;
using SharpDX.Direct2D1;
#endregion

namespace NinjaTrader.NinjaScript.Indicators
{
	// ============================================================================================
	// 1. Support types
	// ============================================================================================
	#region Support types

	public enum SSNodeCategory { DataSource, Solver, Logic, Filter, Output }

	public enum SSPriceSource
	{
		Open, High, Low, Close, Volume,
		Median,     // (H+L)/2
		Typical,    // (H+L+C)/3
		Weighted    // (H+L+2C)/4
	}

	public enum SSBuiltinKind { SMA, EMA, RSI, Momentum, ATR, StdDev }

	public enum SSCrossDirection { Above, Below }

	public enum SSComparisonOp { Greater, Less, GreaterOrEqual, LessOrEqual, Equal, NotEqual }

	public enum SSSlopeDirection { Rising, Falling }

	public struct SignalResult
	{
		public double LongValue;    // 0 or 1
		public double ShortValue;   // 0 or -1
		public int    Direction;    // 1 / -1 / 0
		public double Confidence;   // 0.0 - 1.0
		public bool   HasSignal;

		public static SignalResult None
		{
			get { return new SignalResult { LongValue = 0, ShortValue = 0, Direction = 0, Confidence = 0, HasSignal = false }; }
		}
	}

	// Context passed to every node evaluation. Contains the host indicator,
	// current bar, accumulated BarsAgo lookback offset, and a shared indicator cache.
	public class BarContext
	{
		public Indicator Host;
		public int       CurrentBar;
		public int       BarsAgo;
		public Dictionary<string, IndicatorBase> IndicatorCache;

		public int BarIndex { get { return CurrentBar - BarsAgo; } }

		public BarContext(Indicator host, int currentBar)
		{
			Host           = host;
			CurrentBar     = currentBar;
			BarsAgo        = 0;
			IndicatorCache = null;
		}

		public BarContext WithBarsAgo(int extra)
		{
			return new BarContext(Host, CurrentBar)
			{
				BarsAgo        = BarsAgo + extra,
				IndicatorCache = IndicatorCache
			};
		}
	}

	public class NodeConnection
	{
		public string FromNodeId;
		public int    FromPort;
		public int    ToPort;
		[XmlIgnore]
		public SignalStudioNodeBase FromNode;   // resolved by engine after load
	}

	#endregion

	// ============================================================================================
	// 2. Base node
	// ============================================================================================
	#region Base node

	public abstract class SignalStudioNodeBase
	{
		public string Id           { get; set; }
		public string DisplayName  { get; set; }
		public double X            { get; set; }
		public double Y            { get; set; }
		public List<NodeConnection> Inputs { get; set; }
		public double LastOutput   { get; private set; }

		// Memo cache so that diamond-shaped graphs don't re-evaluate a shared parent
		// multiple times for the same (bar, barsAgo) pair.
		private int    _memoBar     = int.MinValue;
		private int    _memoBarsAgo = int.MinValue;
		private double _memoValue;

		protected SignalStudioNodeBase()
		{
			Inputs = new List<NodeConnection>();
			Id     = Guid.NewGuid().ToString("N");
		}

		public double Evaluate(BarContext ctx)
		{
			if (_memoBar == ctx.CurrentBar && _memoBarsAgo == ctx.BarsAgo)
				return _memoValue;

			double v;
			try   { v = OnEvaluate(ctx); }
			catch { v = 0.0; }

			_memoBar      = ctx.CurrentBar;
			_memoBarsAgo  = ctx.BarsAgo;
			_memoValue    = v;
			LastOutput    = v;
			return v;
		}

		public virtual void ResetMemo()
		{
			_memoBar     = int.MinValue;
			_memoBarsAgo = int.MinValue;
			_memoValue   = 0.0;
		}

		protected abstract double OnEvaluate(BarContext ctx);
		public abstract SSNodeCategory Category { get; }
		public abstract string         TypeKey  { get; }

		// Optional lifecycle hook invoked once after the graph is wired up.
		public virtual void OnGraphLoaded(Indicator host) { }

		protected SignalStudioNodeBase GetInput(int portIndex)
		{
			for (int i = 0; i < Inputs.Count; i++)
				if (Inputs[i].ToPort == portIndex)
					return Inputs[i].FromNode;
			return null;
		}

		protected List<SignalStudioNodeBase> GetAllInputNodes()
		{
			var list = new List<SignalStudioNodeBase>();
			for (int i = 0; i < Inputs.Count; i++)
				if (Inputs[i].FromNode != null)
					list.Add(Inputs[i].FromNode);
			return list;
		}

		protected double EvalInput(int portIndex, BarContext ctx, double fallback)
		{
			var n = GetInput(portIndex);
			return n != null ? n.Evaluate(ctx) : fallback;
		}

		// Subclasses override to write/read their config as a dictionary. Values are
		// restricted to primitives (string, double, int, bool) and the JSON layer handles those.
		public virtual Dictionary<string, object> GetConfig() { return new Dictionary<string, object>(); }
		public virtual void SetConfig(Dictionary<string, object> cfg) { }

		// Helpers for config extraction
		protected static string   CfgStr (Dictionary<string, object> c, string k, string def)
		{
			object v; if (c != null && c.TryGetValue(k, out v) && v != null) return Convert.ToString(v, CultureInfo.InvariantCulture);
			return def;
		}
		protected static int      CfgInt (Dictionary<string, object> c, string k, int def)
		{
			object v; if (c != null && c.TryGetValue(k, out v) && v != null)
			{
				try { return Convert.ToInt32(Convert.ToDouble(v, CultureInfo.InvariantCulture)); } catch { }
			}
			return def;
		}
		protected static double   CfgDbl (Dictionary<string, object> c, string k, double def)
		{
			object v; if (c != null && c.TryGetValue(k, out v) && v != null)
			{
				try { return Convert.ToDouble(v, CultureInfo.InvariantCulture); } catch { }
			}
			return def;
		}
		protected static bool     CfgBool(Dictionary<string, object> c, string k, bool def)
		{
			object v; if (c != null && c.TryGetValue(k, out v) && v != null)
			{
				try { return Convert.ToBoolean(v); } catch { }
			}
			return def;
		}
		protected static TEnum    CfgEnum<TEnum>(Dictionary<string, object> c, string k, TEnum def) where TEnum : struct
		{
			var s = CfgStr(c, k, null);
			if (!string.IsNullOrEmpty(s))
			{
				TEnum e;
				if (Enum.TryParse<TEnum>(s, true, out e)) return e;
			}
			return def;
		}
	}

	#endregion

	// ============================================================================================
	// 3. Data source nodes
	// ============================================================================================
	#region Data source nodes

	// Reads OHLCV / derived price of the host series at (CurrentBar - BarsAgo - localBarsAgo).
	public class PriceSourceNode : SignalStudioNodeBase
	{
		public SSPriceSource Source   { get; set; }
		public int           BarsAgo  { get; set; }

		public override SSNodeCategory Category { get { return SSNodeCategory.DataSource; } }
		public override string         TypeKey  { get { return "PriceSource"; } }

		public PriceSourceNode() { Source = SSPriceSource.Close; BarsAgo = 0; }

		protected override double OnEvaluate(BarContext ctx)
		{
			int idx = ctx.BarIndex - BarsAgo;
			if (ctx.Host == null || ctx.Host.Bars == null || idx < 0 || idx > ctx.Host.CurrentBar)
				return 0.0;

			int offset = ctx.Host.CurrentBar - idx; // BarsAgo in NT's perspective (0 = current)
			if (offset < 0) return 0.0;

			switch (Source)
			{
				case SSPriceSource.Open:     return ctx.Host.Open   [offset];
				case SSPriceSource.High:     return ctx.Host.High   [offset];
				case SSPriceSource.Low:      return ctx.Host.Low    [offset];
				case SSPriceSource.Volume:   return ctx.Host.Volume [offset];
				case SSPriceSource.Median:   return (ctx.Host.High[offset] + ctx.Host.Low[offset]) / 2.0;
				case SSPriceSource.Typical:  return (ctx.Host.High[offset] + ctx.Host.Low[offset] + ctx.Host.Close[offset]) / 3.0;
				case SSPriceSource.Weighted: return (ctx.Host.High[offset] + ctx.Host.Low[offset] + 2.0 * ctx.Host.Close[offset]) / 4.0;
				default:                      return ctx.Host.Close[offset];
			}
		}

		public override Dictionary<string, object> GetConfig()
		{
			return new Dictionary<string, object>
			{
				{ "source",  Source.ToString() },
				{ "barsAgo", BarsAgo }
			};
		}
		public override void SetConfig(Dictionary<string, object> c)
		{
			Source  = CfgEnum<SSPriceSource>(c, "source", SSPriceSource.Close);
			BarsAgo = CfgInt(c, "barsAgo", 0);
		}
	}

	public class ConstantNode : SignalStudioNodeBase
	{
		public double Value { get; set; }

		public override SSNodeCategory Category { get { return SSNodeCategory.DataSource; } }
		public override string         TypeKey  { get { return "Constant"; } }

		protected override double OnEvaluate(BarContext ctx) { return Value; }

		public override Dictionary<string, object> GetConfig()
		{
			return new Dictionary<string, object> { { "value", Value } };
		}
		public override void SetConfig(Dictionary<string, object> c) { Value = CfgDbl(c, "value", 0.0); }
	}

	// Reads a plot from another indicator already present on the chart.
	// Uses the exact same pattern ECT uses in GetPlotSignals(): iterate ChartControl.Indicators,
	// match by Name, then match a plot by its Name. Cached after first successful resolve.
	public class IndicatorPlotNode : SignalStudioNodeBase
	{
		public string IndicatorName { get; set; }
		public string PlotName      { get; set; }
		public int    BarsAgo       { get; set; }

		[XmlIgnore] private IndicatorBase _cachedIndicator;
		[XmlIgnore] private int           _cachedPlotIndex = -1;
		[XmlIgnore] private int           _lastCacheTrySeq = -1;

		public override SSNodeCategory Category { get { return SSNodeCategory.DataSource; } }
		public override string         TypeKey  { get { return "IndicatorPlot"; } }

		public IndicatorPlotNode() { BarsAgo = 0; }

		private void TryResolve(BarContext ctx)
		{
			if (string.IsNullOrEmpty(IndicatorName) || string.IsNullOrEmpty(PlotName)) return;

			// First check the engine-wide cache (keyed by indicator Name).
			if (ctx.IndicatorCache != null)
			{
				IndicatorBase cached;
				if (ctx.IndicatorCache.TryGetValue(IndicatorName, out cached) && cached != null)
				{
					_cachedIndicator = cached;
					_cachedPlotIndex = FindPlotIndex(cached, PlotName);
					return;
				}
			}

			var host = ctx.Host;
			if (host == null || host.ChartControl == null || host.ChartControl.Indicators == null) return;

			for (int i = 0; i < host.ChartControl.Indicators.Count; i++)
			{
				var ind = host.ChartControl.Indicators[i] as IndicatorBase;
				if (ind == null) continue;
				if (!string.Equals(ind.Name, IndicatorName, StringComparison.OrdinalIgnoreCase)) continue;

				int plotIdx = FindPlotIndex(ind, PlotName);
				if (plotIdx < 0) continue;

				_cachedIndicator = ind;
				_cachedPlotIndex = plotIdx;
				if (ctx.IndicatorCache != null) ctx.IndicatorCache[IndicatorName] = ind;
				return;
			}
		}

		private static int FindPlotIndex(IndicatorBase ind, string plotName)
		{
			if (ind == null || ind.Plots == null) return -1;
			for (int p = 0; p < ind.Plots.Length; p++)
			{
				var pl = ind.Plots[p];
				if (pl == null || pl.Name == null) continue;
				if (string.Equals(pl.Name, plotName, StringComparison.OrdinalIgnoreCase))
					return p;
			}
			return -1;
		}

		protected override double OnEvaluate(BarContext ctx)
		{
			if (_cachedIndicator == null || _cachedPlotIndex < 0)
			{
				// Retry resolution periodically (not every bar - other indicators may load later)
				if (_lastCacheTrySeq != ctx.CurrentBar)
				{
					_lastCacheTrySeq = ctx.CurrentBar;
					TryResolve(ctx);
				}
			}

			if (_cachedIndicator == null || _cachedPlotIndex < 0) return 0.0;

			int idx = ctx.BarIndex - BarsAgo;
			if (idx < 0 || idx > ctx.Host.CurrentBar) return 0.0;

			int offset = ctx.Host.CurrentBar - idx;
			try
			{
				var series = _cachedIndicator.Values[_cachedPlotIndex];
				if (series == null || offset < 0 || offset >= series.Count) return 0.0;
				if (!series.IsValidDataPointAt(ctx.Host.CurrentBar - offset)) return 0.0;
				return series[offset];
			}
			catch { return 0.0; }
		}

		public override void OnGraphLoaded(Indicator host)
		{
			_cachedIndicator = null;
			_cachedPlotIndex = -1;
			_lastCacheTrySeq = -1;
		}

		public override Dictionary<string, object> GetConfig()
		{
			return new Dictionary<string, object>
			{
				{ "indicatorName", IndicatorName ?? "" },
				{ "plotName",      PlotName      ?? "" },
				{ "barsAgo",       BarsAgo }
			};
		}
		public override void SetConfig(Dictionary<string, object> c)
		{
			IndicatorName = CfgStr(c, "indicatorName", "");
			PlotName      = CfgStr(c, "plotName",      "");
			BarsAgo       = CfgInt(c, "barsAgo",       0);
		}
	}

	// Self-instantiates a common built-in NinjaScript indicator so templates work even
	// when the user doesn't have that indicator on the chart. Supports SMA, EMA, RSI,
	// Momentum, ATR, StdDev with a single integer period (RSI gets Period + smoothing=3 default).
	public class BuiltinIndicatorNode : SignalStudioNodeBase
	{
		public SSBuiltinKind Kind    { get; set; }
		public int           Period  { get; set; }
		public SSPriceSource Source  { get; set; }
		public int           BarsAgo { get; set; }

		[XmlIgnore] private IndicatorBase _ind;
		[XmlIgnore] private Indicator     _host;

		public override SSNodeCategory Category { get { return SSNodeCategory.DataSource; } }
		public override string         TypeKey  { get { return "Builtin"; } }

		public BuiltinIndicatorNode() { Kind = SSBuiltinKind.EMA; Period = 14; Source = SSPriceSource.Close; BarsAgo = 0; }

		public override void OnGraphLoaded(Indicator host)
		{
			_ind  = null;
			_host = host;
		}

		private ISeries<double> GetInputSeries(Indicator h)
		{
			switch (Source)
			{
				case SSPriceSource.Open:     return h.Open;
				case SSPriceSource.High:     return h.High;
				case SSPriceSource.Low:      return h.Low;
				case SSPriceSource.Volume:   return h.Volume;
				case SSPriceSource.Typical:  return h.Typical;
				case SSPriceSource.Median:   return h.Median;
				case SSPriceSource.Weighted: return h.Weighted;
				default:                      return h.Close;
			}
		}

		private void EnsureIndicator(Indicator h)
		{
			if (_ind != null) return;
			try
			{
				var input = GetInputSeries(h);
				switch (Kind)
				{
					case SSBuiltinKind.SMA:      _ind = h.SMA(input, Math.Max(1, Period)); break;
					case SSBuiltinKind.EMA:      _ind = h.EMA(input, Math.Max(1, Period)); break;
					case SSBuiltinKind.RSI:      _ind = h.RSI(input, Math.Max(1, Period), 3); break;
					case SSBuiltinKind.Momentum: _ind = h.Momentum(input, Math.Max(1, Period)); break;
					case SSBuiltinKind.ATR:      _ind = h.ATR(Math.Max(1, Period)); break;
					case SSBuiltinKind.StdDev:   _ind = h.StdDev(input, Math.Max(1, Period)); break;
				}
			}
			catch { _ind = null; }
		}

		protected override double OnEvaluate(BarContext ctx)
		{
			if (ctx.Host == null) return 0.0;
			if (_ind == null) EnsureIndicator(ctx.Host);
			if (_ind == null) return 0.0;

			int idx = ctx.BarIndex - BarsAgo;
			if (idx < 0 || idx > ctx.Host.CurrentBar) return 0.0;
			int offset = ctx.Host.CurrentBar - idx;

			try
			{
				var series = _ind.Values[0];
				if (series == null || offset < 0 || offset >= series.Count) return 0.0;
				if (!series.IsValidDataPointAt(ctx.Host.CurrentBar - offset)) return 0.0;
				return series[offset];
			}
			catch { return 0.0; }
		}

		public override Dictionary<string, object> GetConfig()
		{
			return new Dictionary<string, object>
			{
				{ "kind",    Kind.ToString()   },
				{ "period",  Period            },
				{ "source",  Source.ToString() },
				{ "barsAgo", BarsAgo           }
			};
		}
		public override void SetConfig(Dictionary<string, object> c)
		{
			Kind    = CfgEnum<SSBuiltinKind>(c, "kind",    SSBuiltinKind.EMA);
			Period  = CfgInt (c, "period",  14);
			Source  = CfgEnum<SSPriceSource>(c, "source",  SSPriceSource.Close);
			BarsAgo = CfgInt (c, "barsAgo", 0);
		}
	}

	#endregion

	// ============================================================================================
	// 4. Solver nodes
	// ============================================================================================
	#region Solver nodes

	// Port 0 = series A, Port 1 = series B.
	// Fires exactly on the bar where A crosses B in the configured direction.
	public class CrossoverSolverNode : SignalStudioNodeBase
	{
		public SSCrossDirection Direction { get; set; }

		public override SSNodeCategory Category { get { return SSNodeCategory.Solver; } }
		public override string         TypeKey  { get { return "Crossover"; } }

		public CrossoverSolverNode() { Direction = SSCrossDirection.Above; }

		protected override double OnEvaluate(BarContext ctx)
		{
			var a = GetInput(0);
			var b = GetInput(1);
			if (a == null || b == null) return 0.0;

			double aNow = a.Evaluate(ctx);
			double bNow = b.Evaluate(ctx);

			var prev = ctx.WithBarsAgo(1);
			double aPrev = a.Evaluate(prev);
			double bPrev = b.Evaluate(prev);

			if (Direction == SSCrossDirection.Above)
				return (aPrev <= bPrev && aNow >  bNow) ? 1.0 : 0.0;
			else
				return (aPrev >= bPrev && aNow <  bNow) ? 1.0 : 0.0;
		}

		public override Dictionary<string, object> GetConfig()
		{
			return new Dictionary<string, object> { { "direction", Direction.ToString() } };
		}
		public override void SetConfig(Dictionary<string, object> c)
		{
			Direction = CfgEnum<SSCrossDirection>(c, "direction", SSCrossDirection.Above);
		}
	}

	// Port 0 = left operand, Port 1 = right operand. OffsetTicks adds to left operand.
	public class ComparisonSolverNode : SignalStudioNodeBase
	{
		public SSComparisonOp Op          { get; set; }
		public double         OffsetTicks { get; set; }

		public override SSNodeCategory Category { get { return SSNodeCategory.Solver; } }
		public override string         TypeKey  { get { return "Comparison"; } }

		public ComparisonSolverNode() { Op = SSComparisonOp.Greater; OffsetTicks = 0.0; }

		protected override double OnEvaluate(BarContext ctx)
		{
			double tick = (ctx.Host != null && ctx.Host.Instrument != null && ctx.Host.Instrument.MasterInstrument != null)
				? ctx.Host.Instrument.MasterInstrument.TickSize : 0.0;

			double a = EvalInput(0, ctx, 0.0) + OffsetTicks * tick;
			double b = EvalInput(1, ctx, 0.0);

			switch (Op)
			{
				case SSComparisonOp.Greater:        return a >  b ? 1.0 : 0.0;
				case SSComparisonOp.Less:           return a <  b ? 1.0 : 0.0;
				case SSComparisonOp.GreaterOrEqual: return a >= b ? 1.0 : 0.0;
				case SSComparisonOp.LessOrEqual:    return a <= b ? 1.0 : 0.0;
				case SSComparisonOp.Equal:          return Math.Abs(a - b) < 1e-12 ? 1.0 : 0.0;
				case SSComparisonOp.NotEqual:       return Math.Abs(a - b) >= 1e-12 ? 1.0 : 0.0;
				default:                             return 0.0;
			}
		}

		public override Dictionary<string, object> GetConfig()
		{
			return new Dictionary<string, object>
			{
				{ "op",          Op.ToString() },
				{ "offsetTicks", OffsetTicks   }
			};
		}
		public override void SetConfig(Dictionary<string, object> c)
		{
			Op          = CfgEnum<SSComparisonOp>(c, "op", SSComparisonOp.Greater);
			OffsetTicks = CfgDbl(c, "offsetTicks", 0.0);
		}
	}

	// Port 0 = source series. Looks back `Period` bars and compares to the current value.
	// Rising  = currentValue > value[Period bars ago] + MinDelta
	// Falling = currentValue < value[Period bars ago] - MinDelta
	public class SlopeSolverNode : SignalStudioNodeBase
	{
		public int               Period    { get; set; }
		public SSSlopeDirection  Direction { get; set; }
		public double            MinDelta  { get; set; }  // minimum absolute difference to count

		public override SSNodeCategory Category { get { return SSNodeCategory.Solver; } }
		public override string         TypeKey  { get { return "Slope"; } }

		public SlopeSolverNode() { Period = 3; Direction = SSSlopeDirection.Rising; MinDelta = 0.0; }

		protected override double OnEvaluate(BarContext ctx)
		{
			var src = GetInput(0);
			if (src == null || Period < 1) return 0.0;

			double now  = src.Evaluate(ctx);
			double then = src.Evaluate(ctx.WithBarsAgo(Period));

			double diff = now - then;
			if (Direction == SSSlopeDirection.Rising)
				return diff >  MinDelta ? 1.0 : 0.0;
			else
				return diff < -MinDelta ? 1.0 : 0.0;
		}

		public override Dictionary<string, object> GetConfig()
		{
			return new Dictionary<string, object>
			{
				{ "period",    Period },
				{ "direction", Direction.ToString() },
				{ "minDelta",  MinDelta }
			};
		}
		public override void SetConfig(Dictionary<string, object> c)
		{
			Period    = Math.Max(1, CfgInt(c, "period", 3));
			Direction = CfgEnum<SSSlopeDirection>(c, "direction", SSSlopeDirection.Rising);
			MinDelta  = CfgDbl(c, "minDelta", 0.0);
		}
	}

	// Value-in-range (or out-of-range) solver. Port 0 = value.
	public class RangeSolverNode : SignalStudioNodeBase
	{
		public double Min         { get; set; }
		public double Max         { get; set; }
		public bool   Inclusive   { get; set; }
		public bool   Inside      { get; set; }  // true: fire when inside; false: when outside

		public override SSNodeCategory Category { get { return SSNodeCategory.Solver; } }
		public override string         TypeKey  { get { return "Range"; } }

		public RangeSolverNode() { Min = 0.0; Max = 1.0; Inclusive = true; Inside = true; }

		protected override double OnEvaluate(BarContext ctx)
		{
			double v = EvalInput(0, ctx, 0.0);
			bool inRange = Inclusive ? (v >= Min && v <= Max) : (v > Min && v < Max);
			bool fire    = Inside ? inRange : !inRange;
			return fire ? 1.0 : 0.0;
		}

		public override Dictionary<string, object> GetConfig()
		{
			return new Dictionary<string, object>
			{
				{ "min", Min }, { "max", Max }, { "inclusive", Inclusive }, { "inside", Inside }
			};
		}
		public override void SetConfig(Dictionary<string, object> c)
		{
			Min       = CfgDbl(c, "min", 0.0);
			Max       = CfgDbl(c, "max", 1.0);
			Inclusive = CfgBool(c, "inclusive", true);
			Inside    = CfgBool(c, "inside",    true);
		}
	}

	public enum SSPatternKind { BullishBar, BearishBar, HigherHigh, LowerLow, HigherLow, LowerHigh, InsideBar, OutsideBar }

	// N consecutive bars matching the selected pattern. BarsAgo counts from current bar backward.
	public class PatternSolverNode : SignalStudioNodeBase
	{
		public SSPatternKind Pattern { get; set; }
		public int           Count   { get; set; }  // number of consecutive bars

		public override SSNodeCategory Category { get { return SSNodeCategory.Solver; } }
		public override string         TypeKey  { get { return "Pattern"; } }

		public PatternSolverNode() { Pattern = SSPatternKind.BullishBar; Count = 1; }

		protected override double OnEvaluate(BarContext ctx)
		{
			if (ctx.Host == null || ctx.Host.Bars == null) return 0.0;
			int needed = Math.Max(1, Count);
			int baseOff = ctx.Host.CurrentBar - ctx.BarIndex; // bars-ago offset of current context
			if (baseOff < 0) return 0.0;
			if (ctx.Host.CurrentBar - baseOff - needed < 1) return 0.0;   // not enough history

			for (int i = 0; i < needed; i++)
			{
				int o = baseOff + i;
				if (!MatchesPattern(ctx.Host, o)) return 0.0;
			}
			return 1.0;
		}

		private bool MatchesPattern(Indicator h, int off)
		{
			double o = h.Open[off];
			double c = h.Close[off];
			double hi = h.High[off];
			double lo = h.Low[off];
			double prevHi = h.CurrentBar - off - 1 >= 0 ? h.High[off + 1] : hi;
			double prevLo = h.CurrentBar - off - 1 >= 0 ? h.Low [off + 1] : lo;

			switch (Pattern)
			{
				case SSPatternKind.BullishBar:  return c > o;
				case SSPatternKind.BearishBar:  return c < o;
				case SSPatternKind.HigherHigh:  return hi > prevHi;
				case SSPatternKind.LowerLow:    return lo < prevLo;
				case SSPatternKind.HigherLow:   return lo > prevLo;
				case SSPatternKind.LowerHigh:   return hi < prevHi;
				case SSPatternKind.InsideBar:   return hi <= prevHi && lo >= prevLo;
				case SSPatternKind.OutsideBar:  return hi >  prevHi && lo <  prevLo;
				default:                         return false;
			}
		}

		public override Dictionary<string, object> GetConfig()
		{
			return new Dictionary<string, object> { { "pattern", Pattern.ToString() }, { "count", Count } };
		}
		public override void SetConfig(Dictionary<string, object> c)
		{
			Pattern = CfgEnum<SSPatternKind>(c, "pattern", SSPatternKind.BullishBar);
			Count   = Math.Max(1, CfgInt(c, "count", 1));
		}
	}

	public enum SSSwingKind { High, Low }

	// Swing high/low detected after Strength bars on both sides. Fires on the confirmation bar,
	// which is Strength bars after the actual pivot.
	public class SwingSolverNode : SignalStudioNodeBase
	{
		public SSSwingKind Kind     { get; set; }
		public int         Strength { get; set; }   // bars of confirmation on each side

		public override SSNodeCategory Category { get { return SSNodeCategory.Solver; } }
		public override string         TypeKey  { get { return "Swing"; } }

		public SwingSolverNode() { Kind = SSSwingKind.High; Strength = 3; }

		protected override double OnEvaluate(BarContext ctx)
		{
			if (ctx.Host == null || ctx.Host.Bars == null) return 0.0;
			int s = Math.Max(1, Strength);
			int baseOff = ctx.Host.CurrentBar - ctx.BarIndex;
			int pivotOff = baseOff + s;
			if (pivotOff + s > ctx.Host.CurrentBar) return 0.0;

			if (Kind == SSSwingKind.High)
			{
				double pivot = ctx.Host.High[pivotOff];
				for (int i = 1; i <= s; i++)
				{
					if (ctx.Host.High[pivotOff - i] >= pivot) return 0.0;
					if (ctx.Host.High[pivotOff + i] >  pivot) return 0.0;
				}
				return 1.0;
			}
			else
			{
				double pivot = ctx.Host.Low[pivotOff];
				for (int i = 1; i <= s; i++)
				{
					if (ctx.Host.Low[pivotOff - i] <= pivot) return 0.0;
					if (ctx.Host.Low[pivotOff + i] <  pivot) return 0.0;
				}
				return 1.0;
			}
		}

		public override Dictionary<string, object> GetConfig()
		{
			return new Dictionary<string, object> { { "kind", Kind.ToString() }, { "strength", Strength } };
		}
		public override void SetConfig(Dictionary<string, object> c)
		{
			Kind     = CfgEnum<SSSwingKind>(c, "kind", SSSwingKind.High);
			Strength = Math.Max(1, CfgInt(c, "strength", 3));
		}
	}

	public enum SSExpressionOp { Add, Subtract, Multiply, Divide, Modulo, Max, Min, Abs, Negate }

	// Simple 2-operand (or 1-operand for Abs/Negate) arithmetic. Port 0 = A, Port 1 = B (if used).
	public class ExpressionSolverNode : SignalStudioNodeBase
	{
		public SSExpressionOp Op { get; set; }

		public override SSNodeCategory Category { get { return SSNodeCategory.Solver; } }
		public override string         TypeKey  { get { return "Expression"; } }

		public ExpressionSolverNode() { Op = SSExpressionOp.Add; }

		protected override double OnEvaluate(BarContext ctx)
		{
			double a = EvalInput(0, ctx, 0.0);
			double b = EvalInput(1, ctx, 0.0);
			switch (Op)
			{
				case SSExpressionOp.Add:      return a + b;
				case SSExpressionOp.Subtract: return a - b;
				case SSExpressionOp.Multiply: return a * b;
				case SSExpressionOp.Divide:   return Math.Abs(b) < 1e-12 ? 0.0 : a / b;
				case SSExpressionOp.Modulo:   return Math.Abs(b) < 1e-12 ? 0.0 : a % b;
				case SSExpressionOp.Max:      return Math.Max(a, b);
				case SSExpressionOp.Min:      return Math.Min(a, b);
				case SSExpressionOp.Abs:      return Math.Abs(a);
				case SSExpressionOp.Negate:   return -a;
				default:                       return 0.0;
			}
		}

		public override Dictionary<string, object> GetConfig()
		{
			return new Dictionary<string, object> { { "op", Op.ToString() } };
		}
		public override void SetConfig(Dictionary<string, object> c)
		{
			Op = CfgEnum<SSExpressionOp>(c, "op", SSExpressionOp.Add);
		}
	}

	public enum SSChartObjectKind { ArrowUp, ArrowDown, Dot, Diamond, Square, TriangleUp, TriangleDown, Any }

	// Detects drawing objects (arrows, dots, etc.) placed on the chart by another indicator.
	// Uses DrawObjects collection. Returns 1 if a matching object is present on the evaluated bar.
	public class ChartObjectSolverNode : SignalStudioNodeBase
	{
		public SSChartObjectKind Kind          { get; set; }
		public string            TagPrefix     { get; set; }  // optional filter on tag prefix

		public override SSNodeCategory Category { get { return SSNodeCategory.Solver; } }
		public override string         TypeKey  { get { return "ChartObject"; } }

		public ChartObjectSolverNode() { Kind = SSChartObjectKind.Any; TagPrefix = ""; }

		protected override double OnEvaluate(BarContext ctx)
		{
			if (ctx.Host == null || ctx.Host.DrawObjects == null) return 0.0;
			DateTime barTime;
			int off = ctx.Host.CurrentBar - ctx.BarIndex;
			if (off < 0 || off > ctx.Host.CurrentBar) return 0.0;
			try { barTime = ctx.Host.Time[off]; } catch { return 0.0; }

			foreach (var obj in ctx.Host.DrawObjects)
			{
				if (obj == null) continue;
				string tag = obj.Tag ?? "";
				if (!string.IsNullOrEmpty(TagPrefix) && !tag.StartsWith(TagPrefix, StringComparison.OrdinalIgnoreCase))
					continue;
				if (!MatchesKind(obj)) continue;

				// Check anchor time matches the evaluated bar
				var ao = obj as NinjaTrader.NinjaScript.DrawingTools.DrawingTool;
				if (ao == null) continue;
				var anchor = TryGetAnchorTime(ao);
				if (!anchor.HasValue) continue;
				if (Math.Abs((anchor.Value - barTime).TotalSeconds) < 1.0) return 1.0;
			}
			return 0.0;
		}

		private bool MatchesKind(object o)
		{
			if (Kind == SSChartObjectKind.Any) return true;
			string tn = o.GetType().Name;
			switch (Kind)
			{
				case SSChartObjectKind.ArrowUp:      return tn.IndexOf("ArrowUp",      StringComparison.OrdinalIgnoreCase) >= 0;
				case SSChartObjectKind.ArrowDown:    return tn.IndexOf("ArrowDown",    StringComparison.OrdinalIgnoreCase) >= 0;
				case SSChartObjectKind.Dot:          return tn.IndexOf("Dot",          StringComparison.OrdinalIgnoreCase) >= 0;
				case SSChartObjectKind.Diamond:      return tn.IndexOf("Diamond",      StringComparison.OrdinalIgnoreCase) >= 0;
				case SSChartObjectKind.Square:       return tn.IndexOf("Square",       StringComparison.OrdinalIgnoreCase) >= 0;
				case SSChartObjectKind.TriangleUp:   return tn.IndexOf("TriangleUp",   StringComparison.OrdinalIgnoreCase) >= 0;
				case SSChartObjectKind.TriangleDown: return tn.IndexOf("TriangleDown", StringComparison.OrdinalIgnoreCase) >= 0;
			}
			return false;
		}

		private static DateTime? TryGetAnchorTime(object drawingTool)
		{
			try
			{
				var t = drawingTool.GetType();
				var anchorsProp = t.GetProperty("Anchors");
				if (anchorsProp == null) return null;
				var anchors = anchorsProp.GetValue(drawingTool) as IEnumerable;
				if (anchors == null) return null;
				foreach (var a in anchors)
				{
					if (a == null) continue;
					var timeProp = a.GetType().GetProperty("Time");
					if (timeProp != null) return (DateTime)timeProp.GetValue(a);
				}
			}
			catch { }
			return null;
		}

		public override Dictionary<string, object> GetConfig()
		{
			return new Dictionary<string, object> { { "kind", Kind.ToString() }, { "tagPrefix", TagPrefix ?? "" } };
		}
		public override void SetConfig(Dictionary<string, object> c)
		{
			Kind      = CfgEnum<SSChartObjectKind>(c, "kind", SSChartObjectKind.Any);
			TagPrefix = CfgStr(c, "tagPrefix", "");
		}
	}

	#endregion

	// ============================================================================================
	// 5. Logic nodes
	// ============================================================================================
	#region Logic nodes

	// All inputs are treated as boolean: a value > 0.5 is "true", otherwise "false".
	// AND short-circuits as soon as a false input is seen.
	public class AndNode : SignalStudioNodeBase
	{
		public override SSNodeCategory Category { get { return SSNodeCategory.Logic; } }
		public override string         TypeKey  { get { return "And"; } }

		protected override double OnEvaluate(BarContext ctx)
		{
			if (Inputs == null || Inputs.Count == 0) return 0.0;
			for (int i = 0; i < Inputs.Count; i++)
			{
				var n = Inputs[i].FromNode;
				if (n == null) return 0.0;
				if (n.Evaluate(ctx) <= 0.5) return 0.0;  // short-circuit
			}
			return 1.0;
		}
	}

	// OR short-circuits as soon as a true input is seen.
	public class OrNode : SignalStudioNodeBase
	{
		public override SSNodeCategory Category { get { return SSNodeCategory.Logic; } }
		public override string         TypeKey  { get { return "Or"; } }

		protected override double OnEvaluate(BarContext ctx)
		{
			if (Inputs == null || Inputs.Count == 0) return 0.0;
			for (int i = 0; i < Inputs.Count; i++)
			{
				var n = Inputs[i].FromNode;
				if (n == null) continue;
				if (n.Evaluate(ctx) > 0.5) return 1.0;  // short-circuit
			}
			return 0.0;
		}
	}

	// NOT inverts the single input at port 0.
	public class NotNode : SignalStudioNodeBase
	{
		public override SSNodeCategory Category { get { return SSNodeCategory.Logic; } }
		public override string         TypeKey  { get { return "Not"; } }

		protected override double OnEvaluate(BarContext ctx)
		{
			var n = GetInput(0);
			if (n == null) return 0.0;
			return n.Evaluate(ctx) > 0.5 ? 0.0 : 1.0;
		}
	}

	// Fires if at least N of M inputs evaluate to true (> 0.5). M is Inputs.Count.
	public class AtLeastNOfMNode : SignalStudioNodeBase
	{
		public int N { get; set; }

		public override SSNodeCategory Category { get { return SSNodeCategory.Logic; } }
		public override string         TypeKey  { get { return "AtLeastNOfM"; } }

		public AtLeastNOfMNode() { N = 2; }

		protected override double OnEvaluate(BarContext ctx)
		{
			if (Inputs == null || Inputs.Count == 0) return 0.0;
			int hit = 0;
			for (int i = 0; i < Inputs.Count; i++)
			{
				var n = Inputs[i].FromNode;
				if (n == null) continue;
				if (n.Evaluate(ctx) > 0.5)
				{
					hit++;
					if (hit >= N) return 1.0;
				}
			}
			return 0.0;
		}

		public override Dictionary<string, object> GetConfig() { return new Dictionary<string, object> { { "n", N } }; }
		public override void SetConfig(Dictionary<string, object> c) { N = Math.Max(1, CfgInt(c, "n", 2)); }
	}

	// Weighted sum of inputs. Each connection's weight is configurable (by index).
	// Output is the raw weighted sum. Attach a Comparison/Range solver downstream to produce a fire/no-fire boolean.
	public class WeightedAverageNode : SignalStudioNodeBase
	{
		public List<double> Weights { get; set; }
		public bool         Normalize { get; set; }  // divide by sum of weights actually used

		public override SSNodeCategory Category { get { return SSNodeCategory.Logic; } }
		public override string         TypeKey  { get { return "WeightedAverage"; } }

		public WeightedAverageNode() { Weights = new List<double>(); Normalize = true; }

		protected override double OnEvaluate(BarContext ctx)
		{
			if (Inputs == null || Inputs.Count == 0) return 0.0;
			double sum = 0.0;
			double wtot = 0.0;
			for (int i = 0; i < Inputs.Count; i++)
			{
				var n = Inputs[i].FromNode;
				if (n == null) continue;
				double w = (Weights != null && i < Weights.Count) ? Weights[i] : 1.0;
				sum  += w * n.Evaluate(ctx);
				wtot += Math.Abs(w);
			}
			if (Normalize && wtot > 1e-12) return sum / wtot;
			return sum;
		}

		public override Dictionary<string, object> GetConfig()
		{
			return new Dictionary<string, object>
			{
				{ "weights",   Weights },
				{ "normalize", Normalize }
			};
		}
		public override void SetConfig(Dictionary<string, object> c)
		{
			Normalize = CfgBool(c, "normalize", true);
			Weights   = new List<double>();
			object v; if (c != null && c.TryGetValue("weights", out v))
			{
				var list = v as IEnumerable;
				if (list != null)
				{
					foreach (var item in list)
					{
						try { Weights.Add(Convert.ToDouble(item, CultureInfo.InvariantCulture)); } catch { Weights.Add(1.0); }
					}
				}
			}
		}
	}

	#endregion

	// ============================================================================================
	// 5b. Filter nodes (time, day, cooldown, bars-since)
	// ============================================================================================
	#region Filter nodes

	// Passes the input through if the current bar's time is within [Start, End] window (local chart time).
	// When no input is connected, outputs 1 if in window, else 0 (useful as an AND-gate input).
	public class TimeFilterNode : SignalStudioNodeBase
	{
		public int StartHour   { get; set; }
		public int StartMinute { get; set; }
		public int EndHour     { get; set; }
		public int EndMinute   { get; set; }
		public bool Invert     { get; set; }   // true = fire OUTSIDE window

		public override SSNodeCategory Category { get { return SSNodeCategory.Filter; } }
		public override string         TypeKey  { get { return "TimeFilter"; } }

		public TimeFilterNode() { StartHour = 9; StartMinute = 30; EndHour = 16; EndMinute = 0; Invert = false; }

		protected override double OnEvaluate(BarContext ctx)
		{
			if (ctx.Host == null) return 0.0;
			int off = ctx.Host.CurrentBar - ctx.BarIndex;
			if (off < 0 || off > ctx.Host.CurrentBar) return 0.0;
			DateTime t;
			try { t = ctx.Host.Time[off]; } catch { return 0.0; }

			int cur   = t.Hour * 60 + t.Minute;
			int start = StartHour * 60 + StartMinute;
			int end   = EndHour   * 60 + EndMinute;

			bool inWindow;
			if (start <= end) inWindow = cur >= start && cur <= end;
			else              inWindow = cur >= start || cur <= end;  // wraps midnight

			bool fire = Invert ? !inWindow : inWindow;
			if (!fire) return 0.0;

			// Pass-through: if an input is wired, AND it with the window check
			var src = GetInput(0);
			return src == null ? 1.0 : (src.Evaluate(ctx) > 0.5 ? 1.0 : 0.0);
		}

		public override Dictionary<string, object> GetConfig()
		{
			return new Dictionary<string, object>
			{
				{ "startHour", StartHour }, { "startMinute", StartMinute },
				{ "endHour",   EndHour   }, { "endMinute",   EndMinute   },
				{ "invert",    Invert    }
			};
		}
		public override void SetConfig(Dictionary<string, object> c)
		{
			StartHour   = CfgInt (c, "startHour",   9);
			StartMinute = CfgInt (c, "startMinute", 30);
			EndHour     = CfgInt (c, "endHour",     16);
			EndMinute   = CfgInt (c, "endMinute",   0);
			Invert      = CfgBool(c, "invert",      false);
		}
	}

	// Day-of-week bitmask (Sun=1, Mon=2, Tue=4, Wed=8, Thu=16, Fri=32, Sat=64). Default = M-F.
	public class DayFilterNode : SignalStudioNodeBase
	{
		public int  Mask   { get; set; }
		public bool Invert { get; set; }

		public override SSNodeCategory Category { get { return SSNodeCategory.Filter; } }
		public override string         TypeKey  { get { return "DayFilter"; } }

		public DayFilterNode() { Mask = 2 | 4 | 8 | 16 | 32; Invert = false; }

		protected override double OnEvaluate(BarContext ctx)
		{
			if (ctx.Host == null) return 0.0;
			int off = ctx.Host.CurrentBar - ctx.BarIndex;
			if (off < 0 || off > ctx.Host.CurrentBar) return 0.0;
			DateTime t;
			try { t = ctx.Host.Time[off]; } catch { return 0.0; }

			int bit = 1 << (int)t.DayOfWeek;
			bool matches = (Mask & bit) != 0;
			bool fire = Invert ? !matches : matches;
			if (!fire) return 0.0;

			var src = GetInput(0);
			return src == null ? 1.0 : (src.Evaluate(ctx) > 0.5 ? 1.0 : 0.0);
		}

		public override Dictionary<string, object> GetConfig()
		{
			return new Dictionary<string, object> { { "mask", Mask }, { "invert", Invert } };
		}
		public override void SetConfig(Dictionary<string, object> c)
		{
			Mask   = CfgInt (c, "mask",   2 | 4 | 8 | 16 | 32);
			Invert = CfgBool(c, "invert", false);
		}
	}

	// Suppresses repeated firings: after input fires, blocks output for N bars.
	public class CooldownFilterNode : SignalStudioNodeBase
	{
		public int Bars { get; set; }

		[XmlIgnore] private int _lastFiredBar = int.MinValue;

		public override SSNodeCategory Category { get { return SSNodeCategory.Filter; } }
		public override string         TypeKey  { get { return "CooldownFilter"; } }

		public CooldownFilterNode() { Bars = 5; }

		public override void OnGraphLoaded(Indicator host) { _lastFiredBar = int.MinValue; }

		protected override double OnEvaluate(BarContext ctx)
		{
			double v = EvalInput(0, ctx, 0.0);
			if (v <= 0.5) return 0.0;

			// Only track and cool-down when evaluating current bar (BarsAgo=0)
			if (ctx.BarsAgo == 0)
			{
				if (_lastFiredBar != int.MinValue && ctx.CurrentBar - _lastFiredBar < Bars)
					return 0.0;
				_lastFiredBar = ctx.CurrentBar;
				return 1.0;
			}
			return v;   // lookback evaluations pass through raw
		}

		public override Dictionary<string, object> GetConfig() { return new Dictionary<string, object> { { "bars", Bars } }; }
		public override void SetConfig(Dictionary<string, object> c) { Bars = Math.Max(0, CfgInt(c, "bars", 5)); }
	}

	// Output 1 if the input has been true within the last N bars. Useful to extend a momentary signal.
	public class BarsSinceFilterNode : SignalStudioNodeBase
	{
		public int Within { get; set; }

		public override SSNodeCategory Category { get { return SSNodeCategory.Filter; } }
		public override string         TypeKey  { get { return "BarsSinceFilter"; } }

		public BarsSinceFilterNode() { Within = 3; }

		protected override double OnEvaluate(BarContext ctx)
		{
			var src = GetInput(0);
			if (src == null) return 0.0;
			int w = Math.Max(0, Within);
			for (int i = 0; i <= w; i++)
			{
				var c = ctx.WithBarsAgo(i);
				if (c.BarIndex < 0) break;
				if (src.Evaluate(c) > 0.5) return 1.0;
			}
			return 0.0;
		}

		public override Dictionary<string, object> GetConfig() { return new Dictionary<string, object> { { "within", Within } }; }
		public override void SetConfig(Dictionary<string, object> c) { Within = Math.Max(0, CfgInt(c, "within", 3)); }
	}

	#endregion

	// ============================================================================================
	// 6. Output nodes
	// ============================================================================================
	#region Output nodes

	// Long output: fires when the trigger input (port 0) is >= Threshold.
	// Confidence (optional port 1) is clamped to [0,1] and surfaced as SignalStrength.
	public class LongOutputNode : SignalStudioNodeBase
	{
		public double Threshold { get; set; }
		public bool   PaintRacingStripe { get; set; }

		public override SSNodeCategory Category { get { return SSNodeCategory.Output; } }
		public override string         TypeKey  { get { return "LongOutput"; } }

		public LongOutputNode() { Threshold = 0.5; PaintRacingStripe = true; }

		protected override double OnEvaluate(BarContext ctx)
		{
			double trig = EvalInput(0, ctx, 0.0);
			return trig >= Threshold ? 1.0 : 0.0;
		}

		public double EvaluateConfidence(BarContext ctx)
		{
			double c = EvalInput(1, ctx, 1.0);
			if (c < 0.0) c = 0.0;
			if (c > 1.0) c = 1.0;
			return c;
		}

		public override Dictionary<string, object> GetConfig()
		{
			return new Dictionary<string, object>
			{
				{ "threshold",         Threshold         },
				{ "paintRacingStripe", PaintRacingStripe }
			};
		}
		public override void SetConfig(Dictionary<string, object> c)
		{
			Threshold         = CfgDbl (c, "threshold",         0.5);
			PaintRacingStripe = CfgBool(c, "paintRacingStripe", true);
		}
	}

	public class ShortOutputNode : SignalStudioNodeBase
	{
		public double Threshold         { get; set; }
		public bool   PaintRacingStripe { get; set; }

		public override SSNodeCategory Category { get { return SSNodeCategory.Output; } }
		public override string         TypeKey  { get { return "ShortOutput"; } }

		public ShortOutputNode() { Threshold = 0.5; PaintRacingStripe = true; }

		protected override double OnEvaluate(BarContext ctx)
		{
			double trig = EvalInput(0, ctx, 0.0);
			return trig >= Threshold ? 1.0 : 0.0;
		}

		public double EvaluateConfidence(BarContext ctx)
		{
			double c = EvalInput(1, ctx, 1.0);
			if (c < 0.0) c = 0.0;
			if (c > 1.0) c = 1.0;
			return c;
		}

		public override Dictionary<string, object> GetConfig()
		{
			return new Dictionary<string, object>
			{
				{ "threshold",         Threshold         },
				{ "paintRacingStripe", PaintRacingStripe }
			};
		}
		public override void SetConfig(Dictionary<string, object> c)
		{
			Threshold         = CfgDbl (c, "threshold",         0.5);
			PaintRacingStripe = CfgBool(c, "paintRacingStripe", true);
		}
	}

	#endregion

	// ============================================================================================
	// 7. Engine
	// ============================================================================================
	#region Engine

	public class SignalStudioEngine
	{
		private readonly List<SignalStudioNodeBase> _nodes = new List<SignalStudioNodeBase>();
		private readonly Dictionary<string, SignalStudioNodeBase> _byId = new Dictionary<string, SignalStudioNodeBase>();
		private List<SignalStudioNodeBase> _topoOrder;
		private LongOutputNode  _longOut;
		private ShortOutputNode _shortOut;

		private Dictionary<string, IndicatorBase> _indicatorCache;
		private int _lastChartIndicatorCount = -1;

		public IReadOnlyList<SignalStudioNodeBase> Nodes { get { return _nodes; } }
		public string TemplateName { get; set; }

		public void Clear()
		{
			_nodes.Clear();
			_byId.Clear();
			_topoOrder = null;
			_longOut   = null;
			_shortOut  = null;
			_indicatorCache = null;
			_lastChartIndicatorCount = -1;
		}

		public void AddNode(SignalStudioNodeBase node)
		{
			if (node == null) return;
			if (string.IsNullOrEmpty(node.Id)) node.Id = Guid.NewGuid().ToString("N");
			_nodes.Add(node);
			_byId[node.Id] = node;
		}

		public void RemoveNode(SignalStudioNodeBase node)
		{
			if (node == null) return;
			_nodes.Remove(node);
			_byId.Remove(node.Id ?? "");
			foreach (var other in _nodes)
			{
				if (other.Inputs == null) continue;
				other.Inputs.RemoveAll(c => c != null && (c.FromNode == node || string.Equals(c.FromNodeId, node.Id, StringComparison.OrdinalIgnoreCase)));
			}
		}

		public SignalStudioNodeBase FindById(string id)
		{
			SignalStudioNodeBase n;
			return _byId.TryGetValue(id ?? "", out n) ? n : null;
		}

		// After all nodes are added and connections populated with FromNodeId strings,
		// resolve FromNode references, perform topological sort, and locate output nodes.
		public void BuildGraph(Indicator host)
		{
			// Resolve connection references
			for (int i = 0; i < _nodes.Count; i++)
			{
				var node = _nodes[i];
				if (node.Inputs == null) continue;
				for (int j = 0; j < node.Inputs.Count; j++)
				{
					var conn = node.Inputs[j];
					if (conn == null) continue;
					conn.FromNode = FindById(conn.FromNodeId);
				}
			}

			// Topological sort (Kahn's algorithm). Cycle edges are silently dropped.
			var indeg = new Dictionary<SignalStudioNodeBase, int>();
			foreach (var n in _nodes) indeg[n] = 0;
			foreach (var n in _nodes)
				foreach (var c in n.Inputs)
					if (c != null && c.FromNode != null && indeg.ContainsKey(n))
						indeg[n]++;

			var q = new Queue<SignalStudioNodeBase>();
			foreach (var kv in indeg) if (kv.Value == 0) q.Enqueue(kv.Key);

			var order = new List<SignalStudioNodeBase>(_nodes.Count);
			while (q.Count > 0)
			{
				var n = q.Dequeue();
				order.Add(n);

				// Decrement children (nodes that use n as an input)
				foreach (var child in _nodes)
				{
					if (child == n) continue;
					foreach (var c in child.Inputs)
					{
						if (c != null && c.FromNode == n)
						{
							indeg[child]--;
							if (indeg[child] == 0) q.Enqueue(child);
						}
					}
				}
			}

			_topoOrder = order.Count == _nodes.Count ? order : new List<SignalStudioNodeBase>(_nodes);

			// Find first long/short output nodes (most recent additions win on ties)
			_longOut  = null;
			_shortOut = null;
			for (int i = _nodes.Count - 1; i >= 0; i--)
			{
				if (_longOut  == null && _nodes[i] is LongOutputNode ) _longOut  = (LongOutputNode) _nodes[i];
				if (_shortOut == null && _nodes[i] is ShortOutputNode) _shortOut = (ShortOutputNode)_nodes[i];
				if (_longOut != null && _shortOut != null) break;
			}

			foreach (var n in _nodes)
			{
				try { n.OnGraphLoaded(host); } catch { }
			}
		}

		private void RefreshIndicatorCache(Indicator host)
		{
			if (host == null || host.ChartControl == null || host.ChartControl.Indicators == null)
			{
				if (_indicatorCache == null) _indicatorCache = new Dictionary<string, IndicatorBase>(StringComparer.OrdinalIgnoreCase);
				return;
			}

			int count = host.ChartControl.Indicators.Count;
			if (_indicatorCache != null && count == _lastChartIndicatorCount) return;

			_indicatorCache = new Dictionary<string, IndicatorBase>(StringComparer.OrdinalIgnoreCase);
			for (int i = 0; i < count; i++)
			{
				var ind = host.ChartControl.Indicators[i] as IndicatorBase;
				if (ind == null || string.IsNullOrEmpty(ind.Name)) continue;
				// First one wins if duplicates
				if (!_indicatorCache.ContainsKey(ind.Name)) _indicatorCache[ind.Name] = ind;
			}
			_lastChartIndicatorCount = count;
		}

		public SignalResult Evaluate(Indicator host, int currentBar)
		{
			if (_nodes.Count == 0 || (_longOut == null && _shortOut == null))
				return SignalResult.None;

			// Reset memo on each bar so diamond graphs re-evaluate for the new bar only once.
			foreach (var n in _nodes) n.ResetMemo();

			RefreshIndicatorCache(host);

			var ctx = new BarContext(host, currentBar) { IndicatorCache = _indicatorCache };

			double longVal  = _longOut  != null ? _longOut .Evaluate(ctx) : 0.0;
			double shortVal = _shortOut != null ? _shortOut.Evaluate(ctx) : 0.0;

			// Long takes precedence if both fire on the same bar (shouldn't happen with well-designed templates)
			int    dir  = longVal >= 0.5 ? 1 : (shortVal >= 0.5 ? -1 : 0);
			double conf = 0.0;
			if (dir == 1  && _longOut  != null) conf = _longOut .EvaluateConfidence(ctx);
			if (dir == -1 && _shortOut != null) conf = _shortOut.EvaluateConfidence(ctx);

			return new SignalResult
			{
				LongValue  = dir == 1  ? 1.0 : 0.0,
				ShortValue = dir == -1 ? -1.0 : 0.0,
				Direction  = dir,
				Confidence = conf,
				HasSignal  = dir != 0
			};
		}

		// Output nodes expose their desired racing-stripe state
		public bool LongStripeRequested  { get { return _longOut  != null && _longOut .PaintRacingStripe; } }
		public bool ShortStripeRequested { get { return _shortOut != null && _shortOut.PaintRacingStripe; } }
	}

	#endregion

	// ============================================================================================
	// 8. Template + tiny JSON
	// ============================================================================================
	#region Template serialization

	public class SignalStudioTemplate
	{
		public int      Version { get; set; }
		public string   Name    { get; set; }
		public DateTime Created { get; set; }
		public List<SSNodeData>       Nodes       { get; set; }
		public List<SSConnectionData> Connections { get; set; }

		public SignalStudioTemplate()
		{
			Version     = 1;
			Name        = "";
			Created     = DateTime.Now;
			Nodes       = new List<SSNodeData>();
			Connections = new List<SSConnectionData>();
		}
	}

	public class SSNodeData
	{
		public string Id;
		public string Type;
		public string DisplayName;
		public double X;
		public double Y;
		public Dictionary<string, object> Config;
	}

	public class SSConnectionData
	{
		public string From;
		public int    FromPort;
		public string To;
		public int    ToPort;
	}

	public static class SignalStudioNodeFactory
	{
		public static SignalStudioNodeBase Create(string typeKey)
		{
			if (string.IsNullOrEmpty(typeKey)) return null;
			switch (typeKey)
			{
				case "PriceSource":      return new PriceSourceNode();
				case "Constant":         return new ConstantNode();
				case "IndicatorPlot":    return new IndicatorPlotNode();
				case "Builtin":          return new BuiltinIndicatorNode();
				case "Crossover":        return new CrossoverSolverNode();
				case "Comparison":       return new ComparisonSolverNode();
				case "Slope":            return new SlopeSolverNode();
				case "Range":            return new RangeSolverNode();
				case "Pattern":          return new PatternSolverNode();
				case "Swing":            return new SwingSolverNode();
				case "Expression":       return new ExpressionSolverNode();
				case "ChartObject":      return new ChartObjectSolverNode();
				case "And":              return new AndNode();
				case "Or":               return new OrNode();
				case "Not":              return new NotNode();
				case "AtLeastNOfM":      return new AtLeastNOfMNode();
				case "WeightedAverage":  return new WeightedAverageNode();
				case "TimeFilter":       return new TimeFilterNode();
				case "DayFilter":        return new DayFilterNode();
				case "CooldownFilter":   return new CooldownFilterNode();
				case "BarsSinceFilter":  return new BarsSinceFilterNode();
				case "LongOutput":       return new LongOutputNode();
				case "ShortOutput":      return new ShortOutputNode();
				default:                  return null;
			}
		}
	}

	public static class SignalStudioTemplateIO
	{
		public static string Serialize(SignalStudioTemplate t)
		{
			var sb = new StringBuilder();
			sb.Append('{');
			WriteKV(sb, "version", t.Version); sb.Append(',');
			WriteKV(sb, "name",    t.Name ?? ""); sb.Append(',');
			WriteKV(sb, "created", t.Created.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)); sb.Append(',');

			sb.Append("\"nodes\":[");
			if (t.Nodes != null)
			{
				for (int i = 0; i < t.Nodes.Count; i++)
				{
					if (i > 0) sb.Append(',');
					WriteNode(sb, t.Nodes[i]);
				}
			}
			sb.Append("],");

			sb.Append("\"connections\":[");
			if (t.Connections != null)
			{
				for (int i = 0; i < t.Connections.Count; i++)
				{
					if (i > 0) sb.Append(',');
					var c = t.Connections[i];
					sb.Append('{');
					WriteKV(sb, "from",     c.From ?? "");  sb.Append(',');
					WriteKV(sb, "fromPort", c.FromPort);    sb.Append(',');
					WriteKV(sb, "to",       c.To ?? "");    sb.Append(',');
					WriteKV(sb, "toPort",   c.ToPort);
					sb.Append('}');
				}
			}
			sb.Append("]}");
			return sb.ToString();
		}

		private static void WriteNode(StringBuilder sb, SSNodeData n)
		{
			sb.Append('{');
			WriteKV(sb, "id",          n.Id ?? "");          sb.Append(',');
			WriteKV(sb, "type",        n.Type ?? "");        sb.Append(',');
			WriteKV(sb, "displayName", n.DisplayName ?? ""); sb.Append(',');
			WriteKV(sb, "x",           n.X);                 sb.Append(',');
			WriteKV(sb, "y",           n.Y);                 sb.Append(',');
			sb.Append("\"config\":"); WriteValue(sb, n.Config ?? new Dictionary<string, object>());
			sb.Append('}');
		}

		private static void WriteKV(StringBuilder sb, string k, object v)
		{
			sb.Append('"').Append(EscapeString(k)).Append("\":");
			WriteValue(sb, v);
		}

		private static void WriteValue(StringBuilder sb, object v)
		{
			if (v == null) { sb.Append("null"); return; }
			if (v is bool)   { sb.Append(((bool)v) ? "true" : "false"); return; }
			if (v is int || v is long || v is short || v is byte)
			{
				sb.Append(Convert.ToInt64(v, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture));
				return;
			}
			if (v is double || v is float || v is decimal)
			{
				double d = Convert.ToDouble(v, CultureInfo.InvariantCulture);
				if (double.IsNaN(d) || double.IsInfinity(d)) { sb.Append('0'); return; }
				sb.Append(d.ToString("R", CultureInfo.InvariantCulture));
				return;
			}
			if (v is IDictionary<string, object>)
			{
				var d = (IDictionary<string, object>)v;
				sb.Append('{');
				bool first = true;
				foreach (var kv in d)
				{
					if (!first) sb.Append(',');
					first = false;
					WriteKV(sb, kv.Key, kv.Value);
				}
				sb.Append('}');
				return;
			}
			if (v is IEnumerable && !(v is string))
			{
				sb.Append('[');
				bool first = true;
				foreach (var item in (IEnumerable)v)
				{
					if (!first) sb.Append(',');
					first = false;
					WriteValue(sb, item);
				}
				sb.Append(']');
				return;
			}
			// string fallback
			sb.Append('"').Append(EscapeString(Convert.ToString(v, CultureInfo.InvariantCulture))).Append('"');
		}

		private static string EscapeString(string s)
		{
			if (s == null) return "";
			var sb = new StringBuilder(s.Length + 2);
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				switch (c)
				{
					case '\\': sb.Append("\\\\"); break;
					case '"':  sb.Append("\\\""); break;
					case '\b': sb.Append("\\b");  break;
					case '\f': sb.Append("\\f");  break;
					case '\n': sb.Append("\\n");  break;
					case '\r': sb.Append("\\r");  break;
					case '\t': sb.Append("\\t");  break;
					default:
						if (c < 0x20) sb.AppendFormat(CultureInfo.InvariantCulture, "\\u{0:X4}", (int)c);
						else          sb.Append(c);
						break;
				}
			}
			return sb.ToString();
		}

		// ---- Parser ----

		public static SignalStudioTemplate Deserialize(string json)
		{
			if (string.IsNullOrEmpty(json)) return null;
			int pos = 0;
			SkipWS(json, ref pos);
			var obj = ParseValue(json, ref pos) as IDictionary<string, object>;
			if (obj == null) return null;

			var t = new SignalStudioTemplate
			{
				Version = ToInt(Get(obj, "version"), 1),
				Name    = ToStr(Get(obj, "name"), ""),
				Created = ToDate(Get(obj, "created"), DateTime.Now)
			};

			var nodes = Get(obj, "nodes") as IList<object>;
			if (nodes != null)
			{
				foreach (var item in nodes)
				{
					var nd = item as IDictionary<string, object>;
					if (nd == null) continue;
					var cfg = Get(nd, "config") as IDictionary<string, object>;
					var dict = cfg != null ? new Dictionary<string, object>(cfg) : new Dictionary<string, object>();
					t.Nodes.Add(new SSNodeData
					{
						Id          = ToStr(Get(nd, "id"), Guid.NewGuid().ToString("N")),
						Type        = ToStr(Get(nd, "type"), ""),
						DisplayName = ToStr(Get(nd, "displayName"), ""),
						X           = ToDbl(Get(nd, "x"), 0.0),
						Y           = ToDbl(Get(nd, "y"), 0.0),
						Config      = dict
					});
				}
			}

			var conns = Get(obj, "connections") as IList<object>;
			if (conns != null)
			{
				foreach (var item in conns)
				{
					var cd = item as IDictionary<string, object>;
					if (cd == null) continue;
					t.Connections.Add(new SSConnectionData
					{
						From     = ToStr(Get(cd, "from"), ""),
						FromPort = ToInt(Get(cd, "fromPort"), 0),
						To       = ToStr(Get(cd, "to"), ""),
						ToPort   = ToInt(Get(cd, "toPort"), 0)
					});
				}
			}

			return t;
		}

		private static object Get(IDictionary<string, object> d, string key)
		{
			object v; return d != null && d.TryGetValue(key, out v) ? v : null;
		}
		private static string   ToStr (object v, string def) { return v == null ? def : Convert.ToString(v, CultureInfo.InvariantCulture); }
		private static int      ToInt (object v, int    def) { if (v == null) return def; try { return Convert.ToInt32(Convert.ToDouble(v, CultureInfo.InvariantCulture)); } catch { return def; } }
		private static double   ToDbl (object v, double def) { if (v == null) return def; try { return Convert.ToDouble(v, CultureInfo.InvariantCulture); } catch { return def; } }
		private static DateTime ToDate(object v, DateTime def)
		{
			var s = ToStr(v, null); if (string.IsNullOrEmpty(s)) return def;
			DateTime dt;
			if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt)) return dt;
			return def;
		}

		private static object ParseValue(string s, ref int pos)
		{
			SkipWS(s, ref pos);
			if (pos >= s.Length) return null;
			char c = s[pos];
			if (c == '{')      return ParseObject(s, ref pos);
			if (c == '[')      return ParseArray (s, ref pos);
			if (c == '"')      return ParseString(s, ref pos);
			if (c == 't' || c == 'f' || c == 'n') return ParseLiteral(s, ref pos);
			return ParseNumber(s, ref pos);
		}

		private static IDictionary<string, object> ParseObject(string s, ref int pos)
		{
			var d = new Dictionary<string, object>();
			pos++; // skip {
			SkipWS(s, ref pos);
			if (pos < s.Length && s[pos] == '}') { pos++; return d; }
			while (pos < s.Length)
			{
				SkipWS(s, ref pos);
				string key = ParseString(s, ref pos);
				SkipWS(s, ref pos);
				if (pos >= s.Length || s[pos] != ':') throw new FormatException("Expected ':'");
				pos++;
				var v = ParseValue(s, ref pos);
				d[key] = v;
				SkipWS(s, ref pos);
				if (pos < s.Length && s[pos] == ',') { pos++; continue; }
				if (pos < s.Length && s[pos] == '}') { pos++; return d; }
				throw new FormatException("Expected ',' or '}'");
			}
			throw new FormatException("Unterminated object");
		}

		private static IList<object> ParseArray(string s, ref int pos)
		{
			var list = new List<object>();
			pos++; // skip [
			SkipWS(s, ref pos);
			if (pos < s.Length && s[pos] == ']') { pos++; return list; }
			while (pos < s.Length)
			{
				list.Add(ParseValue(s, ref pos));
				SkipWS(s, ref pos);
				if (pos < s.Length && s[pos] == ',') { pos++; continue; }
				if (pos < s.Length && s[pos] == ']') { pos++; return list; }
				throw new FormatException("Expected ',' or ']'");
			}
			throw new FormatException("Unterminated array");
		}

		private static string ParseString(string s, ref int pos)
		{
			if (pos >= s.Length || s[pos] != '"') throw new FormatException("Expected string");
			pos++;
			var sb = new StringBuilder();
			while (pos < s.Length)
			{
				char c = s[pos++];
				if (c == '"') return sb.ToString();
				if (c == '\\' && pos < s.Length)
				{
					char esc = s[pos++];
					switch (esc)
					{
						case '"':  sb.Append('"');  break;
						case '\\': sb.Append('\\'); break;
						case '/':  sb.Append('/');  break;
						case 'b':  sb.Append('\b'); break;
						case 'f':  sb.Append('\f'); break;
						case 'n':  sb.Append('\n'); break;
						case 'r':  sb.Append('\r'); break;
						case 't':  sb.Append('\t'); break;
						case 'u':
							if (pos + 4 <= s.Length)
							{
								int code = int.Parse(s.Substring(pos, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
								sb.Append((char)code);
								pos += 4;
							}
							break;
						default: sb.Append(esc); break;
					}
				}
				else sb.Append(c);
			}
			throw new FormatException("Unterminated string");
		}

		private static object ParseLiteral(string s, ref int pos)
		{
			if (pos + 4 <= s.Length && s.Substring(pos, 4) == "true")  { pos += 4; return true; }
			if (pos + 5 <= s.Length && s.Substring(pos, 5) == "false") { pos += 5; return false; }
			if (pos + 4 <= s.Length && s.Substring(pos, 4) == "null")  { pos += 4; return null; }
			throw new FormatException("Invalid literal at " + pos);
		}

		private static object ParseNumber(string s, ref int pos)
		{
			int start = pos;
			if (pos < s.Length && (s[pos] == '-' || s[pos] == '+')) pos++;
			while (pos < s.Length && (char.IsDigit(s[pos]) || s[pos] == '.' || s[pos] == 'e' || s[pos] == 'E' || s[pos] == '+' || s[pos] == '-'))
				pos++;
			string tok = s.Substring(start, pos - start);
			double d;
			if (double.TryParse(tok, NumberStyles.Float, CultureInfo.InvariantCulture, out d))
			{
				// Return int if it's a whole number without decimal/exponent chars
				if (tok.IndexOf('.') < 0 && tok.IndexOf('e') < 0 && tok.IndexOf('E') < 0 && d == Math.Truncate(d) && d >= int.MinValue && d <= int.MaxValue)
					return (int)d;
				return d;
			}
			throw new FormatException("Invalid number '" + tok + "'");
		}

		private static void SkipWS(string s, ref int pos)
		{
			while (pos < s.Length && char.IsWhiteSpace(s[pos])) pos++;
		}
	}

	// Bridges a SignalStudioTemplate (the on-disk/serialized form) to a live SignalStudioEngine.
	public static class SignalStudioTemplateBinder
	{
		public static void Apply(SignalStudioTemplate t, SignalStudioEngine engine, Indicator host)
		{
			if (engine == null) return;
			engine.Clear();
			if (t == null) return;

			engine.TemplateName = t.Name;

			foreach (var nd in t.Nodes)
			{
				var node = SignalStudioNodeFactory.Create(nd.Type);
				if (node == null) continue;
				node.Id          = nd.Id;
				node.DisplayName = nd.DisplayName;
				node.X           = nd.X;
				node.Y           = nd.Y;
				try { node.SetConfig(nd.Config); } catch { }
				engine.AddNode(node);
			}

			foreach (var cd in t.Connections)
			{
				var target = engine.FindById(cd.To);
				if (target == null) continue;
				target.Inputs.Add(new NodeConnection
				{
					FromNodeId = cd.From,
					FromPort   = cd.FromPort,
					ToPort     = cd.ToPort
				});
			}

			engine.BuildGraph(host);
		}

		// Builds the default "EMA(9) crossing EMA(21)" template used when no file is present.
		// Long  = EMA(9) crosses above EMA(21)
		// Short = EMA(9) crosses below EMA(21)
		public static SignalStudioTemplate BuildDefaultEmaCrossover()
		{
			var t = new SignalStudioTemplate { Version = 1, Name = "Default: EMA(9) x EMA(21)", Created = DateTime.Now };

			var fast = new SSNodeData
			{
				Id = "ema-fast", Type = "Builtin", DisplayName = "EMA(9)", X = 40, Y = 40,
				Config = new Dictionary<string, object>
				{
					{ "kind", "EMA" }, { "period", 9 }, { "source", "Close" }, { "barsAgo", 0 }
				}
			};
			var slow = new SSNodeData
			{
				Id = "ema-slow", Type = "Builtin", DisplayName = "EMA(21)", X = 40, Y = 120,
				Config = new Dictionary<string, object>
				{
					{ "kind", "EMA" }, { "period", 21 }, { "source", "Close" }, { "barsAgo", 0 }
				}
			};
			var crossUp = new SSNodeData
			{
				Id = "cross-up", Type = "Crossover", DisplayName = "Fast crosses above Slow", X = 260, Y = 40,
				Config = new Dictionary<string, object> { { "direction", "Above" } }
			};
			var crossDn = new SSNodeData
			{
				Id = "cross-dn", Type = "Crossover", DisplayName = "Fast crosses below Slow", X = 260, Y = 160,
				Config = new Dictionary<string, object> { { "direction", "Below" } }
			};
			var longOut = new SSNodeData
			{
				Id = "long-out",  Type = "LongOutput",  DisplayName = "Long",  X = 500, Y = 40,
				Config = new Dictionary<string, object> { { "threshold", 0.5 }, { "paintRacingStripe", true } }
			};
			var shortOut = new SSNodeData
			{
				Id = "short-out", Type = "ShortOutput", DisplayName = "Short", X = 500, Y = 160,
				Config = new Dictionary<string, object> { { "threshold", 0.5 }, { "paintRacingStripe", true } }
			};

			t.Nodes.AddRange(new [] { fast, slow, crossUp, crossDn, longOut, shortOut });

			t.Connections.AddRange(new []
			{
				new SSConnectionData { From = "ema-fast", FromPort = 0, To = "cross-up", ToPort = 0 },
				new SSConnectionData { From = "ema-slow", FromPort = 0, To = "cross-up", ToPort = 1 },
				new SSConnectionData { From = "ema-fast", FromPort = 0, To = "cross-dn", ToPort = 0 },
				new SSConnectionData { From = "ema-slow", FromPort = 0, To = "cross-dn", ToPort = 1 },
				new SSConnectionData { From = "cross-up", FromPort = 0, To = "long-out",  ToPort = 0 },
				new SSConnectionData { From = "cross-dn", FromPort = 0, To = "short-out", ToPort = 0 }
			});

			return t;
		}
	}

	#endregion

	// ============================================================================================
	// 9. Indicator
	// ============================================================================================

	public partial class aiSignalStudio : Indicator
	{
		private SignalStudioEngine  _engine;
		private string              _resolvedTemplatePath;
		private bool                _defaultTemplateUsed;
		private int                 _lastBarSignaled = int.MinValue;

		// Brushes for racing stripes (built lazily)
		private System.Windows.Media.Brush _longStripeBrush;
		private System.Windows.Media.Brush _shortStripeBrush;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description                     = @"Signal Studio - visual signal designer. Phase 1: core engine, plot-based integration with Enhanced Chart Trader.";
				Name                            = "aiSignalStudio";
				Calculate                       = Calculate.OnBarClose;
				IsOverlay                       = true;
				IsAutoScale                     = false;  // our 0/1/-1 plot values must not pull the Y-axis
				DisplayInDataBox                = false;
				DrawOnPricePanel                = true;   // we draw arrows in OnRender
				DrawHorizontalGridLines         = false;
				DrawVerticalGridLines           = false;
				PaintPriceMarkers               = false;
				ScaleJustification              = NinjaTrader.Gui.Chart.ScaleJustification.Right;
				IsSuspendedWhileInactive        = true;

				// Outputs - values are read by ECT's GetPlotSignals(). Brushes are Transparent so
				// plots don't render any visible line, and AutoScale is turned off in Configure
				// so the 0/1/-1 values don't distort the chart's Y-axis.
				AddPlot(new Stroke(System.Windows.Media.Brushes.Transparent, 1), PlotStyle.Dot, "LongSignal");
				AddPlot(new Stroke(System.Windows.Media.Brushes.Transparent, 1), PlotStyle.Dot, "ShortSignal");
				AddPlot(new Stroke(System.Windows.Media.Brushes.Transparent, 1), PlotStyle.Dot, "SignalDirection");
				AddPlot(new Stroke(System.Windows.Media.Brushes.Transparent, 1), PlotStyle.Dot, "SignalStrength");
				AddPlot(new Stroke(System.Windows.Media.Brushes.Transparent, 1), PlotStyle.Dot, "SignalBar");

				pTemplatePath                   = "";
				pUseDefaultTemplate             = true;
				pEnableEvaluation               = true;
				pShowArrows                     = true;
				pArrowSize                      = 8;
				pArrowOffsetTicks               = 6;
				pPaintRacingStripes             = false;  // off by default now that arrows exist
				pLongStripeColor                = System.Windows.Media.Colors.Lime;
				pShortStripeColor               = System.Windows.Media.Colors.Red;
				pRacingStripeOpacity            = 18;
			}
			else if (State == State.Configure)
			{
				_engine = new SignalStudioEngine();
			}
			else if (State == State.DataLoaded)
			{
				_signalHistory.Clear();
				LoadTemplate();
				RebuildStripeBrushes();
			}
			else if (State == State.Realtime)
			{
				PublishSharedRegistry();
			}
			else if (State == State.Terminated)
			{
				UnpublishSharedRegistry();
				DisposeDxResources();
				_engine = null;
				_longStripeBrush  = null;
				_shortStripeBrush = null;
			}
		}

		// =============================================================
		// Chart rendering (arrows at signal bars, via SharpDX - no DrawingTools)
		// =============================================================

		private struct SignalMark { public int Bar; public int Direction; }
		private readonly List<SignalMark> _signalHistory = new List<SignalMark>(1024);
		private const int SignalHistoryCap = 4096;

		private SharpDX.Direct2D1.SolidColorBrush _dxLongBrush;
		private SharpDX.Direct2D1.SolidColorBrush _dxShortBrush;
		private SharpDX.Direct2D1.SolidColorBrush _dxOutlineBrush;
		private SharpDX.Direct2D1.PathGeometry    _dxUpArrow;
		private SharpDX.Direct2D1.PathGeometry    _dxDownArrow;
		private float _dxArrowSize = 8f;

		public override void OnRenderTargetChanged()
		{
			DisposeDxResources();
			if (RenderTarget == null) return;

			try
			{
				_dxLongBrush    = new SharpDX.Direct2D1.SolidColorBrush(RenderTarget, ToDx(pLongStripeColor));
				_dxShortBrush   = new SharpDX.Direct2D1.SolidColorBrush(RenderTarget, ToDx(pShortStripeColor));
				_dxOutlineBrush = new SharpDX.Direct2D1.SolidColorBrush(RenderTarget, new SharpDX.Color4(0, 0, 0, 0.85f));

				_dxArrowSize = Math.Max(3f, Math.Min(24f, (float)pArrowSize));
				_dxUpArrow   = BuildArrowGeometry(RenderTarget.Factory, true,  _dxArrowSize);
				_dxDownArrow = BuildArrowGeometry(RenderTarget.Factory, false, _dxArrowSize);
			}
			catch (Exception ex) { Print("[aiSignalStudio] DX init failed: " + ex.Message); }
		}

		private void DisposeDxResources()
		{
			if (_dxLongBrush    != null) { _dxLongBrush   .Dispose(); _dxLongBrush    = null; }
			if (_dxShortBrush   != null) { _dxShortBrush  .Dispose(); _dxShortBrush   = null; }
			if (_dxOutlineBrush != null) { _dxOutlineBrush.Dispose(); _dxOutlineBrush = null; }
			if (_dxUpArrow      != null) { _dxUpArrow     .Dispose(); _dxUpArrow      = null; }
			if (_dxDownArrow    != null) { _dxDownArrow   .Dispose(); _dxDownArrow    = null; }
		}

		private static SharpDX.Direct2D1.PathGeometry BuildArrowGeometry(SharpDX.Direct2D1.Factory f, bool up, float size)
		{
			// Filled isoceles triangle, tip at (0,0). For "up" the tip points up and the base is
			// below; for "down" it's inverted.
			float w = size * 0.75f;
			float h = size * 1.3f;

			var g = new SharpDX.Direct2D1.PathGeometry(f);
			var sink = g.Open();
			try
			{
				if (up)
				{
					sink.BeginFigure(new SharpDX.Vector2(0, 0),       SharpDX.Direct2D1.FigureBegin.Filled);
					sink.AddLine   (new SharpDX.Vector2(-w, h));
					sink.AddLine   (new SharpDX.Vector2( w, h));
				}
				else
				{
					sink.BeginFigure(new SharpDX.Vector2(0, 0),       SharpDX.Direct2D1.FigureBegin.Filled);
					sink.AddLine   (new SharpDX.Vector2(-w, -h));
					sink.AddLine   (new SharpDX.Vector2( w, -h));
				}
				sink.EndFigure(SharpDX.Direct2D1.FigureEnd.Closed);
				sink.Close();
			}
			finally { sink.Dispose(); }
			return g;
		}

		private static SharpDX.Color4 ToDx(System.Windows.Media.Color c)
		{
			return new SharpDX.Color4(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);
		}

		protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
		{
			if (!pShowArrows) return;
			if (chartControl == null || chartScale == null || ChartBars == null || Bars == null) return;
			if (RenderTarget == null) return;
			if (_dxLongBrush == null || _dxShortBrush == null || _dxUpArrow == null || _dxDownArrow == null) return;
			if (_signalHistory.Count == 0) return;

			int firstIdx = ChartBars.FromIndex;
			int lastIdx  = ChartBars.ToIndex;
			double tick  = TickSize;
			double offsetPrice = pArrowOffsetTicks * tick;

			var savedTransform = RenderTarget.Transform;
			try
			{
				for (int i = _signalHistory.Count - 1; i >= 0; i--)
				{
					var sm = _signalHistory[i];
					if (sm.Bar < firstIdx) break;        // history is in ascending order, so stop once we go before the visible range
					if (sm.Bar > lastIdx)  continue;

					int barsAgo = CurrentBar - sm.Bar;
					if (barsAgo < 0 || barsAgo >= Close.Count) continue;

					float x;
					try   { x = chartControl.GetXByBarIndex(ChartBars, sm.Bar); }
					catch { continue; }

					double priceAnchor = sm.Direction == 1 ? Low[barsAgo] - offsetPrice : High[barsAgo] + offsetPrice;
					float  y;
					try   { y = chartScale.GetYByValue(priceAnchor); }
					catch { continue; }

					RenderTarget.Transform = new SharpDX.Matrix3x2(1, 0, 0, 1, x, y);
					var geom  = sm.Direction == 1 ? _dxUpArrow    : _dxDownArrow;
					var brush = sm.Direction == 1 ? _dxLongBrush  : _dxShortBrush;
					RenderTarget.FillGeometry(geom, brush);
					RenderTarget.DrawGeometry(geom, _dxOutlineBrush, 1f);
				}
			}
			finally { RenderTarget.Transform = savedTransform; }
		}

		private void RecordSignal(int bar, int direction)
		{
			// Replace if this bar was already recorded (e.g. evaluation re-runs)
			if (_signalHistory.Count > 0 && _signalHistory[_signalHistory.Count - 1].Bar == bar)
			{
				_signalHistory[_signalHistory.Count - 1] = new SignalMark { Bar = bar, Direction = direction };
				return;
			}
			_signalHistory.Add(new SignalMark { Bar = bar, Direction = direction });
			if (_signalHistory.Count > SignalHistoryCap)
				_signalHistory.RemoveRange(0, _signalHistory.Count - SignalHistoryCap);
		}

		// =============================================================
		// Phase 4: AppDomain-shared registry
		// Mirrors DAA's pattern (aiADACopierRegistryV1) so ECT or other
		// indicators can query signal state without touching plots.
		// Slot layout:
		//   [0] Func<int>    CurrentDirection   (1 long, -1 short, 0 none)
		//   [1] Func<double> CurrentConfidence  (0.0-1.0)
		//   [2] Func<string> ActiveTemplateName
		//   [3] Func<string> FiredNodesCsv      (active output node DisplayNames)
		//   [4] Func<bool>   EvaluationEnabled
		//   [5] Action<bool> SetEvaluationEnabled
		// =============================================================

		private const string RegistryKey = "aiSignalStudioRegistryV1";
		[XmlIgnore] private System.Collections.Concurrent.ConcurrentDictionary<int, object[]> _registry;
		[XmlIgnore] private int _registrySlotId = -1;

		private void PublishSharedRegistry()
		{
			try
			{
				var dom = AppDomain.CurrentDomain;
				object existing = dom.GetData(RegistryKey);
				_registry = existing as System.Collections.Concurrent.ConcurrentDictionary<int, object[]>;
				if (_registry == null)
				{
					_registry = new System.Collections.Concurrent.ConcurrentDictionary<int, object[]>();
					dom.SetData(RegistryKey, _registry);
				}

				_registrySlotId = GetHashCode();

				object[] slot = new object[6];
				slot[0] = new Func<int>   (() => _lastDirection);
				slot[1] = new Func<double>(() => _lastConfidence);
				slot[2] = new Func<string>(() => (_engine != null && !string.IsNullOrEmpty(_engine.TemplateName)) ? _engine.TemplateName : (_defaultTemplateUsed ? "(default)" : ""));
				slot[3] = new Func<string>(() => _lastFiredCsv ?? "");
				slot[4] = new Func<bool>  (() => pEnableEvaluation);
				slot[5] = new Action<bool>(v  => { pEnableEvaluation = v; });

				_registry[_registrySlotId] = slot;
			}
			catch (Exception ex) { Print("[aiSignalStudio] registry publish failed: " + ex.Message); }
		}

		private void UnpublishSharedRegistry()
		{
			try
			{
				if (_registry != null && _registrySlotId != -1)
				{
					object[] removed;
					_registry.TryRemove(_registrySlotId, out removed);
				}
			}
			catch { }
			_registry = null;
			_registrySlotId = -1;
		}

		[XmlIgnore] private int    _lastDirection;
		[XmlIgnore] private double _lastConfidence;
		[XmlIgnore] private string _lastFiredCsv = "";

		private void LoadTemplate()
		{
			if (_engine == null) _engine = new SignalStudioEngine();
			if (_signalHistory != null) _signalHistory.Clear();
			SignalStudioTemplate t = null;
			_resolvedTemplatePath = null;
			_defaultTemplateUsed  = false;

			try
			{
				string path = (pTemplatePath ?? "").Trim();
				if (!string.IsNullOrEmpty(path) && File.Exists(path))
				{
					_resolvedTemplatePath = path;
					string json = File.ReadAllText(path);
					t = SignalStudioTemplateIO.Deserialize(json);
				}
			}
			catch (Exception ex)
			{
				Print("[aiSignalStudio] template load failed: " + ex.Message);
				t = null;
			}

			if (t == null && pUseDefaultTemplate)
			{
				t = SignalStudioTemplateBinder.BuildDefaultEmaCrossover();
				_defaultTemplateUsed = true;
			}

			SignalStudioTemplateBinder.Apply(t, _engine, this);
		}

		private void RebuildStripeBrushes()
		{
			_longStripeBrush  = new System.Windows.Media.SolidColorBrush(pLongStripeColor)  { Opacity = Math.Max(0, Math.Min(100, pRacingStripeOpacity)) / 100.0 };
			_shortStripeBrush = new System.Windows.Media.SolidColorBrush(pShortStripeColor) { Opacity = Math.Max(0, Math.Min(100, pRacingStripeOpacity)) / 100.0 };
			if (_longStripeBrush .CanFreeze) _longStripeBrush .Freeze();
			if (_shortStripeBrush.CanFreeze) _shortStripeBrush.Freeze();
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < 2) return;

			// Clear previous bar's plots every bar so signals don't bleed forward
			Values[0][0] = 0.0;
			Values[1][0] = 0.0;
			Values[2][0] = 0.0;
			Values[3][0] = 0.0;
			Values[4][0] = 0.0;

			if (!pEnableEvaluation || _engine == null) return;

			SignalResult r;
			try { r = _engine.Evaluate(this, CurrentBar); }
			catch (Exception ex) { Print("[aiSignalStudio] eval error: " + ex.Message); return; }

			Values[0][0] = r.LongValue;    // 0 or 1
			Values[1][0] = r.ShortValue;   // 0 or -1
			Values[2][0] = r.Direction;    // 1 / -1 / 0
			Values[3][0] = r.Confidence;   // 0.0 - 1.0
			Values[4][0] = r.HasSignal ? 1 : 0;

			_lastDirection  = r.Direction;
			_lastConfidence = r.Confidence;
			_lastFiredCsv   = r.HasSignal ? BuildFiredCsv(r.Direction) : "";

			if (r.HasSignal) RecordSignal(CurrentBar, r.Direction);

			if (r.HasSignal && pPaintRacingStripes)
			{
				if (r.Direction == 1  && _engine.LongStripeRequested)
					BackBrush = _longStripeBrush;
				else if (r.Direction == -1 && _engine.ShortStripeRequested)
					BackBrush = _shortStripeBrush;

				if (CurrentBar != _lastBarSignaled)
				{
					_lastBarSignaled = CurrentBar;
					string tag = _defaultTemplateUsed ? " (default)" : "";
					Print(string.Format("[aiSignalStudio{0}] bar={1} dir={2} conf={3:F2}", tag, CurrentBar, r.Direction, r.Confidence));
				}
			}
		}

		private string BuildFiredCsv(int direction)
		{
			if (_engine == null) return "";
			var sb = new StringBuilder();
			foreach (var n in _engine.Nodes)
			{
				if (direction ==  1 && n is LongOutputNode  && n.LastOutput >= 0.5) Append(sb, n.DisplayName ?? "Long");
				if (direction == -1 && n is ShortOutputNode && n.LastOutput >= 0.5) Append(sb, n.DisplayName ?? "Short");
			}
			return sb.ToString();
		}
		private static void Append(StringBuilder sb, string s)
		{
			if (string.IsNullOrEmpty(s)) return;
			if (sb.Length > 0) sb.Append(',');
			sb.Append(s);
		}

		// =============================================================
		// Public plot accessors - let strategies read the outputs by name
		// =============================================================

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> LongSignal       { get { return Values[0]; } }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> ShortSignal      { get { return Values[1]; } }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> SignalDirection  { get { return Values[2]; } }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> SignalStrength   { get { return Values[3]; } }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> SignalBar        { get { return Values[4]; } }

		// =============================================================
		// Public-facing properties
		// =============================================================

		[Display(Name = "Template path",       Order = 10, GroupName = "Signal Studio")]
		public string pTemplatePath           { get; set; }

		[Display(Name = "Use default template if none loaded", Order = 20, GroupName = "Signal Studio")]
		public bool   pUseDefaultTemplate     { get; set; }

		[Display(Name = "Enable evaluation",   Order = 30, GroupName = "Signal Studio")]
		public bool   pEnableEvaluation       { get; set; }

		[Display(Name = "Show signal arrows",  Order = 35, GroupName = "Signal Studio")]
		public bool   pShowArrows             { get; set; }

		[Range(3, 24)]
		[Display(Name = "Arrow size (px)",     Order = 36, GroupName = "Signal Studio")]
		public int    pArrowSize              { get; set; }

		[Range(0, 100)]
		[Display(Name = "Arrow offset (ticks)", Order = 37, GroupName = "Signal Studio")]
		public int    pArrowOffsetTicks       { get; set; }

		[Display(Name = "Paint racing stripes", Order = 40, GroupName = "Signal Studio")]
		public bool   pPaintRacingStripes     { get; set; }

		[XmlIgnore]
		[Display(Name = "Long stripe color",   Order = 50, GroupName = "Signal Studio")]
		public System.Windows.Media.Color pLongStripeColor { get; set; }

		[Browsable(false)]
		public string pLongStripeColorSerialize
		{
			get { return Serialize.BrushToString(new System.Windows.Media.SolidColorBrush(pLongStripeColor)); }
			set { pLongStripeColor = ((System.Windows.Media.SolidColorBrush)Serialize.StringToBrush(value)).Color; }
		}

		[XmlIgnore]
		[Display(Name = "Short stripe color",  Order = 60, GroupName = "Signal Studio")]
		public System.Windows.Media.Color pShortStripeColor { get; set; }

		[Browsable(false)]
		public string pShortStripeColorSerialize
		{
			get { return Serialize.BrushToString(new System.Windows.Media.SolidColorBrush(pShortStripeColor)); }
			set { pShortStripeColor = ((System.Windows.Media.SolidColorBrush)Serialize.StringToBrush(value)).Color; }
		}

		[Range(1, 100)]
		[Display(Name = "Racing stripe opacity (%)", Order = 70, GroupName = "Signal Studio")]
		public int    pRacingStripeOpacity    { get; set; }

		// Setting this true opens the Signal Studio window and immediately resets itself to false
		// so the checkbox can be toggled again on subsequent invocations.
		private bool _pOpenStudio;
		[Display(Name = "Open Signal Studio window", Order = 80, GroupName = "Signal Studio")]
		public bool   pOpenStudio
		{
			get { return _pOpenStudio; }
			set
			{
				_pOpenStudio = false;
				if (value) OpenStudioWindow();
			}
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private aiSignalStudio[] cacheaiSignalStudio;
		public aiSignalStudio aiSignalStudio()
		{
			return aiSignalStudio(Input);
		}

		public aiSignalStudio aiSignalStudio(ISeries<double> input)
		{
			if (cacheaiSignalStudio != null)
				for (int idx = 0; idx < cacheaiSignalStudio.Length; idx++)
					if (cacheaiSignalStudio[idx] != null &&  cacheaiSignalStudio[idx].EqualsInput(input))
						return cacheaiSignalStudio[idx];
			return CacheIndicator<aiSignalStudio>(new aiSignalStudio(), input, ref cacheaiSignalStudio);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.aiSignalStudio aiSignalStudio()
		{
			return indicator.aiSignalStudio(Input);
		}

		public Indicators.aiSignalStudio aiSignalStudio(ISeries<double> input )
		{
			return indicator.aiSignalStudio(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.aiSignalStudio aiSignalStudio()
		{
			return indicator.aiSignalStudio(Input);
		}

		public Indicators.aiSignalStudio aiSignalStudio(ISeries<double> input )
		{
			return indicator.aiSignalStudio(input);
		}
	}
}

#endregion
