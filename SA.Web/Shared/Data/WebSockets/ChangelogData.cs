using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SA.Web.Shared.Data.WebSockets
{
    public class ChangelogData
    {
        [JsonPropertyName("ChangelogPosts")]
        public List<ChangelogEntryData> ChangelogPosts { get; set; }
    }

    public class ChangelogEntryData
    {
        [JsonPropertyName("Title")]
        public string Title { get; set; }
        [JsonPropertyName("Description")]
        public string Description { get; set; }
        [JsonPropertyName("Additions")]
        public List<string> Additions { get; set; }
        [JsonPropertyName("Removals")]
        public List<string> Removals { get; set; }
        [JsonPropertyName("Changes")]
        public List<string> Changes { get; set; }
    }
}
