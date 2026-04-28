# Task 02 - Student Question Image UI

## Goal

Show the current question image on the student Quiz Answer page above the answer buttons.

## Work

- Inject `IQuizContentService` into `StudentQuizAnswerPanel`.
- Track loaded question key, image data URI, and image loading message in the component.
- Load the image when student answer state has a new current question key.
- Reset image state when the class changes or no current question is available.
- Render the image below the page title/status text and above the answer buttons.
- Show a warning if image loading fails.

## Deliverables

- Updated `StudentQuizAnswerPanel` markup.
- Updated `StudentQuizAnswerPanel` code-behind.
- Any minimal CSS needed to keep the image responsive and consistent with the teacher quiz image.

## Acceptance Criteria

- Student sees the current question image above answer buttons.
- Existing question status message stays visible.
- Image remains visible after submit.
- Image remains visible after the question finishes.
- Image is replaced when the teacher moves to the next question.
- Missing image shows a warning without hiding status text.

## Notes

- Reuse the teacher panel image-loading pattern where practical.
- Keep the student page focused on answering; do not add teacher controls.
