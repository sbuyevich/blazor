# Task 01 - Quiz Configuration and Content

## Goal

Add file-based quiz configuration and content loading for the single Stage 3 quiz.

## Work

- Add `Quiz:RootFolder` to app configuration.
- Add `Quiz:StatusRefreshMilliseconds` to app configuration with default behavior of `1500` milliseconds.
- Define the root quiz file as `quiz.json` with quiz-level metadata only: `{ "title": "..." }`.
- Discover questions by reading immediate subfolders under `Quiz:RootFolder`.
- Sort discovered question folders alphabetically.
- Require each question subfolder to contain `question.json`.
- Load question metadata from `question.json`: title, timeout seconds, and correct answer.
- Require each question subfolder to contain exactly one JPG question image.
- Validate missing, malformed, or ambiguous quiz content and return clear errors to the UI.

## Deliverables

- Quiz options/configuration model
- Quiz content model
- Quiz content loading service
- Validation for quiz root, root JSON, question folders, question JSON, and JPG files

## Acceptance Criteria

- App can load one configured quiz from `Quiz:RootFolder`.
- Root `quiz.json` does not contain or require a questions array.
- Questions are ordered alphabetically by subfolder name.
- Missing root folder, missing `quiz.json`, invalid JSON, no question folders, missing `question.json`, and zero/multiple JPG images fail with clear messages.
- Valid quiz content exposes title, ordered questions, timeout seconds, correct answer, and a safe image reference for each question.

## Notes

- Do not expose arbitrary filesystem paths to the browser.
- Keep this task focused on reading quiz content, not running quiz sessions.
