using LiveChatStream.API.Twitch.Models;
using LiveChatStream.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.Moderation.BanUser;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace LiveChatStream.API.Twitch
{
    internal class Bot
    {
        private bool _isConnected { get; set; }
        private TwitchClient _client { get; set; }
        private Moderation moderation { get; set; }

        private Dictionary<string, string> _listUsersJoin = new Dictionary<string, string>();

        public event EventHandler<OnAddMessageArgs> OnAddMessage;
        public Bot()
        {
            Authorization.Start();
            moderation = new Moderation();
        }
        public async Task AddBotToChannel()
        {
            await CheckBotID();
            var botIsModerator = await BotIsModerator();
            if (!botIsModerator)
                await moderation.AddChannelModeratorAsync(Properties.Settings.Default.TwitchBotID);
        }

        public async Task RemoveBotFromChannel()
        {
            await moderation.DeleteChannelModeratorAsync(Properties.Settings.Default.TwitchBotID);
        }
        private async Task CheckBotID()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.TwitchBotID))
            {
                var resp = await moderation.GetUsersAsync(null, new List<string> { Config.bot_login_name });
                Properties.Settings.Default.TwitchBotID = resp.Users[0].Id;
                Properties.Settings.Default.Save();
            }
        }
        private async Task<bool> BotIsModerator()
        {
            var checkBotModer = await moderation.GetModeratorsAsync(Properties.Settings.Default.TwitchBotID);
            if (checkBotModer.Data.Length == 0) return false;
            return true;
        }

        public async Task ConnectBotToIRCChannel()
        {
            if (!_isConnected)
            {
                //var isStreaming = await OnlineOrOfflineStream();
                Connect();
            }

        }

        private async Task<bool> OnlineOrOfflineStream()
        {
            var getStream = await moderation.GetStreamsAsync();

            if (getStream.Streams.Length > 0) return true;
            return false;
        }

        private void Connect()
        {
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };

            WebSocketClient customClient = new WebSocketClient(clientOptions);
            _client = new TwitchClient(customClient);
            _client.Initialize(new ConnectionCredentials(Config.bot_login_name, Config.bot_access_pass), moderation.broadcaster_channel);

            _client.OnLog += _client_OnLog;
            _client.OnConnected += _client_OnConnected;
            _client.OnJoinedChannel += _client_OnJoinedChannel;
            _client.OnReconnected += _client_OnReconnected;
            _client.OnDisconnected += _client_OnDisconnected;
            _client.OnMessageReceived += _client_OnMessageReceived;
            _client.OnUserTimedout += _client_OnUserTimedout;
            _client.AddChatCommandIdentifier('!');
            _client.OnChatCommandReceived += _client_OnChatCommandReceived;
            _client.OnUserJoined += _client_OnUserJoined;
            _client.OnUserLeft += _client_OnUserLeft;

            _isConnected = _client.Connect();
            
        }

        private void _client_OnUserLeft(object sender, TwitchLib.Client.Events.OnUserLeftArgs e)
        {
            if (_listUsersJoin.ContainsKey(e.Username))
            {
                _listUsersJoin[e.Username] = "2";
            }
        }

        private void _client_OnDisconnected(object sender, TwitchLib.Communication.Events.OnDisconnectedEventArgs e)
        {
            Console.WriteLine("Bot Disconnected");
            _isConnected = false;
        }

        private void _client_OnUserJoined(object sender, TwitchLib.Client.Events.OnUserJoinedArgs e)
        {
            if (_listUsersJoin.Count > 0)
            {
                if (!_listUsersJoin.ContainsKey(e.Username) && !e.Username.Equals(Config.bot_login_name))
                {
                    _listUsersJoin.Add(e.Username, "1");
                    _client.SendMessage(e.Channel, $"{e.Username}, Привет!");
                }
                else
                {
                    if (_listUsersJoin[e.Username].Equals("2"))
                        _client.SendMessage(e.Channel, $"О! {e.Username}, ты вернулся!");
                }
            }

        }

        private void _client_OnReconnected(object sender, TwitchLib.Communication.Events.OnReconnectedEventArgs e)
        {
            Console.WriteLine("Bot Reconnected");
        }

        private void _client_OnChatCommandReceived(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
        {
            if (e.Command.CommandText.Equals("bot")) _client.SendReply(e.Command.ChatMessage.Channel, e.Command.ChatMessage.Id, "Чем могу служить?!");
        }

        private void _client_OnUserTimedout(object sender, TwitchLib.Client.Events.OnUserTimedoutArgs e)
        {
            Console.WriteLine($"Ban: {e.UserTimeout.Username} on {e.UserTimeout.TimeoutDuration}");
        }

        private void _client_OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
        {
            _client.SendMessage(e.Channel, "Я пробрался к вам!");
        }

        private void _client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            var message = new MessageInfo
            {
                Id = e.ChatMessage.Id,
                UserId = e.ChatMessage.UserId,
                TimeCreated = DateTime.Now,
                UserName = e.ChatMessage.DisplayName,
                Message = e.ChatMessage.Message,
                Platform = "Twitch"
            };
            OnAddMessage?.Invoke(this, new OnAddMessageArgs { MessageInfo = message });
        }

        private void _client_OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void _client_OnLog(object sender, TwitchLib.Client.Events.OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime}: {e.BotUsername} - {e.Data}");
        }
    }
}
