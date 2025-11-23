# Data Model — Car booking system

This is a relational, technology‑agnostic model derived from the feature specification. It enumerates entities, attributes, relationships, constraints, and state transitions. Names are indicative; adapt to your ORM/DB conventions.

## Entities overview

- User
- StudyProgram
- Approver
- Request
- Approval
- AuditLog

## Entity reference

### User

Columns

| Column          | Type        | Null | Default | Notes |
|-----------------|-------------|------|---------|-------|
| id              | uuid/string | NO   | gen     | PK |
| email           | string(320) | NO   |         | Unique; role inferred by domain |
| name            | string(200) | YES  |         |  |
| role            | enum        | NO   |         | Values: student | approver | admin |
| study_program_id| uuid/string | YES  |         | FK → StudyProgram.id (students only) |
| verified        | boolean     | YES  | false   | Optional for MVP |
| locale          | string(10)  | YES  |         | e.g., da-DK, en-US |
| created_at      | timestamp   | NO   | now     |  |

Constraints & Indexes
- PK: (id)
- UNIQUE: (email)
- FK: (study_program_id) → StudyProgram(id)

---

### StudyProgram

Columns

| Column        | Type        | Null | Default | Notes |
|---------------|-------------|------|---------|-------|
| id            | uuid/string | NO   | gen     | PK |
| name          | string(200) | NO   |         | Unique |
| description   | text        | YES  |         |  |
| reminder_days | int         | YES  | 8       | Override default |
| expiry_days   | int         | YES  | 10      | Override default |
| created_at    | timestamp   | NO   | now     |  |

Constraints & Indexes
- PK: (id)
- UNIQUE: (name)

---

### Approver

Represents assignment of a user as approver for a study program.

Columns

| Column           | Type        | Null | Default | Notes |
|------------------|-------------|------|---------|-------|
| id               | uuid/string | NO   | gen     | PK |
| user_id          | uuid/string | NO   |         | FK → User(id); must have role=approver |
| study_program_id | uuid/string | NO   |         | FK → StudyProgram(id) |
| active           | boolean     | NO   | true    |  |
| extra_scope      | text/json   | YES  |         | Optional future extension |
| created_at       | timestamp   | NO   | now     |  |

Constraints & Indexes
- PK: (id)
- UNIQUE: (user_id, study_program_id)
- FK: (user_id) → User(id)
- FK: (study_program_id) → StudyProgram(id)

---

### Request

A car booking request submitted by a student.

Columns

| Column           | Type        | Null | Default | Notes |
|------------------|-------------|------|---------|-------|
| id               | uuid/string | NO   | gen     | PK |
| user_id          | uuid/string | NO   |         | FK → User(id) (student) |
| study_program_id | uuid/string | NO   |         | FK → StudyProgram(id) |
| status           | enum        | NO   |         | Values: Pending | Approved | Rejected | Expired |
| submitted_at     | timestamp   | NO   | now     |  |
| expires_at       | timestamp   | YES  |         | Derived from expiry_days at submission |
| approval_retracted_at | timestamp | YES |       | Indicator for MB admins (denormalized from audit) |

Constraints & Indexes
- PK: (id)
- FK: (user_id) → User(id)
- FK: (study_program_id) → StudyProgram(id)
- INDEX: (study_program_id, status)
- INDEX: (user_id, submitted_at DESC)

State transitions
- Pending → Approved | Rejected | Expired
- Approved/Rejected are terminal for MVP. Retraction of approval does not change status; it emits an audit event and toggles the indicator.

---

### Approval

An approver decision on a request.

Columns

| Column      | Type        | Null | Default | Notes |
|-------------|-------------|------|---------|-------|
| id          | uuid/string | NO   | gen     | PK |
| request_id  | uuid/string | NO   |         | FK → Request(id) |
| approver_id | uuid/string | NO   |         | FK → Approver(id) |
| decision    | enum        | NO   |         | approved | rejected |
| comment     | text        | YES  |         | Mandatory on rejection |
| decided_at  | timestamp   | NO   | now     |  |

Constraints & Indexes
- PK: (id)
- UNIQUE: (request_id) — first valid decision wins; enforce single decision per request
- FK: (request_id) → Request(id)
- FK: (approver_id) → Approver(id)

Notes
- Retraction is not an update here; it is a separate audit event.

---

### AuditLog

Records key actions with timestamp, user, and metadata.

Columns

| Column          | Type        | Null | Default | Notes |
|-----------------|-------------|------|---------|-------|
| id              | uuid/string | NO   | gen     | PK |
| timestamp       | timestamp   | NO   | now     |  |
| user_id         | uuid/string | NO   |         | FK → User(id) |
| user_email      | string(320) | NO   |         | Redundant for export convenience |
| role            | enum        | NO   |         | student | approver | admin |
| action          | string(100) | NO   |         | e.g., request.create, approval.approve, approval.reject, approval.retract, study_program.create, config.update |
| request_id      | uuid/string | YES  |         |  |
| study_program_id| uuid/string | YES  |         |  |
| details         | json/text   | YES  |         | e.g., {"comment":"..."} |

Constraints & Indexes
- PK: (id)
- INDEX: (timestamp DESC)
- INDEX: (action)
- INDEX: (request_id)

Retention
- 5 years, per spec.

---

## Relationships (summary)
- User (student) 1—N Request
- StudyProgram 1—N Request
- StudyProgram 1—N Approver
- Approver 1—N Approval
- Request 1—0..1 Approval (enforced by UNIQUE(request_id))
- AuditLog references many entities optionally

## Derived/Computed behaviors
- expires_at = submitted_at + expiry_days (per Request.study_program_id or defaults)
- Reminder state is a UI indicator when now ≥ submitted_at + reminder_days and status=Pending
- approval_retracted_at is set when an approval.retract audit event is recorded for the request

## Privacy & security notes
- Emails stored securely; role enforced by domain on login.
- Exports use CSV with schema defined in contracts/audit-log-export.csv.schema.md.
