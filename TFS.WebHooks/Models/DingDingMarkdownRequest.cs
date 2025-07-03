// Copyright (c) 2023 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

namespace TFS.Webhooks.Models
{
    public partial class DingDingMarkdownRequest
    {
        public string msgtype { get; set; } = "markdown";
        public Markdown markdown { get; set; }
        public NotifyToAccount at { get; set; }
    }

    public partial class NotifyToAccount
    {
        public List<string> atMobiles { get; set; }
        public List<string> atUserIds { get; set; }
        public bool isAtAll { get; set; }

        public NotifyToAccount()
        { 
            this.atMobiles = new List<string>();
            this.atUserIds = new List<string>();
        }
    }

    public partial class Markdown
    {
        public string title { get; set; }
        public string text { get; set; }
    }
}
