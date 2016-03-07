using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Windows;
using Requestify.Properties;
using System.Threading;
using Requestify.Resources;

namespace Requestify
{
    class IrcClient
    {
        private string username;
        private static int currencyCount;

        private static string _channel;
        public static List<Video> listVideos = new List<Video>();

        private TcpClient tcpClient;
        private StreamReader inputStream;
        private StreamWriter outputStream;

        public IrcClient (string ip, int port, string userName, string password)
        {
            username = userName;

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
            sendIrcMessage($":{username}!{username}@{username}.tmi.twitch.tv PRIVMSG #{_channel} :{message}");
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
                    (unformattedMessage.IndexOf("!") - 1)
                ).ToUpper() ;
            string message = unformattedMessage.Substring(
                (
                    unformattedMessage.IndexOf(":", 1) + 1),
                    unformattedMessage.Length - (unformattedMessage.IndexOf(":", 1) + 1)
                 );
            string formattedMessage = $"{user}: {message}";

            return formattedMessage;
        }

        public static void RequestSong(string songRequest)
        {
            IrcClient irc = new IrcClient("irc.twitch.tv", 6667, Settings.Default.username, "oauth:" + Settings.Default.oauthToken);

            irc.joinRoom(_channel);

            irc.sendChatMessage(songRequest);

        }

        public static void CreateIrcThread(string username, string password, string channel)
        {
            Thread ircThread = new Thread(() =>
            {
                CheckForMessages(username, password, channel);
            }
            );
            ircThread.IsBackground = true;
            ircThread.Name = "IRC";
            ircThread.Start();
        }

        public static void CreateIrcSendThread(string username, string password, string channel, string request)
        {
            Thread ircThread = new Thread(() =>
            {
                RequestSong(request);
                Thread.Sleep(2000);
            }
            );
            ircThread.IsBackground = true;
            ircThread.Name = "IRC-Send";
            ircThread.Start();
        }

        public static void CheckForMessages(string username, string pass, string channel)
        {
            IrcClient irc = new IrcClient("irc.twitch.tv", 6667, username, "oauth:" + pass);

            irc.joinRoom(channel);

            Settings.Default.currency = GetCurrencyName(irc);
            Thread.Sleep(2000);
            currencyCount = GetCurrencyCount(irc, Settings.Default.currency);

            while (true)
            {
                string message = irc.readMessage();

                Bot.CheckIncomingMessage(irc, message);
            }
        }

        private static string GetCurrencyName(IrcClient irc)
        {
            do
            {
                string response = irc.readMessage();
                if (response.Contains("PRIVMSG"))
                {
                    Console.WriteLine(formatMessage(response));
                }
                else
                {
                    Console.WriteLine(response);
                }

                
                if (response.Contains(@"End of /NAMES"))
                {
                    break;
                }
            }
            while (true);

            irc.sendChatMessage("!currency");

            do
            {
                string response = irc.readMessage();

                Console.WriteLine(response);

                if (response.Contains("This channel uses "))
                {
                    string msgOnly = response.Substring(response.IndexOf(":", 1) + 1);

                    string currency = msgOnly.Substring(msgOnly.IndexOf("uses") + 5, (msgOnly.IndexOf(".") - (msgOnly.IndexOf("uses") + 5)) - 1);

                    Console.WriteLine($"This channel uses {currency}");

                    Settings.Default.currency = currency;
                    Settings.Default.Save();

                    return currency;
                }

            } while (true);
        }

        private static int GetCurrencyCount(IrcClient irc, string currency)
        {
            irc.sendChatMessage($"!{currency}");

            do
            {
                string response = irc.readMessage();

                Console.WriteLine(response);

                if (response.Contains($"{Settings.Default.username} has "))
                {
                    string msgOnly = response.Substring(response.IndexOf(":", 1) + 1);

                    Console.WriteLine(msgOnly);
                    string count = msgOnly.Substring(msgOnly.IndexOf("has") + 4, (msgOnly.IndexOf(".") - (Settings.Default.currency.Length + 1 ) - (msgOnly.IndexOf("has") + 4)));

                    Console.WriteLine($"{Settings.Default.username}--{count} {currency.ToUpper()}");

                    Settings.Default.currency = currency;
                    Settings.Default.Save();

                    int curCount = Convert.ToInt32(count);

                    return curCount;
                }

            } while (true);
        }
    }
}
