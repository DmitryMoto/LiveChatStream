using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChatStream.API.YouTube
{
    internal class Functions
    {
        private YouTubeService _youtubeService { get; set; }
        private string _liveChatId { get; set; }
        public IList<LiveChatMessage> ListMessages { get; set; } = new List<LiveChatMessage>();
        public Functions(YouTubeService youtubeService, string liveChatID)
        {
            _youtubeService = youtubeService;
            _liveChatId = liveChatID;
        }
        #region Get, Insert, Delete сообщений стрима
        public async Task<bool> GetLiveMessagesAsync()
        {
            if (!String.IsNullOrEmpty(_liveChatId))
            {
                LiveChatMessagesResource.ListRequest liveChatMessages = _youtubeService.LiveChatMessages.List(_liveChatId, "id, snippet, authorDetails");
                LiveChatMessageListResponse lcR = await liveChatMessages.ExecuteAsync();
                ListMessages = lcR.Items;
                if (ListMessages.Count > 0) return true;
            }

            return false;
        }

        public async Task InsertLiveMessageAsync(string message)
        {
            try
            {
                LiveChatMessage liveChatMessage = new LiveChatMessage();
                liveChatMessage.Snippet = new LiveChatMessageSnippet();
                liveChatMessage.Snippet.LiveChatId = _liveChatId;
                liveChatMessage.Snippet.Type = "textMessageEvent";
                liveChatMessage.Snippet.TextMessageDetails = new LiveChatTextMessageDetails();
                liveChatMessage.Snippet.TextMessageDetails.MessageText = message;
                LiveChatMessagesResource.InsertRequest request = _youtubeService.LiveChatMessages.Insert(liveChatMessage, "snippet");
                await request.ExecuteAsync();
            }
            catch (Exception)
            {
                throw new Exception("Произошла ошибка при отправки сообщения");
            }
        }
        public async Task<bool> DeleteLiveMessageAsync(string idMessage)
        {
            LiveChatMessagesResource.DeleteRequest request = _youtubeService.LiveChatMessages.Delete(idMessage);
            String value = await request.ExecuteAsync();
            return false;
        }
        #endregion
    }
}
