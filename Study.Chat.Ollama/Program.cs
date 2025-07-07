// Copyright (c) 2025 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using OllamaSharp;
using OllamaSharp.Models;
using OllamaSharp.Models.Chat;
using Study.Chat.Ollama.Core;

namespace Study.Chat.Ollama
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var ollamaUrl = args.Length > 0 && !string.IsNullOrEmpty(args[0])
                ? args[0]
                : "http://localhost:11434";
            var ollama = new OllamaApiClient(ollamaUrl);

            if (Environment.UserInteractive)
            {
                Console.Title = "Chat Ollama - " + ollama.Config.Uri.Authority;
            }
            
            var modelManager = new ModelManager(ollama);
            modelManager.SwitchModel((await modelManager.GetAvailableModels())?.FirstOrDefault());
            var chatHistory = new List<Message>();
            // console command list
            var commands = new List<ICommand>
            {
                new HelpCommand(),
                new ListCommand(modelManager),
                new ModelSwitchCommand(modelManager),
                new ExitCommand()
            };

            Console.WriteLine($"当前模型: {modelManager.CurrentModel} (输入/help查看命令)");

            while (true)
            {
                Console.Write("\n用户 > ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                var command = commands.FirstOrDefault(c => c.CanExecute(input));
                if (command != null)
                {
                    await command.ExecuteAsync(input, ollama, chatHistory);
                    continue;
                }

                // 普通聊天处理
                chatHistory.Add(new Message { Role = "user", Content = input });
                try
                {
                    var response = await ollama.ChatAsync(new ChatRequest
                    {
                        Model = modelManager.CurrentModel,
                        Messages = chatHistory,
                        Stream = false
                    }).StreamToEndAsync();

                    var assistantMessage = response.Message.Content;
                    Console.WriteLine($"\n助手 > {assistantMessage}");
                    chatHistory.Add(new Message { Role = "assistant", Content = assistantMessage });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n助手 > 请求异常: {ex}");
                }
            }
        }
    }
}
