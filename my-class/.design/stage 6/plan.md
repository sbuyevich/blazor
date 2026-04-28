# Stage 6 Student Question Image Plan

## Summary

Stage 6 shows the current quiz question image on the student Quiz Answer page. The image appears above the answer buttons, follows teacher-driven quiz movement, and updates through the existing SignalR quiz notification flow while keeping the current question status message visible.

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

## Public Interfaces / Types

- `QuizAnswerPageState`: add nullable current question metadata for student display.
- No database schema, quiz JSON, route, or teacher UI changes are required.

## Test Plan

- Student page shows the current question image above answer buttons after teacher starts the quiz.
- Student page keeps the existing status message visible.
- Image remains visible after the student submits an answer.
- Image remains visible after the teacher finishes the current question.
- Image changes when the teacher clicks Next.
- Student page updates through SignalR and still works with the polling fallback.
- Missing image shows a warning without breaking status text or answer buttons.
- `dotnet build .\MyClass.slnx` succeeds once existing locked app processes are stopped.

## Assumptions

- `SingleR` in the BRD means SignalR.
- Current question image means the existing question image loaded by `QuizContentService`, currently `q.jpg` or `q.JPG`.
- Students do not get controls to start, finish, or move quiz questions.

## Task Breakdown

- [ ] [Task 01 - Student Current Question State](tasks/01-student-current-question-state.md)
- [ ] [Task 02 - Student Question Image UI](tasks/02-student-question-image-ui.md)
- [ ] [Task 03 - SignalR Refresh Behavior](tasks/03-signalr-refresh-behavior.md)
- [ ] [Task 10 - Verification](tasks/10-verification.md)
