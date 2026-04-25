# My Class Project

My Class is a Blazor Web App that helps teachers communicate with students during class. Teachers and students use a browser interface while connected to the same local Wi-Fi network. The application uses a SQLite database.

## Authentication

Authentication does not use standard web authentication.

The app uses one shared login page for both teachers and students. After login, the app stores login state in browser `localStorage` and carries an `IsTeacher` flag across the app.

Each POST request includes the username so database updates can be associated with the current user.

## Registration

Students self-register when they do not already have a matching record in the database.

Teacher credentials are configured in app settings.

## Current Class Selection

The database includes `School`, `Class`, and `Student` tables.

Each class has a code. The class code is provided as a query parameter in the app URL.

When the app loads, it reads the school and class from the class code and shows their names in the app header. The teacher works only with students from the current class.

If the class code is missing or invalid, the app shows an error message.

## Roles

There are two roles: teacher and student.

### Teacher

- Uses credentials from app settings.
- Logs in through the shared login page.
- Has `IsTeacher` set to `true` after login.
- Can open the Students menu and view the student grid for the current class.

### Student

- Uses credentials from the `Student` table.
- Self-registers if no matching student record exists.
- Student capabilities are `TBD`.

## Data Model

Known tables:

- `School`
- `Class`
- `Student`

Full table fields, relationships, and validation rules are `TBD`.
