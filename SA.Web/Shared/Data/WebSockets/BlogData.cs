using System;
using System.Collections.Generic;

namespace SA.Web.Shared.Data.WebSockets
{
    public class BlogData
    {
        public List<BlogEntryData> BlogPosts { get; set; }
    }

    public class BlogEntryData
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public DateTime LastEditTime { get; set; }
        public List<string> Content { get; set; } 
    }
}
