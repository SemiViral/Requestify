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
using System.Windows.Media;

namespace Requestify
{
    class Twitch
    {
        // Access scope that the program needs to work with the Twitch API
        public static Dictionary<string, bool> Scopes = new Dictionary<string, bool>()
        {
            {"user_read", false },
            {"user_blocks_read", false },
            {"user_blocks_edit", false },
            {"user_follows_edit", false },
            {"channel_read", false },
            {"channel_editor", false },
            {"channel_commercial", false },
            {"channel_stream", false },
            {"channel_subscriptions", false },
            {"user_subscriptions", false },
            {"channel_check_subscription", false },
            {"chat_login", true }
        };

        /*------------------------------------------------------------------------------------*/
        /*------------------------------------VARIABLES---------------------------------------*/
        /*------------------------------------------------------------------------------------*/

        // Set up some variables
        public static Color redText = new Color() { R = 255, G = 105, B = 105, A = 255 };
        public static Color greenText = new Color() { R = 50, G = 150, B = 50, A = 255 };

        public static string baseApiUrl = "https://api.twitch.tv/kraken";           // Base API URL for Twitch
        public static string clientID = Settings.Default.clientID;                  // Stream Suite client ID
        public static string redirectUri = "http://oauth.requestify.localhost";    // Redirect URI after authorization

        public static string accessToken;
        public static string username;

        // Set the URL that the authorization window will go to
        public static string authUri = $"{baseApiUrl}/oauth2/authorize?response_type=token&client_id={clientID}&redirect_uri={redirectUri}&scope={ScopesToString(Scopes)}";

        /*------------------------------------------------------------------------------------*/
        /*-------------------------------------METHODS----------------------------------------*/
        /*------------------------------------------------------------------------------------*/

        // Returns a String of all scopes with the value of TRUE
        private static string ScopesToString(Dictionary<string, bool> dict)
        {
            string scopeString = "";
            foreach (var scope in dict)
            {
                if (scope.Value)
                {
                    scopeString += scope.Key + "+";
                }
            }
            scopeString = scopeString.TrimEnd('+');
            return scopeString;
        }

        // Returns a String of just the Twitch Access Token after a successful authorization
        public static string ParseAccessToken(string Uri)
        {
            int tokenStart = Uri.IndexOf("=");
            int tokenEnd = (Uri.IndexOf("&") - 1) - tokenStart;
            string token = Uri.Substring(tokenStart + 1, tokenEnd);

            return token;
        }

        public static bool ValidateToken(string token)
        {
            // Create the Request
            var client = new RestClient();
            client.BaseUrl = new Uri(baseApiUrl);
            var request = new RestRequest("?oauth_token=" + token, Method.GET);

            // Make the Request
            IRestResponse response = client.Execute(request);

            // Read what comes back
            TwitchResponse.RootObject results = JsonConvert.DeserializeObject<TwitchResponse.RootObject>(response.Content);

            ((MainWindow)Application.Current.MainWindow).SaveUserSettings(results.token.user_name, token);
            accessToken = Settings.Default.oauthToken;
            username = Settings.Default.username;

            return results.token.valid;
        }

        public static string GetValidatedToken(string uri)
        {
            accessToken = ParseAccessToken(uri);
            if (ValidateToken(accessToken))
            {
                ((MainWindow)Application.Current.MainWindow).authStatus.Fill = new SolidColorBrush(greenText);
                return accessToken;
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).authStatus.Fill = new SolidColorBrush(redText);
                return "";
            }
        }
    }
}