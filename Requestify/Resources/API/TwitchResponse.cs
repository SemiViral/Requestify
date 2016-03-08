using System.Collections.Generic;

namespace Requestify.Resources.API
{
	internal class TwitchResponse
    {
        public class Authorization
        {
            public List<string> scopes { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }

        public class Token
        {
            public Authorization authorization { get; set; }
            public string user_name { get; set; }
            public bool valid { get; set; }
        }

        public class Links
        {
            public string channel { get; set; }
            public string users { get; set; }
            public string user { get; set; }
            public string channels { get; set; }
            public string chat { get; set; }
            public string streams { get; set; }
            public string ingests { get; set; }
            public string teams { get; set; }
            public string search { get; set; }
        }

        public class RootObject
        {
            public Token token { get; set; }
            public Links _links { get; set; }
        }
    }
}
