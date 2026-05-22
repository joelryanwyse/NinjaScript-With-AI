//
// Copyright (C) 2026, Affordable Indicators, Inc.
// aiSignalStudio - 8 starter templates (Phase 2 deliverable).
//
// Each factory builds a SignalStudioTemplate that can be handed directly to
// SignalStudioTemplateBinder.Apply(...) or written to disk via SignalStudioTemplateIO.Serialize(...).
//

using System;
using System.Collections.Generic;

namespace NinjaTrader.NinjaScript.Indicators
{
	public static class SignalStudioStarterTemplates
	{
		public struct Entry
		{
			public string Key;
			public string DisplayName;
			public string Description;
			public Func<SignalStudioTemplate> Factory;
		}

		public static readonly List<Entry> All = new List<Entry>
		{
			new Entry { Key = "ema-crossover",          DisplayName = "EMA(9) x EMA(21)",              Description = "Fast EMA crossing slow EMA. The canonical trend-follow starter.",               Factory = BuildEmaCrossover },
			new Entry { Key = "ema-cross-rsi-filter",   DisplayName = "EMA cross with RSI filter",     Description = "EMA crossover only fires when RSI confirms momentum direction.",                 Factory = BuildEmaCrossWithRsi },
			new Entry { Key = "macd-signal-cross",      DisplayName = "MACD histogram zero cross",     Description = "Momentum turning - fires when the MACD histogram proxy crosses zero.",          Factory = BuildMacdHistogramCross },
			new Entry { Key = "bollinger-breakout",     DisplayName = "Bollinger breakout",            Description = "Price closes beyond Bollinger bands (approximated via StdDev).",                 Factory = BuildBollingerBreakout },
			new Entry { Key = "rsi-extremes",           DisplayName = "RSI extremes reversal",         Description = "Long when RSI is oversold and turning up; short when overbought and turning down.", Factory = BuildRsiExtremes },
			new Entry { Key = "atr-breakout",           DisplayName = "ATR breakout",                  Description = "Price moves N*ATR beyond prior close - volatility-scaled breakout.",             Factory = BuildAtrBreakout },
			new Entry { Key = "swing-pullback",         DisplayName = "Swing pullback trigger",        Description = "Enter on a confirmed swing low (long) or swing high (short) during trend.",      Factory = BuildSwingPullback },
			new Entry { Key = "three-bar-momentum",     DisplayName = "Three bar momentum",            Description = "Three consecutive bullish (or bearish) bars during RTH, with cooldown.",         Factory = BuildThreeBarMomentum }
		};

		// =============================================================================

		private static SSNodeData N(string id, string type, string display, double x, double y, Dictionary<string, object> cfg)
		{
			return new SSNodeData { Id = id, Type = type, DisplayName = display, X = x, Y = y, Config = cfg ?? new Dictionary<string, object>() };
		}

		private static SSConnectionData C(string from, int fp, string to, int tp)
		{
			return new SSConnectionData { From = from, FromPort = fp, To = to, ToPort = tp };
		}

		private static Dictionary<string, object> D() { return new Dictionary<string, object>(); }

		// --- 1. EMA crossover ---
		public static SignalStudioTemplate BuildEmaCrossover()
		{
			return SignalStudioTemplateBinder.BuildDefaultEmaCrossover();   // delegate to the Phase 1 default
		}

