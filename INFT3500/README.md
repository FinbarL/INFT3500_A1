

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


Questions: 

1. Are the items listed in the business scenario the only ones we need to cover? 
2. What does the 'update order details' look like? (UML Diagram)
3. I currently don't allow the user to update the genre, is this okay? 
4. Whats the actual MVP? Is it just the business scenarios?
5. Do we need to actually have email integration? 


## Implementation: 
- Implement modals/some kind of popup for confirmations/errors etc. 
- Fix changing Genre/Source
- Add Validation to all forms
  - Address line contains letters and numbers
  - Emails 
  - Post code contains only numbers
  - State selected from dropdown list
- Create a conistent color theme for store (use bootstrap color variables? i.e. primary/secondary warning etc.)
- Ensure page is responsive to different screen sizes
- Implement tooltips 

## Documentation:
- UI Wireframes
- ER Diagram 