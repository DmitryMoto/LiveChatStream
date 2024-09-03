using System;

namespace LiveChatStream.API
{
    internal class MessageInfo
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public DateTime TimeCreated { get; set; }
        public string Message { get; set; }
        public string Platform { get; set; }

    }
}
