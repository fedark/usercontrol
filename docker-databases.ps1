docker run -d --rm --name sql -p 1434:1433 -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=UserControl!" -v usercontrol-sql-data:/var/opt/mssql mcr.microsoft.com/mssql/server:2019-latest

# issue: postgres volume does not keep db data when running via "docker" command
docker run -d --rm --name pgs -p 5433:5432 -e "POSTGRES_PASSWORD=UserControl!" -v usercontrol-pgs-data:/var/lib/potgresql/data postgres