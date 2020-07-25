using System;
using System.Threading;
using System.Text.Json;

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
        public static string BlogData                         { get; set; } = null;
        public static string ChangelogData               { get; set; } = null;
        public static string RoadmapData                   { get; set; } = null;
        public static string PhotoData            { get; set; } = null;
        public static string VideoData            { get; set; } = null;

        public static Timer DataUpdateTimer                     { get; private set; } = null;
        public static JsonSerializerOptions jsonoptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        public static void StartDataCollection()
        {
            DataUpdateTimer = new Timer(async (object state) => await CIGDataCollector.CollectRoadmapData(), 
                null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }
    }
}
