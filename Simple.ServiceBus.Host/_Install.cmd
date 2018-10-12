C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil -u /DN="Service Bus Host" /SN=ServiceBus.Host "%~dp0\Simple.ServiceBus.Host.exe"
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil /LogToConsole=true  /DN="Service Bus Host" /SN=ServiceBus.Host "%~dp0\Simple.ServiceBus.Host.exe"

sc start "ServiceBus.Host"

pause