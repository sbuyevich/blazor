# Show Question Answer

## Teacher Quiz Page

Add a `Show Answer` button for the current question.

When the teacher clicks `Show Answer`:
- The current question image changes from `q.jpg`/`q.JPG` to `a.jpg`/`a.JPG`.
- The current question is considered revealed.
- The student answers grid shows answer time and correctness.
- Add sorting to student grid
- Correctness is based on each student's submitted answer compared with `CorrectAnswer`.
- Students are notified in real time.

## Student Quiz Answer Page

When the answer is revealed:
- The displayed image changes from the question image to the answer image.
- The student sees a message:
  - correct answer submitted
  - incorrect answer submitted
  - no answer submitted

## Quiz Content

Each question folder should include:
- `q.jpg` or `q.JPG`
- `a.jpg` or `a.JPG`
- `q.json`
