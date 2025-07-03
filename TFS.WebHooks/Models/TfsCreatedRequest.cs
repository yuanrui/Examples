// Copyright (c) 2023 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

namespace TFS.Webhooks.Models
{
    public class TfsCreatedRequest
    {
        public string SubscriptionId { get; set; }
        public long NotificationId { get; set; }
        public string Id { get; set; }
        public string EventType { get; set; }
        public string PublisherId { get; set; }
        public Message Message { get; set; }
        public Message DetailedMessage { get; set; }
        public CreatedResource Resource { get; set; }
        public string ResourceVersion { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    public partial class CreatedResource
    {
        public long Id { get; set; }
        public long Rev { get; set; }
        public Dictionary<string, object> Fields { get; set; }
        //public Links Links { get; set; }
        public string Url { get; set; }
    }

}
