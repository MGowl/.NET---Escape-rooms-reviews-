using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EscapeRoomReviews.Models.Domain;

/// <summary>
/// Replace [ClassName] with your entity name.
/// Use this template for new domain model classes.
/// </summary>
[Table("TableNames")]  // Change to your table name (typically plural)
public class ClassName
{
    /// <summary>
    /// Primary key identifier
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Add your required properties here with appropriate constraints
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description or additional fields
    /// </summary>
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    // Navigation properties for relationships
    // Example:
    // public virtual ICollection<RelatedClass> RelatedItems { get; set; } = new List<RelatedClass>();
    // public int ParentClassId { get; set; }
    // public virtual ParentClass? ParentClass { get; set; }
}
