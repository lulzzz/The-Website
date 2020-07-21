using System;

namespace SA.Web.Shared
{
    public static class Globals
    {
        public static bool IsDevelopmentMode { get; set; }
        public static int MaxWebSocketMessageBufferSize { get; set; } = 512 * 1024;
    }
}
