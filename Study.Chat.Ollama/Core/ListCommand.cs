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
    public class ListCommand : ICommand
    {
        private readonly ModelManager _modelManager;

        public ListCommand(ModelManager modelManager)
        {
            _modelManager = modelManager;
        }

        public bool CanExecute(string input) => input.Equals("/list", StringComparison.OrdinalIgnoreCase);

        public async Task ExecuteAsync(string input, OllamaApiClient ollama, List<Message> chatHistory)
        {
            var models = await _modelManager.GetAvailableModels();
            int selectedIndex = models.IndexOf(_modelManager.CurrentModel);
            selectedIndex = selectedIndex == -1 ? 0 : selectedIndex;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("可用模型列表 (使用↑↓键选择，Enter确认，Esc取消):\n");

                for (int i = 0; i < models.Count; i++)
                {
                    var prefix = i == selectedIndex ? "→ " : "  ";
                    Console.WriteLine($"{prefix}{i + 1}. {models[i]}");
                }

                var key = Console.ReadKey(true).Key;
                if (models.Count == 0 && selectedIndex == 0)
                {
                    return;
                }
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + models.Count) % models.Count;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % models.Count;
                        break;
                    case ConsoleKey.Enter:
                        _modelManager.SwitchModel(models[selectedIndex]);
                        Console.WriteLine($"\n已选择模型: {models[selectedIndex]}");
                        return;
                    case ConsoleKey.Escape:
                        Console.WriteLine("\n取消选择");
                        return;
                }
            }
        }
    }

}
