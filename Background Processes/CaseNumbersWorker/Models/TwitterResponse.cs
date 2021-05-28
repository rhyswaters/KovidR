using System;
using System.Collections.Generic;

namespace CaseNumbersWorker.Models
{
    public class Tweet
    {
        public string id { get; set; }
        public string text { get; set; }
    }

    public class Meta
    {
        public string oldest_id { get; set; }
        public string newest_id { get; set; }
        public int result_count { get; set; }
        public string next_token { get; set; }
    }

    public class TwitterResponse
    {
        public List<Tweet> data { get; set; }
        public Meta meta { get; set; }
    }
}
