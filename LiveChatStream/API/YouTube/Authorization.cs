using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiveChatStream.API.YouTube
{
    internal class Authorization
    {
        private UserCredential _credentials { get; set; }
        public YouTubeService YoutubeService { get; private set; }
        public string LiveChatId { get; set; } = null;
        #region Google Авторизация и инициализация YouTubeService, получение id стрима
        public async Task<bool> GetCredentials()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            try
            {
                cts.CancelAfter(TimeSpan.FromSeconds(60));
                token.ThrowIfCancellationRequested();

                string credentialPath = Path.Combine(Environment.CurrentDirectory, "Auth.Store");
                _credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets()
                    {
                        ClientId = Config.YouTubeClientId,
                        ClientSecret = Config.YouTubeClientSecret
                    },
                    Config.YouTubeScopes,
                    "user",
                    token,
                    new FileDataStore(credentialPath, true)
                );
                if (CheckScopesCredentials())
                {
                    InitYouTubeService();
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cts.Dispose();
            }

            return false;
        }
        public async Task<bool> GetLiveChatId()
        {
            try
            {
                LiveBroadcastsResource.ListRequest listRequest = YoutubeService.LiveBroadcasts.List("snippet");
                listRequest.BroadcastType = LiveBroadcastsResource.ListRequest.BroadcastTypeEnum.All;
                listRequest.Mine = true;
                LiveBroadcastListResponse response = await listRequest.ExecuteAsync();

                foreach (var snippet in response.Items)
                {
                    if (snippet.Snippet.ActualEndTime == null)
                    {
                        LiveChatId = snippet.Snippet.LiveChatId;
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }
        private bool CheckScopesCredentials()
        {
            bool checkYoutubeReadonly = _credentials.Token.Scope.Contains(Config.YouTubeScopes[0]);
            bool checkYoutubeForceSsl = _credentials.Token.Scope.Contains(Config.YouTubeScopes[1]);
            try
            {
                if (checkYoutubeReadonly && checkYoutubeForceSsl) return true;
                else
                {
                    throw new Exception("Ошибка: Вы выдали не все разрешения программе!");
                }
            }
            catch (Exception)
            {
                RevokeToken();
                throw;
            }
        }

        private void RevokeToken()
        {
            _credentials.RevokeTokenAsync(CancellationToken.None);
        }
        private void InitYouTubeService()
        {
            YoutubeService = new YouTubeService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = _credentials,
                ApplicationName = Config.YouTubeApplicationName
            });
        }
        #endregion
    }
}
