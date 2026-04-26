# Task 04 - Student Answer Flow Updates

Status: Complete

## Goal

Update student answer submission to write the student's answer string into the current denormalized answer row.

## Work

- Student button click submits answer value as a string.
- Backend finds the current student's current-question `QuizAnswers` row.
- Backend rejects submit when no current row exists.
- Backend rejects duplicate submit when `Answer` is already set.
- Backend writes `Answer`, end time, and `IsCorrect`.
- Backend compares student `Answer` string to `CorrectAnswer` string.
- Backend treats empty `Answer` as incorrect.
- Student answer-page state reads from denormalized answer rows.

## Deliverables

- Updated answer submit service
- Updated answer-page state service
- Updated student page behavior where needed

## Acceptance Criteria

- Student answer `1`, `2`, `3`, or `4` is saved as a string.
- Student submit updates only that student/current-question row.
- Submit without a current row is rejected with a clear message.
- Duplicate submit for the same question is rejected.
- Matching `Answer` and `CorrectAnswer` sets `IsCorrect = true`.
- Non-matching or empty `Answer` sets `IsCorrect = false`.
- Wrong-role and wrong-class submissions remain rejected.

## Notes

- UI buttons remain clickable; backend validation is authoritative.
- Student `Answer` can be empty only for timeout/no-answer cases.
