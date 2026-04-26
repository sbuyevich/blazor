# Task 05 - Navigation and Security

## Goal

Expose the quiz pages in navigation and enforce role/class restrictions consistently.

## Work

- Add `Quiz` under the Teacher navigation section.
- Add `Quiz Answer` for signed-in student users.
- Hide teacher Quiz navigation from student users.
- Hide student Quiz Answer navigation from teacher users.
- Protect the teacher Quiz page from unsigned-in users and student users.
- Protect the student Quiz Answer page from unsigned-in users and teacher users.
- Validate current class context for all quiz service operations.
- Ensure service methods reject mismatched class codes and missing login state.

## Deliverables

- Updated navigation menu
- Teacher-only Quiz access
- Student-only Quiz Answer access
- Server-side role and current-class validation

## Acceptance Criteria

- Teacher sees Teacher > Quiz.
- Student sees Quiz Answer.
- Unsigned-in users are blocked by the existing login modal.
- Students cannot use teacher Quiz controls.
- Teachers cannot submit student answers.
- Quiz operations do not affect students or sessions from another class.

## Notes

- Navigation visibility is convenience only; services must enforce permissions.
- Keep using the existing local login state model.
