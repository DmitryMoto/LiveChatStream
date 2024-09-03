using Google.Apis.YouTube.v3;

namespace LiveChatStream.API
{
    public static class Config
    {
        public static readonly string TwitchClientId = "";
        public static readonly string TwitchRedirectUri = "http://localhost:8080/";
        public static readonly string TwitchClientSecret = "";
        public static readonly System.Collections.Generic.List<string> TwitchScopes = new System.Collections.Generic.List<string>
        {
            "channel:manage:moderators",
            "channel:moderate",
            "chat:read",
            "whispers:read",
            "whispers:edit",
            "chat:edit",
            "moderator:manage:banned_users",
            "moderation:read",
            "moderator:read:blocked_terms",
            "moderator:manage:blocked_terms"
        };
        public static readonly string bot_login_name = "linkheets_bot";
        public static readonly string bot_access_pass = "";

        public static readonly string YouTubeApplicationName = "LiveChatStream";
        public static readonly string YouTubeClientId = "";
        public static readonly string YouTubeClientSecret = "";
        public static readonly string[] YouTubeScopes = new string[]
        { 
            YouTubeService.Scope.YoutubeReadonly, 
            YouTubeService.Scope.YoutubeForceSsl 
        };
    }
}
