using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Requestify
{
    public class Video
    {
        public string title;
        public string id;

        public Video (string title, string id)
        {
            this.title = title;
            this.id = id;
        }

        public override string ToString()
        {
            return title;
        }

    }
}
