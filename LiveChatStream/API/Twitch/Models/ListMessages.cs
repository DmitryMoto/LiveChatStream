using System.Collections.Generic;

namespace LiveChatStream.API.Twitch.Models
{
    internal class ListMessages
    {
        public List<MessageInfo>List { get; set; }

        public ListMessages()
        {
            List = new List<MessageInfo>();
        }
    }
}
