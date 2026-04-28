# Task 03 - Dynamic Student Answer Buttons

## Goal

Render student answer buttons from quiz state instead of a hard-coded list.

## Work

- Remove the static `AnswerChoices = [1, 2, 3, 4]` list from `StudentQuizAnswerPanel`.
- Render buttons from `_stateResult.State.AnswerChoices`.
- Submit the selected answer string from the rendered choice.
- Disable buttons when:
  - no question is currently answerable
  - the student already answered
  - the question has finished
  - a submit is already in progress
- Keep the existing loading and status-message behavior.

## Deliverables

- Updated `StudentQuizAnswerPanel.razor`.
- Updated `StudentQuizAnswerPanel.razor.cs` as needed.

## Acceptance Criteria

- A question with answer count `2` shows two answer buttons.
- A question with answer count `4` shows four answer buttons.
- Buttons disappear or disable when the student cannot answer.
- Clicking a button submits the matching answer string.

## Notes

- Preserve the existing MudBlazor visual style.

