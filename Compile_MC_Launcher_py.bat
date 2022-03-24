@echo off
rem This compiles the MC_Launcher.py file to an executable.


color 0a
pyinstaller --onefile ./MC_Launcher.py
if %ERRORLEVEL% NEQ 0 goto error

move .\dist\MC_Launcher.exe .\
rmdir /Q dist /s
rmdir /Q build /s
rmdir /Q __pycache__ /s
goto EOF

:error
pause

:EOF
