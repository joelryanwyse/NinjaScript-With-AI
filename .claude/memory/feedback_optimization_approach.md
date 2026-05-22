---
name: optimization-approach
description: "How the user wants substantial work (optimization, refactors, feature builds) on his NinjaScript indicators approached — thorough, full scope over conservative."
metadata: 
  node_type: memory
  type: feedback
  originSessionId: b019aecc-1316-4ead-b8c9-c1a9cade9ff6
---

On substantial work in [[user-role]]'s NinjaScript indicators — optimization
passes, refactors, and feature builds — the user wants thorough work at full
scope. Offered a scoped-vs-full choice he consistently picks full: large
refactors (e.g. a 119-use field-type conversion of `CellLayout` from TextLayout
to Size2F) and, when building features, the richer of the design options on
offer (e.g. for a new latency column he chose current+session-peak display plus
a configurable threshold input over the plain-number option).

**Why:** He's the developer and compiles/tests in NinjaTrader himself, so
"can't compile in this environment" is an accepted workflow — he validates
downstream. He has repeatedly said "keep improving all of this."

**How to apply:** Don't be overly conservative or stop early to ask permission
for each increment; flag genuine risks once, then proceed. Prefer changes that
are behavior-preserving and compiler-self-checking — a missed spot should
surface as a compile error, not a silent runtime bug. Always still call out
that NinjaScript must be recompiled/tested before live use. See
[[ect-render-perf-regression]] for the work this supports.
