using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study.Chat.Ollama.Plugins
{
    public class TimePlugin
    {
        [KernelFunction, Description("获取当前时间")]
        public DateTimeOffset Time() => DateTimeOffset.Now;
    }
}
