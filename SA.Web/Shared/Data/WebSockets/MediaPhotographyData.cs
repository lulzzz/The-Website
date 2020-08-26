using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SA.Web.Shared.Data.WebSockets
{
    public class MediaPhotographyData
    {
        [JsonPropertyName("Photos")]
        public List<MediaPhoto> Photos { get; set; }
    }

    public class MediaPhoto
    {
        [JsonPropertyName("Title")]
        public string Title { get; set; }
        [JsonPropertyName("Thumbnail")]
        public string Thumbnail { get; set; }
        [JsonPropertyName("TakenGameVersion")]
        public string TakenGameVersion { get; set; }
        [JsonPropertyName("TakenDate")]
        public DateTime TakenDate { get; set; }
        [JsonPropertyName("Resolutions")]
        public List<string> Resolutions { get; set; }
    }
}
