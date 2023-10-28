# Wait for SQL server to init
sleep 60

/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P P4ssw0rd! -d master -i scripted_db.sql