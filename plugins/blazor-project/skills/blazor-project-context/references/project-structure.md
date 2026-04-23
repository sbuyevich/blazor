# Project Structure

## Repository Root

- `sample/`: currently contains planning artifacts for the Blazor Contacts CRUD app.
- `.agents/plugins/marketplace.json`: repo-local marketplace file for project plugins.
- `plugins/blazor-project/`: repo-local plugin that stores project knowledge in Git.

## Purpose

This repository-local plugin exists so project data can be committed to GitHub instead of staying only in private home directories such as `~/.agents` or `~/.codex/skills`.

## Guidance

- Put repo-specific skills, references, and reusable project instructions under `plugins/blazor-project/skills/`.
- Keep placeholders in plugin metadata until the public repo details are known.
- Avoid storing secrets, tokens, or machine-specific paths in committed skill files.
