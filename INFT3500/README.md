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
~~5. Do we need to actually have email integration?~~
5. I have implemented a automatic password reset, is this enough? 
6. Do we need support adding multiple sources? 

### **TODO:**

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