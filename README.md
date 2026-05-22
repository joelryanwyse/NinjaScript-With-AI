# NinjaScript-With-AI

Custom NinjaScript indicators for NinjaTrader 8. Source-of-truth git repo synced between the office and home machines via a private GitHub remote.

The detailed engineering context (file layout, NT version range, deploy rules, lock discipline) is in [CLAUDE.md](CLAUDE.md). This file covers per-machine setup.

## First-time setup on a new machine

1. **Clone outside OneDrive** — OneDrive corrupts `.git` folders. Recommended path: `C:\Users\<you>\source\NinjaScript-With-AI`. (Use Git for Windows or GitHub Desktop; no PowerShell required for this step beyond what those tools provide.)

2. **Run setup — pick one:**
   - **Open Claude Code in the repo folder.** A SessionStart hook auto-runs the setup (junctions memory, installs git hooks, seeds NinjaTrader's Indicators folder on first run). Subsequent sessions are silent no-ops.
   - **Or double-click `install.bat`** at the repo root. Same setup, no Claude required.

3. Open NinjaTrader, compile (F5 in NinjaScript Editor), verify indicators load.

That's it. Nothing else to type into PowerShell.

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
| `auto-setup.ps1` | Self-healing setup. Run by Claude's SessionStart hook and by `install.bat`. Junctions memory, installs git hooks, seeds NT on first run. Idempotent. |
| `link-memory.ps1` | (Called by auto-setup.) Junctions in-repo memory into Claude's project memory path. |
| `install-git-hooks.ps1` | (Called by auto-setup.) Installs the `post-merge` hook so `git pull` auto-deploys. |
| `deploy-all.ps1` | Manual safety-net. Copies every root `.cs` to NT's Indicators folder. |
| `deploy-to-ninjatrader.ps1` | PostToolUse hook called by Claude Code after each Edit/Write. Best-effort. |
| `post-merge-deploy.ps1` | Helper called from the post-merge git hook to copy changed files into NT. |
| `post-merge` (installed) | Git hook. After `git pull`, copies changed `.cs` files into NT's Indicators folder. |

## What goes where

- Source of truth: this repo.
- What NT runs: `<MyDocuments>\NinjaTrader 8\bin\Custom\Indicators\`. Always a deployed copy of the repo's `.cs` files.
- Memory (what Claude knows about this project): `.claude/memory/` — versioned in git.
- Hook error log (best-effort deploy failures): `%USERPROFILE%\.claude\nt-deploy-hook-errors.log`.
