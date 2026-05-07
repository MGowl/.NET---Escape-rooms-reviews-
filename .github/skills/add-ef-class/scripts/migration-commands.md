# Migration Commands Reference

## Common dotnet ef Commands

### 1. Generate a Migration
```bash
dotnet ef migrations add MigrationName
```
- Creates a new migration file in `Migrations/`
- Include descriptive name (e.g., `AddCategoryTable`, `AddUserPreferences`)
- Use PascalCase for migration names

### 2. Apply Migration to Database
```bash
dotnet ef database update
```
- Applies all pending migrations to the database
- Creates database file if it doesn't exist (SQLite)

### 3. Apply to Specific Migration
```bash
dotnet ef database update MigrationName
```
- Updates database to a specific migration point
- Useful for rolling back or targeting a specific state

### 4. List All Migrations
```bash
dotnet ef migrations list
```
- Shows all migrations and which ones have been applied

### 5. Remove Last Migration
```bash
dotnet ef migrations remove
```
- Removes the last unapplied migration
- Use if you want to redo or modify a recent migration

### 6. Script SQL for Migration
```bash
dotnet ef migrations script --output migration.sql
```
- Generates SQL script without applying it
- Useful for reviewing SQL or manual database updates

## Notes
- Always run from the project root (where `.csproj` file is)
- Ensure project builds successfully before running migration commands
- Use descriptive migration names to track database schema changes over time
