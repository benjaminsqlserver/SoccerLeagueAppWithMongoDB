// ============================================================================
// FILE: Domain/Common/BaseEntity.cs
// ============================================================================
namespace SoccerLeague.Domain.Common
{
    /// <summary>
    /// Base entity class for MongoDB documents.
    /// All domain entities inherit from this class to provide common properties
    /// for tracking, auditing, and soft deletion functionality.
    /// </summary>
    /// <remarks>
    /// This base class implements:
    /// - Unique identification (Id)
    /// - Audit trail (CreatedDate, ModifiedDate, CreatedBy, ModifiedBy)
    /// - Soft deletion pattern (IsDeleted)
    /// </remarks>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// MongoDB typically uses ObjectId, but we're using string for flexibility.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the UTC date and time when the entity was created.
        /// Automatically initialized to current UTC time.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the UTC date and time when the entity was last modified.
        /// Null if the entity has never been modified since creation.
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who created this entity.
        /// Useful for audit trails and tracking entity ownership.
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who last modified this entity.
        /// Updated each time the entity is modified.
        /// </summary>
        public string? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this entity is soft-deleted.
        /// Soft deletion allows data to be hidden from normal queries while preserving
        /// it in the database for audit purposes or potential restoration.
        /// </summary>
        public bool IsDeleted { get; set; } = false;
    }
}
