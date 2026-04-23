---
name: blazor-project-context
description: Use for repo-specific work in this Blazor project, especially when implementing or updating the planned Contacts CRUD app, project conventions, or design artifacts that live in the repository.
metadata:
  short-description: Repo-specific Blazor project context
---

# Blazor Project Context

## Overview

This skill contains repository-local context for the `c:\AI\blazor` workspace so project knowledge can live in GitHub instead of private home folders.

Use this skill when working on:
- the planned Blazor Contacts CRUD app
- repository-specific design artifacts
- project conventions, structure, and implementation workflow

## Workflow

1. Read [references/project-structure.md](references/project-structure.md) first to understand the current repo shape.
2. Read [references/current-plan.md](references/current-plan.md) when the task touches the Contacts CRUD app.
3. Prefer repo-local planning artifacts over assumptions.
4. Keep new project knowledge inside this plugin's `skills/` or `references/` folders when it belongs in source control.

## Current Project Direction

- The current planned app is a `.NET 10` Blazor Web App.
- The preferred interaction model is `Interactive Server`.
- Authentication should use `Individual` auth.
- Persistence should use SQLite.
- The initial business feature is Contacts CRUD.

## Where To Look

- Repo planning tasks: `sample/.design/`
- Project-local skill files: `plugins/blazor-project/skills/blazor-project-context/`

## Notes

- Keep personal or machine-specific data out of this skill.
- Prefer updating these repo-local references instead of storing project knowledge only in `~/.agents` or `~/.codex/skills`.
