# Task 10 - Verification

## Goal

Verify Stage 4 quiz reading and answer saving end to end.

## Work

- Build the solution.
- Verify quiz file loading from `.assets/quiz`.
- Verify root `TimeLimitSeconds` default.
- Verify question-level `TimeLimitSeconds` override.
- Verify teacher Start, Finish, Next, and timeout behavior.
- Verify denormalized `QuizAnswers` records.
- Verify student answer submission and duplicate prevention.
- Verify role and class-scope protections.

## Deliverables

- Successful build
- Manual verification notes
- Any focused automated tests that fit the current project structure

## Acceptance Criteria

- `dotnet build .\MyClass.slnx` succeeds with no warnings or errors.
- Root `quiz.json` with `title` and `TimeLimitSeconds` loads successfully.
- Question `a.json` with no `TimeLimitSeconds` uses root default.
- Question `a.json` with `TimeLimitSeconds` overrides root default.
- Start quiz clears old `QuizAnswers` rows.
- Start quiz creates rows for first question and active students.
- Next creates rows for the next question and active students.
- Student answer updates only that student/current-question row.
- `Answer` and `CorrectAnswer` are compared as strings.
- Empty `Answer` is marked incorrect.
- Timeout leaves unanswered rows with empty `Answer` and `IsCorrect = false`.
- Wrong-role and wrong-class submissions are rejected server-side.

## Notes

- Keep verification scoped to Stage 4 plus regressions in Stage 3 quiz UI flow.
