using System;

namespace SAWebsite.Shared
{
    public static class Globals
    {
        public static bool IsDevelopmentMode { get; set; }
        public static int MaxWebSocketMessageBufferSize { get; set; } = 512 * 1024;
    }
}
