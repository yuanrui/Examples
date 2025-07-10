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
    public interface ICommand
    {
        bool CanExecute(string input);
        Task ExecuteAsync(string input);
    }
}
