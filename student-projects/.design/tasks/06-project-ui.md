# Task 06 - Project UI

## Goal
Build the authenticated Blazor pages for managing projects.

## Work
- Create a protected `/projects` page that lists the current user's projects.
- Create a protected `/projects/new` page with a form for:
  - `Name`
  - `Description`
- Create a protected `/projects/{id}/edit` page for updating owned projects.
- Add delete behavior with confirmation from the project list or edit flow.
- Wire the pages to `IProjectService`.
- Keep components focused on presentation, form handling, and navigation.

## Deliverables
- `/projects`
- `/projects/new`
- `/projects/{id}/edit`
- working create, edit, and delete flows

## Acceptance Criteria
- Logged-in users can create a project with name and description.
- Logged-in users can view their own projects.
- Logged-in users can edit and delete their own projects.
- Pages do not call a separate HTTP API.

## Notes
- The UI should stay minimal and functional.
- Data access must remain in services rather than page components.
