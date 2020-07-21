using System;
using System.Collections.Generic;

namespace SA.Web.Shared.Data.WebSockets
{
    public class ChangelogData
    {
        public List<ChangelogEntryData> ChangelogPosts { get; set; }
    }

    public class ChangelogEntryData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Additions { get; set; }
        public List<string> Removals { get; set; }
        public List<string> Changes { get; set; }
    }
}
