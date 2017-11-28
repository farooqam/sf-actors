using System;
using System.Runtime.Serialization;

namespace FootballStatsApi.Dal.Common.Dto
{
    [DataContract]
    public class TeamStatsDto
    {
        [DataMember]
        public short Year { get; set; }

        [DataMember]
        public byte Week { get; set; }

        [DataMember]
        public string TeamId { get; set; }

        [DataMember]
        public string TeamName { get; set; }

        [DataMember]
        public short PointsFor { get; set; }

        [DataMember]
        public short PointsAgainst { get; set; }

        [DataMember]
        public byte Wins { get; set; }

        [DataMember]
        public byte Losses { get; set; }

        [DataMember]
        public byte GamesPlayed { get; set; }

        [DataMember]
        public DateTime Created { get; set; }

        [DataMember]
        public DateTime LastUpdated { get; set; }
    }
}