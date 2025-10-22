using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.DTOs.Match
{
    public class CreateMatchEventDto
    {
        public string MatchId { get; set; } = string.Empty;
        public string MatchEventTypeId { get; set; } = string.Empty;
        public int Minute { get; set; }
        public string PlayerId { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public string TeamId { get; set; } = string.Empty;
        public string? AssistPlayerId { get; set; }
        public string? AssistPlayerName { get; set; }
        public string? SubstitutedPlayerId { get; set; }
        public string? SubstitutedPlayerName { get; set; }
        public string? Notes { get; set; }
    }
}

