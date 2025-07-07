// Copyright (c) 2025 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

using OllamaSharp;
using OllamaSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study.Chat.Ollama.Core
{
    public class ModelManager
    {
        private readonly OllamaApiClient _ollama;
        private string _currentModel;

        public ModelManager(OllamaApiClient ollama)
        {
            _ollama = ollama;
        }


        public ModelManager(OllamaApiClient ollama, string defaultModel)
        {
            _ollama = ollama;
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

        public void SwitchModel(string modelName)
        {
            _currentModel = modelName;
        }
    }
}
