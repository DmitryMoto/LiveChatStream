using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.Moderation.BanUser;
using TwitchLib.Api.Helix.Models.Moderation.GetBannedUsers;
using TwitchLib.Api.Helix.Models.Moderation.GetModerators;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace LiveChatStream.API.Twitch
{
    internal class Moderation
    {
        private string _broadcaster_id { get; set; }
        public string broadcaster_channel { get; private set; }
        private ModerationHelps _moderHelp { get; set; }

        public Moderation()
        {
            var user = Task.Run(() => GetUsersAsync()).Result;
            _broadcaster_id = user.Users[0].Id;
            broadcaster_channel = user.Users[0].Login;
            _moderHelp = new ModerationHelps();
        }
        #region Add, Get, Remove Blocked terms on channel
        public async Task<bool> AddBlockedTermAsync(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                try
                {
                    await Authorization.API.Helix.Moderation.AddBlockedTermAsync(_broadcaster_id, _broadcaster_id, term, Properties.Settings.Default.TwitchAccessToken);
                    return true;
                }
                catch (Exception)
                {
                    throw new Exception($"Неудачная попытка добавить фразу: {term}. Возможно не достаточно прав, повторите попытку позже");
                }
            }
            return false;
        }
        public async Task<Dictionary<string, string>> GetBlockedTermsAsync(string pag = null)
        {
            var terms = await Authorization.API.Helix.Moderation.GetBlockedTermsAsync(_broadcaster_id, _broadcaster_id, pag, 50, Properties.Settings.Default.TwitchAccessToken);
            return _moderHelp.GetDictionaryTerms(terms);
        }

        public async Task<bool> RemoveBlockedTermAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    await Authorization.API.Helix.Moderation.DeleteBlockedTermAsync(_broadcaster_id, _broadcaster_id, id, Properties.Settings.Default.TwitchAccessToken);
                    return true;
                }
                catch (Exception)
                {
                    throw new Exception($"Неудачная попытка удалить фразу: {id}. Возможно не достаточно прав, повторите попытку позже");
                }
            }
            return false;
        }
        #endregion
        #region Add, Get, Remove Moderators on channel
        public async Task AddChannelModeratorAsync(string userID)
        {
            try
            {
                await Authorization.API.Helix.Moderation.AddChannelModeratorAsync(_broadcaster_id, userID, Properties.Settings.Default.TwitchAccessToken);
            }
            catch (Exception)
            {
                throw new Exception($"BadAddModerator");
            }
        }

        public async Task<GetModeratorsResponse> GetModeratorsAsync(string userID = null, string after = null)
        {
            try
            {
                var checkBotModer = await Authorization.API.Helix.Moderation.GetModeratorsAsync(_broadcaster_id, new List<string> { userID }, 20, after, Properties.Settings.Default.TwitchAccessToken);
                return checkBotModer;
            }
            catch (Exception)
            {
                if (!string.IsNullOrEmpty(userID))
                    throw new Exception($"BadGetModerator");
                else
                    throw new Exception($"BadGetModerators of channel");
            }
        }

        public async Task DeleteChannelModeratorAsync(string userID)
        {
            try
            {
                await Authorization.API.Helix.Moderation.DeleteChannelModeratorAsync(_broadcaster_id, userID, Properties.Settings.Default.TwitchAccessToken);
            }
            catch (Exception)
            {
                throw new Exception($"BadRemoveModerator");
            }
        }
        #endregion
        #region Add, Get, Remove Ban Users
        public async Task BanUserAsync(string userID, string reason = null)
        {
            BanUserRequest banUserRequest = new BanUserRequest();
            banUserRequest.UserId = userID;
            banUserRequest.Duration = TimeSpan.FromSeconds(30).Seconds;
            banUserRequest.Reason = reason;
            try
            {
                await Authorization.API.Helix.Moderation.BanUserAsync(_broadcaster_id, _broadcaster_id, banUserRequest, Properties.Settings.Default.TwitchAccessToken);
            }
            catch(Exception)
            {
                throw new Exception("BadBanUser");
            }
        }
        public async Task<GetBannedUsersResponse> GetBannedUsersAsync(List<string> usersID, string after = null, string before = null)
        {
            GetBannedUsersResponse bannedUsers = null;
            try
            {
                bannedUsers = await Authorization.API.Helix.Moderation.GetBannedUsersAsync(_broadcaster_id, usersID, 50, after, before, Properties.Settings.Default.TwitchAccessToken);
                return bannedUsers;
            }
            catch(Exception)
            {
                throw new Exception("BadGetBannedUsers");
            }
        }
        public async Task RemoveBannedUserAsyncs(string userID)
        {
            try
            {
                await Authorization.API.Helix.Moderation.UnbanUserAsync(_broadcaster_id, _broadcaster_id, userID, Properties.Settings.Default.TwitchAccessToken);
            }
            catch(Exception)
            {
                throw new Exception("BadRemoveBannedUsers");
            }
        }
        #endregion
        public async Task<GetUsersResponse> GetUsersAsync(List<string> usersID = null, List<string> logins = null)
        {
            try
            {
                GetUsersResponse resp = await Authorization.API.Helix.Users.GetUsersAsync(usersID, logins, Properties.Settings.Default.TwitchAccessToken);
                return resp;
            }
            catch (Exception)
            {
                throw new Exception($"BadGetUser");
            }
        }
        public async Task<GetStreamsResponse> GetStreamsAsync()
        {
            try
            {
                var getStream = await Authorization.API.Helix.Streams.GetStreamsAsync(null, 5, null, null, new List<string> { _broadcaster_id }, null, Properties.Settings.Default.TwitchAccessToken);
                //getStream.Streams[0].ViewerCount;
                return getStream;
            }
            catch(Exception)
            {
                throw new Exception($"BadGetStream");
            }
        }
      
    }
}
