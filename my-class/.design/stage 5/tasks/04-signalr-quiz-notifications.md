# Task 04 - SignalR Quiz Notifications

## Goal

Refresh student quiz-answer pages immediately when teacher quiz state changes.

## Work

- Add a SignalR hub for quiz notifications, mapped at `/hubs/quiz`.
- Support class-code groups so notifications are scoped to one class.
- Add a notification service using `IHubContext` to send `QuizStateChanged`.
- Notify the current class after teacher actions:
  - start quiz
  - restart quiz
  - finish question
  - move next question
  - automatic timeout finish
- Connect `StudentQuizAnswerPanel` to the hub.
- Join the current class group after the component has login/class context.
- Reload answer state when `QuizStateChanged` is received.
- Keep reconnect/fallback behavior so the student page can recover if a message is missed.

## Deliverables

- New quiz SignalR hub.
- New quiz notification service and DI registration.
- Updated teacher quiz service calls that publish notifications after successful state changes.
- Updated student answer panel SignalR connection lifecycle.

## Acceptance Criteria

- Teacher start/restart refreshes connected student answer pages without waiting for normal polling.
- Teacher finish/next refreshes connected student answer pages without waiting for normal polling.
- Notifications for one class do not refresh students connected to another class.
- Student answer panel disposes the hub connection cleanly.

## Notes

- SignalR messages carry only a state-change notification; answer details still come from existing services.

