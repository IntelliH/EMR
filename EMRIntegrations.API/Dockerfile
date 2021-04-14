FROM mcr.microsoft.com/dotnet/framework/aspnet:4.7.2
#FROM mcr.microsoft.com/dotnet/framework/runtime:4.6.2
COPY ./ /inetpub/wwwroot/webapp/
# RUN dir /inetpub/wwwroot/webapp
SHELL ["powershell", "-command"]
ADD runapp.bat /runapp.bat
RUN powershell -Command Invoke-WebRequest -Uri https://github.com/DataDog/dd-trace-dotnet/releases/download/v1.25.0/datadog-dotnet-apm-1.25.0-x64.msi -OutFile datadog-apm.msi
RUN powershell -Command Start-Process .\datadog-apm.msi -ArgumentList '/quiet' -Wait
ENTRYPOINT ["C:\\runapp.bat"]
