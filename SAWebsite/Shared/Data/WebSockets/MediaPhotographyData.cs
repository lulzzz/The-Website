using System;
using System.Collections.Generic;

namespace SAWebsite.Shared.Data.WebSockets
{
    public class MediaPhotographyData
    {
        public List<MediaPhoto> Photos { get; set; }
    }

    public class MediaPhoto
    {
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public string TakenGameVersion { get; set; }
        public DateTime TakenDate { get; set; }
        public List<string> Resolutions { get; set; }
    }
}
