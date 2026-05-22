# CLAUDE.md

Authoritative context for any Claude Code session in this repo. Project memory at [.claude/memory/](.claude/memory/) layers on top of this.

## What this is

A suite of NinjaScript indicators for NinjaTrader 8 — commercial indicators distributed to customers. Two are very large single-file indicators that drive most of the work:

- [aiEnhancedChartTrader.cs](aiEnhancedChartTrader.cs) — chart-side order entry, ATM strategy planning, position display, SharpDX-rendered preview UI. ~96k lines.
- [aiDuplicateAccountActions.cs](aiDuplicateAccountActions.cs) — multi-account trade-copier dashboard. Mirrors a master account to follower accounts. SharpDX table, risk manager, latency analysis. ~54k lines.

Other indicators (`aiBest*`, `aiSIG*`, `aiSR*`, `aiSignalStudio*`, `VeritasOrderFlow`) are smaller and edited less often.

## Reading the giant files

Full reads are infeasible. Use Grep for symbols and targeted Read calls with `offset`/`limit`. Both files are flat — one giant partial class — so jump straight to the symbol you need.

## NT version compatibility

Indicators must work across the full NT8 range customers run: **8.0.28.0 → 8.1.7.0+**. The user's dev machine is 8.1.7.0 — "works on mine" is not enough.

- Prefer documented APIs (`TypeConverter`, plain WPF `Window`) over NT-internal or "unsupported" ones.
- NinjaTrader reworked the property grid between 8.0.x and 8.1.x — grid/editor internals are version-fragile.

## The deploy step is part of finishing the task

NinjaTrader runs the deployed copy, not the source. An edit that doesn't reach `<MyDocuments>\NinjaTrader 8\bin\Custom\Indicators\<file>.cs` is invisible to NT.

- A PostToolUse hook at [.claude/scripts/deploy-to-ninjatrader.ps1](.claude/scripts/deploy-to-ninjatrader.ps1) is supposed to auto-copy after Edit/Write/MultiEdit. **It is unreliable** — has silently failed on multiple days.
- **Manual deploy is mandatory as the final step** of any `.cs` edit task: either `Copy-Item` the specific file, or run [.claude/scripts/deploy-all.ps1](.claude/scripts/deploy-all.ps1) to copy every root `.cs` to NT's Indicators folder.
- After `git pull`, the post-merge hook (installed by [.claude/scripts/install-git-hooks.ps1](.claude/scripts/install-git-hooks.ps1)) auto-deploys any changed `.cs` — so pulling on the other machine immediately updates NT.

NT regenerates a `#region NinjaScript generated code` block at the end of the deployed `.cs` on compile (DST ~4-5KB larger than SRC). Overwriting DST drops that region — harmless, NT recreates it on next compile. The working file in the repo must never contain that region.

## Multi-machine workflow

Git is the sync mechanism between the user's office and home machines. **Not OneDrive** — OneDrive corrupts `.git` folders. Per-machine setup is in [README.md](README.md).

Day-to-day:
1. **Session start:** `git pull` to get any work from the other machine.
2. **Edit.** Hook tries to deploy each edit; consider it best-effort.
3. **End of task:** confirm the file is deployed (`deploy-all.ps1` if unsure), then commit + push.
4. **Other machine:** `git pull` → post-merge hook auto-deploys the changed files to NT.

Memory at `.claude/memory/` is junctioned into Claude Code's standard memory path by [.claude/scripts/link-memory.ps1](.claude/scripts/link-memory.ps1) so it rides on git too. The junction is created automatically on first Claude Code session via [.claude/scripts/auto-setup.ps1](.claude/scripts/auto-setup.ps1) (wired as a SessionStart hook in [.claude/settings.json](.claude/settings.json)); on subsequent sessions it's a fast no-op. The user can also double-click [install.bat](install.bat) to seed a fresh machine without opening Claude Code.

## How the user wants substantial work approached

Thorough at full scope, not conservative — the user is the developer and compiles/tests in NinjaTrader himself, so "I can't compile here" is fine. Flag genuine risks once, then proceed.

- Prefer behavior-preserving, compiler-self-checking changes — a missed spot should surface as a compile error, not a silent runtime bug.
- Always remind the user to recompile/test in NT before live use.
- Lock discipline in `aiEnhancedChartTrader.cs`: anything touching `LinkedOrders` / `MovingOrders` / `SelectedOrders` / `RRIsOnOrders` off the UI thread takes `_accountStateLock`. **Never** hold `_accountStateLock` across a NinjaTrader API call (`myAccount.Orders` etc.) — that caused a deadlock once, see [.claude/memory/project_ect_render_regression.md](.claude/memory/project_ect_render_regression.md).
- Lock discipline in `aiDuplicateAccountActions.cs`: `TrackedMasterPosition` desync is a recurring real-money bug — read [.claude/memory/project_copier_flatten_reversal.md](.claude/memory/project_copier_flatten_reversal.md) before touching flatten/copy paths.

## What not to commit

`.gitignore` already excludes them, but for clarity — `*.corrupt-backup-*.cs`, `*.merged.cs`, `*.bak`, `*~`, `*.orig`, and `.claude/worktrees/` (Claude Code's per-session isolated checkouts) never go in commits on `main`.

## Worktree-isolation note

Claude Code may spawn each session in a fresh worktree under `.claude/worktrees/<random-name>/` on branch `claude/<random-name>`. The branch must be merged into `main` and pushed for the other machine to see the work. End-of-session: merge to `main`, push, then optionally `git worktree remove .claude/worktrees/<name>` and `git branch -D claude/<name>`.
