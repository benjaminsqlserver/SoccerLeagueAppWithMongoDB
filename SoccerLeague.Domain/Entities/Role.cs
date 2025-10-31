namespace SoccerLeague.Domain.Entities
{
    using SoccerLeague.Domain.Common;

    /// <summary>
    /// Represents a role in the role-based access control (RBAC) system.
    /// Roles group permissions and are assigned to users.
    /// </summary>
    public class Role : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique name of the role.
        /// Examples: "Administrator", "Manager", "User", "Fan"
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the normalized name for case-insensitive comparisons.
        /// Automatically set to uppercase version of Name.
        /// </summary>
        public string NormalizedName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the role and its purpose.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this is a system role.
        /// System roles cannot be deleted or have their names changed.
        /// </summary>
        public bool IsSystemRole { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether this role is active.
        /// Inactive roles cannot be assigned to new users.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the display order for sorting roles in lists.
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the collection of permission codes assigned to this role.
        /// </summary>
        public List<string> Permissions { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets additional metadata for the role in JSON format.
        /// Can store custom properties specific to the application.
        /// </summary>
        public string? Metadata { get; set; }
    }
}
