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

        public ClearCommand(ChatHistory chatHistroy) 
        {
            _chatHistroy = chatHistroy;
        }

        public bool CanExecute(string input)
        {
             return input.Equals("/clear", StringComparison.OrdinalIgnoreCase);
        }

        public Task ExecuteAsync(string input)        
        {
            ClearChatHistroy(_chatHistroy);
            //ClearChatHistroy(_chatHistroy, _sysChatCount);
            Console.Clear();
            return Task.CompletedTask;
        }

        public static void ClearChatHistroy(ChatHistory chatHistroy)
        {
            if (chatHistroy == null)
            {
                return;
            }

            var systemMessages = chatHistroy.Where(m => m.Role == AuthorRole.System).ToList();
            chatHistroy.Clear();
            // Keep system message
            chatHistroy.AddRange(systemMessages);
        }

        [Obsolete]
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
