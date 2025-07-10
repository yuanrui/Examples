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
    public class ModelSwitchCommand : ICommand
    {
        private readonly ModelManager _modelManager;

        public ModelSwitchCommand(ModelManager modelManager)
        {
            _modelManager = modelManager;
        }

        public bool CanExecute(string input) => input.StartsWith("/model ", StringComparison.OrdinalIgnoreCase);

        public Task ExecuteAsync(string input)
        {
            var modelName = input.Substring("/model ".Length).Trim();
            _modelManager.SetModel(modelName);
            Console.WriteLine($"已切换到模型: {modelName}");
            return Task.CompletedTask;
        }
    }
}
