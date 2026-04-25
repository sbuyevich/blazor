# My Class Project
It is tool to help teacher communicate with student in class using browser as interface and local wifi connection. 

## Auth
Auth is not using standard web auth. User registrars with record in database. Each POST request has username to update database.

## Current class selection
Database has School, Class and Student tables. Class has code which should be query param in app loaded url.
App reads shool and class based on class code and show it's names in app header. Teacher works with students only from this current class.

## Roles
There are only 2 roles: teacher and student

### Teacher
- has credentials hardcoded in appsetting
- can see Students in menu to load student grid for the current class

### Student 
- has credentials in Student table
- must be register if not record in table

## Solution
Use Blazor web app with SQLite database.