# Quickstart — Car booking system (MVP)

This guide shows how to run the app locally and manually validate the MVP flows defined in the spec.

## Run locally

Prerequisites
- .NET 9 SDK installed

Steps
1) Restore & build
   - dotnet restore
   - dotnet build
2) Run (development)
   - dotnet run --project WebApplication.csproj
3) Browse
   - https://localhost:XXXX

## Demo login (MVP)
- Use any email with the required domain to simulate role mapping:
  - Student: user@student.dk
  - Approver: approver@admin.dk
  - MB Admin: admin@mb.dk
- Password: demo

Note: MVP uses domain-based role mapping; no email verification.

## Manual test scenarios (map to acceptance criteria)

### Student — submit request (P1)
1. Sign in as user@student.dk (password: demo)
2. Open the request form
3. Check both mandatory boxes (valid license + read guidelines)
4. Submit
5a. If study program has ≥1 approver → status: Pending approval; visible in approver queue
5b. If study program has 0 approvers → status: Approved immediately
6. Uncheck any box and attempt submit → should be blocked with validation

### Approver — decide (P2)
1. Sign in as approver@admin.dk
2. Open approval queue → see pending request
3. Approve with optional short comment → student sees Approved with comment; audit updated
4. Reject (use another request) → comment required; student sees Rejected with comment; audit updated
5. Retract a previous approval → MB admin sees indicator; audit updated

### MB Admin — config & audit (P3)
1. Sign in as admin@mb.dk
2. Create a study program; assign/unassign approvers
3. View all requests; filter/search/paginate
4. Open audit log; filter by action and export CSV → file matches contracts/audit-log-export.csv.schema.md

### Time‑based behaviors
- Reminder: set study program reminder_days=1, submit request, then verify approver queue shows reminder after threshold
- Expiry: set expiry_days=1, leave request pending; it becomes Expired after threshold and is removed from actionable queue

### Theming (clarification)
- Verify dark theme applied globally: body text white on dark background; headings orange. Navbar uses dark variant with light links.

## Notes
- CSV export is the only external contract in MVP. No API contracts are generated.
- If/when an API is added, contracts/ will contain OpenAPI schemas and examples.
