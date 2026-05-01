# Task 03 - Selected Quiz Content Loading

## Goal

Load quiz content and images from the selected quiz folder instead of a single fixed quiz folder.

## Work

- Extend quiz content loading so callers can specify a quiz folder path.
- Load selected quiz `quiz.json` and question data from that folder.
- Load question images from the selected quiz folder.
- Load answer images from the selected quiz folder.
- Keep existing image naming and missing-image warning behavior.
- Return clear failures when the selected quiz cannot be loaded.

## Deliverables

- Quiz content loading by selected quiz path.
- Question image loading by selected quiz path.
- Answer image loading by selected quiz path.

## Acceptance Criteria

- Selected quiz content loads from its folder.
- Question and answer images load from the selected quiz folder.
- Missing selected quiz content returns a warning/failure instead of falling back silently to another quiz.
- Existing single-quiz behavior is preserved through the default selected quiz fallback.
