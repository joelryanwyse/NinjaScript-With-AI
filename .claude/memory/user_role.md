---
name: user-role
description: "User builds custom NinjaScript trading indicators (NinjaTrader 8) — two very large single-file indicators: aiEnhancedChartTrader.cs (~96k lines) and aiDuplicateAccountActions.cs (~54k lines)."
metadata: 
  node_type: memory
  type: user
  originSessionId: aaa3bc7e-ec08-4f75-b486-1500b5728c6e
---

User develops custom NinjaScript indicators for NinjaTrader 8. The directory holds many indicators (aiBest*, aiSIG*, aiSR*, aiSignalStudio*, VeritasOrderFlow); two are very large, actively-edited single-file indicators:
- `aiEnhancedChartTrader.cs` (~96k lines) — chart-side order entry, ATM strategy planning, position display, and a lot of custom plan-preview UI drawn via SharpDX.
- `aiDuplicateAccountActions.cs` (~54k lines) — a multi-account trade-copier dashboard: mirrors a master account's executions/orders to follower accounts, with a big interactive SharpDX table (sortable/reorderable columns, right-click column picker), an account risk manager, and copier-latency analysis.

NinjaScript-specific notes that affect how to help:
- Code lives in one giant `.cs` file; full reads are infeasible. Use Grep/targeted Reads.
- Heavy use of NT8 APIs: `AtmStrategy`, `Bracket`, `Order`, `BracketUnitsToTicks` / `TicksToBracketUnits`, `OnRender` via SharpDX, `atmStrategySelector`.
- Concepts the user thinks in: ATM Strategy Plan, live resting orders, preview plans (sentinel keys like 10 / -10 / 12 / -12 / 13), Initial Exits modes (Default / High / Low / ATR), Risk-Reward toggle, IE: badge.
