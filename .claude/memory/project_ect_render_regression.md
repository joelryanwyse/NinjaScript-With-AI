---
name: ect-render-perf-regression
description: New ECT release has a UI-freeze regression during multi-order actions; rendering-efficiency work on aiEnhancedChartTrader.cs is driven by this.
metadata: 
  node_type: memory
  type: project
  originSessionId: b019aecc-1316-4ead-b8c9-c1a9cade9ff6
---

The newer Enhanced Chart Trader release (v26.x) has a UI-freeze / "squishy"
performance regression versus the prior version. The chart locks up ~3-5s
during multi-order actions (Move All Exits, BE+, partialing) once roughly 5+
orders are stacked; the older version (last-known-good for the customer:
AIEnhancedChartTrader_NT8_25.12.16.1) felt sub-frame and event-driven.
NinjaTrader's trace shows `D2DERR_WRONG_STATE` "Chart rendering failed" raised
from an `OnRender` method. Surfaced by customer Benjamin Zamora ("Ben", Discord
handle Zinovate, zinovate@gmail.com).

**Investigation state (2026-05-22, version now 26.5.21.12):** Several fixes
shipped this session:
- Thread-safety: `_accountStateLock` + per-frame snapshots of `LinkedOrders`/
  `MovingOrders`/`SelectedOrders`/`RRIsOnOrders`; `OnOrderUpdate2` mutations
  locked. (Concurrent `OnOrderUpdate2` on 20+ threadpool threads is real and
  confirmed in the log, but was NOT the full root cause.)
- DEADLOCK regression + fix (26.5.21.7): the thread-safety fix above
  introduced a lock-order inversion — `OnRender` held `_accountStateLock`
  across `_renderOrdersSnapshotBuild.AddRange(myAccount.Orders)`, which
  enumerates NT's own collection (needs NT's internal Orders lock), while an
  NT account thread held that Orders lock and waited on `_accountStateLock`
  inside `OnOrderUpdate2`. Symptom: ~5s of TOTAL ECT-log silence after a
  burst of multi-order clicks (e.g. 5 rapid "Move All Exits"), `Render_GAP`
  27-41s. Fixed by moving the `myAccount.Orders` snapshot (Clear/AddRange/
  swap) OUT of the lock — those `_render*` snapshot fields are render-thread-
  only, so they need no lock. The lock now spans only pure in-process `List`
  copies of the ECT-owned collections. RULE: never hold `_accountStateLock`
  across a NinjaTrader API call.
- `SetOrderFlags` clamp: `Values[currentplot]` plot-array write was unbounded
  → "Index was outside the bounds of the array" in the TriggerCustomEvent
  callback; clamped with `if (currentplot < Values.Length)`. This fixed the
  "key not present" / array-overflow custom-event errors.
- Diagnostics added: `RenderTest` (called at 25 points in OnRender) now does a
  `Flush()` D2D-state probe + `SlowSpan` timing; `[RENDER]` phase catches log
  full stack traces.

**Current understanding of the remaining freeze:** a SILENT Direct2D
corruption in the MOVING-ORDER render path, triggered by clicking or moving an
order (which sets `MovingOrder`). It surfaces only inside NinjaTrader's own `EndDraw` AFTER OnRender
returns (as `D2DERR_WRONG_STATE` + cascade `D2DERR_PUSH_POP_UNBALANCED`), so it
is INVISIBLE to in-render `Flush()` probes — they never trip. The clip stack
(`PushClipDX`/`PopClipDX`, `_clipPushDepth`) is tracked and balanced. A
separate `[RENDER] OrderDisplay | Index out of range` (a `List<T>` indexer
overflow in DrawOrders) is real but gets caught by the phase try/catch and
recovers — not the freeze itself.

