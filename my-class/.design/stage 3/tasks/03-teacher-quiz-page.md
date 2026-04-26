# Task 03 - Teacher Quiz Page

## Goal

Add a teacher-only Quiz page for presenting questions and controlling the live quiz flow.

## Work

- Add a teacher Quiz page route.
- Load and display the configured quiz title.
- Show the current question JPG for shared-screen presentation.
- Show countdown for the current question.
- Add Start, Finish, and Next controls.
- Start shows the first question and creates in-progress answer records for active students.
- Finish closes the current question and records failures for active students who did not answer.
- Next advances to the next question.
- Show active student answer status in a grid on the right side of the question image.
- Poll status using `Quiz:StatusRefreshMilliseconds`.
- Show answered/not answered status only; do not show selected answer or correctness live.
- Show a complete state after the last question.

## Deliverables

- Teacher Quiz page
- Question image display
- Countdown display
- Start, Finish, and Next controls
- Active student status grid with polling

## Acceptance Criteria

- Only teacher login state can use Quiz page controls.
- Teacher can start the first question.
- Teacher can see the question image, countdown, and active student grid.
- Student status changes from not answered to answered after polling.
- Finish closes submissions and records failures for non-answering active students.
- Timeout automatically finishes the question.
- Next advances to the next question.
- After the last question, the page shows quiz complete.

## Notes

- The teacher displays the question image on a shared screen.
- Use MudBlazor components consistently with the existing app.
