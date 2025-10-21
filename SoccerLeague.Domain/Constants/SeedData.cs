// ============================================================================
// FILE: Domain/Constants/SeedData.cs
// ============================================================================
namespace SoccerLeague.Domain.Constants
{
    /// <summary>
    /// Contains seed data for lookup entities
    /// These can be used to populate the database initially
    /// </summary>
    public static class SeedData
    {
        public static class PlayerPositions
        {
            public static readonly (string Name, string Code, string Description, int Order)[] DefaultPositions =
            {
                ("Goalkeeper", "GK", "Primary position responsible for preventing goals", 1),
                ("Defender", "DF", "Defensive position focused on stopping opposing attackers", 2),
                ("Midfielder", "MF", "Plays between defense and attack, linking the team", 3),
                ("Forward", "FW", "Attacking position focused on scoring goals", 4)
            };
        }

        public static class MatchStatuses
        {
            public static readonly (string Name, string Code, string Description, string ColorCode, int Order)[] DefaultStatuses =
            {
                ("Scheduled", "SCH", "Match is scheduled to be played", "#6366f1", 1),
                ("In Progress", "INP", "Match is currently being played", "#22c55e", 2),
                ("Completed", "COM", "Match has been completed", "#64748b", 3),
                ("Postponed", "PST", "Match has been postponed to a later date", "#f59e0b", 4),
                ("Cancelled", "CAN", "Match has been cancelled", "#ef4444", 5)
            };
        }

        public static class TeamStatuses
        {
            public static readonly (string Name, string Code, string Description, string ColorCode, int Order)[] DefaultStatuses =
            {
                ("Active", "ACT", "Team is actively participating", "#22c55e", 1),
                ("Inactive", "INA", "Team is not currently active", "#94a3b8", 2),
                ("Suspended", "SUS", "Team is temporarily suspended", "#ef4444", 3)
            };
        }

        public static class SeasonStatuses
        {
            public static readonly (string Name, string Code, string Description, string ColorCode, int Order)[] DefaultStatuses =
            {
                ("Not Started", "NST", "Season has not yet begun", "#6366f1", 1),
                ("In Progress", "INP", "Season is currently ongoing", "#22c55e", 2),
                ("Completed", "COM", "Season has been completed", "#64748b", 3),
                ("Cancelled", "CAN", "Season has been cancelled", "#ef4444", 4)
            };
        }

        public static class MatchEventTypes
        {
            public static readonly (string Name, string Code, string Description, string Icon, string ColorCode, bool AffectsScore, bool AffectsDiscipline, int Order)[] DefaultEventTypes =
            {
                ("Goal", "GOAL", "Regular goal scored", "⚽", "#22c55e", true, false, 1),
                ("Penalty Scored", "PENS", "Goal scored from penalty", "⚽", "#10b981", true, false, 2),
                ("Own Goal", "OWNG", "Goal scored by opposing team player", "⚽", "#f59e0b", true, false, 3),
                ("Yellow Card", "YELL", "Caution given to player", "🟨", "#eab308", false, true, 4),
                ("Red Card", "RED", "Player sent off", "🟥", "#ef4444", false, true, 5),
                ("Substitution", "SUB", "Player substitution", "🔄", "#6366f1", false, false, 6),
                ("Penalty Missed", "PENM", "Penalty kick missed", "❌", "#94a3b8", false, false, 7)
            };
        }
    }
}