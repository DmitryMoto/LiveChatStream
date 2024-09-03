using LiveChatStream.API;
using System;


namespace LiveChatStream.Events
{
    internal class OnAddMessageArgs : EventArgs
    {
        public MessageInfo MessageInfo;
    }
}
