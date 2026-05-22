---
name: project-ninjatrader-deploy
description: "After completing edits to any NinjaScript .cs file, ALWAYS deploy it as the final step — copy from the working dir to NinjaTrader's Indicators folder. The auto-hook is unreliable; deploy manually and verify."
metadata: 
  node_type: memory
  type: project
  originSessionId: aaa3bc7e-ec08-4f75-b486-1500b5728c6e
---

**Rule: deploying is part of finishing the task.** After completing any modification to a NinjaScript `.cs` file, the final step — before reporting the work done — is to copy that file from the working dir into NinjaTrader's live Indicators folder. A change is not complete until it is deployed.

- Edit here (source of truth): `C:\Users\joelw\source\NinjaScript-With-AI\` (or a `.claude\worktrees\<name>\` subfolder of it during isolated sessions)
- Deploy to here (what NT runs): `<MyDocuments>\NinjaTrader 8\bin\Custom\Indicators\`
- Deploy + verify: `Copy-Item -LiteralPath <SRC> -Destination <DST> -Force`, then confirm SRC/DST sizes match and DST contains the new code.

**Why:** The user runs NinjaTrader against the deployed copy. An edit that only changes the working file is invisible to NT — they would compile stale code. The user explicitly asked (2026-05-21) for deployment on every completed change so they never have to remember.

**How to apply:**
- Deploy once per task, after ALL edits for that task are done — not after each individual Edit call.
- A PostToolUse hook (`.claude\settings.json` → `.claude\scripts\deploy-to-ninjatrader.ps1`, matches `Edit|Write|MultiEdit`) is meant to auto-copy but is unreliable — it silently failed to fire on 2026-05-19, -20, and -21. NEVER assume it ran; always deploy manually as the explicit final step. Hook errors, when it runs at all, log to `%USERPROFILE%\.claude\nt-deploy-hook-errors.log`.
- A `deploy-all.ps1` script under `.claude\scripts\` copies every root `.cs` to NT's Indicators folder as a belt-and-suspenders manual deploy.
- Expect DST to differ from SRC: NT regenerates a `#region NinjaScript generated code` block at the end of the deployed file on every compile (a freshly-compiled DST is ~4-5KB larger). A manual `Copy-Item` overwrites DST and drops that region — harmless, NT recreates it on the next compile. The working file should never contain that region.
