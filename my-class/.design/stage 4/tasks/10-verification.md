# Task 10 - Verification

Status: Complete

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
- Question `q.json` with no `TimeLimitSeconds` uses root default.
- Question `q.json` with `TimeLimitSeconds` overrides root default.
- Start quiz clears old `QuizAnswers` rows.
- Start quiz creates rows for first question and active students.
- Next creates rows for the next question and active students without deleting previous question rows.
- Student answer updates only that student/current-question row.
- `Answer` and `CorrectAnswer` are compared as strings.
- Empty `Answer` is marked incorrect.
- Timeout leaves unanswered rows with empty `Answer` and `IsCorrect = false`.
- Wrong-role and wrong-class submissions are rejected server-side.

## Notes

- Keep verification scoped to Stage 4 plus regressions in Stage 3 quiz UI flow.

## Verification Notes

- `dotnet build .\MyClass.slnx --no-restore -p:UseAppHost=false -p:DebugType=none -p:OutputPath=bin\codex-check\` succeeded with 0 warnings and 0 errors.
- `.assets/quiz/quiz.json` loads with `title = Test Quiz` and `TimeLimitSeconds = 10`.
- Question folders `01` through `04` each contain exactly one `q.jpg`/`q.JPG` and one `q.json`.
- Question `01` defines `TimeLimitSeconds = 5`, verifying the question-level override case.
- Questions `02`, `03`, and `04` omit `TimeLimitSeconds`, verifying root default coverage.
- All question `correctAnswer` values are strings in the supported `1` through `4` range.
- Static service verification confirms teacher Start clears `QuizAnswers`, while Next appends new question rows through `CreateQuestionRowsAsync`.
- Static service verification confirms Finish/timeout update only current question rows and set `IsCorrect` by string comparison.
- Static service verification confirms student submit accepts a string answer, rejects duplicate/currently closed rows, and writes `Answer`, `EndedAtUtc`, and `IsCorrect`.
- Static service verification confirms teacher and student service methods keep server-side role/class checks.
- Browser-driven manual UI verification was not run in this pass.
