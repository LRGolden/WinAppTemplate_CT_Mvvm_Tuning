@echo off
setlocal enabledelayedexpansion

REM ──────────────────────────────────────────────────────────
REM  rename-template.bat
REM
REM  Renames a cloned template project from "MiPlantillaBase"
REM  to a new name of your choice — files, folders, and content.
REM
REM  Usage:
REM    rename-template.bat <NewName>
REM    rename-template.bat --help
REM
REM  Examples:
REM    rename-template.bat MyNewApp
REM    rename-template.bat "My New App"
REM ──────────────────────────────────────────────────────────

set "OLDNAME=MiPlantillaBase"
set "NEWNAME=%~1"

REM ── Help ──────────────────────────────────────────────────
if /i "%NEWNAME%"=="--help" goto :help
if /i "%NEWNAME%"=="/?" goto :help
if "%NEWNAME%"=="" goto :help

echo.
echo Renaming project from "%OLDNAME%" to "%NEWNAME%"...
echo.

REM ── 1. Rename files (reverse order to avoid conflicts) ────
for /f "delims=" %%F in ('dir /s /b /a-d 2^>nul ^| sort /r') do (
    set "FPATH=%%F"
    set "NAME=%%~nxF"
    if not "!NAME!"=="!NAME:%OLDNAME%=!" (
        set "NEWPATH=!FPATH:%OLDNAME%=%NEWNAME%!"
        echo   [FILE] Renaming "%%F"
        ren "%%F" "!NAME:%OLDNAME%=%NEWNAME%!" 2>nul
        if errorlevel 1 echo   [WARN] Could not rename "%%F"
    )
)

REM ── 2. Rename folders (deepest first) ─────────────────────
for /f "delims=" %%D in ('dir /s /b /ad 2^>nul ^| sort /r') do (
    set "DPATH=%%D"
    set "NAME=%%~nxD"
    if not "!NAME!"=="!NAME:%OLDNAME%=!" (
        set "NEWPATH=!DPATH:%OLDNAME%=%NEWNAME%!"
        echo   [DIR]  Renaming "%%D"
        ren "%%D" "!NAME:%OLDNAME%=%NEWNAME%!" 2>nul
        if errorlevel 1 echo   [WARN] Could not rename "%%D"
    )
)

REM ── 3. Replace content in text files ──────────────────────
for /r %%F in (*) do (
    set "FPATH=%%F"
    set "EXT=%%~xF"
    set "SKIP="
    REM Skip binary files
    for %%E in (.exe .dll .png .jpg .jpeg .ico .bmp .gif .zip .nupkg .pdb) do (
        if /i "!EXT!"=="%%E" set "SKIP=1"
    )
    if not "!SKIP!"=="1" (
        findstr /m /c:"%OLDNAME%" "%%F" >nul 2>&1
        if !errorlevel! equ 0 (
            echo   [CONTENT] %%F
            powershell -NoProfile -Command "try { $c = Get-Content -LiteralPath '%%F' -Raw; $c = $c -replace '%OLDNAME%', '%NEWNAME%'; Set-Content -LiteralPath '%%F' -Value $c -Encoding UTF8 -NoNewline } catch { write-host '  [SKIP] ' + $_.Exception.Message }"
        )
    )
)

echo.
echo [OK] Project renamed to "%NEWNAME%".
echo.
echo Next steps:
echo   1. dotnet restore
echo   2. dotnet build
echo   3. dotnet test
echo.
pause
goto :eof

REM ── Help screen ───────────────────────────────────────────
:help
echo.
echo rename-template.bat — Renames a cloned template project
echo.
echo This script replaces all occurrences of "%OLDNAME%" in
echo file/folder names and file contents with your new project name.
echo.
echo USAGE:
echo   %~nx0 NewProjectName
echo.
echo EXAMPLES:
echo   %~nx0 MyNewApp
echo   %~nx0 "My New App"
echo.
echo OPTIONS:
echo   --help, /?    Show this help message
echo.
echo TIP: Run this from the root of the copied template folder.
echo.
pause
endlocal
