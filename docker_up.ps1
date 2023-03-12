docker stop user
docker build -t fedark/usercontrol -f Dockerfile .
docker run -td --rm --name user -p 7887:7887 --link sql fedark/usercontrol
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=UserControl!" -p 1433:1433 -v usercontrol-data:/var/opt/mssql -d --name sql mcr.microsoft.com/mssql/server:2019-latest