using Google.Apis.YouTube.v3;
using LiveChatStream.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LiveChatStream.API.YouTube
{
    internal class Client
    {
        private Authorization _auth { get; set; }
        private YouTubeService _youtubeService { get; set; }
        private string _liveChatId { get; set; } = null;
        private Timer _timerGetLiveId { get; set; } = null;
        private Timer _timerUpdateLiveMessages { get; set; } = null;
        private bool _isAuth { get; set; } = false;
        private Functions _functions { get; set; } = null;
        private int _countMessageReceived { get; set; } = 0;
        public event EventHandler<OnGetLiveChatIdArgs> OnGetLiveChatId;
        public event EventHandler<OnAddMessageArgs> OnAddMessage;

        public bool Connect()
        {
            _auth = new Authorization();
            try
            {   
                if(!_isAuth)
                    _isAuth = Task.Run(() => _auth.GetCredentials()).Result;
                return true;
            }
            catch (Exception)
            {
                throw new Exception("BadAuthYoutubeException");
            }
            finally
            {
                if (_isAuth)
                {
                    _youtubeService = _auth.YoutubeService;
                    GetLiveChatIdByTimer();
                }
                else throw new Exception("BadAuthYoutubeFalse");
            }
        }
        public void GetLiveChatIdByTimer()
        {
            if (_timerGetLiveId == null)
            {
                _timerGetLiveId = new Timer { Interval = 10000 };
                _timerGetLiveId.Elapsed += _timerGetLiveId_Elapsed;
            }
            if(!_timerGetLiveId.Enabled)
                _timerGetLiveId.Start();
        }
        private async void _timerGetLiveId_Elapsed(object sender, ElapsedEventArgs e)
        {
            var isGetChatId = await _auth.GetLiveChatId();
            OnGetLiveChatId?.Invoke(this, new OnGetLiveChatIdArgs { IsLiveChatId = isGetChatId });
            if(isGetChatId)
            {
                _timerGetLiveId.Stop();
            }
        }
        public void GetLiveChatMessageByTimer()
        {
            if (_timerUpdateLiveMessages == null)
            {
                _timerUpdateLiveMessages = new Timer { Interval = 1000 };
                _functions = new Functions(_auth.YoutubeService, _auth.LiveChatId);
                _countMessageReceived = 0;
                _timerUpdateLiveMessages.Elapsed += _timerUpdateLiveMessages_Elapsed;
            }
            if (!_timerUpdateLiveMessages.Enabled)
                _timerUpdateLiveMessages.Start();
        }

        private async void _timerUpdateLiveMessages_Elapsed(object sender, ElapsedEventArgs e)
        {
            var isGetMessage = await _functions.GetLiveMessagesAsync();
            if(isGetMessage)
            {
                for(int i= _countMessageReceived; i<_functions.ListMessages.Count;i++)
                {
                    var message = new MessageInfo
                    {
                        Id = _functions.ListMessages[i].Id,
                        UserId = _functions.ListMessages[i].AuthorDetails.ChannelId,
                        TimeCreated = DateTime.Now,
                        UserName = _functions.ListMessages[i].AuthorDetails.DisplayName,
                        Message = _functions.ListMessages[i].Snippet.DisplayMessage,
                        Platform = "YouTube"
                    };
                    OnAddMessage?.Invoke(this, new OnAddMessageArgs { MessageInfo = message });
                }
                _countMessageReceived = _functions.ListMessages.Count;
            }
        }
        public void TimerLiveChatMessageStop()
        {
            if (_timerUpdateLiveMessages.Enabled)
                _timerUpdateLiveMessages.Stop();
        }
    }
}
