# Task 05 - Student Uses Active Teacher Quiz

## Goal

Ensure students see the quiz currently selected by the teacher.

## Work

- Use shared runtime state for the active selected quiz path.
- Set shared runtime state from the teacher quiz dropdown.
- Do not store the selected quiz path in `QuizAnswers` or any database table.
- Ensure student answer state loads content from the current teacher-selected quiz.
- Ensure student question and answer images load from the current teacher-selected quiz folder.
- Notify students through the existing SignalR refresh when the teacher changes the dropdown.
- Prevent students from selecting their own quiz.

## Deliverables

- Shared runtime active quiz selection controlled by the teacher dropdown.
- Student quiz answer service uses the current teacher-selected quiz.
- Student image loading uses the current teacher-selected quiz folder.
- Existing SignalR refresh reacts to teacher dropdown changes.

## Acceptance Criteria

- Student page loads the quiz currently selected by the teacher.
- Teacher dropdown change updates student page after SignalR or polling refresh.
- Teacher dropdown change can switch students to the new quiz without waiting for start/restart.
- No `QuizFolderPath` field is added to `QuizAnswer`, `QuizAnswerPageState`, or EF DB mapping for persistence.
- Question and answer images load from the current teacher-selected quiz folder.
- If no teacher selection exists in runtime state, the student service falls back to the first valid discovered quiz.
- SignalR and polling refresh continue to work.
