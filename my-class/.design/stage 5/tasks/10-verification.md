# Task 10 - Verification

## Goal

Verify Stage 5 configurable answer counts and SignalR refresh behavior.

## Work

- Build the solution.
- Verify quiz loading with omitted, root-level, and question-level `answerCount`.
- Verify invalid `answerCount` and invalid `correctAnswer` handling.
- Verify student button rendering for configured answer counts.
- Verify server-side answer validation.
- Verify SignalR refresh after teacher quiz state changes.
- Verify fallback/reconnect behavior remains acceptable.

## Deliverables

- Successful build.
- Manual verification notes.
- Any focused automated tests that fit the current project structure.

## Acceptance Criteria

- `dotnet build .\MyClass.slnx` succeeds after stopping any locked running app processes.
- Missing `answerCount` everywhere preserves four-answer behavior.
- Root `answerCount` applies to questions without overrides.
- Question `answerCount` overrides root `answerCount`.
- Invalid configuration produces clear loading errors.
- Student page renders the configured number of buttons.
- Out-of-range submissions are rejected server-side.
- SignalR refreshes connected student pages after teacher start, restart, finish, and next.
- Existing quiz answer behavior from Stage 4 does not regress.

## Notes

- If build output files are locked by a running app or Visual Studio, stop those processes before final verification.

