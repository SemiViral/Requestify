using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Requestify.Resources
{
    class Chat
    {
        private string _username;
        private string _pass;
        private string _channel;
        private ChatOut _messageOut;


        public Chat(string user, string pass, string channel, ChatOut messageOut)
        {
            _username = user;
            _pass = pass;
            _channel = channel;
            _messageOut = messageOut;
        }
    }

    class ChatOut
    {

    }
}
