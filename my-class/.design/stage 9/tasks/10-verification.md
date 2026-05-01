# Task 10 - Verification

## Goal

Verify Stage 9 behavior and regression-sensitive quiz flows.

## Work

- Verify quiz discovery with multiple valid quiz folders.
- Verify invalid quiz folders are skipped.
- Verify teacher dropdown default selection.
- Verify selected quiz path survives teacher page refresh.
- Verify removed saved quiz path falls back to the first valid quiz.
- Verify teacher start/restart uses selected quiz.
- Verify students see teacher-selected quiz content and images.
- Verify answer reveal still swaps to the selected quiz answer image.
- Verify no valid quizzes disables start and shows a warning.
- Run `dotnet build .\MyClass.slnx`.

## Acceptance Criteria

- All manual verification items pass.
- Build succeeds.
- Existing Stage 8 quiz behavior does not regress.
