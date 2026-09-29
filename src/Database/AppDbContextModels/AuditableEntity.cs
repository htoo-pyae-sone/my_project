namespace Database.AppDbContextModels;

/// <summary>
/// Adds soft deletion and actor references to entities that need an audit trail.
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    /// <summary>Marks the record as logically removed while retaining it in the database.</summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>Internal ID of the user who created the record, when known.</summary>
    public long? CreatedBy { get; set; }

    /// <summary>Internal ID of the user who last updated the record, when known.</summary>
    public long? UpdatedBy { get; set; }
}
