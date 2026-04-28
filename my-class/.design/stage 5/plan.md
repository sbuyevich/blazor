# Stage 5 Answer Count And SignalR Plan

## Summary
Stage 5 makes the number of answer choices configurable from quiz content and updates the student answer page in real time when the teacher changes quiz state. Root `quiz.json` defines the default answer count, each question `q.json` may override it, and `StudentQuizAnswerPanel` renders buttons from the current question instead of the hard-coded `1..4` list.

## Key Changes
- Add quiz content support for `answerCount`:
  - Root `quiz.json` accepts `answerCount`, defaulting to `4` when omitted.
  - Each question `q.json` may override `answerCount`; omitted questions use the root value.
  - Valid answer counts are positive integers, with a practical UI range of `1` through `4`.
  - `correctAnswer` remains a string, but must be a number between `1` and the effective answer count.
- Extend quiz service contracts:
  - Add effective `AnswerCount` to `QuizContent`/`QuizQuestionContent`.
  - Add answer choices to `QuizAnswerPageState` as strings `"1"` through `AnswerCount`.
  - Include answer count in the current-question lookup used by `QuizAnswerService`.
- Update student answer behavior:
  - Remove the hard-coded `AnswerChoices = [1, 2, 3, 4]`.
  - Render buttons from `QuizAnswerPageState.AnswerChoices`.
  - Disable answer buttons when no question is in progress, the student already answered, or submission is in progress.
  - Validate submitted answers server-side against the current question answer count.
- Add explicit SignalR quiz notifications:
  - Add a `QuizHub` endpoint such as `/hubs/quiz` with class-code groups.
  - Add `IQuizNotificationService` that uses `IHubContext<QuizHub>` to send a lightweight `QuizStateChanged` message to the current class group.
  - Notify students after teacher actions that change availability: start/restart, finish, next question, and automatic timeout finish.
  - `StudentQuizAnswerPanel` connects to the hub, joins the current class group, and reloads answer state when `QuizStateChanged` arrives.
  - Keep a low-frequency fallback refresh or reconnect handling so a missed SignalR event does not leave the page stale.

## Public Interfaces / Types
- `quiz.json`: add optional `answerCount`.
- Question `q.json`: add optional `answerCount`.
- `QuizQuestionContent`: add `AnswerCount`.
- `QuizAnswerPageState`: add `IReadOnlyList<string> AnswerChoices`.
- Web project: add SignalR client package matching the existing ASP.NET Core package version if needed by the Blazor component hub connection.

## Test Plan
- Root `answerCount` controls buttons when a question omits `answerCount`.
- Question `answerCount` overrides the root value.
- Missing `answerCount` everywhere keeps four buttons.
- Invalid answer counts and out-of-range `correctAnswer` values fail quiz loading with clear messages.
- Student page shows exactly the configured number of answer buttons.
- Submitting an answer outside the configured range is rejected server-side.
- Teacher start/restart/finish/next causes connected student pages to refresh through SignalR.
- Student page still refreshes after reconnect or fallback refresh if a SignalR message is missed.
- `dotnet build .\MyClass.slnx` succeeds once existing locked app processes are stopped.

## Assumptions
- JSON property name is `answerCount`.
- Answer labels remain numeric strings.
- No database schema change is required because stored answers and correct answers are already strings.
- SignalR messages carry no answer data; they only tell clients to reload state from existing services.

## Task Breakdown

- [x] [Task 01 - Answer Count Quiz Content](tasks/01-answer-count-quiz-content.md)
- [x] [Task 02 - Answer State and Validation](tasks/02-answer-state-and-validation.md)
- [ ] [Task 03 - Dynamic Student Answer Buttons](tasks/03-dynamic-student-answer-buttons.md)
- [ ] [Task 04 - SignalR Quiz Notifications](tasks/04-signalr-quiz-notifications.md)
- [ ] [Task 10 - Verification](tasks/10-verification.md)
