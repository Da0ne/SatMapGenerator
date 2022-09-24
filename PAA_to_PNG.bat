REM simple script to convert all .paa files in directory the batch is executed in to .png :)

for /F "Tokens=2* skip=2" %%A In ('REG QUERY "HKEY_CURRENT_USER\SOFTWARE\Bohemia Interactive\Dayz Tools\ImageToPAA" /v "Tool"') DO SET ImageToPAAPath=%%B

@ECHO OFF
setlocal enabledelayedexpansion
for %%f in (%CD%\*.paa) do (
  set /p val=<%%f
  start "convert to PAA PNG" /B "%ImageToPAAPath%" -size=256 %%f %CD%\%%~nf.png
)


for %%f in (%CD%\*.paa) do (
  set /p val=<%%f
  del /Q %%f
)