using Newtonsoft.Json;
using Requestify.Properties;
using Requestify.Resources.API;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Requestify
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            passbox_authToken.Password = Properties.Settings.Default.oauthToken;
        }

        List<YoutubeResponse.Item> playlistItems = new List<YoutubeResponse.Item>();
        static List<Video> listVideos = new List<Video>();
        public static string username;
        public static string pass;
        public static string channel;


        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnJoinChannel_Click(object sender, RoutedEventArgs e)
        {
            username = txtbox_username.Text;
            pass = passbox_authToken.Password;
            channel = txtbox_channel.Text;

            Thread ircThread = new Thread( () =>
              {
                  CheckForMessages(username, pass, channel);
              }
            );
            ircThread.IsBackground = true;
            ircThread.Name = "IRC";
            ircThread.Start();
        }

        private void btnGetVideos_Click(object sender, RoutedEventArgs e)
        {
            playlistItems = Youtube.GetPlaylistItems(txtbox_playlistID.Text);
            listVideos = Youtube.CreatePlaylist(playlistItems);

            playlistQueue.ItemsSource = listVideos;
            lbl_selectedSong.Content = $"Selected Song 0/{listVideos.Count}";
        }

        private void playlistQueue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedSong.Text = $"{listVideos[playlistQueue.SelectedIndex].title} - {listVideos[playlistQueue.SelectedIndex].id}";
            lbl_selectedSong.Content = $"Selected Song {playlistQueue.SelectedIndex +1}/{listVideos.Count}";
        }

        
        private void btnRequestSong_Click(object sender, RoutedEventArgs e)
        {
            IrcClient.RequestSong(sender, e);
            //ircThread.sendChatMessage($"!song add {listVideos[playlistQueue.SelectedIndex].id}");
            //Console.Write($"Requesting {listVideos[playlistQueue.SelectedIndex].title}...");
        }
        

        public static void CheckForMessages(string username, string pass, string channel)
        {
            IrcClient irc = new IrcClient("irc.twitch.tv", 6667, username, pass, listVideos);
            irc.joinRoom(channel);

            while (true)
            {
                string message = irc.readMessage();

                if (message.StartsWith(":"))
                {
                    try
                    {
                        Console.WriteLine(IrcClient.formatMessage(message));
                    }
                    catch
                    {

                    }
                }
                else
                {
                    Console.WriteLine(message);
                    if (message.Contains("PING :tmi.twitch.tv"))
                    {
                        irc.sendIrcMessage("PONG :tmi.twitch.tv");
                        Console.WriteLine("PONGing...");
                    }
                }
            }
        }
    }
}