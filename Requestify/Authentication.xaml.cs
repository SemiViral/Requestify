using Requestify.Properties;
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
    public partial class Authentication : Window
    {
        public Authentication()
        {
            InitializeComponent();
        }

        // The URI that we are redirected to
        Uri RedirectUrl;

        // The base URI that we should be redirected to.
        Uri ExpectedRedirectUrl = new Uri("http://oauth.requestify.localhost");

        // Checks the new URL and if it's what we expect, it handles the data and closes the Authentication Window
        private void OnBrowserNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (IsExpectedUri(e.Uri))
            {
                e.Cancel = true;
                RedirectUrl = e.Uri;
                authBrowser.Navigate("about:blank");
                Close();

                Settings.Default.oauthToken = Twitch.GetValidatedToken(e.Uri.ToString());
            }
        }

        // Checks to see if the URL we were redirected to is the one we expected to be redirected to
        private bool IsExpectedUri(Uri uri)
        {
            var withoutParams = new UriBuilder(uri) { Fragment = "", Query = "" };
            return withoutParams.Uri == ExpectedRedirectUrl;
        }
    }
}
