---
name: nt-version-compat
description: "NinjaScript solutions must work across the full NT8 version range the user's customers run; avoid version-fragile or NT-\"unsupported\" APIs"
metadata: 
  node_type: memory
  type: feedback
  originSessionId: dd79f0b3-4567-43bc-ab45-cca207c21f5a
---

The user's NinjaScript indicators are distributed to customers running a spread of NinjaTrader 8 versions — versions named so far span **8.0.28.0 through 8.1.7.0+**. Any solution has to work across that whole range, not just the user's own 8.1.7.0.

**Why:** these are commercial indicators sold to many customers ([[user-role]]). An approach that works on one NT version but breaks or renders differently on another becomes a support burden. NinjaTrader reworked the property grid between the 8.0.x and 8.1.x lines, so grid/editor internals are especially version-sensitive.

**How to apply:** when choosing an implementation approach, prefer standard, stable, documented APIs (e.g. `TypeConverter`, plain WPF `Window`) over NT-"unsupported" or internal ones (e.g. custom `WpfPropertyGrid` property editors). Treat "works on the user's 8.1.7.0 but unverified on older versions" as a real downside, not a footnote. This is what steered the ECT color-input redesign toward a TypeConverter dropdown + custom WPF popup window instead of a custom property editor.
