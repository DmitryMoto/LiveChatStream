using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChatStream.API.Twitch.Models
{
    public class Authorization
    {
        public string Code { get; }

        public Authorization(string code)
        {
            Code = code;
        }
    }
}
