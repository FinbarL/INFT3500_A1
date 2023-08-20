1.
> dotnet new tool-manifest

> dotnet tool install dotnet-ef
2. Create your database and populate the data from the script
2. Update your appsettings.json to point to your database
3. Run ef database update to apply the migration scripts
4. Run dotnet run to run the program

LIST OF AVAILABLE USERS: 
1. Admin
    - Username: admin
    - Password: admin
2. User
    - Username: user
    - Password: user
3. Staff
    - Username: staff
    - Password: staff