		// --- 2. EMA crossover with RSI confirmation ---
		public static SignalStudioTemplate BuildEmaCrossWithRsi()
		{
			var t = new SignalStudioTemplate { Name = "EMA cross with RSI filter", Created = DateTime.Now };
			t.Nodes.Add(N("fast",    "Builtin",    "EMA(9)",  40,   40, new Dictionary<string, object> { { "kind", "EMA" }, { "period", 9  }, { "source", "Close" } }));
			t.Nodes.Add(N("slow",    "Builtin",    "EMA(21)", 40,  120, new Dictionary<string, object> { { "kind", "EMA" }, { "period", 21 }, { "source", "Close" } }));
			t.Nodes.Add(N("rsi",     "Builtin",    "RSI(14)", 40,  220, new Dictionary<string, object> { { "kind", "RSI" }, { "period", 14 }, { "source", "Close" } }));
			t.Nodes.Add(N("c50",     "Constant",   "50",       40,  300, new Dictionary<string, object> { { "value", 50.0 } }));
			t.Nodes.Add(N("cross_u", "Crossover",  "Fast>Slow", 280,  40, new Dictionary<string, object> { { "direction", "Above" } }));
			t.Nodes.Add(N("cross_d", "Crossover",  "Fast<Slow", 280, 160, new Dictionary<string, object> { { "direction", "Below" } }));
			t.Nodes.Add(N("rsi_up",  "Comparison", "RSI>50",    280, 260, new Dictionary<string, object> { { "op", "Greater" } }));
			t.Nodes.Add(N("rsi_dn",  "Comparison", "RSI<50",    280, 340, new Dictionary<string, object> { { "op", "Less" } }));
			t.Nodes.Add(N("and_l",   "And",        "Long gate",   520,  80, D()));
			t.Nodes.Add(N("and_s",   "And",        "Short gate",  520, 220, D()));
			t.Nodes.Add(N("long",    "LongOutput", "Long",        760,  80, new Dictionary<string, object> { { "threshold", 0.5 }, { "paintRacingStripe", true } }));
			t.Nodes.Add(N("short",   "ShortOutput","Short",       760, 220, new Dictionary<string, object> { { "threshold", 0.5 }, { "paintRacingStripe", true } }));

			t.Connections.Add(C("fast",    0, "cross_u", 0));
			t.Connections.Add(C("slow",    0, "cross_u", 1));
			t.Connections.Add(C("fast",    0, "cross_d", 0));
			t.Connections.Add(C("slow",    0, "cross_d", 1));
			t.Connections.Add(C("rsi",     0, "rsi_up",  0));
			t.Connections.Add(C("c50",     0, "rsi_up",  1));
			t.Connections.Add(C("rsi",     0, "rsi_dn",  0));
			t.Connections.Add(C("c50",     0, "rsi_dn",  1));
			t.Connections.Add(C("cross_u", 0, "and_l",   0));
			t.Connections.Add(C("rsi_up",  0, "and_l",   1));
			t.Connections.Add(C("cross_d", 0, "and_s",   0));
			t.Connections.Add(C("rsi_dn",  0, "and_s",   1));
			t.Connections.Add(C("and_l",   0, "long",    0));
			t.Connections.Add(C("and_s",   0, "short",   0));
			return t;
		}

		// --- 3. MACD-style histogram zero cross (approximated via EMA(12)-EMA(26) crossing zero) ---
		public static SignalStudioTemplate BuildMacdHistogramCross()
		{
			var t = new SignalStudioTemplate { Name = "MACD histogram zero cross", Created = DateTime.Now };
			t.Nodes.Add(N("fast",   "Builtin",     "EMA(12)", 40,   40, new Dictionary<string, object> { { "kind", "EMA" }, { "period", 12 }, { "source", "Close" } }));
			t.Nodes.Add(N("slow",   "Builtin",     "EMA(26)", 40,  120, new Dictionary<string, object> { { "kind", "EMA" }, { "period", 26 }, { "source", "Close" } }));
			t.Nodes.Add(N("hist",   "Expression",  "F - S",    240,   80, new Dictionary<string, object> { { "op", "Subtract" } }));
			t.Nodes.Add(N("zero",   "Constant",    "0",        240, 180, new Dictionary<string, object> { { "value", 0.0 } }));
			t.Nodes.Add(N("up",     "Crossover",   "hist>0",   460,   40, new Dictionary<string, object> { { "direction", "Above" } }));
			t.Nodes.Add(N("dn",     "Crossover",   "hist<0",   460, 160, new Dictionary<string, object> { { "direction", "Below" } }));
			t.Nodes.Add(N("long",   "LongOutput",  "Long",     700,   40, new Dictionary<string, object> { { "threshold", 0.5 } }));
			t.Nodes.Add(N("short",  "ShortOutput", "Short",    700, 160, new Dictionary<string, object> { { "threshold", 0.5 } }));

			t.Connections.Add(C("fast", 0, "hist", 0));
			t.Connections.Add(C("slow", 0, "hist", 1));
			t.Connections.Add(C("hist", 0, "up",   0));
			t.Connections.Add(C("zero", 0, "up",   1));
			t.Connections.Add(C("hist", 0, "dn",   0));
			t.Connections.Add(C("zero", 0, "dn",   1));
			t.Connections.Add(C("up",   0, "long", 0));
			t.Connections.Add(C("dn",   0, "short",0));
			return t;
		}

