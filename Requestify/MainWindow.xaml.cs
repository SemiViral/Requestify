using Newtonsoft.Json;
using Requestify.Properties;
using Requestify.Resources.API;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        List<YoutubeResponse.Item> playlistItems = new List<YoutubeResponse.Item>();
        List<Video> listVideos = new List<Video>();

        private void btnJoinChannel_Click(object sender, RoutedEventArgs e)
        {
            chatWindow.Source = new Uri($"http://www.twitch.tv/{txtbox_channel.Text}/chat");
        }

        private void btnGetVideos_Click(object sender, RoutedEventArgs e)
        {
            playlistItems = Youtube.GetPlaylistItems(txtbox_playlistID.Text);
            listVideos = Youtube.CreatePlaylist(playlistItems);

            playlistQueue.ItemsSource = listVideos;
        }

        private void playlistQueue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedSong.Text = listVideos[playlistQueue.SelectedIndex].title;
        }
    }
}