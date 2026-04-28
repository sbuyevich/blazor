# Task 05 - Student Question Position Label

## Goal

Show a current question position label on the student Quiz Answer page, matching the teacher quiz page format such as `Question 3 of 4`.

## Work

- Add question position state to the student answer page state using the existing current-question lookup.
- Include current question index and total question count, or a preformatted label derived from those values.
- Render the label near the student status and timer area.
- Match the teacher quiz page format and one-based display numbering.
- Update the label when SignalR or polling refreshes student answer state.
- Hide the label when there is no current question.

## Deliverables

- Updated student answer state contract with question position data.
- Updated `QuizAnswerService` state population.
- Updated `StudentQuizAnswerPanel` rendering.

## Acceptance Criteria

- Student sees `Question 1 of N` after the teacher starts the quiz.
- Student sees the correct question number after the teacher clicks Next.
- Label matches the teacher quiz page wording and one-based numbering.
- Label remains visible after the student submits an answer for the current question.
- Label remains visible after the teacher finishes the current question.
- Label is hidden before a question starts.
- Existing image, timer, status message, and answer button behavior continue to work.

## Notes

- Reuse existing quiz content/current-question data.
- Do not add database fields or quiz JSON fields.
- Do not add student controls for quiz movement.
