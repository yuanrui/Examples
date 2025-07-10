// Copyright (c) 2025 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Study.Chat.Ollama.Plugins
{
    public class NetworkToolsPlugin
    {
        // 检查端口是否开放
        [KernelFunction, Description("检查指定主机和端口的连接状态")]
        public static async Task<string> CheckPortStatus(
            [Description("目标主机名或IP地址")] string host,
            [Description("要检查的端口号")] int port,
            [Description("超时时间(毫秒)")] int timeout = 2000)
        {
            try
            {
                using var client = new TcpClient();
                var task = client.ConnectAsync(host, port);

                if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
                {
                    client.Close();
                    return $"{host}:{port} 端口开放";
                }
                else
                {
                    client.Close();
                    return $"{host}:{port} 端口关闭";
                }
            }
            catch (Exception ex)
            {
                return $"错误: {ex.Message}";
            }
        }

        // 检查网络是否可达 (Ping)
        [KernelFunction, Description("使用Ping检测网络可达性")]
        public static async Task<string> CheckNetworkReachability(
            [Description("目标主机名或IP地址")] string host,
            [Description("Ping次数")] int count = 4,
            [Description("超时时间(毫秒)")] int timeout = 1000)
        {
            try
            {
                using var ping = new Ping();
                var replies = new List<PingReply>();

                for (int i = 0; i < count; i++)
                {
                    replies.Add(await ping.SendPingAsync(host, timeout));
                    await Task.Delay(10); // 间隔10ms
                }

                var successCount = replies.Count(r => r.Status == IPStatus.Success);
                if (successCount == 0)
                {
                    return $"无法访问目标主机:{host}";
                }

                var avgTime = replies.Where(r => r.Status == IPStatus.Success)
                                    .Average(r => r.RoundtripTime);
                
                return $"{host} 可达性: {successCount}/{count} 成功, 平均延迟: {avgTime:F1}ms";
            }
            catch (Exception ex)
            {
                return $"错误: {ex.Message}";
            }
        }

        // 获取本地IP地址
        [KernelFunction, Description("获取本机的IP地址信息")]
        public static string GetLocalIPAddresses(
            [Description("IP地址版本 (v4/v6)")] string version = "v4")
        {
            try
            {
                var ipVersion = version.ToLower() == "v6" ?
                    AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork;

                var addresses = Dns.GetHostAddresses(Dns.GetHostName())
                    .Where(ip => ip.AddressFamily == ipVersion)
                    .Select(ip => ip.ToString());

                return $"本地 {version} IP地址: {string.Join(", ", addresses)}";
            }
            catch (Exception ex)
            {
                return $"错误: {ex.Message}";
            }
        }

        // 解析DNS记录
        [KernelFunction, Description("解析域名的DNS记录")]
        public static async Task<string> ResolveDns(
            [Description("域名")] string domain,
            [Description("记录类型 (A/MX/AAAA)")] string recordType = "A")
        {
            try
            {
                var records = new List<string>();
                recordType = recordType.ToUpper();

                switch (recordType)
                {
                    case "A":
                        var aRecords = await Dns.GetHostAddressesAsync(domain);
                        records.AddRange(aRecords
                            .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                            .Select(ip => ip.ToString()));
                        break;

                    case "AAAA":
                        var aaaaRecords = await Dns.GetHostAddressesAsync(domain);
                        records.AddRange(aaaaRecords
                            .Where(ip => ip.AddressFamily == AddressFamily.InterNetworkV6)
                            .Select(ip => ip.ToString()));
                        break;
                    default:
                        return $"不支持的记录类型: {recordType}";
                }

                return $"{domain} 的 {recordType} 记录: {string.Join(", ", records)}";
            }
            catch (Exception ex)
            {
                return $"错误: {ex.Message}";
            }
        }

    }
}
