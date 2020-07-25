using System;

namespace SA.Web.Shared.Data.WebSockets
{
    public enum Commands : int
    {
        GetUpdateData,
        GetBlogData,
        GetRoadmapData,
        GetChangelogData,
        GetPhotographyData,
        GetVideographyData
    }
}
