using System;
using System.Collections.Generic;

namespace SA.Web.Shared.Data.WebSockets
{
    public class MediaVideographyData
    {
        public List<MediaVideo> Videos { get; set; }
    }

    public class MediaVideo
    {
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public string URL { get; set; }
    }
}
