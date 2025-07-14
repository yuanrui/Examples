// Copyright (c) 2025 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

#pragma warning disable SKEXP0070
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using OllamaSharp;
using OllamaSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Study.Chat.Ollama.Plugins;
using Microsoft.SemanticKernel.Plugins.Core;
using TimePlugin = Microsoft.SemanticKernel.Plugins.Core.TimePlugin;
using Study.Chat.Ollama.Core;

namespace Study.Chat.Ollama.Commands
{
    public class ModelManager
    {
        private readonly OllamaApiClient _ollama;
        private string _currentModel;
        ChatHistory _chatHistroy;

        public IChatCompletionService ChatCompletionService { get; private set; }
        public Kernel SemanticKernel { get; private set; }

        public ModelManager(OllamaApiClient ollama, ChatHistory chatHistroy)
        {
            _ollama = ollama;
            _chatHistroy = chatHistroy;
            _currentModel = GetAvailableModels().GetAwaiter().GetResult()?.FirstOrDefault();
        }

        public ModelManager(OllamaApiClient ollama, ChatHistory chatHistroy, string defaultModel)
        {
            _ollama = ollama;
            _chatHistroy = chatHistroy;
            _currentModel = defaultModel;
        }

        public string CurrentModel => _currentModel;

        public async Task<List<string>> GetAvailableModels()
        {
            try
            {
                if (_ollama == null)
                {
                    return new List<string>(0);
                }

                var models = await _ollama.ListLocalModelsAsync();
                return models.Select(m => m.Name).ToList();
            }
            catch
            {
                return new List<string>(0);
            }
        }

        protected void Init(string modelName)
        {
            var builder = Kernel.CreateBuilder();

            builder.Services.AddOllamaChatCompletion(modelName, _ollama.Config.Uri);
            if (!modelName.StartsWith("deepseek", StringComparison.OrdinalIgnoreCase))
            {
                // Microsoft.SemanticKernel.Plugins.Core
                builder.Plugins
                    .AddFromType<FileIOPlugin>()
                    .AddFromType<HttpPlugin>()
                    .AddFromType<TextPlugin>()
                    .AddFromType<TimePlugin>();

                // self plugins 
                builder.AutoRegisterPluginsFromDirectory();
            }

            SemanticKernel = builder.Build();

            ChatCompletionService = SemanticKernel.GetRequiredService<IChatCompletionService>();
        }

        public void SetModel(string modelName)
        {
            if (_currentModel == modelName)
            {
                return;
            }

            _currentModel = modelName;
            Init(modelName);
            ClearCommand.ClearChatHistroy(_chatHistroy);
        }
    }
}
