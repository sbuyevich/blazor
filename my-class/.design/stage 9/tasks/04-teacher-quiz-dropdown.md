# Task 04 - Teacher Quiz Dropdown

## Goal

Add a quiz dropdown to the Teacher Quiz page and use it for quiz start/restart.

## Work

- Add a `Quiz` dropdown at the top of the Teacher Quiz page.
- Populate it with discovered quiz `name` values.
- Disable start/restart when no valid quiz is selected.
- Save selection changes to browser session storage.
- Pass the selected quiz path into teacher quiz actions that need to load quiz content.
- Show a warning when no valid quizzes are available.

## Deliverables

- Teacher Quiz page dropdown.
- Selected quiz state in the teacher component.
- Start/restart wired to the selected quiz.
- Empty/invalid state messaging.

## Acceptance Criteria

- Teacher can select a quiz before starting.
- Dropdown selection controls which quiz starts.
- Restart uses the currently selected quiz.
- No valid quizzes disables start and shows a warning.
- Existing teacher controls still work after a quiz is selected.
