# Task 05 - Student Uses Active Teacher Quiz

## Goal

Ensure students see the quiz selected by the teacher for the active quiz session.

## Work

- Persist or derive active quiz selection from shared quiz-session state when the teacher starts/restarts.
- Ensure student answer state loads content from the active teacher-selected quiz.
- Ensure student question and answer images load from the active quiz folder.
- Ensure SignalR refresh keeps students on the active selected quiz.
- Prevent students from selecting their own quiz.

## Deliverables

- Shared active quiz selection for the current quiz session.
- Student quiz answer service uses active quiz selection.
- Student image loading uses active quiz folder.

## Acceptance Criteria

- Student pages show the same quiz the teacher started.
- Student refresh still uses the active teacher-selected quiz.
- Teacher changing dropdown alone does not unexpectedly change an in-progress student quiz until start/restart creates the active session.
- SignalR and polling refresh continue to work.
