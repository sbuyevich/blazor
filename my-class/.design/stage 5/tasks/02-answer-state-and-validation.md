# Task 02 - Answer State and Validation

## Goal

Expose configured answer choices to the student answer page and validate submitted answers against the current question.

## Work

- Add `IReadOnlyList<string> AnswerChoices` to `QuizAnswerPageState`.
- Populate `AnswerChoices` from the effective current question answer count.
- Include answer count in the current-question lookup used by `QuizAnswerService`.
- Replace hard-coded submit validation for `1`, `2`, `3`, and `4`.
- Reject submitted answers that are not numeric strings in the configured range for the current question.

## Deliverables

- Updated `QuizAnswerPageState` contract.
- Updated `QuizAnswerService` state loading.
- Updated `QuizAnswerService.SubmitAnswerAsync` validation.

## Acceptance Criteria

- Student answer state returns exactly `"1"` through the effective answer count when a question is answerable.
- Student answer state returns an empty answer list when no question is available.
- Submitting an out-of-range answer is rejected server-side.
- Existing duplicate-answer, expired-question, wrong-role, and wrong-class protections still apply.

## Notes

- No database change is required.
- Stored `Answer` and `CorrectAnswer` remain strings.

