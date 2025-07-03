// Copyright (c) 2023 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

using System.Text;
using System.Text.Json;
using TFS.Webhooks.Models;

namespace TFS.Webhooks.Core
{
    public class SendDingDing
    {
        static TimeSpan _timeOut = TimeSpan.FromSeconds(30);

        public static void Send(TfsChangedRequest body, string token, Dictionary<string, string> account)
        {
            if (body == null || string.IsNullOrEmpty(token))
            {
                return;
            }

            var state = RetrieveFieldValue("System.State", body.Resource.Revision.Fields);
            if (state == "已关闭" || state == "Closed")
            {
                return;
            }
            var workItemType = RetrieveFieldValue("System.WorkItemType", body.Resource.Revision.Fields)?.ToString();
            if (string.IsNullOrEmpty(workItemType))
            {
                workItemType = "工作项";
            }
            var changedBy = RetrieveFieldValue("System.ChangedBy", body.Resource.Revision.Fields)?.ToString();
            var assignedToChanged = GetChangedField("System.AssignedTo", body.Resource.Fields);
            var assignedTo = assignedToChanged?.NewValue?.ToString();

            if (assignedTo == null || string.IsNullOrEmpty(assignedTo) 
                || string.Equals(changedBy, assignedTo, StringComparison.OrdinalIgnoreCase)
                || (assignedToChanged == null || assignedToChanged.NewValue == assignedToChanged.OldValue))
            {
                return;
            }

            Console.WriteLine(body.Message.Markdown);
            var req = new DingDingMarkdownRequest();
            req.markdown = new Markdown { title = workItemType + " #" + body.Resource?.WorkItemId, text = body.Message.Markdown };
            req.at = new NotifyToAccount();

            if (account != null)
            {
                foreach (var pair in account)
                {
                    if (assignedTo.Contains(pair.Key) && !string.IsNullOrEmpty(pair.Value))
                    {
                        req.at.atMobiles.Add(pair.Value);
                        req.at.atUserIds.Add(pair.Key);
                        if (req.markdown.text.Contains(pair.Key))
                        {
                            req.markdown.text = req.markdown.text.Replace(pair.Key, $"@{pair.Value}");
                        }
                        else
                        {
                            req.markdown.text = req.markdown.text + $" \n@{pair.Value}";
                        }
                    }
                }
            }

            var uri = new Uri($"https://oapi.dingtalk.com/robot/send?access_token={token}");
            var client = new HttpClient();
            client.Timeout = _timeOut;
            var json = JsonSerializer.Serialize(req);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync(uri, content).Result;

            var result = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(result);
        }

        public static void Send(TfsCreatedRequest body, string token, Dictionary<string, string> account)
        {
            if (body == null || string.IsNullOrEmpty(token))
            {
                return;
            }

            var state = RetrieveFieldValue("System.State", body.Resource.Fields);
            if (state == "已关闭" || state == "Closed")
            {
                return;
            }

            var workItemType = RetrieveFieldValue("System.WorkItemType", body.Resource.Fields)?.ToString();
            if (string.IsNullOrEmpty(workItemType))
            {
                workItemType = "工作项";
            }
            var assignedTo = RetrieveFieldValue("System.AssignedTo", body.Resource.Fields)?.ToString();
            var createdBy = RetrieveFieldValue("System.CreatedBy", body.Resource.Fields)?.ToString();

            if (string.IsNullOrEmpty(assignedTo) || string.Equals(assignedTo, createdBy, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Console.WriteLine(body.Message.Markdown);
            var req = new DingDingMarkdownRequest();
            req.markdown = new Markdown { title = workItemType + " #" + body.Resource?.Id, text = body.Message.Markdown };
            req.at = new NotifyToAccount();

            if (account != null)
            {
                foreach (var pair in account)
                {
                    if (assignedTo.Contains(pair.Key) && !string.IsNullOrEmpty(pair.Value))
                    {
                        req.at.atMobiles.Add(pair.Value);
                        req.at.atUserIds.Add(pair.Key);

                        if (req.markdown.text.Contains(pair.Key))
                        {
                            req.markdown.text = req.markdown.text.Replace(pair.Key, $"@{pair.Value}");
                        }
                        else
                        {
                            req.markdown.text = req.markdown.text + $" \n@{pair.Value}";
                        }
                    }
                }
            }

            var uri = new Uri($"https://oapi.dingtalk.com/robot/send?access_token={token}");
            var client = new HttpClient();
            client.Timeout = _timeOut;
            var json = JsonSerializer.Serialize(req);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync(uri, content).Result;

            var result = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(result);
        }

        private static ChangedField GetChangedField(String key, Dictionary<string, ChangedField> fields)
        {
            if (fields.TryGetValue(key, out ChangedField field))
            {
                return field;
            }

            return null;
        }

        public static String RetrieveFieldValue(String key, Dictionary<string, object> fields)
        {
            if (String.IsNullOrEmpty(key))
                return String.Empty;

            var result = fields.FirstOrDefault(s => s.Key == key);

            if (result.Value == null)
            {
                return null;
            }

            return result.Value.ToString();
        }
    }
}
