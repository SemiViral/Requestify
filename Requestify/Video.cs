using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Requestify
{
    class Video
    {
        public Video (string title, string id)
        {
            this.title = title;
            this.id = id;
        }

        string title;
        string id;
    }
}
