using Newtonsoft.Json;
using Requestify.Properties;
using Requestify.Resources;
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
        }

        List<YoutubeResponse.Item> playlistItems = new List<YoutubeResponse.Item>();
        public static List<Video> listVideos = new List<Video>();
        public static List<Playlist> listPlaylists = new List<Playlist>();
        public static List<Video> requests = new List<Video>();
        public static string username;
        public static string pass;
        public static string channel;
        public static string currency;

        private void btnJoinChannel_Click(object sender, RoutedEventArgs e)
        {
            username = Settings.Default.username;
            pass = Settings.Default.oauthToken;
            channel = txtbox_channel.Text;

            IrcClient.CreateIrcThread(username, pass, channel);
        }
        
        private void GetVideos(object sender, SelectionChangedEventArgs e)
        {
            listVideos = listPlaylists[playlistSelector.SelectedIndex].videos;

            playlistQueue.ItemsSource = listVideos;
            playlistQueue.SelectedItem = 0;
            playlistQueue.SelectedIndex = 0;
            lbl_selectedSong.Content = $"Selected Song {playlistQueue.SelectedIndex + 1}/{listVideos.Count}";
        }

        private void playlistQueue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (playlistQueue.SelectedIndex == -1)
            {
                lbl_selectedSong.Content = $"Selected Song 0/{listVideos.Count}";
            }
            else
            {
                lbl_selectedSong.Content = $"Selected Song {playlistQueue.SelectedIndex + 1}/{listVideos.Count}";
            }
        }
        
        private void btnRequestSong_Click(object sender, RoutedEventArgs e)
        {
            if (autoRequests.IsChecked == true)
            {
                Bot.AutoRequest(listVideos, requests, channel);
            }
            else
            {
                IrcClient.CreateIrcSendThread(Settings.Default.username, Settings.Default.oauthToken, channel, listVideos[((MainWindow)Application.Current.MainWindow).playlistQueue.SelectedIndex].id);

                requests.Add(listVideos[playlistQueue.SelectedIndex]);
                requestBox.Items.Refresh();

                listVideos.RemoveAt(playlistQueue.SelectedIndex);
                playlistQueue.Items.Refresh();
            }

            
        }

        private void btnAddPlaylist_Click(object sender, RoutedEventArgs e)
        {
            Playlist playlist = new Playlist(txtbox_playlistID.Text);

            listPlaylists.Add(playlist);
            playlistSelector.ItemsSource = listPlaylists;

            playlistSelector.SelectedIndex = listPlaylists.Count - 1;
            txtbox_playlistID.Text = "";
        }

        private void btnGetAuthToken_Click(object sender, RoutedEventArgs e)
        {
            // Create a new Authentication Window w/ a WebBrowser control and send it to the proper Oauth URL
            Authentication authWindow = new Authentication();
            authWindow.authBrowser.Source = new Uri(Twitch.authUri);
            authWindow.Show();
        }

        public void SaveUserSettings(string username, string accessToken)
        {
            Settings.Default.oauthToken = accessToken;
            Settings.Default.username = username;
            Settings.Default.Save();
        }

        private void DefaultWindow_Loaded(object sender, RoutedEventArgs e)
        {
            requestBox.ItemsSource = requests;
            LoadIfTokenValid();
        }

        private void LoadIfTokenValid()
        {
            if (Twitch.ValidateToken(Settings.Default.oauthToken))
            {
                Twitch.accessToken = Settings.Default.oauthToken;
                Twitch.username = Settings.Default.username;
                ((MainWindow)Application.Current.MainWindow).authStatus.Fill = new SolidColorBrush(Twitch.greenText);
                ((MainWindow)Application.Current.MainWindow).btnGetAuthToken.IsEnabled = false;
                ((MainWindow)Application.Current.MainWindow).txtbox_username.IsEnabled = false;
                SaveUserSettings(Settings.Default.username, Settings.Default.oauthToken);
                txtbox_username.Text = Settings.Default.username;
                btnGetAuthToken.IsEnabled = false;
                txtbox_username.IsEnabled = false;
            }
        }
    }
}