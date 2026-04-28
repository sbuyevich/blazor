# Task 03 - SignalR Refresh Behavior

## Goal

Ensure the student question image follows teacher-driven quiz movement through the existing SignalR refresh path.

## Work

- Reuse the existing quiz hub and `QuizStateChanged` method.
- After a SignalR refresh, reload student answer state.
- Load a new image only when the current question key changes.
- Keep polling as a fallback for missed SignalR messages or reconnect timing.
- Confirm teacher start, restart, finish, and next remain the only quiz movement actions.

## Deliverables

- Student panel refresh behavior that updates both state and image.
- No new hub methods unless the existing refresh event is insufficient.

## Acceptance Criteria

- Teacher Start causes the student page to show the first question image.
- Teacher Next causes the student page to show the next question image.
- Teacher Finish keeps the current image visible while the status changes.
- Reconnect or polling eventually corrects the student image if a SignalR event is missed.
- No student UI can advance quiz questions.

## Notes

- SignalR messages should remain lightweight state-change notifications.
- Do not send image data over SignalR.