		// --- 4. Bollinger breakout (Close vs SMA +/- N*StdDev) ---
		public static SignalStudioTemplate BuildBollingerBreakout()
		{
			var t = new SignalStudioTemplate { Name = "Bollinger breakout", Created = DateTime.Now };
			t.Nodes.Add(N("close",  "PriceSource", "Close",    40,  40, new Dictionary<string, object> { { "source", "Close" } }));
			t.Nodes.Add(N("sma",    "Builtin",     "SMA(20)",  40, 120, new Dictionary<string, object> { { "kind", "SMA" },    { "period", 20 } }));
			t.Nodes.Add(N("sd",     "Builtin",     "StdDev(20)", 40, 200, new Dictionary<string, object> { { "kind", "StdDev" }, { "period", 20 } }));
			t.Nodes.Add(N("k",      "Constant",    "2",         40, 280, new Dictionary<string, object> { { "value", 2.0 } }));
			t.Nodes.Add(N("kstd",   "Expression",  "k*sd",     260, 240, new Dictionary<string, object> { { "op", "Multiply" } }));
			t.Nodes.Add(N("upper",  "Expression",  "sma+kstd", 460, 180, new Dictionary<string, object> { { "op", "Add" } }));
			t.Nodes.Add(N("lower",  "Expression",  "sma-kstd", 460, 280, new Dictionary<string, object> { { "op", "Subtract" } }));
			t.Nodes.Add(N("gt",     "Comparison",  "C>upper",  680,  80, new Dictionary<string, object> { { "op", "Greater" } }));
			t.Nodes.Add(N("lt",     "Comparison",  "C<lower",  680, 300, new Dictionary<string, object> { { "op", "Less" } }));
			t.Nodes.Add(N("long",   "LongOutput",  "Long",     880,  80, new Dictionary<string, object> { { "threshold", 0.5 } }));
			t.Nodes.Add(N("short",  "ShortOutput", "Short",    880, 300, new Dictionary<string, object> { { "threshold", 0.5 } }));

			t.Connections.Add(C("k",     0, "kstd",  0));
			t.Connections.Add(C("sd",    0, "kstd",  1));
			t.Connections.Add(C("sma",   0, "upper", 0));
			t.Connections.Add(C("kstd",  0, "upper", 1));
			t.Connections.Add(C("sma",   0, "lower", 0));
			t.Connections.Add(C("kstd",  0, "lower", 1));
			t.Connections.Add(C("close", 0, "gt",    0));
			t.Connections.Add(C("upper", 0, "gt",    1));
			t.Connections.Add(C("close", 0, "lt",    0));
			t.Connections.Add(C("lower", 0, "lt",    1));
			t.Connections.Add(C("gt",    0, "long",  0));
			t.Connections.Add(C("lt",    0, "short", 0));
			return t;
		}

		// --- 5. RSI extremes reversal ---
		public static SignalStudioTemplate BuildRsiExtremes()
		{
			var t = new SignalStudioTemplate { Name = "RSI extremes reversal", Created = DateTime.Now };
			t.Nodes.Add(N("rsi",    "Builtin",    "RSI(14)", 40,  80, new Dictionary<string, object> { { "kind", "RSI" }, { "period", 14 } }));
			t.Nodes.Add(N("os",     "Comparison", "RSI<30",   260,  40, new Dictionary<string, object> { { "op", "Less" } }));
			t.Nodes.Add(N("os_v",   "Constant",   "30",       260, 120, new Dictionary<string, object> { { "value", 30.0 } }));
			t.Nodes.Add(N("ob",     "Comparison", "RSI>70",   260, 220, new Dictionary<string, object> { { "op", "Greater" } }));
			t.Nodes.Add(N("ob_v",   "Constant",   "70",       260, 300, new Dictionary<string, object> { { "value", 70.0 } }));
			t.Nodes.Add(N("up",     "Slope",      "RSI rising",  460,  80, new Dictionary<string, object> { { "period", 2 }, { "direction", "Rising" } }));
			t.Nodes.Add(N("dn",     "Slope",      "RSI falling", 460, 260, new Dictionary<string, object> { { "period", 2 }, { "direction", "Falling" } }));
			t.Nodes.Add(N("and_l",  "And",        "Long gate",   680,  80, D()));
			t.Nodes.Add(N("and_s",  "And",        "Short gate",  680, 260, D()));
			t.Nodes.Add(N("long",   "LongOutput", "Long",        880,  80, new Dictionary<string, object> { { "threshold", 0.5 } }));
			t.Nodes.Add(N("short",  "ShortOutput","Short",       880, 260, new Dictionary<string, object> { { "threshold", 0.5 } }));

			t.Connections.Add(C("rsi",   0, "os", 0));
			t.Connections.Add(C("os_v",  0, "os", 1));
			t.Connections.Add(C("rsi",   0, "ob", 0));
			t.Connections.Add(C("ob_v",  0, "ob", 1));
			t.Connections.Add(C("rsi",   0, "up", 0));
			t.Connections.Add(C("rsi",   0, "dn", 0));
			t.Connections.Add(C("os",    0, "and_l", 0));
			t.Connections.Add(C("up",    0, "and_l", 1));
			t.Connections.Add(C("ob",    0, "and_s", 0));
			t.Connections.Add(C("dn",    0, "and_s", 1));
			t.Connections.Add(C("and_l", 0, "long",  0));
			t.Connections.Add(C("and_s", 0, "short", 0));
			return t;
		}

