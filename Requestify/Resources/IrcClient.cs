using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Windows;

namespace Requestify
{
    class IrcClient
    {
        private string userName;
        private string channel;
        public static List<Video> listVideos = new List<Video>();

        private TcpClient tcpClient;
        private StreamReader inputStream;
        private StreamWriter outputStream;

        public IrcClient (string ip, int port, string userName, string password, List<Video> videoList)
        {
            this.userName = userName;
            listVideos = videoList;

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
            this.channel = channel;
            outputStream.WriteLine($"JOIN #{channel}");
            outputStream.Flush();
        }

        public void sendIrcMessage(string message)
        {
            outputStream.WriteLine(message);
            outputStream.Flush();
        }

        public void sendChatMessage(string message)
        {
            sendIrcMessage($":{userName}!{userName}@{userName}.tmi.twitch.tv PRIMSG #{channel} :{message}");
        }

        public string readMessage()
        {
            string message = inputStream.ReadLine();
            return message;
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
                );
            string message = unformattedMessage.Substring(
                (
                    unformattedMessage.IndexOf(":", 1) + 1),
                    unformattedMessage.Length - (unformattedMessage.IndexOf(":", 1) + 1)
                 );
            string formattedMessage = $"{user}: {message}";

            return formattedMessage;
        }

        public static void RequestSong(object sender, RoutedEventArgs e)
        {
            //irc.sendChatMessage($"!song add {listVideos[playlistQueue.SelectedIndex].id}");
            Console.Write($"Requesting {listVideos[((MainWindow)Application.Current.MainWindow).playlistQueue.SelectedIndex].title}...");
        }
    }
}
