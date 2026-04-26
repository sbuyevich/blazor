# Stage 4 Quiz Reading And Answer Saving Plan

## Summary
Stage 4 aligns quiz loading with the real `.assets/quiz` file structure and uses one denormalized active-run `QuizAnswers` table. The teacher loads one quiz into memory when opening the Quiz page. Starting a quiz clears prior answers once, and each newly started question adds answer rows for all active students.

## Key Changes
- Update quiz file reading:
  - Root `quiz.json` maps to `Quiz` with `title` and `TimeLimitSeconds`.
  - Question subfolders contain `q.jpg` and `q.json`.
  - `q.json` maps to `QuizQuestion` with string `correctAnswer` and optional `TimeLimitSeconds`.
  - `Quiz.TimeLimitSeconds` is the default; `QuizQuestion.TimeLimitSeconds` overrides it when present.
- Update answer storage:
  - Use one denormalized `QuizAnswers` table for the current active quiz run.
  - Truncate/clear `QuizAnswers` when teacher clicks Start quiz.
  - For any newly started question, add one `QuizAnswers` row per active student.
  - Store student first name, last name, display name, question identifier/title, start time, end time, `Answer`, `CorrectAnswer`, and `IsCorrect`.
  - Student `Answer` is a string and may be empty when time expires before answer submission.
  - `correctAnswer` is a string and is matched against the student `Answer` string.
  - Empty `Answer` is treated as incorrect.
- Update quiz flow:
  - Start quiz clears old rows and starts the first question.
  - Next starts the next question and appends rows for that question.
  - Finish/timeout updates end time, empty answer where needed, and `IsCorrect`.
  - Do not use or create `QuizSessions` or `QuizSessionQuestions`; active quiz state comes from `QuizAnswers`.

## Public Interfaces / Types
- Add/update in-memory quiz models:
  - `Quiz`: `Title`, `TimeLimitSeconds`, ordered `QuizQuestion` list.
  - `QuizQuestion`: folder key, image path/reference, `CorrectAnswer`, effective `TimeLimitSeconds`.
- Replace answer persistence contract with denormalized answer records:
  - `QuizAnswer`: student snapshot fields, question snapshot fields, `Answer`, `CorrectAnswer`, `IsCorrect`, start/end timestamps.
- Update services so teacher and student pages use `QuizAnswers` directly.

## Test Plan
- Root `quiz.json` with `title` and `TimeLimitSeconds` loads successfully.
- Question `q.json` with no `TimeLimitSeconds` uses root default.
- Question `q.json` with `TimeLimitSeconds` overrides root default.
- `correctAnswer` string matches submitted answer string.
- Start quiz clears previous `QuizAnswers` rows and creates first-question rows for active students.
- Next creates rows for each active student for the new question and preserves previous question rows.
- Student answer updates only that student/current-question row.
- Timeout leaves unanswered student `Answer` empty and marks `IsCorrect = false`.
- Wrong-role and wrong-class submissions remain rejected server-side.
- `dotnet build .\MyClass.slnx` succeeds.

## Assumptions
- Use `title`, not `name`, in root `quiz.json`.
- Use the correctly spelled class names `Quiz` and `QuizQuestion`.
- `q.json` is the source of answer-validation data.
- `q.jpg` is the displayed question image.
- Stage 4 supports one active quiz run at a time.

## Task Breakdown

- [x] [Task 01 - Quiz File Model and Loading](tasks/01-quiz-file-model-and-loading.md)
- [x] [Task 02 - Denormalized QuizAnswers Table](tasks/02-denormalized-quizanswers-table.md)
- [x] [Task 03 - Teacher Quiz Flow Updates](tasks/03-teacher-quiz-flow-updates.md)
- [x] [Task 04 - Student Answer Flow Updates](tasks/04-student-answer-flow-updates.md)
- [x] [Task 10 - Verification](tasks/10-verification.md)
