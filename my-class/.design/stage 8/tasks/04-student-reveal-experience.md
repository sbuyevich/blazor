# Task 04 - Student Reveal Experience

## Goal

Update the Student Quiz Answer page to show the revealed answer image and configured result message.

## Work

- Load student answer reveal state from `IQuizAnswerService`.
- Swap from question image to answer image when the teacher reveals the answer.
- Choose exactly one configured message:
  - `Correct` when the student answered correctly
  - `Incorrect` when the student answered incorrectly
  - `NoAnswer` when the student submitted no answer
- Keep existing SignalR refresh and polling fallback behavior.
- Keep answer buttons disabled after reveal.

## Deliverables

- Student answer state updates.
- Student page answer-image swap.
- Configured result message display.

## Acceptance Criteria

- Student sees the answer image after teacher reveal.
- Student sees the configured correct, incorrect, or no-answer message.
- Student page updates through SignalR after teacher clicks `Show Answer`.
- Refreshing the student page still shows revealed state.
- Existing answer submission behavior does not regress before reveal.

## Notes

- Messages come from `QuizOptions.RevealMessages`.
- Keep message tone controlled by configuration, not hard-coded UI text.
