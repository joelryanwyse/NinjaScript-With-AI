# NinjaScript-With-AI

Custom NinjaScript indicators for NinjaTrader 8. Source-of-truth git repo synced between the office and home machines via a private GitHub remote.

The detailed engineering context (file layout, NT version range, deploy rules, lock discipline) is in [CLAUDE.md](CLAUDE.md). This file covers per-machine setup.

## First-time setup on a new machine

1. **Clone outside OneDrive** — OneDrive corrupts `.git` folders. Recommended path: `C:\Users\<you>\source\NinjaScript-With-AI`.
   ```powershell
   git clone https://github.com/joelryanwyse/NinjaScript-With-AI.git C:\Users\$env:USERNAME\source\NinjaScript-With-AI
   cd C:\Users\$env:USERNAME\source\NinjaScript-With-AI
   ```

2. **Link memory into Claude's standard path** — so Claude reads the in-repo memory:
   ```powershell
   .\.claude\scripts\link-memory.ps1
   ```
   This creates a junction from `~\.claude\projects\<project-key>\memory` to `<repo>\.claude\memory\`. Backs up any pre-existing memory at that location first.

3. **Install git hooks** — so a `git pull` auto-deploys updated `.cs` files to NinjaTrader:
   ```powershell
   .\.claude\scripts\install-git-hooks.ps1
   ```

4. **One-time full deploy** to seed NT with the current versions:
   ```powershell
   .\.claude\scripts\deploy-all.ps1
   ```

5. Open NinjaTrader, compile (F5 in NinjaScript Editor), verify indicators load.

## Day-to-day workflow

```powershell
# Office: open the repo, pull anything from home
git pull

# ... edit, test in NT ...

# End of session
git add -A
git commit -m "what changed"
git push
```

```powershell
# Home: pull → post-merge hook auto-deploys changed .cs to NT
git pull

# ... NT picks up the changes on its next compile ...
```

## Scripts

All under [.claude/scripts/](.claude/scripts/):

| Script | Purpose |
|---|---|
| `link-memory.ps1` | One-time per machine. Junctions in-repo memory into Claude's project memory path. |
| `install-git-hooks.ps1` | One-time per machine. Installs the `post-merge` hook so `git pull` auto-deploys. |
| `deploy-all.ps1` | Manual safety-net. Copies every root `.cs` to NT's Indicators folder. |
| `deploy-to-ninjatrader.ps1` | PostToolUse hook called by Claude Code after each Edit/Write. Best-effort. |
| `post-merge` (installed) | Git hook. After `git pull`, copies changed `.cs` files into NT's Indicators folder. |

## What goes where

- Source of truth: this repo.
- What NT runs: `<MyDocuments>\NinjaTrader 8\bin\Custom\Indicators\`. Always a deployed copy of the repo's `.cs` files.
- Memory (what Claude knows about this project): `.claude/memory/` — versioned in git.
- Hook error log (best-effort deploy failures): `%USERPROFILE%\.claude\nt-deploy-hook-errors.log`.
