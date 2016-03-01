using Newtonsoft.Json;
using RestSharp;
using Requestify.Properties;
using Requestify.Resources.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Requestify
{
    class Youtube
    {
        public static List<YoutubeResponse.Item> GetPlaylistItems(string playlistID)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri($"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&maxResults=50&playlistId={playlistID}&fields=items%2CnextPageToken%2CpageInfo%2CprevPageToken&key={Settings.Default.apiKey}");
            var request = new RestRequest(Method.GET);

            IRestResponse response = client.Execute(request);
            var results = JsonConvert.DeserializeObject<YoutubeResponse.RootObject>(response.Content);

            return results.items;
        }

        public static Dictionary<string, string> CreatePlaylist(List<YoutubeResponse.Item> playlistItems)
        {
            Dictionary<string, string> playlist = new Dictionary<string, string>();

            foreach (var item in playlistItems)
            {
                playlist.Add(item.snippet.title, item.snippet.resourceId.videoId);
            }

            return playlist;
        }
    }
}
