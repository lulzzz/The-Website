using System;
using System.Threading;
using System.Text.Json;
using System.Text.Json.Serialization;

using SA.Web.Shared.Data.WebSockets;
using SA.Web.Shared;

namespace SA.Web.Server.Data
{
    public static class ServerState
    {
        public static LastUpdateTimes UpdateTimes               { get; set; } = null;
        public static string NewsData                           { get; set; } = null;
        public static string ChangelogData                      { get; set; } = null;
        public static string RoadmapData                        { get; set; } = null;
        public static string PhotoData                          { get; set; } = null;
        public static string VideoData                          { get; set; } = null;

        public static Timer DataUpdateTimer                     { get; private set; } = null;
        public static JsonSerializerOptions jsonoptions = new JsonSerializerOptions
        {
            DefaultBufferSize = Globals.MaxSocketBufferSize,
            NumberHandling = JsonNumberHandling.Strict
        };

        public static void StartDataCollection()
        {
            DataUpdateTimer = new Timer(async (object state) => await CIGDataCollector.CollectRoadmapData(), 
                null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }
    }
}
