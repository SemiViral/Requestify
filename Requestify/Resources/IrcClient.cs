using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using Requestify.Properties;
using System.Threading;
using Requestify.Resources;

namespace Requestify
{
	internal class IrcClient
    {
        private readonly string _username;
        private static int currencyCount;

        private static string _channel;
        public static List<Video> listVideos = new List<Video>();

        private TcpClient tcpClient;
        private readonly StreamReader inputStream;
        private readonly StreamWriter outputStream;

        public IrcClient (string ip, int port, string userName, string password)
        {
            _username = userName;

            tcpClient = new TcpClient(ip, port);
            inputStream = new StreamReader(tcpClient.GetStream());
            outputStream = new StreamWriter(tcpClient.GetStream());

            outputStream.WriteLine($"PASS {password}");
            outputStream.WriteLine($"NICK {userName}");
            outputStream.WriteLine($"USER {userName}");
            outputStream.Flush();
        }

        public void joinRoom(string channel)
        {
            _channel = channel;
            outputStream.WriteLine($"JOIN #{channel}");
            outputStream.Flush();
        }

        public void sendIrcMessage(string message)
        {
            IrcClient irc = new IrcClient("irc.twitch.tv", 6667, Settings.Default.username, "oauth:" + Settings.Default.oauthToken);

            outputStream.WriteLine(message);
            outputStream.Flush();
        }

        public void sendChatMessage(string message)
        {
            sendIrcMessage($":{_username}!{_username}@{_username}.tmi.twitch.tv PRIVMSG #{_channel} :{message}");
        }

        public string readMessage()
        {
            string message = inputStream.ReadLine();
            return message;
        }

        public static string formatMessage(string unformattedMessage)
        {
            /* 
            
            :steve!steve@steve.tmi.twitch.tv PRIVMSG #venalis_21 :how are you
            01234567890123456789012345678901234567890123456789012345678901234
                      1         2         3         4         5         6       
            */

            string user = unformattedMessage.Substring
                (
                    1,
                    (unformattedMessage.IndexOf("!", StringComparison.Ordinal) - 1)
                ).ToUpper() ;
            string message = unformattedMessage.Substring(
                (
                    unformattedMessage.IndexOf(":", 1, StringComparison.Ordinal) + 1),
                    unformattedMessage.Length - (unformattedMessage.IndexOf(":", 1, StringComparison.Ordinal) + 1)
                 );
            string formattedMessage = $"{user}: {message}";

            return formattedMessage;
        }

        public static void RequestSong(string songRequest)
        {
            IrcClient irc = new IrcClient("irc.twitch.tv", 6667, Settings.Default.username, "oauth:" + Settings.Default.oauthToken);

            irc.joinRoom(_channel);

            irc.sendChatMessage($"!song add {songRequest}");
        }

        public static void CreateIrcThread(string username, string password, string channel)
        {
	        Thread ircThread = new Thread(() => { CheckForMessages(username, password, channel); }
		        ) {
			        IsBackground = true,
			        Name = "IRC"
		        };
	        ircThread.Start();
        }

        public static void CreateIrcSendThread(string username, string password, string channel, string request)
        {
	        Thread ircThread = new Thread(() => {
		        RequestSong(request);
		        Thread.Sleep(2000);
	        }
		        ) {
			        IsBackground = true,
			        Name = "IRC-Send"
		        };
	        ircThread.Start();
        }

        public static void CheckForMessages(string username, string pass, string channel)
        {
            IrcClient irc = new IrcClient("irc.twitch.tv", 6667, username, "oauth:" + pass);

            irc.joinRoom(channel);
            do
            {
                string response = irc.readMessage();
	            Console.WriteLine(response.Contains("PRIVMSG") ? formatMessage(response) : response);

	            if (response.Contains(@"End of /NAMES"))
                {
                    break;
                }
            }
            while (true);
            /*
            Settings.Default.currency = GetCurrencyName(irc);
            Thread.Sleep(2000);
            currencyCount = GetCurrencyCount(irc, Settings.Default.currency);
            */
            while (true)
            {
                string message = irc.readMessage();

                Bot.CheckIncomingMessage(irc, message);
            }
        }

        private static string GetCurrencyName(IrcClient irc)
        {
            irc.sendChatMessage("!currency");

            do
            {
                string response = irc.readMessage();

                Console.WriteLine(response);

	            if (!response.Contains("This channel uses ")) continue;

	            string msgOnly = response.Substring(response.IndexOf(":", 1, StringComparison.Ordinal) + 1);

	            string currency = msgOnly.Substring(msgOnly.IndexOf("uses", StringComparison.Ordinal) + 5,
		            msgOnly.IndexOf(".", StringComparison.Ordinal) - (msgOnly.IndexOf("uses", StringComparison.Ordinal) + 5) - 1);

	            Console.WriteLine($"This channel uses {currency}");

	            Settings.Default.currency = currency;
	            Settings.Default.Save();

	            return currency;
            } while (true);
        }

        private static int GetCurrencyCount(IrcClient irc, string currency)
        {
            irc.sendChatMessage($"!{currency}");

            do
            {
                string response = irc.readMessage();

                Console.WriteLine(response);

	            if (!response.Contains($"{Settings.Default.username} has ")) continue;

	            string msgOnly = response.Substring(response.IndexOf(":", 1, StringComparison.Ordinal) + 1);

	            Console.WriteLine(msgOnly);
	            string count = msgOnly.Substring(msgOnly.IndexOf("has", StringComparison.Ordinal) + 4,
		            msgOnly.IndexOf(".", StringComparison.Ordinal) - (Settings.Default.currency.Length + 1) -
					 (msgOnly.IndexOf("has", StringComparison.Ordinal) + 4));

	            Console.WriteLine($"{Settings.Default.username}--{count} {currency.ToUpper()}");

	            Settings.Default.currency = currency;
	            Settings.Default.Save();

	            int curCount = Convert.ToInt32(count);

	            return curCount;
            } while (true);
        }
    }
}
