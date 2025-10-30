namespace SoccerLeague.Application.DTOs.Player
{
    public class CreatePlayerDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public int JerseyNumber { get; set; }
        public string PlayerPositionId { get; set; } = string.Empty;
        public string TeamId { get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string PreferredFoot { get; set; } = string.Empty;
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
    }
}
