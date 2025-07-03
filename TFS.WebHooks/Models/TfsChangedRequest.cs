// Copyright (c) 2023 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

namespace TFS.Webhooks.Models
{
    public partial class TfsChangedRequest
    {
        public string SubscriptionId { get; set; }
        public long NotificationId { get; set; }
        public string Id { get; set; }
        public string EventType { get; set; }
        public string PublisherId { get; set; }
        public Message Message { get; set; }
        public Message DetailedMessage { get; set; }
        public ChangedResource Resource { get; set; }
        public string ResourceVersion { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    public partial class Message
    {
        public string Text { get; set; }
        public string Html { get; set; }
        public string Markdown { get; set; }
    }

    public partial class ChangedResource
    {
        public long Id { get; set; }
        public long WorkItemId { get; set; }
        public long Rev { get; set; }
        public Account RevisedBy { get; set; }
        public DateTime RevisedDate { get; set; }
        public Dictionary<string, ChangedField>  Fields { get; set; }
        //public Links Links { get; set; }
        public string Url { get; set; }
        public Revision Revision { get; set; }
    }

    public partial class Account
    {
            public string Id { get; set; }
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string Url { get; set; }
            public string UniqueName { get; set; }
            public string ImageUrl { get; set; }
    }
        
    public partial class ChangedField
    {
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }

    public partial class SystemAssignedTo
    {
        public string NewValue { get; set; }
    }

    public partial class Links
    {
        public HrefUrl Self { get; set; }
        public HrefUrl Parent { get; set; }
        public HrefUrl WorkItemUpdates { get; set; }
    }

    public partial class HrefUrl
    {
        public string Href { get; set; }
    }

    public partial class Revision
    {
        public long Id { get; set; }
        public long Rev { get; set; }
        public Dictionary<string, object> Fields { get; set; }
        public string Url { get; set; }
    }

}