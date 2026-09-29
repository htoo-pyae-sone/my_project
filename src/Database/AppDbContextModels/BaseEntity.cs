namespace Database.AppDbContextModels;

/// <summary>
/// Supplies the primary key and timestamps shared by persisted entities.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Database primary key used for internal relationships.</summary>
    public long Id { get; set; }

    /// <summary>Time when the record was first created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Time of the most recent change to the record.</summary>
    public DateTime UpdatedAt { get; set; }
}
