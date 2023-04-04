FROM mcr.microsoft.com/dotnet/sdk:6.0 as sdk
WORKDIR /usercontrol

COPY ./UserControl/*.csproj ./UserControl/
COPY ./Data/*.csproj ./Data/
COPY ./EfAccess/*.csproj ./EfAccess/
COPY ./EfAccess.SqlServerMigrations/*.csproj ./EfAccess.SqlServerMigrations/
COPY ./EfAccess.PostgreSqlMigrations/*.csproj ./EfAccess.PostgreSqlMigrations/
COPY ./EfAccess.SqliteMigrations/*.csproj ./EfAccess.SqliteMigrations/
RUN dotnet restore ./UserControl/UserControl.csproj

COPY . ./
WORKDIR /usercontrol/UserControl
RUN dotnet publish -c Release -o publish

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet ef migrations bundle -o publish/efbundle

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
WORKDIR /usercontrol
COPY --from=sdk /usercontrol/UserControl/publish/. ./

RUN echo "#!/bin/bash" > entry-point.sh
RUN echo "./efbundle" >> entry-point.sh
RUN echo "dotnet UserControl.dll" >> entry-point.sh
RUN chmod +x entry-point.sh

ENTRYPOINT [ "./entry-point.sh" ]