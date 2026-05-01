# Task 10 - Verification

## Goal

Verify the Stage 8 show-answer flow across teacher and student pages.

## Work

- Build the solution.
- Verify answer image loading.
- Verify teacher reveal flow.
- Verify student reveal flow.
- Verify configured messages.
- Verify SignalR refresh behavior.
- Verify missing answer image behavior.

## Deliverables

- Successful build.
- Manual verification notes.
- Any focused automated tests that fit the current project structure.

## Acceptance Criteria

- `dotnet build .\MyClass.slnx` succeeds.
- Teacher can click `Show Answer`.
- Teacher image changes from question image to answer image.
- Teacher grid shows answer time and correctness.
- Student page changes from question image to answer image.
- Student sees the configured message for correct, incorrect, or no answer.
- Student page updates via SignalR and remains correct after refresh.
- Missing answer image shows a clear warning without breaking quiz flow.
- Existing start, finish, next, restart, timer, and answer-submit behavior still works.

## Notes

- Run commands from `my-class/src`.
- If build output files are locked by a running app or Visual Studio, stop those processes before final verification.
