FROM mcr.microsoft.com/dotnet/sdk:6.0 as sdk
WORKDIR /usercontrol

COPY ./UserControl/*.csproj ./UserControl/
RUN dotnet restore ./UserControl/UserControl.csproj

COPY ./UserControl/. ./UserControl/
WORKDIR /usercontrol/UserControl
RUN dotnet publish -c Release -o publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
WORKDIR /usercontrol
COPY --from=sdk /usercontrol/UserControl/publish/. ./

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 7887
ENTRYPOINT [ "dotnet", "UserControl.dll" ]