﻿using System;

namespace SA.Web.Shared
{
    public static class Globals
    {
#if DEBUG
        public static Uri CoreLink                              { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/TEST");

        public static Uri LastUpdateTimesLink                   { get; private set; } = new Uri(CoreLink + "/update-times.json");
        public static Uri BlogDataLink                          { get; private set; } = new Uri(CoreLink + "/blog-data.json");
        public static Uri ChangelogDataLink                     { get; private set; } = new Uri(CoreLink + "/changelog-posts.json");
        public static Uri RoadmapVersionsLink                   { get; private set; } = new Uri(CoreLink + "/roadmap-versions.json");
        public static Uri PhotographyDataLink                   { get; private set; } = new Uri(CoreLink + "/photography-data.json");
        public static Uri VideographyDataLink                   { get; private set; } = new Uri(CoreLink + "/videography-data.json");

        public static Uri RoadmapVersionsIndividualLink         { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/TEST/roadmap-cards/");
#else
        public static Uri CoreLink                              { get; private set; } = new Uri("https://raw.githubusercontent.com/Star-Athenaeum/Data-Vault/master/LIVE");

        public static Uri LastUpdateTimesLink                   { get; private set; } = new Uri(CoreLink + "/update-times.json");
        public static Uri BlogDataLink                          { get; private set; } = new Uri(CoreLink + "/blog-data.json");
        public static Uri ChangelogDataLink                     { get; private set; } = new Uri(CoreLink + "/changelog-posts.json");
        public static Uri RoadmapVersionsLink                   { get; private set; } = new Uri(CoreLink + "/roadmap-versions.json");
        public static Uri PhotographyDataLink                   { get; private set; } = new Uri(CoreLink + "/photography-data.json");
        public static Uri VideographyDataLink                   { get; private set; } = new Uri(CoreLink + "/videography-data.json");

        public static Uri RoadmapVersionsIndividualLink         { get; private set; } = new Uri(CoreLink + "/roadmap-cards/");
#endif


        public static bool IsDevelopmentMode { get; set; }
        public static int MaxWebSocketMessageBufferSize { get; set; } = 512 * 1024;
    }
}
