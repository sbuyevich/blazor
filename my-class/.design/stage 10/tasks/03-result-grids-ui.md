# Task 03 - Result Grids UI

## Goal

Build the MudBlazor UI for teacher-facing student and question result summaries.

## Work

- Add the Quiz Result page UI using MudBlazor components.
- Add a student summary grid with sortable columns:
  - student display name
  - correct count
  - incorrect count
  - percent correct
  - total answer time
- Add a question summary grid with sortable columns:
  - question text
  - correct count
  - incorrect count
  - percent correct
  - total answer time
- Add loading, error, and empty states.
- Format percentages consistently.
- Format total answer time for quick classroom scanning.

## Deliverables

- Result page component markup and code-behind as needed.
- Sortable student summary table.
- Sortable question summary table.
- Empty/error/loading states.

## Acceptance Criteria

- Both grids render for a teacher when result rows exist.
- Both grids are sortable by every required column.
- Empty results show a clear empty state.
- Errors from the service show a concise warning or error message.
- Percent and time formatting is readable and consistent.
- Layout works on desktop and narrower browser widths without overlapping text.

## Notes

- Use MudBlazor components consistently with existing pages.
- Keep UI text concise and action-oriented for classroom use.
