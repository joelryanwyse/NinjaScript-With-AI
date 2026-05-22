---
name: ect-followerexit-lag-bug
description: "Active NT 8.1.7.0 bug — copier follower-account exits lag ~2s each when ECT is charted on the followers' traded instrument. Root cause confirmed; exact hot code still being pinpointed."
metadata: 
  node_type: memory
  type: project
  originSessionId: 04da950a-a366-40fe-b361-687cbcdf09b8
---

Closing/flattening positions across the copier's follower accounts (aiDuplicateAccountActions copies a master to ~10 followers) is extremely slow — each follower fill lands ~2s after the previous (≈16-20s total) — but ONLY when the Enhanced Chart Trader (aiEnhancedChartTrader) is open AND its chart is on the same instrument the followers trade. Appeared with NinjaTrader 8.1.7.0.

**Why:** Confirmed 2026-05-20 by an A/B chart-template test the user ran (`templates/Chart/lagect-haslag.xml` charted on MNQ = followers' instrument → lag; `lagect-nolag.xml` on NQ → no lag). The user verified the lag follows the chart instrument. This is blocking — the user wants ECT usable on the followers' instrument.

**How to apply:**
- Ruled out: ECT's `OnOrderUpdate2`/`OnExecutionUpdate2`/`OnPositionUpdate2` Cbi handlers — instrumented, all run <3ms.
- ECT uses `Calculate.OnEachTick` (aiEnhancedChartTrader.cs:5426) so `OnBarUpdate` (~6,200-line method) runs every tick of the chart instrument. Working hypothesis: ECT's per-tick processing contends with NT 8.1.7.0's fill processing for that same instrument.
- Diagnostic instrumentation in place (logs `PERF` lines to ECT's own log `Affordable Indicators\Enhanced Chart Trader\log\ECT_<date>.log`): handler ENTER/EXIT timing, plus `BarUpdate_GAP`/`MktData_GAP`/`Render_GAP` gap timers on the per-tick/per-frame hot paths.
- Workaround the user can use now: chart ECT on a different instrument than the followers trade.
- Possibly related to [[ect-render-perf-regression]] (separate symptom, also ECT per-frame/per-tick cost).
