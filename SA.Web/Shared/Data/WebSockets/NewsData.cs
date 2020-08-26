using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SA.Web.Shared.Data.WebSockets
{
    public class NewsData
    {
        [JsonPropertyName("NewsPosts")]
        public List<NewsEntryData> NewsPosts { get; set; }
    }

    public class NewsEntryData
    {
        [JsonPropertyName("Title")]
        public string Title { get; set; }
        [JsonPropertyName("Date")]
        public DateTime Date { get; set; }
        [JsonPropertyName("LastEditTime")]
        public DateTime? LastEditTime { get; set; }
        [JsonPropertyName("Content")]
        public List<string> Content { get; set; } 
    }
}
