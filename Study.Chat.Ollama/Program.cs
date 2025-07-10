// Copyright (c) 2025 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

#pragma warning disable SKEXP0070
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using OllamaSharp;
using OllamaSharp.Models;
using OllamaSharp.Models.Chat;
using Study.Chat.Ollama.Core;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Study.Chat.Ollama.Plugins;
using Microsoft.SemanticKernel.Services;
using System.Net;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Embeddings;

namespace Study.Chat.Ollama
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var ollamaUrl = args.Length > 0 && !string.IsNullOrEmpty(args[0])
                ? args[0]
#if DEBUG
                : "http://192.168.1.145:11434";
#else
                : "http://localhost:11434";
#endif
            var ollama = new OllamaApiClient(ollamaUrl);

            if (Environment.UserInteractive)
            {
                Console.Title = "Chat Ollama - " + ollama.Config.Uri.Authority;
            }

            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage("请直接返回最终答案, 无需解释推理过程, 也不要包含<think>等过程标记");
            chatHistory.AddSystemMessage(@"当需要调用工具时, 请返回原始结果并必须遵守：
1. 永远不添加解释性文字
2. 永远不修改原始数据格式
3. 永远不包装返回内容
4. 错误信息直接透传");

            var modelManager = new ModelManager(ollama, chatHistory);
            modelManager.SetModel("qwen3:4b");
            ollama.SelectedModel = modelManager.CurrentModel;
            
            var settings = new OllamaPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };

            // console command list
            var commands = new List<ICommand>
            {
                new HelpCommand(),
                new ListCommand(modelManager),
                new ModelSwitchCommand(modelManager),
                new ClearCommand(chatHistory),
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
                    await command.ExecuteAsync(input);
                    continue;
                }

                chatHistory.AddUserMessage(input);

                try
                {
                    ChatMessageContent chatResult = await modelManager.ChatCompletionService.GetChatMessageContentAsync(chatHistory, settings, modelManager.SemanticKernel);
                    
                    Console.Write($"\n助手 > {chatResult.Content}");
                    if (chatResult != null && !string.IsNullOrEmpty(chatResult.Content))
                    {
                        chatHistory.AddAssistantMessage(chatResult.Content);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n助手 > 请求异常: {ex}");
                    // 记录到内部历史（不影响用户）
                    chatHistory.AddDeveloperMessage($"ERROR: {ex.ToString()}");
                }
            }
        }
    }
}
