# Task 02 - Denormalized QuizAnswers Table

## Goal

Replace live quiz answer persistence with one denormalized `QuizAnswers` table representing only the current live quiz.

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
- Stop using session tables for live quiz behavior.

## Deliverables

- Updated `QuizAnswer` entity/table shape
- Updated DbContext mapping
- Compatibility/startup database updates for local SQLite
- Service queries updated to use the denormalized table

## Acceptance Criteria

- `QuizAnswers` can represent all active students for the current question.
- `Answer` is stored as a string.
- `CorrectAnswer` is stored as a string.
- Empty `Answer` is valid storage and means no answer was submitted.
- Empty `Answer` produces `IsCorrect = false`.
- Old session tables are not queried or written by Stage 4 quiz flow.

## Notes

- Existing old quiz tables may remain in the database.
- Stage 4 should use the denormalized `QuizAnswers` table as the live source of truth.
