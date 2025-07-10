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

namespace Study.Chat.Ollama.Commands
{
    public class ExitCommand : ICommand
    {
        public bool CanExecute(string input) => input.Equals("/exit", StringComparison.OrdinalIgnoreCase);

        public Task ExecuteAsync(string input)
        {
            Console.WriteLine("正在退出程序...");
            Environment.Exit(0);
            return Task.CompletedTask;
        }
    }

}
