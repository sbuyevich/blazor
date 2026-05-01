# Task 01 - Quiz Discovery and Metadata

## Goal

Discover all valid quiz folders under `Quiz:RootFolder` and expose their metadata for selection.

## Work

- Read direct child folders under `Quiz:RootFolder`.
- For each child folder, attempt to read `quiz.json`.
- Treat folders with missing or invalid `quiz.json` as invalid and skip them.
- Add a quiz summary model containing:
  - display name
  - title
  - time limit seconds
  - quiz folder path
- Add service support for listing available quiz summaries.
- Keep discovery failures scoped to each folder so one bad quiz does not break all quiz loading.

## Deliverables

- Quiz summary model.
- Quiz discovery service method.
- Metadata parsing from each quiz folder's `quiz.json`.

## Acceptance Criteria

- Valid quiz folders appear in the discovered quiz list.
- Invalid quiz folders are skipped.
- Missing `Quiz:RootFolder` or no valid quizzes produces an empty list or warning result without crashing.
- Discovered quiz summaries include the path needed to load that quiz later.
