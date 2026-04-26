# Stage 3 Quiz Plan

## Summary
Stage 3 adds a teacher-led live quiz flow. The teacher opens the Quiz page, clicks Start, displays the current question image on the shared screen, and students submit answers from a simple Quiz Answer page with four large buttons. Answers and missed-answer failures are persisted in the database.

## Key Changes
- Add quiz configuration in `appsettings.json`:
  - `Quiz:RootFolder`: folder containing the single configured quiz.
  - `Quiz:StatusRefreshMilliseconds`: teacher grid polling interval, default `1500`.
- Use this file contract:
  - Root `quiz.json`: `{ "title": "..." }`
  - The app discovers questions by reading immediate subfolders under `Quiz:RootFolder`.
  - Question order is the alphabetical order of subfolder names.
  - Each question folder contains `question.json`: `{ "title": "...", "timeoutSeconds": 30, "correctAnswer": 1 }`
  - Each question folder contains exactly one JPG question image.
- Add teacher navigation item `Quiz` under Teacher and student navigation item `Quiz Answer` after login.
- Teacher Quiz page:
  - Loads the single configured quiz.
  - Teacher clicks the Start button:
    - to show the first question
    - to create in-progress answer records for each active student for the current question
    - to show active student status in a grid on the right side of the question image.
  - Shows current question JPG, countdown, Finish, and Next buttons.
  - Shows active students with answered/not answered status only.
  - Auto-refreshes status using configured polling interval.
  - When timeout reaches zero, finishes the question and records failed results for active students who did not answer.
- Student Quiz Answer page:
  - Shows four large buttons labeled `1`, `2`, `3`, `4`.
  - Answer buttons remain clickable in the UI.
  - On submit, the backend validates that an in-progress answer record exists for the current student and current question.
  - If no valid in-progress answer record exists, the submit is rejected and the student sees a waiting/not available message.
  - If a valid in-progress answer record exists, the backend saves the selected answer and prevents duplicate answers for that question.

## Public Interfaces / Types
- Add EF entities/tables:
  - `QuizSession`: current classroom quiz run, class scope, title, status, active question index.
  - `QuizSessionQuestion`: per-session question state, started/finished timestamps, timeout, correct answer.
  - `QuizAnswer`: student answer per session question, selected answer, correctness, submitted timestamp, failure flag for timeout/no answer.
- Add quiz services:
  - `IQuizContentService` reads and validates configured quiz files and serves question image references.
  - `IQuizSessionService` starts/finishes/advances questions and records timeout failures.
  - `IQuizAnswerService` submits student answers and returns current answer-page state.
- Add a local image endpoint or component-safe file serving path for configured JPG files; do not expose arbitrary filesystem paths.
- Keep authorization rules server-side: only teachers can start/finish/advance quizzes, only students in the current class can submit answers.

## Test Plan
- `dotnet build .\MyClass.slnx` succeeds.
- App fails gracefully with a clear message if quiz folder or JSON is missing/invalid.
- Teacher can start the first question and see image, countdown, and active student grid.
- Student can open Quiz Answer page and submit one answer for the active question.
- Duplicate student submissions for the same question are rejected.
- Teacher grid changes from not answered to answered after polling.
- Timeout finishes the question and creates failed results for active non-answering students.
- Finish manually closes submissions and records failures for non-answering active students.
- Next advances to the next question; after the last question, the quiz shows complete.
- Students from another class cannot answer the current class quiz.
- Non-teachers cannot access teacher Quiz controls.

## Assumptions
- Stage 3 supports one configured quiz only.
- Each question subfolder must contain exactly one JPG image.
- Students do not see the question image in their own page; the teacher displays it on the shared screen.
- Teacher status grid shows answered/not answered only, not selected answer or correctness.
- Correctness is stored for results/failure tracking but not shown live in the teacher grid.
- Auto-refresh uses polling, not SignalR push.

## Task Breakdown

- [x] [Task 01 - Quiz Configuration and Content](tasks/01-quiz-configuration-and-content.md)
- [x] [Task 02 - Quiz Data and Services](tasks/02-quiz-data-and-services.md)
- [x] [Task 03 - Teacher Quiz Page](tasks/03-teacher-quiz-page.md)
- [x] [Task 04 - Student Quiz Answer Page](tasks/04-student-quiz-answer-page.md)
- [x] [Task 05 - Navigation and Security](tasks/05-navigation-and-security.md)
- [ ] [Task 10 - Verification](tasks/10-verification.md)
