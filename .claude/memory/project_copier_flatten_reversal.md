---
name: copier-flatten-reversal-incident
description: 2026-05-21 incident — copier mis-sizes/reverses follower positions around Flatten All; two customers; aiDuplicateAccountActions.cs v26.5.5.1 + NT 8.1.7.0
metadata: 
  node_type: memory
  type: project
  originSessionId: b0f1971f-6efd-4ade-b1b5-46899b9336f7
---

Live incident opened 2026-05-21. The copier's internal master-position tracker
(`TrackedMasterPosition` in aiDuplicateAccountActions.cs) desyncs from the real
master position around a Flatten All, so followers get wrong-sized or
wrong-direction orders. Two customers reported different faces of it:

- **Customer A ("double size"):** after flattening while in a position,
  followers came out oversized (often exactly double). Root cause: the
  "seeding double-count correction" in `ProcessMasterExecution` had a
  tautological test that always fired on NT8's pre-fill `GetPosition()`,
  snapping the tracker one fill into the past. Fixed this session — block
  removed + an idle resync (`ResyncTrackedMasterPositionFromLive`) added.

- **Customer B (Michael Weprin, Bulenox/Rithmic):** on Flatten All, followers
  closed then **reversed to the opposite side**. The master's own flatten
  `Close` order fills in partials; the copier's flatten-suppression guard was a
  fixed ~3s wall-clock window, so partials landing after 3s were copied to the
  just-flattened followers as a fresh opposite entry. Confirmed in his diag
  (FillId 000037–000039). Fixed this session — order-scoped suppression:
  `_masterFlattenCloseOrders` tracks the master's flatten `Close` orders
  (captured in OnOrderUpdate3), and all three copy paths skip their fills for
  the order's whole life regardless of the timer.

- **Jason Vicente (Tradovate via TradingView), 2026-05-21:** on the *released*
  v26.5.5.1, followers came out 3x size and wrong-direction. His diag proved the
  mechanism fill-by-fill: every `OrderName=Close` fill set `TrackedMasterPosition`
  to the pre-close size, so each flatten leaked its size into the tracker; by
  FillId 00001F (08:33) the tracker read +3 while the master was -1 and the
  followers bought 3 on a short. v26.5.5.1 is the buggy build customers still
  run; fixes 1-5 are in the dev build, now v26.5.21.1.

**Why:** real money bug on live funded prop accounts. **How to apply:** when
touching copier/flatten code in aiDuplicateAccountActions.cs, preserve all five
fixes: (1) removed the tautological seeding double-count
correction; (2) idle resync of `TrackedMasterPosition` to live in OnTimerTick3;
(3) order-scoped flatten suppression (`_masterFlattenCloseOrders`); (4) in
`ProcessMasterExecution`, a flatten-Close fill pins `TrackedMasterPosition` to 0
so the tracker is correct the instant a flatten finishes (no stale window);
(5) `OnPositionUpdate3` -> `ReconcileTrackedMasterFromPositionUpdate` snaps
`TrackedMasterPosition` to the master's authoritative position on every settled
position tick (1s settle gate, drainer-interlocked) so drift self-heals within a
tick and can't survive into the next trade (added 2026-05-21, v26.5.21.1).
Related: [[project_ninjatrader_deploy]], [[feedback_optimization_approach]].
