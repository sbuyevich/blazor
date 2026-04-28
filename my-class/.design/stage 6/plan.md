# Stage 6 Student Question Image Plan

## Summary

Stage 6 shows the current quiz question image, question position, and timer on the student Quiz Answer page. The image appears above the answer buttons, the question label and timer match the teacher quiz page, and all student display state follows teacher-driven quiz movement through the existing SignalR quiz notification flow while keeping the current question status message visible.

## Key Changes

- Extend student quiz answer state:
  - Add nullable current question metadata to `QuizAnswerPageState`, including `QuestionKey` and `QuestionTitle`.
  - Populate that metadata whenever a current question exists for the class.
  - Leave it empty when no question has started or no current question is available.
- Update the student answer panel:
  - Load the current question image through `IQuizContentService.LoadQuestionImageAsync`.
  - Render the image above answer buttons and below the title/status text.
  - Keep the image visible for the current question after submit or finish until the teacher moves to another question.
  - Reset image state when the class changes or there is no current question.
- Reuse existing SignalR behavior:
  - Keep teacher actions as the only source of quiz question movement.
  - Reload student answer state on `QuizStateChanged`.
  - Reload the image when the current question key changes.
  - Keep the polling fallback.
- Add student timer display:
  - Show the current question countdown on the student Quiz Answer page using the same timer style as the teacher quiz page.
  - Keep the timer synchronized from server quiz state and SignalR refreshes.
  - Continue updating locally between refreshes while a question is in progress.
  - Stop the timer at zero when the question finishes or expires.
- Add student question position label:
  - Show a label such as `Question 3 of 4` on the student Quiz Answer page.
  - Match the teacher quiz page label format and one-based numbering.
  - Update the label when the teacher starts, restarts, or moves to another question.

## Public Interfaces / Types

- `QuizAnswerPageState`: add nullable current question metadata, timer state, and question position data for student display.
- No database schema, quiz JSON, route, or teacher UI changes are required.

## Test Plan

- Student page shows the current question image above answer buttons after teacher starts the quiz.
- Student page keeps the existing status message visible.
- Image remains visible after the student submits an answer.
- Image remains visible after the teacher finishes the current question.
- Image changes when the teacher clicks Next.
- Student page shows a timer matching the teacher quiz page for the current question.
- Timer updates after teacher Start, Finish, Restart, and Next through SignalR or fallback polling.
- Student page shows `Question N of M` matching the teacher quiz page for the current question.
- Student page updates through SignalR and still works with the polling fallback.
- Missing image shows a warning without breaking status text or answer buttons.
- `dotnet build .\MyClass.slnx` succeeds once existing locked app processes are stopped.

## Assumptions

- `SingleR` in the BRD means SignalR.
- Current question image means the existing question image loaded by `QuizContentService`, currently `q.jpg` or `q.JPG`.
- Student timer uses the same current-question timing data as the teacher quiz page.
- Student question position uses the same current-question index and quiz question count as the teacher quiz page.
- Students do not get controls to start, finish, or move quiz questions.

## Task Breakdown

- [x] [Task 01 - Student Current Question State](tasks/01-student-current-question-state.md)
- [x] [Task 02 - Student Question Image UI](tasks/02-student-question-image-ui.md)
- [ ] [Task 03 - SignalR Refresh Behavior](tasks/03-signalr-refresh-behavior.md)
- [x] [Task 04 - Student Timer Display](tasks/04-student-timer-display.md)
- [x] [Task 05 - Student Question Position Label](tasks/05-student-question-position-label.md)
- [ ] [Task 10 - Verification](tasks/10-verification.md)
