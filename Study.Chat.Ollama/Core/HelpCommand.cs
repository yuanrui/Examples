// Copyright (c) 2025 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

using OllamaSharp;
using OllamaSharp.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study.Chat.Ollama.Core
{
    public class HelpCommand : ICommand
    {
        public bool CanExecute(string input) => input.Equals("/help", StringComparison.OrdinalIgnoreCase);

        public Task ExecuteAsync(string input, OllamaApiClient ollama, List<Message> chatHistory)
        {
            Console.WriteLine("可用命令:\n" +
                            "/list - 列出可用模型\n" +
                            "/model <名称> - 切换模型\n" +
                            "/exit - 退出程序\n" +
                            "/help - 显示帮助");
            return Task.CompletedTask;
        }
    }
}
