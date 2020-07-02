using System;

namespace SAWebsite.Shared.Data.WebSockets
{
    public enum Commands
    {
        GetUpdateData       = 0b_0000_0001,
        GetBlogData         = 0b_0000_0010,
        GetRoadmapData      = 0b_0000_0011,
        GetChangelogData    = 0b_0000_0100,
        GetPhotographyData  = 0b_0000_0101,
        GetVideographyData  = 0b_0000_0110
    }
}
