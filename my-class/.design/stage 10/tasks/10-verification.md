# Task 10 - Verification

## Goal

Verify Stage 10 Quiz Result behavior and confirm it does not disturb existing quiz flows.

## Work

- Build the solution.
- Manually verify teacher-only access.
- Manually verify empty state.
- Manually verify student and question summary calculations.
- Manually verify sorting on both grids.
- Manually verify CSV export.
- Manually verify existing quiz flow behavior.

## Deliverables

- Successful build.
- Manual verification notes.
- Any focused automated tests that fit the current project structure.

## Acceptance Criteria

- `dotnet build .\MyClass.slnx` succeeds after stopping any locked running app processes.
- Teacher can access `/quiz-result`.
- Student and unauthenticated users cannot use the page.
- Empty active quiz results show an empty state and disabled export.
- Student summary correct, incorrect, percent, and time totals are correct.
- Question summary correct, incorrect, percent, and time totals are correct.
- No-answer/timeouts count as incorrect and add full question time to aggregate time.
- Submitted answers use elapsed time from `StartedAtUtc` to `EndedAtUtc`.
- CSV export includes only current class rows and required columns.
- Existing start, restart, finish, show answer, next, timer, SignalR, and polling behavior does not regress.

## Notes

- Run commands from `my-class/src`.
- If build output files are locked by a running app or Visual Studio, stop those processes before final verification.