		// --- 6. ATR breakout ---
		public static SignalStudioTemplate BuildAtrBreakout()
		{
			var t = new SignalStudioTemplate { Name = "ATR breakout", Created = DateTime.Now };
			t.Nodes.Add(N("close",  "PriceSource", "Close",     40,   40, new Dictionary<string, object> { { "source", "Close" } }));
			t.Nodes.Add(N("pclose", "PriceSource", "Close[1]",  40,  120, new Dictionary<string, object> { { "source", "Close" }, { "barsAgo", 1 } }));
			t.Nodes.Add(N("atr",    "Builtin",     "ATR(14)",    40,  200, new Dictionary<string, object> { { "kind", "ATR" }, { "period", 14 } }));
			t.Nodes.Add(N("k",      "Constant",    "1.5",        40,  280, new Dictionary<string, object> { { "value", 1.5 } }));
			t.Nodes.Add(N("katr",   "Expression",  "k*atr",     260,  240, new Dictionary<string, object> { { "op", "Multiply" } }));
			t.Nodes.Add(N("up_t",   "Expression",  "pc+katr",   460,  160, new Dictionary<string, object> { { "op", "Add" } }));
			t.Nodes.Add(N("dn_t",   "Expression",  "pc-katr",   460,  260, new Dictionary<string, object> { { "op", "Subtract" } }));
			t.Nodes.Add(N("gt",     "Comparison",  "c>up_t",    680,   60, new Dictionary<string, object> { { "op", "Greater" } }));
			t.Nodes.Add(N("lt",     "Comparison",  "c<dn_t",    680,  280, new Dictionary<string, object> { { "op", "Less" } }));
			t.Nodes.Add(N("long",   "LongOutput",  "Long",      880,   60, new Dictionary<string, object> { { "threshold", 0.5 } }));
			t.Nodes.Add(N("short",  "ShortOutput", "Short",     880,  280, new Dictionary<string, object> { { "threshold", 0.5 } }));

			t.Connections.Add(C("k",     0, "katr", 0));
			t.Connections.Add(C("atr",   0, "katr", 1));
			t.Connections.Add(C("pclose",0, "up_t", 0));
			t.Connections.Add(C("katr",  0, "up_t", 1));
			t.Connections.Add(C("pclose",0, "dn_t", 0));
			t.Connections.Add(C("katr",  0, "dn_t", 1));
			t.Connections.Add(C("close", 0, "gt",   0));
			t.Connections.Add(C("up_t",  0, "gt",   1));
			t.Connections.Add(C("close", 0, "lt",   0));
			t.Connections.Add(C("dn_t",  0, "lt",   1));
			t.Connections.Add(C("gt",    0, "long", 0));
			t.Connections.Add(C("lt",    0, "short",0));
			return t;
		}

