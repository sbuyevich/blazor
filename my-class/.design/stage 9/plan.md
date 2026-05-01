# Stage 9 Select Quiz by Teacher Plan

## Summary

Stage 9 lets the teacher choose which quiz is active. The app discovers quizzes from `Quiz:RootFolder`, lists valid quiz folders in a Teacher Quiz page dropdown, stores the teacher's selected quiz in browser session storage, and uses the selected quiz for the shared teacher/student quiz flow.

## Key Changes

- Discover valid quiz folders from the configured quiz root folder.
- Read each quiz folder's `quiz.json` metadata.
- Add selected quiz state and session-storage support.
- Update quiz content loading to use the selected quiz folder instead of a single fixed quiz folder.
- Add a teacher-facing quiz dropdown at the top of the Teacher Quiz page.
- Ensure students see the quiz selected by the teacher for the active quiz session.
- Add warnings for missing, invalid, or unavailable quiz selections.

## Quiz Metadata

Each direct child folder under `Quiz:RootFolder` can be a quiz folder. A valid quiz folder must contain `quiz.json`.

Required metadata fields:

- `name`: display name used in the teacher dropdown.
- `title`: quiz title shown on quiz pages.
- `timeLimitSeconds`: default time limit for questions.

Example:

```json
{
  "name": "Astronomy - easy",
  "title": "How good you know Astronomy?",
  "timeLimitSeconds": 30
}
```

## Public Interfaces / Types

- Add a quiz summary model for discovered quizzes.
- Add service support for listing available quizzes.
- Add service support for loading quiz content/images from a selected quiz folder.
- Add browser session-storage support for the selected teacher quiz path.
- Add enough active quiz-session state so students load the teacher-selected quiz.

## Test Plan

- Teacher sees a quiz dropdown with valid discovered quizzes.
- Invalid quiz folders are skipped without breaking the page.
- Teacher selection survives refresh in the same browser session.
- If a saved selected quiz no longer exists, the first valid quiz is selected.
- Start/restart uses the selected quiz.
- Student pages load the same quiz selected by the teacher for the active session.
- Question image and answer image loading continue to work from the selected quiz folder.
- Missing/invalid selected quiz shows a warning and prevents quiz start.
- Existing start, finish, show answer, next, restart, timer, SignalR, and polling behavior does not regress.
- `dotnet build .\MyClass.slnx` succeeds.

## Assumptions

- Students do not choose quizzes.
- Teacher quiz selection is stored in browser session storage for page refresh continuity.
- The active quiz used by students must come from shared quiz-session state created when the teacher starts/restarts a quiz.
- Existing question and answer image naming rules continue inside each selected quiz folder.
- `Quiz:RootFolder` points to the parent folder that contains all quiz folders.

## Task Breakdown

- [ ] [Task 01 - Quiz Discovery and Metadata](tasks/01-quiz-discovery-and-metadata.md)
- [ ] [Task 02 - Selected Quiz Session Storage](tasks/02-selected-quiz-session-storage.md)
- [ ] [Task 03 - Selected Quiz Content Loading](tasks/03-selected-quiz-content-loading.md)
- [ ] [Task 04 - Teacher Quiz Dropdown](tasks/04-teacher-quiz-dropdown.md)
- [ ] [Task 05 - Student Uses Active Teacher Quiz](tasks/05-student-uses-active-teacher-quiz.md)
- [ ] [Task 10 - Verification](tasks/10-verification.md)
