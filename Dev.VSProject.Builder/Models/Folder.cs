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
    public class Folder
    {
        public string Name { get; set; }

        public List<Folder> SubFolders { get; set; }

        public List<ProjectItem> ProjectItems { get; set; }

        public Folder()
        {
            this.SubFolders = new List<Folder>();
            this.ProjectItems = new List<ProjectItem>();
        }

        public virtual void AddProjectItem(string name)
        {
            if (this.ProjectItems == null)
            {
                return;
            }

            this.ProjectItems.Add(new ProjectItem(name));
        }

        public virtual void AddSubFolder(string folderName)
        {
            if (this.SubFolders == null)
            {
                return;
            }

            this.SubFolders.Add(new Folder() { Name = folderName });
        }

        public virtual void AddSubFolder(Folder folder)
        {
            if (this.SubFolders == null)
            {
                return;
            }

            this.SubFolders.Add(folder);
        }
    }
}
