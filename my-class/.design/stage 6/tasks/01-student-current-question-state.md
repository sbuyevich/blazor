# Task 01 - Student Current Question State

## Goal

Expose the current quiz question metadata needed by the student Quiz Answer page.

## Work

- Add nullable current question metadata to `QuizAnswerPageState`.
- Include at least `QuestionKey` for image loading.
- Include `QuestionTitle` for image alt text or accessible labeling.
- Populate the metadata in `QuizAnswerService.GetAnswerPageStateAsync` whenever the class has a current question.
- Preserve existing answer choices, status messages, and submit-state behavior.

## Deliverables

- Updated `QuizAnswerPageState` model.
- Updated student answer state creation in `QuizAnswerService`.

## Acceptance Criteria

- Current question metadata is present while a current question exists.
- Metadata remains present after a student answers the current question.
- Metadata remains present after the current question finishes.
- Metadata is empty before the teacher starts a question.
- Existing status messages still come from the answer state.

## Notes

- Do not add database fields or quiz JSON fields.
- Use the existing current-question lookup in `QuizAnswerService`.
