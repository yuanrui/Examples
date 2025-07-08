using Microsoft.SemanticKernel.ChatCompletion;
using OllamaSharp;
using OllamaSharp.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study.Chat.Ollama.Core
{
    public class ClearCommand : ICommand
    {
        ChatHistory _chatHistroy;
        int _sysChatCount;

        public ClearCommand(ChatHistory chatHistroy) 
        {
            _chatHistroy = chatHistroy;
            _sysChatCount = chatHistroy.Count;             
        }

        public ClearCommand(ChatHistory chatHistroy, int initCount)
        {
            _chatHistroy = chatHistroy;
            _sysChatCount = initCount;
        }

        public bool CanExecute(string input)
        {
             return input.Equals("/clear", StringComparison.OrdinalIgnoreCase);
        }

        public Task ExecuteAsync(string input)
        {
            ClearChatHistroy(_chatHistroy, _sysChatCount);
            return Task.CompletedTask;
        }

        public static void ClearChatHistroy(ChatHistory chatHistroy, int keepCount)
        {
            if (chatHistroy == null || keepCount < 0 || chatHistroy.Count < keepCount)
            {
                return;
            }

            chatHistroy.RemoveRange(keepCount, chatHistroy.Count - keepCount);
        }
    }
}
