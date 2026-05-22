---
name: copier-rejection-storm
description: "2026-05-19 incident — master BuyToCover orders rejected by NinjaTrader triggered a 425x resubmit storm in aiDuplicateAccountActions.cs; root cause (why NT rejects) still unknown; behind customer Eric Wilson's refund. MASTER_REJECT diag logging added 2026-05-21."
metadata: 
  node_type: memory
  type: project
  originSessionId: 0dcefef3-3805-4d51-b937-be508f8b5be4
---

2026-05-19 (M2K JUN26): the master account's strategy exit order ("Close",
BuyToCover Market) was rejected by NinjaTrader ~13ms after submission. The ADS
"order rejection handling" feature (`pRejectedOrderHandling` + `pResubmitMaster`)
resubmitted it as a market order; that resubmit was rejected too, looping
**425 times over ~46 minutes** across two ADS restarts (diag_2026-05-19.txt,
14:04–14:50). The order never filled. The master stayed stranded short while
the 10 followers had already exited flat. When a `Synchronize` BuyToCover
finally filled and flattened the master, that fill was copied onto the
already-flat followers, putting all 10 LONG — a reversal, same family as
[[copier-flatten-reversal-incident]].

**Root cause is still UNKNOWN** — *why* NinjaTrader rejected those BuyToCover
orders. The diag never recorded a reject reason for master orders. This is the
bug behind customer Eric Wilson's refund (support chat 2026-05-17 → 05-21).

**Why:** real money on live prop accounts; cost a customer and a refund.
**How to apply:**
- 2026-05-21 this session added master-side reject-reason logging: a new
  `EXECUTION|MASTER_REJECT` diag event and a `Reason=` field on
  `ORDER|REJECTED_RESUBMIT` / `REJECTED_RESUBMIT_CAPPED`. The NEXT diag from this
  customer should show NinjaTrader's actual reject reason — read it before
  changing any resubmit behavior. Diagnostics-only pass; no logic changed.
- The `ResubmitGuardAllows` circuit breaker (3 attempts / 30s window) already
  caps the storm spam; the 425x runaway in the diag predates it.
- NOT yet fixed (user deferred at 2026-05-21): (1) the Synchronize/sync fill
  being copied onto already-flat followers; (2) `isExitOrder` only matches
  "Stop"/"Target", so "Close"/"Synchronize" exit orders are mis-handled as
  resubmittable entries.
Related: [[copier-flatten-reversal-incident]], [[ect-followerexit-lag-bug]],
[[project_ninjatrader_deploy]], [[feedback_optimization_approach]].
