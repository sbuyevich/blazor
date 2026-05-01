# Task 02 - Selected Quiz Session Storage

## Goal

Persist the teacher's selected quiz in browser session storage.

## Work

- Add session-storage APIs for selected quiz path.
- Load the selected quiz path when the Teacher Quiz page initializes.
- Save the selected quiz path when the teacher changes the dropdown.
- If the saved path is missing from the discovered quiz list, fall back to the first valid quiz.
- If no valid quizzes exist, leave selection empty.

## Deliverables

- Selected quiz session-storage contract methods.
- Session-storage implementation.
- Selection fallback behavior.

## Acceptance Criteria

- Teacher quiz selection survives page refresh in the same browser session.
- Removed or invalid saved quiz path does not break the page.
- First valid discovered quiz is selected by default when no saved selection exists.
