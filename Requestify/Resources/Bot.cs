using Requestify.Properties;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Requestify.Resources
{
	internal class Bot
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
                irc.sendIrcMessage("PONG :tmi.twitch.tv");
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
                    (unformattedMessage.IndexOf("!", StringComparison.Ordinal) - 1)
                ).ToUpper();
            string message = unformattedMessage.Substring(
                (
                    unformattedMessage.IndexOf(":", 1, StringComparison.Ordinal) + 1),
                    unformattedMessage.Length - (unformattedMessage.IndexOf(":", 1, StringComparison.Ordinal) + 1)
                 );
            string formattedMessage = $"{user}: {message}";

            return formattedMessage;
        }

        public static void AutoRequest(List<Video> vidList, List<Video> requestedList, string channel)
        {
	        Thread ircThread = new Thread(() => {
		        IrcClient irc = new IrcClient("irc.Twitch.tv", 6667, Settings.Default.username,
			        "oauth:" + Settings.Default.oauthToken);
		        irc.joinRoom(channel);

		        while (vidList.Count > 0) {
			        AddVideoToQueue(vidList, requestedList, irc);
		        }
	        }
		        ) {
			        IsBackground = true,
			        Name = "Request-Thread"
		        };
	        ircThread.Start();
        }

        public static void AddVideoToQueue(List<Video> vidList, List<Video> requestedList, IrcClient irc)
        {
	        if (requestedList.Count >= 5) return;

	        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
		        ((MainWindow)Application.Current.MainWindow).playlistQueue.ItemsSource = vidList;
		        ((MainWindow)Application.Current.MainWindow).playlistQueue.Items.Refresh();
		        ((MainWindow)Application.Current.MainWindow).requestBox.ItemsSource = requestedList;
		        ((MainWindow)Application.Current.MainWindow).requestBox.Items.Refresh();
	        }));

	        for (int i = requestedList.Count; i < 5; i++)
	        {
		        IrcClient.RequestSong(vidList[0].id);
		        string response;

		        do
		        {
			        response = irc.readMessage();


		        } while (!response.Contains($"{Settings.Default.username}, Bought {vidList[0].title}"));
                    
		        requestedList.Add(vidList[0]);
		        vidList.RemoveAt(0);
		        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
			        ((MainWindow)Application.Current.MainWindow).playlistQueue.Items.Refresh();
			        ((MainWindow)Application.Current.MainWindow).requestBox.Items.Refresh();
		        }));

		        Thread.Sleep(1000);
	        }

	        do
	        {
		        Thread.Sleep(30000);
		        irc.sendChatMessage("!song");

		        string response;
		        bool isMatch = false;

		        do
		        {
			        response = irc.readMessage();

		        } while (!response.Contains("Currently playing "));


		        for (int i = 0; i < requestedList.Count; i++)
		        {
			        if (!response.Contains(requestedList[i].title)) continue;

			        isMatch = true;
			        requestedList.Remove(requestedList[i]);
			        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
			        {
				        ((MainWindow)Application.Current.MainWindow).requestBox.Items.Refresh();
			        }));
			        break;
		        }
		        if (!isMatch)
		        {
			        Thread.Sleep(120000);
		        }

	        } while (requestedList.Count == 5);
        }

    }
}
