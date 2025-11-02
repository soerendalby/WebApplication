<!--
Sync Impact Report
- Version change: 1.0.1 → 1.0.2
- Modified principles: None
- Added sections: Technical Defaults & Reference (summary only)
- Removed sections: None
- Templates requiring updates:
	- .specify/templates/plan-template.md: ⚠ pending (reflect principles in "Constitution Check")
	- .specify/templates/spec-template.md: ⚠ pending (tests optional unless requested; include testability notes)
	- .specify/templates/tasks-template.md: ✅ aligned (tests OPTIONAL already)
	- README.md: ✅ aligned
	- .github/copilot-instructions.md: ✅ updated (full technical guidance)
- Follow-up TODOs: None
-->
Use `/.github/copilot-instructions.md` as the authoritative technical guide.

# WebApplication Constitution

## Core Principles

### I. Testable-by-Design (No Mandatory Tests)
Code MUST be designed for testability (clear seams, DI-friendly services, pure functions where
practical), but automated tests are NOT required unless explicitly requested in a feature spec.
Expose minimal, stable contracts to enable later testing without refactors. Rationale: fast
iteration with the option to add tests when value is clear.

### II. Modern, Sleek UI/UX
UI MUST feel modern and responsive: consistent typography, spacing, and motion; mobile-friendly
layouts; accessible contrast and semantics. Prefer Bootstrap utilities/components; keep custom CSS
in `wwwroot/css` and minimize jQuery-driven DOM mutations in favor of unobtrusive behaviors.
Rationale: higher perceived quality and reduced rework.

### III. Conventional ASP.NET Core Structure
Follow ASP.NET Core conventions for routing, assets, and tag helpers. Use tag helpers for anchors
and assets (e.g., `asp-controller`/`asp-action`, `asp-append-version`). Keep UI changes in Views
or Pages and service wiring in `Program.cs`. Rationale: easier navigation and lower cognitive load
for contributors and AI agents.

### IV. Incremental, Localized Changes
Changes MUST be as small and localized as possible. Avoid cross-cutting edits unless explicitly
required. Prefer adding new controller actions/views or page handlers over broad refactors.
Rationale: reduces regression risk and simplifies reviews.

### V. Backward Compatibility & Semantic Versioning
Public-facing behaviors and URLs SHOULD remain stable. SemVer applies to the constitution and any
consumed contracts. MAJOR for breaking governance/rules, MINOR for new principles/sections,
PATCH for clarifications. Rationale: predictability for contributors and downstream users.

<!-- Technical .NET implementation guidance has been moved to
		 .github/copilot-instructions.md to keep the constitution focused on principles
		 and governance. -->

## Technical Defaults & Reference

The following defaults are policy-level preferences; implementation details live in
`/.github/copilot-instructions.md`.

- Platform: .NET 9, ASP.NET Core MVC (Controllers + Views) as the default web pattern.
- UI: Bootstrap-based design with minimal jQuery; modern, responsive, and accessible UI REQUIRED.
- Data: Prefer EF Core + SQLite by default for local/dev. Other providers MAY be added with
  explicit justification.
- Conventions: Use tag helpers (`asp-controller`, `asp-action`, `asp-append-version`) and keep
  static assets under `wwwroot/`.

## Development Workflow & Quality Gates

- Build/Run: `dotnet restore`, `dotnet build`, `dotnet run --project WebApplication.csproj`.
- No mandatory automated tests. When tests are requested, keep them minimal and focused.
- Code MUST be testable: small functions, DI for services, minimal static coupling, clear
	boundaries.
- UI reviews: ensure responsiveness, accessibility basics (labels, contrast), and consistent
	spacing/typography.
- PRs: small scope, describe user impact and how to validate manually.

## Governance

- This constitution supersedes ad-hoc practices for this repository.
- Amendments: open a PR with a rationale and proposed wording. On merge, bump
	`CONSTITUTION_VERSION` per SemVer and set `LAST_AMENDED_DATE` to the merge date.
- Compliance: reviewers MUST verify alignment with Core Principles and Quality Gates. Deviations
	require explicit justification in the PR description.

**Version**: 1.0.2 | **Ratified**: 2025-11-01 | **Last Amended**: 2025-11-02
