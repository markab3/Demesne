# Database

PostgreSQL. Raw SQL migrations applied in order.

## Prerequisites

- PostgreSQL 15+

## Setup

```bash
psql -U postgres -c "CREATE DATABASE demesne;"
psql -U postgres -d demesne -f migrations/001_initial_schema.sql
```

## Migrations

| File | Description |
|---|---|
| `001_initial_schema.sql` | Full schema from `docs/design/data-model.md` |

Run migrations in numeric order. Each file is idempotent (`CREATE TABLE IF NOT EXISTS`, `CREATE TYPE IF NOT EXISTS`).
