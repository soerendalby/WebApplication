# Audit Log CSV Export — Contract

Purpose: Define the CSV schema produced by the MB admin export (SC-005), so consumers can parse it reliably.

- File format: CSV (UTF-8, header row present)
- Delimiter: comma (,)
- Quote char: double quote (")
- Newline: \n
## Columns (in order)

1. id (string) — unique audit entry id
2. timestamp (ISO-8601 string) — e.g., 2025-11-02T12:34:56Z
3. user_id (string)
4. user_email (string)
5. role (enum: student|approver|admin)
6. action (string)
7. request_id (string|null)
8. study_program_id (string|null)
9. details (string|null) — compact JSON string for extra metadata

## Notes

- Missing values are rendered as empty fields.
- details column may contain JSON; consumers should parse when non-empty.
- Timezone: UTC in export; UI may display local time.
