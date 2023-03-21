dotnet ef migrations add Init -s "./UserControl" -p "./Data.SqlServerMigrations" -- "DbProvider=SqlServer"
dotnet ef migrations add Init -s "./UserControl" -p "./Data.PostgreSqlMigrations" -- "DbProvider=PostgreSql"
dotnet ef migrations add Init -s "./UserControl" -p "./Data.SqliteMigrations" -- "DbProvider=Sqlite"