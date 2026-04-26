# Task 02 - Denormalized QuizAnswers Table

Status: Complete

## Goal

Replace live quiz answer persistence with one denormalized `QuizAnswers` table representing the current active quiz run.

## Work

- Update the live quiz answer table shape to store denormalized answer rows.
- Store student snapshot values:
  - student id
  - first name
  - last name
  - display name
  - username
- Store question snapshot values:
  - question folder/key
  - question title or label
  - question order/index
  - correct answer string
- Store live answer values:
  - start time
  - end time
  - student `Answer` string
  - `IsCorrect`
- Ensure `Answer` can be empty.
- Treat empty `Answer` as incorrect.
- Do not create `QuizSessions` or `QuizSessionQuestions`.
- Use `QuizAnswers` as the active quiz-run source.

## Deliverables

- Updated `QuizAnswer` entity/table shape
- Updated DbContext mapping
- Startup database updates for local SQLite
- Service queries updated to use the denormalized table

## Acceptance Criteria

- `QuizAnswers` can represent all active students for every started question in the active quiz run.
- `Answer` is stored as a string.
- `CorrectAnswer` is stored as a string.
- Empty `Answer` is valid storage and means no answer was submitted.
- Empty `Answer` produces `IsCorrect = false`.
- Stage 4 live answer queries use `QuizAnswers` directly with `QuestionIndex`, `QuestionKey`, and `StudentId`.
- Stage 4 database model does not include `QuizSessions` or `QuizSessionQuestions`.

## Notes

- Stage 4 should use the denormalized `QuizAnswers` table as the active quiz-run source of truth.
