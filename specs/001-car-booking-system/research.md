# Research notes — Car booking system (MVP)

This document captures quick research, options, and recommendations for MVP decisions.

## Auth for demo roles
Options
- Cookie auth with custom claims + simple sign-in page (email → role mapping)
- ASP.NET Core Identity (heavier for demo)
Recommendation
- Use cookie auth + minimal sign-in; map domain to claims (Student/Approver/MB Admin). Replace with IdP later.
Notes
- Add anti-forgery on POST; validate resource ownership for Student views.

## Background processing (reminders, expiry)
Options
- IHostedService/BackgroundService (built-in)
- Quartz.NET/Hangfire (scheduling, dashboard)
Recommendation
- BackgroundService + periodic timer (e.g., 5–10 min); keep idempotent; encapsulate logic behind IReminderService. Upgrade to Quartz/Hangfire if scheduling sophistication is needed.
References
- Microsoft Docs: Background tasks with hosted services in ASP.NET Core.

## Persistence
Options
- EF Core + SQLite (file DB) — simple migrations, queryable, good for MVP
- In-memory repositories for POC only (no durability)
Recommendation
- EF Core + SQLite, with DbContext registered in Program.cs and connection string in appsettings.json. Use indices on Request.State and timestamps for queue performance.

## CSV export
Concerns
- Encoding (UTF-8 BOM for Excel compatibility)
- Delimiters and quoting (escape commas/newlines, quote fields with commas)
- Date/time formatting (ISO 8601 with timezone)
Recommendation
- Follow `contracts/audit-log-export.csv.schema.md` strictly. Emit UTF-8 with BOM, CRLF newlines, and `"`-escaped quotes. Ensure column order is fixed.

## Razor Pages vs Controllers
Observation
- Repo uses Razor Pages; no need to switch to MVC.
Recommendation
- Keep Razor Pages. Use page handlers (OnPostApprove, OnPostReject, etc.). Keep logic in services.

## Validation & UX
- Use DataAnnotations + ModelState checks; server-side first; unobtrusive validation for client-side
- Required checkboxes for declarations; prevent submit when unchecked
- Keep user input on validation errors; show inline errors

## Theming accessibility (dark mode)
- Ensure contrast (≥ 4.5:1 for body text); orange headings on dark must meet contrast
- Focus styles: visible outline/focus ring on dark backgrounds
- Link hover/visited states readable; ensure :focus-visible styles

## Filtering, search, and pagination
- Pagination via simple query params (page, pageSize)
- Basic filtering for queues (state, study program, date range)
- Indexes: Requests(State, CreatedAt); AuditLogs(Timestamp, Action)

## Open questions
- Do we need multi-approver workflows in MVP? (Assume single approver per request for now)
- Exact reminder/expiry thresholds per program vs global defaults (assume per program with sensible defaults)
- Any locale/formatting constraints for dates/numbers in UI beyond ISO for CSV?
