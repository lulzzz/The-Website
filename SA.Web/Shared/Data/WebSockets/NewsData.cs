﻿using System;
using System.Collections.Generic;

namespace SA.Web.Shared.Data.WebSockets
{
    public class NewsData
    {
        public List<NewsEntryData> NewsPosts { get; set; }
    }

    public class NewsEntryData
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public DateTime? LastEditTime { get; set; }
        public List<string> Content { get; set; } 
    }
}