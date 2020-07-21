using System;
using System.Threading;

using Newtonsoft.Json;

using SA.Web.Shared.Data.WebSockets;

namespace SA.Web.Server.Data
{
    public static class ServerState
    {
        public static Uri LastUpdateTimesLink                   { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/update-times.json");
        public static Uri BlogDataLink                          { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/blog-data.json");
        public static Uri ChangelogDataLink                     { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/changelog-posts.json");
        public static Uri RoadmapVersionsLink                   { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/roadmap-versions.json");
        public static Uri PhotographyDataLink                   { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/photography-data.json");
        public static Uri VideographyDataLink                   { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/videography-data.json");

        public static LastUpdateTimes UpdateTimes               { get; set; } = null;
        public static BlogData BlogData                         { get; set; } = null;
        public static ChangelogData ChangelogData               { get; set; } = null;
        public static RoadmapData RoadmapData                   { get; set; } = null;
        public static MediaPhotographyData PhotoData            { get; set; } = null;
        public static MediaVideographyData VideoData            { get; set; } = null;

        public static Timer DataUpdateTimer                     { get; private set; } = null;
        public static JsonSerializerSettings JsonSettings       { get; set; } = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
            MissingMemberHandling = MissingMemberHandling.Error
        };

        public static void StartDataCollection()
        {
            DataUpdateTimer = new Timer(async (object state) => await CIGDataCollector.CollectRoadmapData(), 
                null, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1));
        }
    }
}
