@echo off
REM Double-clickable per-machine setup. Equivalent to opening Claude Code in
REM this repo (which runs the same script via SessionStart). Safe to run
REM repeatedly — fast no-op after the first time.
REM
REM What it does:
REM   1. Junctions Claude Code's memory path to .claude\memory in this repo.
REM   2. Installs git hooks (post-merge auto-deploys .cs to NT after pulls).
REM   3. First run only: copies all .cs to NT's Indicators folder so NT can
REM      compile immediately.

setlocal
cd /d "%~dp0"
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0.claude\scripts\auto-setup.ps1"
echo.
echo Done. You can close this window.
pause
