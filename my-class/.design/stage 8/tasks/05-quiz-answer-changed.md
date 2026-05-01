# Task 05 - Quiz Answer Page Changes

## Goal

Improve the Student Quiz Answer page layout and replace the text question-position label with a visual numbered progress row.

## Work

- Move answer buttons to the center of the answer page.
- Remove the blue answer/status bar that currently appears under the answer buttons.
- Move the answer/result message under the question image and center it.
- Show the answer/result message inside a colored bar:
  - light green for correct answer
  - light red for incorrect answer
  - yellow for missed/no answer
- Replace the `Question 1 of 4` text with a row of numbered circles.
- Show one circle per quiz question.
- Make past question circles and the current question circle bold.
- Keep future question circles neutral and not bold.
- Color past question circles by this student's result:
  - yellow for missed/no answer
  - light green for correct answer
  - light red for incorrect answer
- Keep the current question circle neutral until the answer is revealed.
- Add student quiz answer state needed to show previous question results on the answer page.

## Deliverables

- Student answer page layout update.
- Numbered question progress circles.
- Student answer state/service support for previous question result history.
- Centered result message bar with correct/missed/incorrect colors.

## Acceptance Criteria

- Student answer buttons are centered.
- The old blue answer message bar under the buttons is no longer shown.
- After answer reveal, the result message appears centered under the question image.
- Result message color matches the revealed result:
  - correct = light green
  - incorrect = light red
  - missed/no answer = yellow
- The text `Question 1 of 4` is replaced by numbered circles.
- Past and current question circles are bold.
- Past question circles use the student's stored result color.
- Current and future unanswered/unrevealed circles remain neutral.
- Refreshing the page preserves the same progress circle colors from stored quiz answer data.
