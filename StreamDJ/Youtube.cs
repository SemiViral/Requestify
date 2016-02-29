using Newtonsoft.Json;
using RestSharp;
using Requestify.Properties;
using Requestify.Resources.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Requestify
{
    class Youtube
    {
        public static List<YoutubeResponse.Snippet> GetPlaylistItems(string playlistID)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri($"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&playlistId={playlistID}&key={Settings.Default.apiKey}");
            var request = new RestRequest(Method.GET);

            IRestResponse response = client.Execute(request);
            var results = JsonConvert.DeserializeObject<List<YoutubeResponse.Snippet>>(response.Content);
            return results;
        }

        public static Dictionary<string, string> CreatePlaylist(List<YoutubeResponse.Snippet> playlistItems)
        {
            Dictionary<string, string> playlist = new Dictionary<string, string>();

            foreach (var item in playlistItems)
            {
                playlist.Add(item.title, item.resourceId.videoId);
            }

            return playlist;
        }
    }
}
