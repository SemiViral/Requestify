using Requestify.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Requestify.Resources
{
    class Bot
    {
        // Handling of the incoming messages
        public static void CheckIncomingMessage(IrcClient irc, string message)
        {
            if (message.Contains("PRIVMSG"))
            {
                Console.WriteLine(IrcClient.formatMessage(message));

                if (IrcClient.formatMessage(message).Contains($""))
                {

                }
            }
            else if (message.StartsWith("PING :tmi.twitch.tv"))
            {
                irc.sendChatMessage("PONG :tmi.twitch.tv");
            }
            
        }

        public static string formatMessage(string unformattedMessage)
        {
            /* 
            
            :steve!steve@steve.tmi.twitch.tv PRIMSG #venalis_21 :how are you
            0123456789012345678901234567890123456789012345678901234567890123
                      1         2         3         4         5         6       
            */

            string user = unformattedMessage.Substring
                (
                    1,
                    (unformattedMessage.IndexOf("!") - 1)
                ).ToUpper();
            string message = unformattedMessage.Substring(
                (
                    unformattedMessage.IndexOf(":", 1) + 1),
                    unformattedMessage.Length - (unformattedMessage.IndexOf(":", 1) + 1)
                 );
            string formattedMessage = $"{user}: {message}";

            return formattedMessage;
        }


    }
}
