using System;

namespace SA.Web.Shared.Data.WebSockets
{
    public class LastUpdateTimes
    {
        public DateTime RoadmapDataUpdate { get; set; }
        public DateTime BlogDataUpdate { get; set; }
        public DateTime ChangelogDataUpdate { get; set; }
        public DateTime PhotographyDataUpdate { get; set; }
        public DateTime VideographyDataUpdate { get; set; }
    }
}
