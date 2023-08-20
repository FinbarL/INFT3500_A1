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


Questions: 

1. Are the items listed in the business scenario the only ones we need to cover? 
2. What does the 'update order details' look like? (UML Diagram)
3. I currently don't allow the user to update the genre, is this okay? 
4. Whats the actual MVP? Is it just the business scenarios?


TODO:
- Admins can create new users, include setting isAdmin and isStaff
- Email verification and account recovery (forgot password)
- User Order History
- Show all items in the admin product view 
