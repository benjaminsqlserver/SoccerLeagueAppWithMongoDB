// ============================================================================
// FILE: Domain/Entities/Match.cs
// ============================================================================
namespace SoccerLeague.Domain.Entities
{
    using SoccerLeague.Domain.Common;

    public class Match : BaseEntity
    {
        public string SeasonId { get; set; } = string.Empty;
        public string HomeTeamId { get; set; } = string.Empty;
        public string AwayTeamId { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public string Venue { get; set; } = string.Empty;
        public int Round { get; set; } // Matchday/Round number
        public string MatchStatusId { get; set; } = string.Empty; // Foreign key to MatchStatus

        // Match Results
        public int? HomeTeamScore { get; set; }
        public int? AwayTeamScore { get; set; }
        public int? HomeTeamHalfTimeScore { get; set; }
        public int? AwayTeamHalfTimeScore { get; set; }

        // Match Officials
        public string? Referee { get; set; }
        public string? AssistantReferee1 { get; set; }
        public string? AssistantReferee2 { get; set; }
        public string? FourthOfficial { get; set; }

        // Additional Info
        public int? Attendance { get; set; }
        public string? WeatherConditions { get; set; }
        public string? Notes { get; set; }
        public DateTime? ActualKickoffTime { get; set; }
        public DateTime? FullTimeTime { get; set; }

        // Match Events (stored as embedded documents)
        public List<MatchEvent> Events { get; set; } = new List<MatchEvent>();

        // Computed properties
        public string Result
        {
            get
            {
                if (!HomeTeamScore.HasValue || !AwayTeamScore.HasValue)
                    return "N/A";//not available

                if (HomeTeamScore > AwayTeamScore) return "H";//home win
                if (AwayTeamScore > HomeTeamScore) return "A";//away win
                return "D";//draw
            }
        }
    }

    public class MatchEvent
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string MatchEventTypeId { get; set; } = string.Empty; // Foreign key to MatchEventType
        public int Minute { get; set; }
        public string PlayerId { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public string TeamId { get; set; } = string.Empty;
        public string? AssistPlayerId { get; set; }
        public string? AssistPlayerName { get; set; }
        public string? SubstitutedPlayerId { get; set; } // For substitutions
        public string? SubstitutedPlayerName { get; set; }
        public string? Notes { get; set; }
    }
}