**Done since:** defensive hardening pass on the render path (rect-sanitizing
draw wrappers `FillOrderRect`/`DrawOrderRect`/`DrawButtonBorder` via
`RectOK`/`BadRect`; unguarded `SortedDictionary` indexers → `TryGetValue`;
`SetOrderFlags` plot-array clamp; plot-19/20 render-path `TriggerCustomEvent`
anti-pattern removed). The user directed skipping the 25.12 diff ("we've made
a ton of improvements to rendering otherwise"). Probes ruled out degenerate
rects (`BadRectSkipped` never fired).

- Deadlock fix CONFIRMED effective (26.5.21.7): the freeze changed from ~5s
  of silent ECT-log silence to a normal `D2DERR_WRONG_STATE` + cascade
  `D2DERR_PUSH_POP_UNBALANCED` trace — i.e. the lock-order inversion is gone
  and the bug is back to the original silent Direct2D corruption.
- NaN-coordinate hardening (26.5.21.8): the leading theory is that an order
  whose price field is momentarily non-finite during the order storm (or a
  degenerate chart scale) yields a NaN rect; a NaN coord handed to a void/
  batched D2D draw call draws nothing (matches the user seeing one sell-limit
  flag vanish) and corrupts the render target, surfacing only at NT's
  `EndDraw`. Guards added: `TotalTheOrders` skips orders with `BadF` price
  fields; `DrawOrders` bails the frame on a degenerate `chartScale`. New
  diag loglines `[RENDER] ChartScaleDegenerate` confirm/deny if they fire.

- 26.5.21.8 still froze. After ~8 versions of guess-and-ship hardening the
  user pushed back; agreed static analysis had hit diminishing returns and
  switched to EMPIRICAL ISOLATION via the new NinjaScript input
  `RenderDiagMode` (int, default 0, group "Position Flag Features").
- Checkpoint-bisection (.9/.10) FAILED structurally. The idea: stop OnRender
  at one of its 25 `RenderTest` checkpoints. The catch-22: this bug's trigger
  (clicking an order / Move All Exits) is itself DRAWN by the order-rendering
  code, so you can never skip that code and still have something to click.
  LESSON: bisection-by-skipping cannot isolate a bug whose trigger lives in
  the skipped code.
- Narrowing achieved before the catch-22 was hit: freeze is triggered by
  CLICKING/MOVING an order (sets `MovingOrder`); reproduces at checkpoint 18,
  so `DrawMoveOrders` (called at line ~73289, after cp18) is RULED OUT; the
  moving-order-reactive code ≤ cp18 mostly just recomputes `CurrentMousePrice`.
  Conclusion: the corruption is in the MOVING-ORDER render path.
- 26.5.21.11: `RenderDiagMode == 1` now makes OnRender neutralize the
  order-moving state for the whole frame (saves+nulls `MovingOrder`/
  `MovingOrders`, clears `_renderMovingOrdersSnapshot` inside the snapshot
  lock block, restores at frame end). Full UI still draws — no catch-22.
  Test: set to 1, click an order — freeze stops ⇒ CONFIRMED the moving-order
  render path; persists ⇒ it is not.

- 26.5.21.11 moving-state test: froze anyway — but AMBIGUOUS: the diagnostic
  only neutralizes the moving state while an order is actively held; the
  freeze may land later, during the post-release order churn.
- DIFF FINDING (breakthrough): diffed the render code against the user's
  "Before Claude" file (`C:\Users\joelw\OneDrive\Desktop\AI - Before\`). That
  last-known-good source contains NO `PushAxisAlignedClip` anywhere — it does
  zero axis-aligned clipping. The panel clip (`PushClipDX`/`PopClipDX`/
  `_clipPushDepth`), the `_accountStateLock` snapshots, and
  `cachedPanelBackdropBrushDX` are ALL new in the 26.x line.
  `D2DERR_PUSH_POP_UNBALANCED` is literally a clip push/pop imbalance —
  impossible without clip pushes — so the new panel clip is strongly
  implicated. LESSON: for a regression, diff the last-known-good source; the
  user's early "too much changed to be useful" was backwards.
- 26.5.21.12: panel clip DISABLED via `_clipDisabled = true` (`PushClipDX`/
  `PopClipDX` early-return), reverting to the last-known-good no-clip behavior.

- 26.5.21.12 (panel clip disabled) STILL froze ⇒ panel clip RULED OUT. So
  `D2DERR_PUSH_POP_UNBALANCED` is a downstream cascade of `D2DERR_WRONG_STATE`,
  not the cause.
- New symptom (user, on .12): during Move All Exits, target flags briefly
  draw side-by-side as if sharing one price though their prices differ —
  order positions collapse during the change-storm. Sometimes coincides with
  the freeze, sometimes not.

- Diff done (Before vs current, ~51k lines). User CONFIRMED the "Before"
  file IS the version that didn't freeze. Three systems are new in 26.x and
  absent from it: (1) the label-band anti-overlap positioning
  (`GetOverlapShift`/`RecordLabelBand`/`OccupiedLabelBands` — this IS the
  "side by side" mechanism the user sees); (2) the SharpDX brush caching
  (~100 cached brushes, dispose/recreate); (3) the thread-safety order
  snapshots. The regression is in one of those three.
- 26.5.21.13: hardened `GetOverlapShift`/`RecordLabelBand` against non-finite
  coords — `Math.Min(x, NaN) == NaN` would poison a flag's X into NaN, and a
  NaN coordinate silently corrupts D2D. Panel clip re-enabled
  (`_clipDisabled=false`) since .12 ruled it out. Honest framing given to the
  user: this is a real fix for a real fragility but a "shot," not a certain
  root-cause fix.

- 26.5.21.13 result: freeze "happens less often but still happening" — strong
  signal the bad-geometry theory is right (each hardened path reduces it) and
  that more than one path feeds it.
- 26.5.21.14: COMPREHENSIVE draw-call hardening. All 527 `RenderTarget.DrawXxx/
  FillXxx` call sites routed through 10 `Safe*` wrapper methods (defined just
  before `OnRender`, ~line 65090) that drop a draw with non-finite/absurd
  geometry (`BadF`) or a null/disposed brush before it reaches D2D. Done via
  file-wide `replace_all` of `RenderTarget.X(` → `SafeX(`, then the wrappers
  added AFTER (so their own real `RenderTarget.X(` calls aren't re-replaced).
  Wrappers use optional params to cover all overloads. New diag logline
  `[RENDER] SafeDrawSkipped` (capped 40) names any dropped draw.
  Behaviour-identical for valid input — no feature/display change.

- 26.5.21.14 result (read from ECT_2026-05-22.log): `SafeDrawSkipped` NEVER
  fired — every one of the 527 wrapped draws got valid geometry + a valid
  brush, yet it still froze. DECISIVELY rules out bad geometry and null/
  disposed brushes. Only render diagnostic in the whole log: `SlowSpan 17->18`
  (DrawOrders 50-300ms). No OnRender exception. Freeze rides a massive order
  storm (OOU2 on 10+ threads). ⇒ leading theory is now a THREADING race.
- Threading scan: `OnExecutionUpdate2` (background thread) → `RunExecutions`
  rebuilds `AllTrades`/`AllLongEntries`/`AllShortEntries` — collections
  OnRender reads — un-synchronized. Real bug, but the repro is order *changes*
  not fills, so maybe not THE one. The threading exposure is systemic (26.x
  protected only 4 order collections).
- 26.5.21.15: off-thread Direct2D TRIPWIRE. New `D2DThreadCheck(where)` is
  called by all 10 `Safe*` draw wrappers + `GetATMColorBrush`; OnRender stamps
  `_renderThreadId` each frame. If any D2D op runs on a non-render thread it
  logs `[RENDER] OffThreadD2D` with the call name + a full stack trace.
  `OnRenderTargetChanged` logs its own thread too.

- 26.5.21.15 result (ECT_2026-05-22.log): `OffThreadD2D` NEVER fired ⇒
  off-thread Direct2D access RULED OUT. BUT — the breakthrough — `[RENDER]
  OnRenderTargetChanged` fires every ~2 seconds (in pairs) in the 12s before
  the freeze. The chart's Direct2D render target is being RECREATED
  constantly — abnormal; a chart RT is normally stable for hours. Each
  recreation invalidates ECT's ~100 cached D2D brushes ⇒ constant stale-
  resource churn ⇒ D2DERR_WRONG_STATE. Explains the user's partially-drawn
  order flags (corruption mid-way through an order's flag).
- 26.5.21.16: (a) `OnRenderTargetChanged` logs a stack trace (first 12 calls,
  `_ortcCount`) to reveal WHY NinjaTrader recreates the RT. (b)
  `OnRenderTargetChanged` now recreates ALL deferred cached brushes
  synchronously (`CreatePropertyBrushes`/`CreateOrderBrushes`/
  `CreateAdditionalBrushes`/`CreateMarketDepthBrushes`/`CreateTriangleBrushes`)
  instead of only setting `*NeedRefresh` flags — closes the stale-brush window.

- 26.5.21.16 BROKE loading: calling `CreatePropertyBrushes()` from
  `OnRenderTargetChanged` threw `NullReferenceException` in `GetOutlineColor`
  — `OnRenderTargetChanged` fires during indicator load (from NT's
  `IndicatorRenderBase.set_RenderTarget`) BEFORE ECT state like
  `ThisChartBackground` is initialized. The `*NeedRefresh` flag-deferral
  exists precisely to avoid this. LESSON: never call `Create*Brushes()`
  outside the OnRender refresh path.
- 26.5.21.17: reverted the .16 immediate-recreate (loads again). KEPT the
  `OnRenderTargetChanged` stack-trace logging (first 12 calls, `_ortcCount`).
- ROOT CAUSE (read from `OnRenderTargetChanged` stack traces in
  ECT_2026-05-22.log): the freeze is RE-ENTRANT `OnRender`. The count=4 trace
  proves `ChartControl.InvalidateVisual()` is NOT an async invalidate — NT
  chains it SYNCHRONOUSLY: `InvalidateVisual()` -> `UpdateFormDataBoxCrossHair()`
  -> `Control.Update()` -> `OnPaint` -> `RenderToTarget` -> `OnRender`. ECT
  calls `ChartControl.InvalidateVisual()` from INSIDE `OnRender` (panel-hide
  transition) and `ChartPanel.InvalidateVisual()` (margin change). When the
  panel-hide path runs mid-frame it re-enters `OnRender` on the single-
  threaded, non-re-entrant Direct2D render target -> clip/draw-state
  corruption -> `D2DERR_WRONG_STATE` + `D2DERR_PUSH_POP_UNBALANCED` at
  `EndDraw`. The panel-accordion code is new in 26.x => matches the regression.
  Note: RT recreation per se (HitTest, bar-update repaints) is NORMAL NT
  behavior and was a red herring; the bug is the synchronous re-entrant
  repaint, not RT-thrash.
- 26.5.21.18: added `DeferRenderInvalidate()` helper (posts the invalidate via
  `ChartControl.Dispatcher.InvokeAsync` so it runs AFTER the current frame's
  `EndDraw`). Both in-`OnRender` invalidate sites now call it instead of a
  synchronous `InvalidateVisual()`. LESSON: never call
  `ChartControl.InvalidateVisual()` / `ForceRefresh()` from inside `OnRender`.
- 26.5.21.18 STILL FROZE (verified `v=26.5.21.18` in SessionStart; froze 16s
  after load, during a 30-market-order stress spam + repeated Move All Exits).
  So the panel-hide re-entrancy was not the (whole) cause. The freeze is STILL
  a silent D2D corruption: NO ECT diagnostic fires at the freeze — no managed
  exception, no `SafeDrawSkipped`, no `OffThreadD2D`, no `ClipImbalance`, no
  `SlowSpan`. `_clipPushDepth` is balanced every frame (`ClipImbalance` never
  logged); the `panelClipActive` clip is dead code (declared, never set true).
- 26.5.21.19: DECISIVE re-entrancy tripwire. `OnRender` is now a thin wrapper
  around the renamed `OnRenderImpl`; it `Interlocked.Increment`s a
  `_onRenderDepth` field and, if depth > 1, logs `[RENDER] ReentrantOnRender`
  + full stack trace and SKIPS the nested frame (returns). This both detects
  AND mitigates re-entrancy. Also trimmed log noise (user request, file was
  growing fast): dropped `OOU2_ENTER`/`OEU2_ENTER`, gated `OOU2_EXIT`/
  `OEU2_EXIT` to slow-only (`>5ms`, renamed `*_SLOW`), raised GAP thresholds
  (MktData/BarUpdate 250->2000ms, Render 500->1500ms), RT-change stack-trace
  cap 12->4.

- 26.5.21.19 result: `ReentrantOnRender` NEVER fired => RE-ENTRANCY IS
  DEFINITIVELY RULED OUT. `OnRender` is never re-entered. The .18 panel-hide
  re-entrancy theory is dead.
- DEATH-SPIRAL DISCOVERY (.19 freeze log, instance #4 14:30:05-18): the freeze
  is a self-sustaining render-target recreation LOOP. `OnRenderTargetChanged`
  fires in pairs ~6/sec, count climbing 8 -> 81+, with NOTHING else logged,
  until the log dies. Mechanism: ECT corrupts the RT -> NT's EndDraw fails ->
  NT recreates the RT -> ECT corrupts the fresh one -> repeat forever. The
  chart is "frozen" to the user but internally spinning. KEY DEDUCTION: in the
  spiral the RT is fresh AND all 5 deferred brush categories are rebuilt every
  frame (OnRenderTargetChanged sets the flags, OnRender's refresh recreates
  them) => the corruption is NOT stale brushes. Obvious per-frame leaks are
  already fixed (triangle `PathGeometry` `using` blocks ~line 71008;
  `tempBottomBrush` disposed in a `finally` ~line 74995). Root cause of what
  corrupts a fresh RT every frame: still unknown.
- 26.5.21.20: render-target death-spiral CIRCUIT BREAKER. `OnRenderTargetChanged`
  counts calls in a rolling 4s window; >=16 trips `_renderCircuitOpen`. While
  open, the `OnRender` wrapper renders NOTHING (returns early) and holds the
  circuit open until 2s after the last RT change, then resumes. Logs
  `[RENDER] CircuitOpen` / `CircuitClosed`. This converts the permanent freeze
  into a ~2s self-recovering glitch AND is the decisive diagnostic.

- 26.5.21.20 result: froze again; `CircuitOpen` NEVER fired. Cause: the .20
  detector (>=16 RT-changes in a 4s window) was calibrated for the .19 spiral
  rate (~6/sec); the .20 freeze (instance #2, 14:46:23-33) spiraled at only
  ~2/sec (count 20->41 over 10s, pairs ~1.1s apart) => only ~8 in 4s, never hit
  16. The spiral rate varies with chart/market activity, so a rate-based
  threshold is unreliable.
- 26.5.21.21: rate-INDEPENDENT spiral detector. New signal: count real renders
  between render-target recreations (`_onRenderSinceOrtc`, bumped per
  `OnRenderImpl`). A healthy chart renders many frames between recreations; a
  spiral renders <=2. A run of >=6 consecutive "tight" recreations
  (`_tightOrtcRun`) trips the circuit. Realtime-gated so chart-load RT churn is
  ignored. Logs `[RENDER] SpiralDetected`. Cannot false-trip on normal
  interaction (the chart renders fine between clicks). Circuit behaviour
  unchanged from .20 (skip OnRenderImpl, 2s cooldown, CircuitOpen/CircuitClosed).

**Diagnostic state:** EVERY ECT render tripwire is clean — `SafeDrawSkipped`,
`OffThreadD2D`, `ReentrantOnRender`, `ClipImbalance`, `ClipRectInvalid`,
`ClipUnderflow`, `PushClip`/`PopClip`, `D2DStateProbe`,
`D2DStateAfterDrawOrders`, the `OrderDisplay` phase catch — NONE ever fired.
ECT's clip stack is balanced; `Flush()` probes never throw. The corruption is
fully silent to ECT and surfaces only at NinjaTrader's own `EndDraw`
(`D2DERR_WRONG_STATE` + cascade `D2DERR_PUSH_POP_UNBALANCED`). Root cause of
what corrupts a fresh render target each frame is still unidentified after
~14 versions of instrumentation.

**Next step (decisive):** test 26.5.21.21. (a) `SpiralDetected` + `CircuitOpen`
then `CircuitClosed` + chart visually recovers => ECT's `OnRenderImpl` drawing
IS the corruptor; next bisect `OnRenderImpl`. (b) `CircuitOpen` but no
`CircuitClosed` => corruptor is NOT ECT's per-frame drawing (OnRenderTargetChanged
brush work, another indicator, or a non-freeing native leak). Strongly
recommended: ship customer Ben the working 25.12.16.1 build now — the .26 line
has had a UI-freeze regression unfixed across ~14 diagnostic iterations.

**Build-verification gotcha:** for much of this session the user's recompiles
were not picking up deployed changes (diagnostics produced zero log output).
Resolved by bumping `pCurrentVersionName` so the running build is verifiable
on the chart panel + in `SessionStart`. Always bump the version when a
deployed change must be confirmed live.

**Why:** The customer is blocked from trading the new version on live accounts;
the regression is the motivation for rendering-efficiency work on the ECT.

**How to apply:** Rendering changes to [[user-role]]'s aiEnhancedChartTrader.cs
should prioritize `OnRender` hot-path cost and avoid per-frame native-resource
(SharpDX TextFormat/TextLayout/PathGeometry/brush) allocation, which both slows
frames and accumulates toward the `D2DERR_WRONG_STATE` failure. Any code that
touches the four order collections above off the UI thread must take
`_accountStateLock` — see [[optimization-approach]].
