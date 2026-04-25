# Task 02 - Data and Database

## Goal

Add the SQLite-backed EF Core data layer for schools, classes, and students.

## Work

- Add `ApplicationDbContext`.
- Add entities for `School`, `Class`, and `Student`.
- Configure relationships:
  - a school has many classes
  - a class belongs to one school
  - a class has many students
  - a student belongs to one class
- Configure a unique class code.
- Configure duplicate student usernames to be rejected within the same class.
- Register `IDbContextFactory<ApplicationDbContext>` with SQLite.
- Add startup database creation or migrations for the app's own database.
- Add minimal seed data so a valid class code exists for testing.

## Deliverables

- EF Core entities
- `ApplicationDbContext`
- SQLite configuration
- Migration or startup database initialization
- Seed school/class data

## Acceptance Criteria

- The app creates or migrates its own SQLite database successfully.
- `School`, `Class`, and `Student` tables exist.
- Class codes are unique.
- Student usernames cannot be duplicated inside the same class.
- Data is stored in the `my-class` database, not the `student-projects` database.

## Notes

- Final table fields are still `TBD`; keep v1 fields minimal and implementation-focused.
- If student passwords or secrets are used, store hashes rather than plain text.
