C:\Windows\Microsoft.NET\Framework\v2.0.50727\InstallUtil -u "%~dp0\BigFile.exe"

sc stop "BigFile"
sc delete "BigFile"

pause