---
name: git-multimachine
description: Project is git-synced across the user's computers via a private GitHub repo; lives outside OneDrive; pull at session start, commit and push when done.
metadata:
  type: project
---

On 2026-05-22 the project was moved out of OneDrive to `C:\Users\joelw\source\NinjaScript-With-AI` and made a git repo synced to a private GitHub remote (`origin` = github.com/joelryanwyse/NinjaScript-With-AI), so the user can work on the indicator suite from more than one computer.

**Why:** The user wanted to write code from multiple machines and keep it synced. Git (not OneDrive) is the sync mechanism — OneDrive corrupts `.git` folders, so the repo must stay outside OneDrive.

**How to apply:** At session start consider `git pull` — the other machine may have newer work. When work is done, commit it and push so the other machine receives it; confirm before pushing unless told otherwise. Never relocate the repo back under OneDrive. The old path `...\OneDrive\Desktop\NinjaScript With AI` is a stale pre-migration copy — do not edit it (the user may delete it once the new setup is verified). Memory itself lives inside the repo at `.claude\memory\` and is junctioned into the standard `~\.claude\projects\<key>\memory` path via `.claude\scripts\link-memory.ps1` — so memory rides on git too.
