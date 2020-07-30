﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.IO;

using Microsoft.Net.Http.Headers;

using SA.Web.Shared.Data.WebSockets;
using SA.Web.Server.WebSockets;
using SA.Web.Shared;

namespace SA.Web.Server.Data
{
    public static class CIGDataCollector
    {
        private static bool firstRound = true;

        public static async Task CollectRoadmapData()
        {
            bool sendUpdate = false;

            await Logger.LogInfo("Checking for repository updates...");
            WebRequest request = WebRequest.Create(Globals.LastUpdateTimesLink);
            MemoryStream upTimesStream = new MemoryStream(Encoding.UTF8.GetBytes(await DownloadDataString(Globals.LastUpdateTimesLink)));
            LastUpdateTimes upTimes = JsonSerializer.DeserializeAsync<LastUpdateTimes>(upTimesStream).Result;
            if (ServerState.RoadmapData == null || DateTime.Compare(upTimes.RoadmapDataUpdate.ToUniversalTime(), ServerState.UpdateTimes.RoadmapDataUpdate.ToUniversalTime()) > 0)
            {
                await GetData("Roadmap Data", Globals.RoadmapVersionsLink, async (result) =>
                {
                    RoadmapData r = new RoadmapData { Cards = new List<RoadmapCard>() };
                    foreach (string v in JsonSerializer.Deserialize<RoadmapCardVersions>(result).Versions)
                    {
                        try
                        {
                            r.Cards.Add(JsonSerializer.Deserialize<RoadmapCard>(Encoding.UTF8.GetBytes(await DownloadDataString(
                                new Uri(Globals.RoadmapVersionsIndividualLink + v + ".json"))), ServerState.jsonoptions));
                        }
                        catch (JsonException e) { await Logger.LogError(e.Message); }
                    }
                    ServerState.RoadmapData = JsonSerializer.Serialize(r, typeof(RoadmapData));
                    if (!firstRound) await ((StateSocketHandler)Startup.Services.GetService(typeof(StateSocketHandler))).SendMessageToAllAsync("JSON." + typeof(RoadmapData).Name + ServerState.RoadmapData);
                });
            }
            if (ServerState.BlogData == null || DateTime.Compare(upTimes.BlogDataUpdate.ToUniversalTime(), ServerState.UpdateTimes.BlogDataUpdate.ToUniversalTime()) > 0)
            {
                await GetData("Blog Data", Globals.BlogDataLink, async (result) => 
                { 
                    ServerState.BlogData = result;
                    if (!firstRound) await ((StateSocketHandler)Startup.Services.GetService(typeof(StateSocketHandler))).SendMessageToAllAsync("JSON." + typeof(BlogData).Name + ServerState.BlogData);
                });
            }
            if (ServerState.ChangelogData == null || DateTime.Compare(upTimes.ChangelogDataUpdate.ToUniversalTime(), ServerState.UpdateTimes.ChangelogDataUpdate.ToUniversalTime()) > 0)
            {
                await GetData("Changelog Data", Globals.ChangelogDataLink, async (result) => 
                {
                    ServerState.ChangelogData = result;
                    if (!firstRound) await ((StateSocketHandler)Startup.Services.GetService(typeof(StateSocketHandler))).SendMessageToAllAsync("JSON." + typeof(ChangelogData).Name + ServerState.ChangelogData);
                });
            }
            if (ServerState.PhotoData == null || DateTime.Compare(upTimes.PhotographyDataUpdate.ToUniversalTime(), ServerState.UpdateTimes.PhotographyDataUpdate.ToUniversalTime()) > 0)
            {
                await GetData("Photography Data", Globals.PhotographyDataLink, async (result) => 
                {
                    ServerState.PhotoData = result;
                    if (!firstRound) await ((StateSocketHandler)Startup.Services.GetService(typeof(StateSocketHandler))).SendMessageToAllAsync("JSON." + typeof(MediaPhotographyData).Name + ServerState.PhotoData);
                });
            }
            if (ServerState.VideoData == null || DateTime.Compare(upTimes.VideographyDataUpdate.ToUniversalTime(), ServerState.UpdateTimes.VideographyDataUpdate.ToUniversalTime()) > 0)
            {
                await GetData("Videography Data", Globals.VideographyDataLink, async (result) => 
                {
                    ServerState.VideoData = result;
                    if (!firstRound) await ((StateSocketHandler)Startup.Services.GetService(typeof(StateSocketHandler))).SendMessageToAllAsync("JSON." + typeof(MediaVideographyData).Name + ServerState.VideoData);
                });
            }
            ServerState.UpdateTimes = upTimes;
            if (Startup.Services != null && sendUpdate) await ((StateSocketHandler)Startup.Services.GetService(typeof(StateSocketHandler))).SendMessageToAllAsync(
                Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(ServerState.UpdateTimes)));

            firstRound = false;

            async Task GetData(string logName, Uri link, Action<string> notify)
            {
                await Logger.LogInfo("  -  " + logName + " update available.");
                notify.Invoke(await DownloadDataString(link));
                sendUpdate = true;
            }
        }

        private static async Task<string> DownloadDataString(Uri uri)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage msg = new HttpRequestMessage
                {
                    RequestUri = uri,
                    Method = HttpMethod.Get,
                };
                msg.Headers.Add(HeaderNames.UserAgent, "StarAthenaeumWebsiteServer");
                string result = string.Empty;
                await client.SendAsync(msg).ContinueWith(async (msgTask) => result = await msgTask.Result.Content.ReadAsStringAsync());
                return result;
            }
        }
    }
}
