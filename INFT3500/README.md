

## Setup: 
1. Go into the project file (e.g. INFT3500)
2. Run 'docker-compose up --build' to build the docker image and run the container. After 90 seconds the DB will be populated with data
3. Run 'dotnet run --project INFT3500' to run the project
4. Go to 'localhost:5189' to view the website


Additional Notes: 
- Please ensure that port 1433 is available when running the docker image
- The Program.cs file in the INFT3500 project contains the connection string to the database. If you choose to not use docker, please change the connection string to your own database.
- The docker image will automatically create the database and tables for you. If you choose to not use docker, please create the database and tables using the scripts in the 'Database' folder.
- The docker image will automatically seed the database with some data. If you choose to not use docker, please seed the database using the scripts in the 'Database' folder.
- The Program.cs file is configured so that migrations will automatically run when the project is ran. This relies on having the database in its original form the script provided.
- The user table is seeding with teh users listed below 

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



TODO:
- Admins can create new users, include setting isAdmin and isStaff
- Email verification and account recovery (forgot password)
- User Order History
- Show all items in the admin product view 
