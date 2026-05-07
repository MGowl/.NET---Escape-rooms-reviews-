---
name: add-ef-class
description: 'Add a new Entity Framework domain model class and generate database migrations. Use when: creating a new database entity (table), adding DbSet to context, creating and applying EF Core migrations.'
argument-hint: 'Enter the class name (e.g., Category, Tag, Rating)'
---

# Add Entity Framework Class & Migrate Database

## When to Use
- Adding a new domain model entity to the database
- Setting up database relationships or constraints
- Keeping the database schema in sync with code-first model changes

## Overview
This workflow ensures proper EF Core domain model creation with automatic migration generation and application to maintain database consistency.

## Step-by-Step Procedure

### 1. Create Domain Model Class
1. Create a new `.cs` file in `EscapeRoomReviews/Models/Domain/`
2. Use the [domain model template](./assets/domain-model-template.cs)
3. Key elements required:
   - `[Table("TableName")]` attribute for table naming
   - `[Key]` attribute on primary key property (typically `Id`)
   - `[Required]` and `[MaxLength()]` for column constraints
   - `public` properties with `get; set;`
4. Add navigation properties for relationships if applicable
5. Save the file

### 2. Register in DbContext
1. Open `EscapeRoomReviews/Data/ApplicationDbContext.cs`
2. Add a `DbSet` property for your new class:
   ```csharp
   public DbSet<YourClassName> YourClassNamePlural => Set<YourClassName>();
   ```
3. If the class has relationships, configure them in `OnModelCreating()` method:
   ```csharp
   modelBuilder.Entity<YourClassName>()
       .HasOne(x => x.RelatedEntity)
       .WithMany(x => x.YourClassNameCollection)
       .HasForeignKey(x => x.RelatedEntityId)
       .OnDelete(DeleteBehavior.Cascade);
   ```

### 3. Generate Migration
Run the migration command in the project root:
```bash
dotnet ef migrations add DescriptiveNameHere
```
**Notes:**
- Migration name should describe what changed (e.g., `AddCategoryTable`, `AddUserRoleColumn`)
- New migration file appears in `EscapeRoomReviews/Migrations/`

### 4. Apply Migration to Database
```bash
dotnet ef database update
```
This executes pending migrations and updates the SQLite database.

### 5. Verify Changes
- Check `Migrations/` folder for the new migration file
- Confirm `.Designer.cs` and `ModelSnapshot.cs` are updated
- Test database operations if applicable

## Common Commands Reference
See [migration-commands.md](./scripts/migration-commands.md) for additional dotnet ef commands.

## Troubleshooting

**Migration won't generate?**
- Ensure DbSet is added to ApplicationDbContext
- Check for compilation errors: `dotnet build`
- Verify model is in `Models/Domain/` namespace

**Migration fails to apply?**
- Check for SQL syntax errors in generated migration
- Ensure no other instances of the app are accessing the database
- Review the migration file logic and adjust if needed

**Column constraints not applied?**
- Verify data annotations (`[Required]`, `[MaxLength]`, etc.) are on model properties
- Regenerate migration after adding annotations
- Check that annotations match your intent
