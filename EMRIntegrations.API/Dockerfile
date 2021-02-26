FROM mcr.microsoft.com/dotnet/framework/aspnet:4.7.2
#FROM mcr.microsoft.com/dotnet/framework/runtime:4.6.2
COPY ./ /inetpub/wwwroot/webapp/
# RUN dir /inetpub/wwwroot/webapp
SHELL ["powershell", "-command"]
ADD runapp.bat /runapp.bat
ENTRYPOINT ["C:\\runapp.bat"]