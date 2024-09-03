using LiveChatStream.API;
using LiveChatStream.API.Twitch;
using LiveChatStream.API.YouTube;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using TwitchLib.Api.Auth;

namespace LiveChatStream
{
    public partial class Form1 : Form
    {
        private Client _youTubeClient { get; set; } = null;
        private Bot _twitchBot { get; set; } = null;
        public Form1()
        {
            InitializeComponent();
            if(Twitch_checkBox.Checked)
            {
                TwitchInitialize();
            }
            if (YouTube_checkBox.Checked)
            {
                YouTubeInitialize();
            }

        }
        private async void TwitchInitialize()
        {
            _twitchBot = new Bot();
            await _twitchBot.AddBotToChannel();
            await _twitchBot.ConnectBotToIRCChannel();
            _twitchBot.OnAddMessage += Bot_OnAddMessage;
        }
        private void YouTubeInitialize()
        {
            _youTubeClient = new Client();
            var isConnect = _youTubeClient.Connect();
            if(isConnect)
            {
                _youTubeClient.GetLiveChatIdByTimer();
                _youTubeClient.OnGetLiveChatId += YouTubeClient_OnGetLiveChatId;
                _youTubeClient.OnAddMessage += YouTubeClient_OnAddMessage;
            }
        }

        private void YouTubeClient_OnAddMessage(object sender, Events.OnAddMessageArgs e)
        {
            listBox1.Invoke(new Action(() =>
            {
                listBox1.Items.Add($"{e.MessageInfo.Platform} - {e.MessageInfo.UserName}: {e.MessageInfo.Message}");
            }));
        }

        private void YouTubeClient_OnGetLiveChatId(object sender, Events.OnGetLiveChatIdArgs e)
        {
            if(e.IsLiveChatId) _youTubeClient.GetLiveChatMessageByTimer();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TwitchInitialize();
            YouTubeInitialize();
        }

        private void Bot_OnAddMessage(object sender, Events.OnAddMessageArgs e)
        {
            listBox1.Invoke(new Action(() =>
            {
                listBox1.Items.Add($"{e.MessageInfo.Platform} - {e.MessageInfo.UserName}: {e.MessageInfo.Message}");
            }));
        }

        private void TwitchClearPropertySettings()
        {
            Properties.Settings.Default.TwitchAccessToken = null;
            Properties.Settings.Default.TwitchRefreshToken = null;
            Properties.Settings.Default.TwitchBotID = null;
            Properties.Settings.Default.Save();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new API.Twitch.RevokeToken();
            //TwitchClearPropertySettings();
        }
    }
}
