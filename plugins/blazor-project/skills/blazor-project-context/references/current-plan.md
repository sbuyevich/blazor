# Current Plan

## Product Direction

`MyClass` is a classroom app for a teacher and students on a local Wi-Fi network. The current implementation centers on:

- teacher/student access
- class context from links such as `/?c=demo`
- student registration and participation
- live quiz flow backed by SQLite
- MudBlazor UI with interactive server rendering
- SignalR refresh notifications for student quiz pages

## Current Implementation State

Stages 1 through 4 are implemented in the source tree. Stage 5, answer count and SignalR, has implementation tasks marked complete except final verification in:

- `my-class/.design/stage 5/plan.md`
- `my-class/.design/stage 5/tasks/`

The latest open product notes are in:

- `my-class/.design/todo.md`

Current TODO themes:

- add teacher "Show Answer" behavior for quiz questions
- add answer time and correctness columns to student answer grids
- create a teacher-only quiz result page with CSV export from `QuizAnswers`
- create a teacher-only school/class management page with student search
- keep configurable answer counts and SignalR behavior aligned with quiz content

## Durable Constraints

- Preserve custom authentication; do not introduce ASP.NET Core Identity or cookie auth unless explicitly requested.
- Preserve startup database initialization; do not switch to EF migrations unless explicitly requested.
- Preserve the current quiz data model: `QuizAnswers` stores denormalized live quiz rows and is not connected to quiz session tables.
- Use services in `MyClass.Core\Services` for business rules and keep Razor components focused on rendering and interaction.
- Use `IDbContextFactory<ApplicationDbContext>` in services.
- Avoid mutating local database files unless the task requires seed/current data changes.

## Intended Use

When implementing features, use the relevant `.design` plan/task files as the primary checklist, then update `AGENTS.md`, `README.md`, and this repo-local plugin when durable project guidance changes.
