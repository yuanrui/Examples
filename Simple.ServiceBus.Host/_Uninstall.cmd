C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil -u "%~dp0\Simple.ServiceBus.Host.exe"

sc stop "ServiceBus.Host"
sc delete "ServiceBus.Host"

pause