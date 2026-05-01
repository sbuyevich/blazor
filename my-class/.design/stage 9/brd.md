# Select Quiz by Teacher

## Goal

Allow the teacher to choose which quiz is active before starting the quiz flow.

## Quiz Discovery

- `Quiz:RootFolder` in `appsettings.json` points to a folder that contains quiz subfolders.
- Each direct subfolder represents one quiz.
- Each quiz subfolder must contain a `quiz.json` metadata file.
- App reads every valid `quiz.json` under the direct subfolders of `Quiz:RootFolder`.
- App stores discovered quiz metadata together with the quiz folder path.
- Invalid quiz folders are skipped and should not break the page.

Example `quiz.json` metadata:

```json
{
  "name": "Astronomy - easy",
  "title": "How good you know Astronomy?",
  "timeLimitSeconds": 30
}
```

Required metadata fields:

- `name`: display name used in the teacher dropdown.
- `title`: quiz title shown on quiz pages.
- `timeLimitSeconds`: default time limit for questions.

Existing quiz content and image loading continue to work from the selected quiz folder.

## Selection Storage

- Store the selected quiz path in browser session storage for the teacher.
- The selected quiz should remain selected after refreshing the teacher quiz page in the same browser session.
- If no quiz is selected yet, use the first discovered valid quiz as the default.
- The selected quiz is used by the existing quiz services when the teacher starts/restarts/questions the quiz.

## Teacher Quiz Page

- Add a `Quiz` dropdown at the top of the Teacher Quiz page.
- The dropdown lists discovered quizzes by `name`.
- Changing the dropdown changes the current selected quiz.
- Starting or restarting the quiz uses the selected quiz.
- Existing teacher quiz behavior should continue after a quiz is selected:
  - start question
  - finish question
  - show answer
  - next question
  - restart quiz

## Student Behavior

- Students see the quiz selected by the teacher for the active quiz session.
- When the teacher starts/restarts using a selected quiz, student pages use that same quiz content and images.
- Students do not choose quizzes.

## Error / Empty States

- If no valid quizzes are found, the Teacher Quiz page shows a warning and disables quiz start.
- If the saved selected quiz path no longer exists, fall back to the first valid discovered quiz.
- If the selected quiz cannot be loaded, show a warning and do not start the quiz.

