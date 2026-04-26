# Task 04 - Student Quiz Answer Page

## Goal

Add a student Quiz Answer page with four large answer buttons and backend-enforced answer validation.

## Work

- Add a student Quiz Answer page route.
- Show four large buttons labeled `1`, `2`, `3`, and `4`.
- Keep answer buttons clickable in the UI.
- On click, submit the selected answer to the backend service.
- Backend validates that an in-progress answer record exists for the current student and current question.
- Show a waiting/not available message when no valid in-progress answer record exists.
- Show an answered/waiting state after a successful answer.
- Prevent duplicate answers for the current question through backend validation.

## Deliverables

- Student Quiz Answer page
- Four-button answer UI
- Submit flow wired to quiz answer service
- Waiting/not available, answered, and error states

## Acceptance Criteria

- Student can open Quiz Answer page after login.
- Buttons are visible and clickable before, during, and after a question.
- Clicking a button before a teacher-started question shows waiting/not available.
- Clicking a button for an active in-progress answer record saves the selected answer.
- Clicking again for the same question is rejected or shown as already answered.
- Student users cannot answer for another class or another student.

## Notes

- Students do not see the question image on this page.
- UI button state is not trusted; backend validation is the source of truth.
