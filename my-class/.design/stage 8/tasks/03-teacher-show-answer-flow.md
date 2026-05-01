# Task 03 - Teacher Show Answer Flow

## Goal

Add the teacher-facing `Show Answer` behavior and answer-result columns.

## Work

- Add a `Show Answer` button to the Teacher Quiz page for the current question.
- Enable the button when a current question exists and the answer has not already been revealed.
- On click, mark the current question answer as revealed.
- Swap the displayed image from the question image to the answer image.
- Populate teacher grid columns for:
  - answer time
  - correctness
- Notify the class through the existing SignalR quiz notification flow.

## Deliverables

- Teacher quiz service action for showing the answer.
- Teacher quiz page button and image swap.
- Teacher student grid answer time and correctness columns.

## Acceptance Criteria

- Teacher can reveal the answer for the current question.
- Revealing answer swaps teacher image from question to answer image.
- Re-clicking reveal does not duplicate work or break state.
- Teacher grid shows answer time and correctness after reveal.
- Student pages receive a refresh notification.

## Notes

- Correctness comes from exact comparison between `Answer` and `CorrectAnswer`.
- Answer time should be based on the stored answer completion/submission data available in `QuizAnswers`.
