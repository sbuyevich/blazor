# Task 04 - Student Timer Display

## Goal

Show the current question countdown timer on the student Quiz Answer page in the same style as the teacher quiz page.

## Work

- Add timer state to the student answer page state using the existing current-question timing source.
- Include enough data to render remaining time for the current question and determine whether it is in progress.
- Render a timer chip on the student Quiz Answer page near the question status message.
- Match the teacher quiz page timer format, including `mm:ss`.
- Update timer display after SignalR `QuizStateChanged` refreshes.
- Continue local countdown updates between SignalR or polling refreshes while the question is in progress.
- Stop the timer at `00:00` when the question finishes, expires, or is no longer in progress.

## Deliverables

- Updated student answer state contract with timer data.
- Updated `QuizAnswerService` timer state population.
- Updated `StudentQuizAnswerPanel` timer rendering and local countdown behavior.

## Acceptance Criteria

- Student sees the timer when a question is in progress.
- Student timer format matches the teacher page timer format.
- Timer updates when the teacher starts, restarts, finishes, or moves to the next question.
- Timer reaches and stays at `00:00` when the current question expires or finishes.
- SignalR refresh keeps the student timer aligned with teacher-driven quiz state.
- Polling fallback eventually corrects the timer if a SignalR event is missed.
- Existing answer buttons, status message, and question image behavior continue to work.

## Notes

- Reuse the existing quiz hub and `QuizStateChanged` notification.
- Do not send timer ticks over SignalR.
- Do not add student controls for quiz movement.
