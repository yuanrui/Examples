C:\Windows\Microsoft.NET\Framework\v2.0.50727\InstallUtil -u /DN="BigFile Http Server" /SN=BigFile "%~dp0\BigFile.exe"
C:\Windows\Microsoft.NET\Framework\v2.0.50727\InstallUtil /LogToConsole=true  /DN="BigFile Http Server" /SN=BigFile "%~dp0\BigFile.exe"

sc start "BigFile"

pause