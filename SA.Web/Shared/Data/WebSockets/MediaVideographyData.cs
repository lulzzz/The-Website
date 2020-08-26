using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SA.Web.Shared.Data.WebSockets
{
    public class MediaVideographyData
    {
        [JsonPropertyName("Videos")]
        public List<MediaVideo> Videos { get; set; }
    }

    public class MediaVideo
    {
        [JsonPropertyName("Title")]
        public string Title { get; set; }
        [JsonPropertyName("Thumbnail")]
        public string Thumbnail { get; set; }
        [JsonPropertyName("URL")]
        public string URL { get; set; }
    }
}
