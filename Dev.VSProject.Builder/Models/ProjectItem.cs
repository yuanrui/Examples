// Copyright (c) 2021 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.VSProject.Builder.Models
{
    public class ProjectItem
    {
        static string[] SafeExtensions = new string[] { ".cs", ".vb", ".cpp", ".java", ".js", ".json", ".config", ".xml", ".json", ".vue", ".ts", ".csproj", ".ini" };

        public string Name { get; set; }

        public bool ReplaceParameters { get; set; }

        public ProjectItem()
        {
        }

        public ProjectItem(string name)
        {
            this.Name = name;
            this.ReplaceParameters = CanReplaceParameters(this.Name);
        }

        public bool CanReplaceParameters(string name)
        {
            if (SafeExtensions == null || string.IsNullOrEmpty(name))
            {
                return false;
            }

            foreach (var item in SafeExtensions)
            {
                if (name.EndsWith(item, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
