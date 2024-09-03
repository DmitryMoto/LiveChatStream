using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LiveChatStream.API.Twitch
{
    internal class RevokeToken
    {
        public RevokeToken()
        {
            if(!string.IsNullOrEmpty(Properties.Settings.Default.TwitchAccessToken))
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Properties.Settings.Default.TwitchAccessToken);
                var validToken = Task.Run(() => client.GetAsync("https://id.twitch.tv/oauth2/validate")).Result;
                var content = Task.Run(() => validToken.Content.ReadAsStringAsync()).Result;

                var splitPoint = content.IndexOf(":", 1);
                var endpoint = content.IndexOf(",", 1);
                var splitClient_id = content.Substring(1);
                splitClient_id = content.Substring(splitPoint + 2, endpoint - splitPoint - 3);
                Console.WriteLine(splitClient_id);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"https://id.twitch.tv/oauth2/revoke?client_id={splitClient_id}&token={Properties.Settings.Default.TwitchAccessToken}");
                var response = Task.Run(() => client.SendAsync(request)).Result;
                Console.WriteLine(response);
            }
        }
    }
}
