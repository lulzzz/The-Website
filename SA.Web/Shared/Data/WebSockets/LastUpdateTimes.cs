using System;
using System.Text.Json.Serialization;

namespace SA.Web.Shared.Data.WebSockets
{
    public class LastUpdateTimes
    {
        [JsonPropertyName("RoadmapDataUpdate")]
        public DateTime RoadmapDataUpdate { get; set; }
        [JsonPropertyName("BlogDataUpdate")]
        public DateTime BlogDataUpdate { get; set; }
        [JsonPropertyName("ChangelogDataUpdate")]
        public DateTime ChangelogDataUpdate { get; set; }
        [JsonPropertyName("PhotographyDataUpdate")]
        public DateTime PhotographyDataUpdate { get; set; }
        [JsonPropertyName("VideographyDataUpdate")]
        public DateTime VideographyDataUpdate { get; set; }
    }
}
