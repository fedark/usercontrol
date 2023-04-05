dotnet ef migrations add Init -s "./UserControl" -p "./EfAccess.SqlServerMigrations" -- "DbProvider=SqlServer"
dotnet ef migrations add Init -s "./UserControl" -p "./EfAccess.PostgreSqlMigrations" -- "DbProvider=PostgreSql"
dotnet ef migrations add Init -s "./UserControl" -p "./EfAccess.SqliteMigrations" -- "DbProvider=Sqlite"