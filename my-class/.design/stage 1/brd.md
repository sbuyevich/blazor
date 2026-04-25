# My Class Project

My Class is a Blazor Web App that helps teachers communicate with students during class. Teachers and students use a browser interface while connected to the same local Wi-Fi network. The application uses a SQLite database.

The UI uses MudBlazor components.

## Authentication

Authentication does not use ASP.NET Core Identity or cookie authentication.

The app uses one shared login page for both teachers and students. The login page is available at `/login`; `/` also currently renders the login page so a class-link URL such as `/?c=demo` can initialize the current class context and present login immediately.

After login, the app stores login state in browser `localStorage` under `my-class.loginState` and carries an `IsTeacher` flag across the app. The browser state is a convenience state only. Server-side services are responsible for enforcing role and class scope when they perform database work.

## Registration

Students self-register at `/register` when they do not already have a matching username in the current class.

Registration captures first name, last name, username, password, and password confirmation. Student passwords are stored as PBKDF2-SHA256 hashes. Successful registration writes a student login state with `IsTeacher = false`.

Teacher credentials are configured in app settings.

## Current Class Selection

The database includes `School`, `Class`, and `Student` tables.

Each class has a code. The class code is provided as the `c` query parameter on the landing page URL.

When the landing page loads, the app reads the school and class from the class code, stores the current class context for later pages, and shows the school and class names in the app header. The teacher works only with students from the current class.

If the class code is missing or invalid and no class context has already been loaded, the app shows an error message through the shared class-context view and app header. There is not a separate `/class-error` page in the current code.

## Roles

There are two roles: teacher and student.

### Teacher

- The app has only one teacher account.
- Uses credentials from app settings.
- Logs in through the shared login page.
- Has `IsTeacher` set to `true` after login.
- Can see the Teacher menu section and Students menu item after login.
- The `/students` page currently exists as a placeholder; the actual current-class student grid is still task 06.

### Student

- Uses credentials from the `Student` table.
- Self-registers if no matching student record exists.
- Student capabilities are `TBD`.

## Data Model

Known tables:

- `School`
- `Class`
- `Student`

Current fields:

- `School`: `Id`, `Name`, `Classes`
- `Class`: `Id`, `SchoolId`, `Code`, `Name`, `School`, `Students`
- `Student`: `Id`, `ClassId`, `UserName`, `FirstName`, `LastName`, `DisplayName`, `PasswordHash`, `CreatedAtUtc`, `Class`

Current constraints:

- `School.Name` is required with max length 200.
- `Class.Code` is required, max length 50, and unique.
- `Class.Name` is required with max length 200.
- `Student.UserName` is required with max length 100.
- `Student.FirstName` and `Student.LastName` are required with max length 100.
- `Student.DisplayName` is required with max length 200.
- `Student.PasswordHash` is required with max length 500.
- `Student` has a unique index on `ClassId` and `UserName`.

Database creation currently uses EF Core `EnsureCreatedAsync()` and seeds a `Demo School` / `Demo Class` with class code `demo` when the database is empty. EF migrations are not present yet.
