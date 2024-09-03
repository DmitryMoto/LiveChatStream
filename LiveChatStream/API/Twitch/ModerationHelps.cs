using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.Moderation.BlockedTerms;

namespace LiveChatStream.API.Twitch
{
    internal class ModerationHelps
    {
        public Dictionary<string, string> GetDictionaryTerms(GetBlockedTermsResponse terms)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (terms.Data.Length > 0)
            {
                foreach (var data in terms.Data)
                {
                    result.Add(data.Id, data.Text);
                }
            }

            return result;
        }
    }
}
