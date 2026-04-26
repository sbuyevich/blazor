# Task 03 - Teacher Quiz Flow Updates

Status: Complete

## Goal

Update teacher quiz controls to use the in-memory quiz model and denormalized live answer rows.

## Work

- Start quiz clears/truncates existing `QuizAnswers` rows.
- Start quiz loads the in-memory Quiz object.
- Start quiz starts the first question.
- For each newly started question, create one `QuizAnswers` row per active student.
- Store current question start time on each created answer row.
- Finish current question updates end time for current question rows.
- Finish current question sets empty `Answer` where students did not answer.
- Finish current question calculates `IsCorrect`.
- Timeout follows the same behavior as Finish.
- Next starts the next question and creates rows for active students.
- Teacher status grid reads from denormalized `QuizAnswers`.

## Deliverables

- Updated teacher quiz service flow
- Updated teacher status model/query
- Updated timeout/finish handling
- Teacher grid backed by denormalized answer rows

## Acceptance Criteria

- Start quiz removes previous live answer rows.
- Start quiz creates first-question rows for all active students.
- Next creates new rows for all active students for the next question.
- Finish updates end time and correctness for current question rows.
- Timeout updates end time and correctness for current question rows.
- Teacher grid shows answered/not answered based on `Answer`.
- Teacher grid does not use old session tables.

## Notes

- Stage 4 supports one live quiz session at a time.
- Teacher still displays question image on the shared screen.
