# Task 02 - Answer Image Loading

## Goal

Load answer images from each question folder so teacher and student pages can swap from question image to answer image.

## Work

- Add answer-image loading support to the quiz content service.
- Accept `a.jpg`.
- Keep current question-image loading behavior unchanged.
- Return clear failure messages when the answer image is missing or unreadable.
- Prevent invalid question keys from escaping the configured quiz root folder.

## Deliverables

- Quiz content service method for answer images.
- Service contract update.

## Acceptance Criteria

- Answer image loads from `a.jpg`.
- Missing answer image returns a warning-friendly failure result.
- Existing `q.jpg` image loading still works.
- Invalid image keys are rejected.

## Notes

- Match the existing image-loading result pattern using `Result<string>`.
