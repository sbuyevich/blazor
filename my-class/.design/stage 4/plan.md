# Stage 4 Quiz Reading And Answer Saving Plan

## Summary
Stage 4 aligns quiz loading with the real `.assets/quiz` file structure and replaces the session-based answer model with one denormalized live `QuizAnswers` table. The teacher loads one quiz into memory when opening the Quiz page. Starting a quiz clears prior answers, and each newly started question creates answer rows for all active students.

## Key Changes
- Update quiz file reading:
  - Root `quiz.json` maps to `Quiz` with `title` and `TimeLimitSeconds`.
  - Question subfolders contain `q.jpg` and `a.json`.
  - `a.json` maps to `QuizQuestion` with string `correctAnswer` and optional `TimeLimitSeconds`.
  - `Quiz.TimeLimitSeconds` is the default; `QuizQuestion.TimeLimitSeconds` overrides it when present.
- Update answer storage:
  - Use one denormalized `QuizAnswers` table for the current live quiz only.
  - Truncate/clear `QuizAnswers` when teacher clicks Start quiz.
  - For any newly started question, create one `QuizAnswers` row per active student.
  - Store student first name, last name, display name, question identifier/title, start time, end time, `Answer`, `CorrectAnswer`, and `IsCorrect`.
  - Student `Answer` is a string and may be empty when time expires before answer submission.
  - `correctAnswer` is a string and is matched against the student `Answer` string.
  - Empty `Answer` is treated as incorrect.
- Update quiz flow:
  - Start quiz clears old rows and starts the first question.
  - Next starts the next question and creates rows for that question.
  - Finish/timeout updates end time, empty answer where needed, and `IsCorrect`.
  - Stop using session tables for live quiz behavior; existing old tables may remain but should no longer be queried or written by the quiz flow.

## Public Interfaces / Types
- Add/update in-memory quiz models:
  - `Quiz`: `Title`, `TimeLimitSeconds`, ordered `QuizQuestion` list.
  - `QuizQuestion`: folder key, image path/reference, `CorrectAnswer`, effective `TimeLimitSeconds`.
- Replace answer persistence contract with denormalized answer records:
  - `QuizAnswer`: student snapshot fields, question snapshot fields, `Answer`, `CorrectAnswer`, `IsCorrect`, start/end timestamps.
- Update services so teacher and student pages use the new live `QuizAnswers` table rather than session-based tables.

## Test Plan
- Root `quiz.json` with `title` and `TimeLimitSeconds` loads successfully.
- Question `a.json` with no `TimeLimitSeconds` uses root default.
- Question `a.json` with `TimeLimitSeconds` overrides root default.
- `correctAnswer` string matches submitted answer string.
- Start quiz clears previous `QuizAnswers` rows and creates first-question rows for active students.
- Next creates rows for each active student for the new question.
- Student answer updates only that student/current-question row.
- Timeout leaves unanswered student `Answer` empty and marks `IsCorrect = false`.
- Wrong-role and wrong-class submissions remain rejected server-side.
- `dotnet build .\MyClass.slnx` succeeds.

## Assumptions
- Use `title`, not `name`, in root `quiz.json`.
- Use the correctly spelled class names `Quiz` and `QuizQuestion`.
- `a.json` is the source of answer-validation data.
- `q.jpg` is the displayed question image.
- Stage 4 supports one live quiz session at a time.

## Task Breakdown

- [ ] [Task 01 - Quiz File Model and Loading](tasks/01-quiz-file-model-and-loading.md)
- [ ] [Task 02 - Denormalized QuizAnswers Table](tasks/02-denormalized-quizanswers-table.md)
- [ ] [Task 03 - Teacher Quiz Flow Updates](tasks/03-teacher-quiz-flow-updates.md)
- [ ] [Task 04 - Student Answer Flow Updates](tasks/04-student-answer-flow-updates.md)
- [ ] [Task 10 - Verification](tasks/10-verification.md)
