using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using TwitchLib.Api.Auth;

namespace LiveChatStream.API.Twitch
{
    internal static class Authorization
    {
        public static TwitchLib.Api.TwitchAPI API { get; private set; }
        public static void Start()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            API = new TwitchLib.Api.TwitchAPI();
            
            API.Settings.ClientId = Config.TwitchClientId;
            var isValid = Task.Run(() => ValidateAccessToken()).Result;
            
            if (!isValid)
            {
                ValidateCreds();
                
                string authCode = GetCodeForAuth();

                AuthCodeResponse resp = new AuthCodeResponse();
                try
                {
                    resp = API.Auth.GetAccessTokenFromCodeAsync(authCode, Config.TwitchClientSecret, Config.TwitchRedirectUri, Config.TwitchClientId).Result;
                }
                catch (Exception)
                {
                    throw new Exception("Don't get access token");
                }
                API.Settings.AccessToken = resp.AccessToken;
                Properties.Settings.Default.TwitchAccessToken = resp.AccessToken;
                Properties.Settings.Default.TwitchRefreshToken = resp.RefreshToken;
                Properties.Settings.Default.Save();
            }
            else
            {
                var refresh = API.Auth.RefreshAuthTokenAsync(Properties.Settings.Default.TwitchRefreshToken, Config.TwitchClientSecret).Result;
                API.Settings.AccessToken = refresh.AccessToken;
                Properties.Settings.Default.TwitchAccessToken = refresh.AccessToken;
                Properties.Settings.Default.TwitchRefreshToken = refresh.RefreshToken;
                Properties.Settings.Default.Save();
            }
        }

        private static async Task<bool> ValidateAccessToken()
        {
            try
            {
                var isValidToken = await API.Auth.ValidateAccessTokenAsync(Properties.Settings.Default.TwitchAccessToken);
                if (isValidToken == null) return false;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        private static string GetCodeForAuth()
        {
            var server = new WebServer(Config.TwitchRedirectUri);
            System.Diagnostics.Process.Start(GetAuthorizationCodeUrl(Config.TwitchClientId, Config.TwitchRedirectUri, Config.TwitchScopes));
            var auth = server.Listen();
            server.Stop();
            return auth.Code;
        }
        private static string GetAuthorizationCodeUrl(string clientId, string redirectUri, List<string> scopes)
        {
            var scopesStr = String.Join("+", scopes.ToArray());
            string result = "https://id.twitch.tv/oauth2/authorize?" +
                   $"client_id={clientId}&" +
                   $"redirect_uri={HttpUtility.UrlEncode(redirectUri)}&" +
                   "response_type=code&" +
                   $"scope={scopesStr}";
            return result;
        }
        private static void ValidateCreds()
        {
            if (String.IsNullOrEmpty(Config.TwitchClientId))
                throw new Exception("client id cannot be null or empty");
            if (String.IsNullOrEmpty(Config.TwitchClientSecret))
                throw new Exception("client secret cannot be null or empty");
            if (String.IsNullOrEmpty(Config.TwitchRedirectUri))
                throw new Exception("redirect uri cannot be null or empty");
        }
    }
}
