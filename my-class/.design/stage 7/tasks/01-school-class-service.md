# Task 01 - School/Class Service

## Goal

Add the service layer needed for the School/Class page to manage schools, classes, and selected-class students without depending on the current class context.

## Work

- Add `ISchoolClassService` under `MyClass.Core\Services\SchoolClass\Contracts`.
- Add `SchoolClassService` under `MyClass.Core\Services\SchoolClass`.
- Register the service in `Program.cs`.
- Add lightweight models/results for:
  - school options with class counts
  - class options with student counts
  - selected-class student rows
  - list/action results following the existing `Succeeded`, `Message`, payload style
- Implement teacher authorization from `LoginState` and `TeacherOptions`.
- Implement school operations:
  - list all schools ordered by name
  - create school from a trimmed required name
  - rename school from a trimmed required name
  - delete school only when it has no classes
- Implement class operations:
  - list classes filtered by `SchoolId`
  - create class for a valid school with required trimmed name and code
  - update class name/code
  - normalize class codes consistently with existing class context lookup
  - reject duplicate class codes
  - delete class only when it has no students
- Implement student operations:
  - list students for selected `ClassId`
  - filter by trimmed `DisplayName` search text
  - remove a student only when the student belongs to the selected `ClassId`

## Deliverables

- New service contract and implementation.
- New result/view model records.
- DI registration in the web project.

## Acceptance Criteria

- Service methods do not require a `ClassContext`.
- Non-teacher and unauthenticated calls return failure results.
- School and class names cannot be blank.
- Class codes cannot be blank and are stored in a normalized form.
- Duplicate class codes are rejected before or during save.
- Deleting a school with classes fails with a clear message.
- Deleting a class with students fails with a clear message.
- Removing a student deletes only a student from the requested selected class.

## Notes

- Do not add migrations or database schema changes.
- Do not edit the local SQLite database directly.
- Use `IDbContextFactory<ApplicationDbContext>`.
