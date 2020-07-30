using System;
using System.Threading;
using System.Text.Json;

using SA.Web.Shared.Data.WebSockets;
using SA.Web.Shared;

namespace SA.Web.Server.Data
{
    public static class ServerState
    {
#if DEBUG
        public static Uri LastUpdateTimesLink { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/TEST/update-times.json");
        public static Uri BlogDataLink { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/TEST/blog-data.json");
        public static Uri ChangelogDataLink { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/TEST/changelog-posts.json");
        public static Uri RoadmapVersionsLink { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/TEST/roadmap-versions.json");
        public static Uri PhotographyDataLink { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/TEST/photography-data.json");
        public static Uri VideographyDataLink { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/TEST/videography-data.json");

        public static Uri RoadmapVersionsIndividualLink { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/TEST/roadmap-cards/");
#else
        public static Uri LastUpdateTimesLink                   { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/LIVE/update-times.json");
        public static Uri BlogDataLink                          { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/LIVE/blog-data.json");
        public static Uri ChangelogDataLink                     { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/LIVE/changelog-posts.json");
        public static Uri RoadmapVersionsLink                   { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/LIVE/roadmap-versions.json");
        public static Uri PhotographyDataLink                   { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/LIVE/photography-data.json");
        public static Uri VideographyDataLink                   { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/LIVE/videography-data.json");

        public static Uri RoadmapVersionsIndividualLink         { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/LIVE/roadmap-cards/");
#endif

        public static LastUpdateTimes UpdateTimes               { get; set; } = null;
        public static string BlogData                         { get; set; } = null;
        public static string ChangelogData               { get; set; } = null;
        public static string RoadmapData                   { get; set; } = null;
        public static string PhotoData            { get; set; } = null;
        public static string VideoData            { get; set; } = null;

        public static Timer DataUpdateTimer                     { get; private set; } = null;
        public static JsonSerializerOptions jsonoptions = new JsonSerializerOptions
        {
            DefaultBufferSize = Globals.MaxWebSocketMessageBufferSize,
            MaxDepth = Globals.MaxWebSocketMessageBufferSize
        };

        public static void StartDataCollection()
        {
            DataUpdateTimer = new Timer(async (object state) => await CIGDataCollector.CollectRoadmapData(), 
                null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }
    }
}
