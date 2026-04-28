# Task 10 - Verification

## Goal

Verify Stage 6 student question image behavior.

## Work

- Build the solution.
- Verify the teacher/student quiz flow manually.
- Verify SignalR-driven student refresh.
- Verify polling fallback behavior remains acceptable.
- Verify missing-image behavior.

## Deliverables

- Successful build.
- Manual verification notes.
- Any focused automated tests that fit the current project structure.

## Acceptance Criteria

- `dotnet build .\MyClass.slnx` succeeds after stopping any locked running app processes.
- Student page shows the current question image above answer buttons.
- Student page keeps the question status message visible.
- Image remains visible after submit and after teacher finish.
- Image updates when teacher moves to the next question.
- Missing image produces a warning without breaking answer submission.
- Existing answer count and SignalR behavior from Stage 5 does not regress.

## Notes

- Run commands from `my-class/src`.
- If build output files are locked by a running app or Visual Studio, stop those processes before final verification.
