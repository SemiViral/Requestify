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

        public static List<Video> CreatePlaylist(List<YoutubeResponse.Item> playlistItems)
        {
            List<Video> playlist = new List<Video>();

            foreach (var item in playlistItems)
            {
                playlist.Add(new Video(item.snippet.title, item.snippet.resourceId.videoId));
            }

            return playlist;
        }

        public static string BuildApiUri(string pageToken, string apiBase, string apiPart, string apiMaxResults, string apiNextPageToken, string apiPlaylistId, string apiFields, string apiKey)
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

        public static int GetTotalPages(int totalResults, int resultsPerPage)
        {
            int pages = Convert.ToInt32(Math.Floor(Convert.ToDouble(totalResults) / Convert.ToDouble(resultsPerPage)));

            if (totalResults % resultsPerPage != 0)
            {
                pages += 1;
            }

            return pages;
        }
    }
}