		// --- 7. Swing pullback ---
		public static SignalStudioTemplate BuildSwingPullback()
		{
			var t = new SignalStudioTemplate { Name = "Swing pullback trigger", Created = DateTime.Now };
			t.Nodes.Add(N("close",   "PriceSource", "Close",    40,   40, new Dictionary<string, object> { { "source", "Close" } }));
			t.Nodes.Add(N("ema200",  "Builtin",     "EMA(200)", 40,  120, new Dictionary<string, object> { { "kind", "EMA" }, { "period", 200 } }));
			t.Nodes.Add(N("above",   "Comparison",  "C>EMA200", 260,   40, new Dictionary<string, object> { { "op", "Greater" } }));
			t.Nodes.Add(N("below",   "Comparison",  "C<EMA200", 260,  160, new Dictionary<string, object> { { "op", "Less" } }));
			t.Nodes.Add(N("swlo",    "Swing",       "Swing low",    260,  260, new Dictionary<string, object> { { "kind", "Low"  }, { "strength", 3 } }));
			t.Nodes.Add(N("swhi",    "Swing",       "Swing high",   260,  340, new Dictionary<string, object> { { "kind", "High" }, { "strength", 3 } }));
			t.Nodes.Add(N("and_l",   "And",         "Long gate",  500,   80, D()));
			t.Nodes.Add(N("and_s",   "And",         "Short gate", 500,  240, D()));
			t.Nodes.Add(N("long",    "LongOutput",  "Long",        720,   80, new Dictionary<string, object> { { "threshold", 0.5 } }));
			t.Nodes.Add(N("short",   "ShortOutput", "Short",       720,  240, new Dictionary<string, object> { { "threshold", 0.5 } }));

			t.Connections.Add(C("close",  0, "above", 0));
			t.Connections.Add(C("ema200", 0, "above", 1));
			t.Connections.Add(C("close",  0, "below", 0));
			t.Connections.Add(C("ema200", 0, "below", 1));
			t.Connections.Add(C("above",  0, "and_l", 0));
			t.Connections.Add(C("swlo",   0, "and_l", 1));
			t.Connections.Add(C("below",  0, "and_s", 0));
			t.Connections.Add(C("swhi",   0, "and_s", 1));
			t.Connections.Add(C("and_l",  0, "long",  0));
			t.Connections.Add(C("and_s",  0, "short", 0));
			return t;
		}

		// --- 8. Three-bar momentum with RTH + cooldown ---
		public static SignalStudioTemplate BuildThreeBarMomentum()
		{
			var t = new SignalStudioTemplate { Name = "Three bar momentum", Created = DateTime.Now };
			t.Nodes.Add(N("bull3",  "Pattern",       "3 bullish bars",  40,   40, new Dictionary<string, object> { { "pattern", "BullishBar" }, { "count", 3 } }));
			t.Nodes.Add(N("bear3",  "Pattern",       "3 bearish bars",  40,  140, new Dictionary<string, object> { { "pattern", "BearishBar" }, { "count", 3 } }));
			t.Nodes.Add(N("rth",    "TimeFilter",    "RTH 09:30-16:00", 40,  240, new Dictionary<string, object> { { "startHour", 9 }, { "startMinute", 30 }, { "endHour", 16 }, { "endMinute", 0 } }));
			t.Nodes.Add(N("wk",     "DayFilter",     "Weekdays",        40,  340, new Dictionary<string, object> { { "mask", 2 | 4 | 8 | 16 | 32 } }));
			t.Nodes.Add(N("and_l",  "And",           "Long gate",      280,   60, D()));
			t.Nodes.Add(N("and_s",  "And",           "Short gate",     280,  160, D()));
			t.Nodes.Add(N("cool_l", "CooldownFilter","Cooldown L",     500,   60, new Dictionary<string, object> { { "bars", 10 } }));
			t.Nodes.Add(N("cool_s", "CooldownFilter","Cooldown S",     500,  160, new Dictionary<string, object> { { "bars", 10 } }));
			t.Nodes.Add(N("long",   "LongOutput",    "Long",           720,   60, new Dictionary<string, object> { { "threshold", 0.5 } }));
			t.Nodes.Add(N("short",  "ShortOutput",   "Short",          720,  160, new Dictionary<string, object> { { "threshold", 0.5 } }));

			t.Connections.Add(C("bull3",  0, "and_l", 0));
			t.Connections.Add(C("rth",    0, "and_l", 1));
			t.Connections.Add(C("wk",     0, "and_l", 2));
			t.Connections.Add(C("bear3",  0, "and_s", 0));
			t.Connections.Add(C("rth",    0, "and_s", 1));
			t.Connections.Add(C("wk",     0, "and_s", 2));
			t.Connections.Add(C("and_l",  0, "cool_l", 0));
			t.Connections.Add(C("and_s",  0, "cool_s", 0));
			t.Connections.Add(C("cool_l", 0, "long",   0));
			t.Connections.Add(C("cool_s", 0, "short",  0));
			return t;
		}
	}
}
