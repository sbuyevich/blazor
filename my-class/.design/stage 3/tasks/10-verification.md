# Task 10 - Verification

## Goal

Verify stage 3 quiz behavior end to end.

## Work

- Build the solution.
- Run the app with the HTTP launch profile.
- Verify quiz configuration loading and validation.
- Verify teacher Quiz page controls.
- Verify student Quiz Answer submission flow.
- Verify polling updates on the teacher status grid.
- Verify timeout, Finish, and Next behavior.
- Verify role and class-scope protections.

## Deliverables

- Successful build
- Manual verification notes
- Any focused automated tests that fit the current project structure

## Acceptance Criteria

- `dotnet build .\MyClass.slnx` succeeds with no warnings or errors.
- App opens at `http://localhost:5555/?c=demo`.
- Valid quiz folder loads successfully.
- Invalid quiz folder or JSON shows a clear error.
- Teacher can start the first question and see image, countdown, and active student grid.
- Starting a question creates in-progress answer records for active students.
- Student can submit one answer for the active current question.
- Student submit without a valid in-progress answer record is rejected with a clear message.
- Duplicate student submissions for the same question are rejected.
- Teacher grid changes from not answered to answered after polling.
- Timeout finishes the question and creates failed results for active non-answering students.
- Finish manually closes submissions and records failures for non-answering active students.
- Next advances to the next question.
- After the last question, the quiz shows complete.
- Students from another class cannot answer the current class quiz.
- Non-teachers cannot access teacher Quiz controls.

## Notes

- Keep verification scoped to stage 3 behavior plus regressions in login, class context, and student active state.
