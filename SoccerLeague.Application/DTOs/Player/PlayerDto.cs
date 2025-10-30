using System;

namespace SoccerLeague.Application.DTOs.Player
{
    public class PlayerDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public int JerseyNumber { get; set; }

        // Position Information
        public string PlayerPositionId { get; set; } = string.Empty;
        public string PlayerPositionName { get; set; } = string.Empty;
        public string PlayerPositionCode { get; set; } = string.Empty;

        // Team Information
        public string TeamId { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public string TeamLogo { get; set; } = string.Empty;

        // Physical Attributes
        public string Photo { get; set; } = string.Empty;
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string PreferredFoot { get; set; } = string.Empty;

        // Status
        public bool IsActive { get; set; }

        // Contract Information
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }

        // Statistics
        public int Appearances { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int YellowCards { get; set; }
        public int RedCards { get; set; }
        public int MinutesPlayed { get; set; }

        // Audit
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
