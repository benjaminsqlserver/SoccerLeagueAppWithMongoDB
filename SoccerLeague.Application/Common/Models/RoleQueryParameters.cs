namespace SoccerLeague.Application.Common.Models
{
    public class RoleQueryParameters : QueryParameters
    {
        public bool? IsActive { get; set; }
        public bool? IsSystemRole { get; set; }
    }
}
