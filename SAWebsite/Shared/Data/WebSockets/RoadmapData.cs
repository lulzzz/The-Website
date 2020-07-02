using System;
using System.Collections.Generic;

namespace SAWebsite.Shared.Data.WebSockets
{
    public class RoadmapData
    {
        public List<RoadmapCard> Cards { get; set; }
    }

    public class RoadmapCard
    {
        public byte MajorVersion { get; set; }
        public byte MinorVersion { get; set; }
        public string Description { get; set; }
        public List<RoadmapFeature> VersionFeatures { get; set; }
        public Dictionary<byte, RoadmapCardChangelog> Patches { get; set; }
    }

    public class RoadmapCardVersions
    {
        public List<string> Versions { get; set; }
    }

    public enum RoadmapCardCategory
    {
        Characters,
        Locations,
        AI,
        Gameplay,
        ShipsAndVehicles,
        WeaponsAndItems,
        CoreTech
    }

    public class RoadmapCardChangelog
    {
        public string VersionString { get; set; }
        public DateTime? PUReleaseDate { get; set; }
        public DateTime? EvocatiTestingDate { get; set; }
        public DateTime? PTUTestingStartDate { get; set; }
        public string SpectrumPatchNotes { get; set; }
    }

    public class RoadmapFeature
    {
        public string Title { get; set; }
        public RoadmapCardCategory Category { get; set; }
        public Dictionary<DateTime, RoadmapFeatureStatus> Status { get; set; }
        public Dictionary<DateTime, short> TaskCount { get; set; }
        public Dictionary<DateTime, short> TasksCompleted { get; set; }
        public string Description { get; set; }
    }

    public enum RoadmapFeatureStatus
    {
        Scheduled,
        InDevelopment,
        Polishing,
        Released
    }
}
