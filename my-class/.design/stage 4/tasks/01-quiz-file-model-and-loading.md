# Task 01 - Quiz File Model and Loading

## Goal

Update quiz loading to match the real `.assets/quiz` file structure and populate in-memory Quiz and QuizQuestion values.

## Work

- Load the Quiz object when the teacher opens the Quiz page.
- Read root `quiz.json` from the configured `Quiz:RootFolder`.
- Map root `quiz.json` fields:
  - `title` to `Quiz.Title`
  - `TimeLimitSeconds` to `Quiz.TimeLimitSeconds`
- Discover question subfolders under `Quiz:RootFolder`.
- Keep question order sorted by subfolder name.
- Require each question subfolder to contain `q.jpg`.
- Require each question subfolder to contain `a.json`.
- Map question `a.json` fields:
  - string `correctAnswer` to `QuizQuestion.CorrectAnswer`
  - optional `TimeLimitSeconds` to `QuizQuestion.TimeLimitSeconds`
- Apply timeout precedence:
  - `Quiz.TimeLimitSeconds` is the default
  - `QuizQuestion.TimeLimitSeconds` overrides the quiz default when present

## Deliverables

- Updated Quiz and QuizQuestion content models
- Updated quiz content loading service
- Validation for `quiz.json`, `q.jpg`, and `a.json`
- Safe image reference for `q.jpg`

## Acceptance Criteria

- Root `quiz.json` with `title` and `TimeLimitSeconds` loads successfully.
- Missing or invalid root `quiz.json` returns a clear error.
- Question folders are discovered and ordered by folder name.
- Missing `q.jpg` or `a.json` returns a clear error.
- `correctAnswer` is loaded as a string.
- Question-specific `TimeLimitSeconds` overrides root `TimeLimitSeconds`.
- Questions without `TimeLimitSeconds` use the root default.

## Notes

- Use `title`, not `name`, for root quiz title.
- Keep file access behind the quiz content service.
