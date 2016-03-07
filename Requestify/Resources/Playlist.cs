using Newtonsoft.Json;
using Requestify.Properties;
using Requestify.Resources.API;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Requestify
{
    public class Playlist
    {
        private string _title;

        private string _id;

        public List<Video> videos;

        public Playlist(string id)
        {
            _id = id;

            _title = SetTitle(id);

            videos = SetVideos(id);
        }

        private string SetTitle(string id)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri($"https://www.googleapis.com/youtube/v3/playlists?part=snippet&id={id}&maxResults=1&key={Settings.Default.apiKey}");
            var request = new RestRequest(Method.GET);

            IRestResponse response = client.Execute(request);
            var results = JsonConvert.DeserializeObject<PlaylistResponse.RootObject>(response.Content);

            var itemList = results.items;

            return itemList[0].snippet.title;
        }

        private List<Video> SetVideos(string id)
        {
            var videoList = GetPlaylistItems(id);
            List<Video> songList = new List<Video>();

            foreach (YoutubeResponse.Item video in videoList)
            {
                songList.Add(new Video(video.snippet.title, video.snippet.resourceId.videoId));
            }

            return songList;
        }

        private List<YoutubeResponse.Item> GetPlaylistItems(string playlistID)
        {
            string pageToken = "";

            string apiBase = "https://www.googleapis.com/youtube/v3/playlistItems?";
            string apiPart = "part=snippet";
            string apiMaxResults = "&maxResults=50";
            string apiNextPageToken = $"&pageToken={pageToken}";
            string apiPlaylistId = $"&playlistId={playlistID}";
            string apiFields = "&fields=items%2CnextPageToken%2CpageInfo%2CprevPageToken";
            string apiKey = $"&key={Settings.Default.apiKey}";

            YoutubeResponse.RootObject results;
            List<YoutubeResponse.Item> itemResults = new List<YoutubeResponse.Item>();

            int totalPages;
            int pagesComplete = 0;

            do
            {
                var client = new RestClient();
                client.BaseUrl = new Uri(BuildApiUri(pageToken, apiBase, apiPart, apiMaxResults, apiNextPageToken, apiPlaylistId, apiFields, apiKey));
                var request = new RestRequest(Method.GET);

                IRestResponse response = client.Execute(request);
                results = JsonConvert.DeserializeObject<YoutubeResponse.RootObject>(response.Content);

                itemResults.AddRange(results.items);

                if (!string.IsNullOrWhiteSpace(results.nextPageToken))
                {
                    pageToken = results.nextPageToken;
                }

                totalPages = GetTotalPages(results.pageInfo.totalResults, results.pageInfo.resultsPerPage);

                pagesComplete += 1;
            }
            while (pagesComplete < totalPages);

            return itemResults;
        }

        private string BuildApiUri(string pageToken, string apiBase, string apiPart, string apiMaxResults, string apiNextPageToken, string apiPlaylistId, string apiFields, string apiKey)
        {
            string uriString;

            if (pageToken == "")
            {
                uriString = apiBase + apiPart + apiMaxResults + apiPlaylistId + apiFields + apiKey;
            }
            else
            {
                apiNextPageToken = $"&pageToken={pageToken}";
                uriString = apiBase + apiPart + apiMaxResults + apiNextPageToken + apiPlaylistId + apiFields + apiKey;
            }

            return uriString;
        }

        private int GetTotalPages(int totalResults, int resultsPerPage)
        {
            int pages = Convert.ToInt32(Math.Floor(Convert.ToDouble(totalResults) / Convert.ToDouble(resultsPerPage)));

            if (totalResults % resultsPerPage != 0)
            {
                pages += 1;
            }

            return pages;
        }

        public override string ToString()
        {
            return _title;
        }
    }
}
