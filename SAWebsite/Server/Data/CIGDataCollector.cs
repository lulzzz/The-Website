using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

using Microsoft.Net.Http.Headers;

using Newtonsoft.Json;

using SAWebsite.Shared.Data.WebSockets;
using SAWebsite.Server.WebSockets;

namespace SAWebsite.Server.Data
{
    public static class CIGDataCollector
    {
        public static async Task CollectRoadmapData()
        {
            bool sendUpdate = false;

            await Logger.Log(LogLevel.Info, "Checking for repository updates...");
            WebRequest request = WebRequest.Create(ServerState.LastUpdateTimesLink);
            LastUpdateTimes upTimes = JsonConvert.DeserializeObject<LastUpdateTimes>(await DownloadDataString(ServerState.LastUpdateTimesLink));
            if (ServerState.RoadmapData == null || DateTime.Compare(upTimes.RoadmapDataUpdate.ToUniversalTime(), ServerState.UpdateTimes.RoadmapDataUpdate.ToUniversalTime()) > 0)
            {
                await GetData("Roadmap Data", ServerState.RoadmapVersionsLink, async (result) =>
                {
                    RoadmapData r = new RoadmapData { Cards = new List<RoadmapCard>() };
                    foreach (string v in result.Versions)
                    {
                        try
                        {
                            r.Cards.Add(JsonConvert.DeserializeObject<RoadmapCard>(await DownloadDataString(new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/roadmap-cards/" + v + ".json"))));
                        }
                        catch (JsonSerializationException e) { await Logger.Log(LogLevel.Error, e.Message); }
                    }
                    ServerState.RoadmapData = r;
                }, out RoadmapCardVersions result);
            }
            if (ServerState.BlogData == null || DateTime.Compare(upTimes.BlogDataUpdate.ToUniversalTime(), ServerState.UpdateTimes.BlogDataUpdate.ToUniversalTime()) > 0)
            {
                await GetData("Blog Data", ServerState.BlogDataLink, (result) => { }, out BlogData result);
                ServerState.BlogData = result;
            }
            if (ServerState.ChangelogData == null || DateTime.Compare(upTimes.ChangelogDataUpdate.ToUniversalTime(), ServerState.UpdateTimes.ChangelogDataUpdate.ToUniversalTime()) > 0)
            {
                await GetData("Changelog Data", ServerState.ChangelogDataLink, (result) => { }, out ChangelogData result);
                ServerState.ChangelogData = result;
            }
            if (ServerState.PhotoData == null || DateTime.Compare(upTimes.PhotographyDataUpdate.ToUniversalTime(), ServerState.UpdateTimes.PhotographyDataUpdate.ToUniversalTime()) > 0)
            {
                await GetData("Photography Data", ServerState.PhotographyDataLink, (result) => { }, out MediaPhotographyData result);
                ServerState.PhotoData = result;
            }
            if (ServerState.VideoData == null || DateTime.Compare(upTimes.VideographyDataUpdate.ToUniversalTime(), ServerState.UpdateTimes.VideographyDataUpdate.ToUniversalTime()) > 0)
            {
                await GetData("Videography Data", ServerState.VideographyDataLink, (result) => { }, out MediaVideographyData result);
                ServerState.VideoData = result;
            }
            ServerState.UpdateTimes = upTimes;
            if (Startup.Services != null && sendUpdate) await ((StateSocketHandler)Startup.Services.GetService(typeof(StateSocketHandler))).SendMessageToAllAsync(JsonConvert.SerializeObject(ServerState.UpdateTimes));

            Task GetData<T>(string logName, Uri link, Action<T> notify, out T result)
            {
                Logger.Log(LogLevel.Info, "  -  " + logName + " update available.");
                result = JsonConvert.DeserializeObject<T>(DownloadDataString(link).GetAwaiter().GetResult(), ServerState.JsonSettings);
                notify.Invoke(result);
                sendUpdate = true;
                return Task.CompletedTask;
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
                msg.Headers.Add(HeaderNames.UserAgent, "StarAthenaeumServer");
                string result = string.Empty;
                await client.SendAsync(msg).ContinueWith(async (msgTask) => result = await msgTask.Result.Content.ReadAsStringAsync());
                return result;
            }
        }
    }
}
