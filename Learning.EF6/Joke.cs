using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Learning.EF6
{
    public class Joke
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string Content { get; set; }

        public string Url { get; set; }

        public bool Valid { get; set; }

        public int SupportCount { get; set; }

        public int OpposeCount { get; set; }

        public string CategoryId { get; set; }

        public string OriginalUrl { get; set; }

        public long GrabTime { get; set; }

        public long PublishTime { get; set; }
    }
}
