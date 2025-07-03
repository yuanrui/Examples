using Dev.VSProject.Builder.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Dev.VSProject.Builder
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputDir = "";
            if (args != null && args.Length > 0)
            {
                inputDir = args[0];
            }
            else
            {
                //inputDir = "C:\\Users\\YuanRui\\Documents\\Visual Studio 2019\\My Exported Templates\\Banana.WebUI";
            }
            
            if (string.IsNullOrEmpty(inputDir))
            {
                Console.WriteLine("input dir is empty");
                return;
            }

            try
            {
                var list = new List<Folder>();

                BuildFolders(inputDir, list);
                var xml = CreateXml(list);

                File.WriteAllText("vs.xml", xml);
                Console.WriteLine("build success.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.ReadLine();
        }

        static void BuildFolders(string dir, List<Folder> list)
        {
            var ignoreDirs = new string[] { "obj", "bin", "node_modules", ".vs", ".vscode" };
            if (string.IsNullOrEmpty(dir) || list == null)
            {
                return;
            }

            var dirInfo = new DirectoryInfo(dir);
            if (!dirInfo.Exists)
            {
                return;
            }

            foreach (var item in ignoreDirs)
            {
                if (string.Equals(dirInfo.Name, item, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            var folder = new Folder();
            folder.Name = dirInfo.Name;
            list.Add(folder);

            var files = dirInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);
            foreach (var item in files)
            {
                folder.AddProjectItem(item.Name);
            }

            var subDirs = dirInfo.GetDirectories();
            foreach (var subItem in subDirs)
            {
                BuildFolders(subItem.FullName, folder.SubFolders);
            }
        }

        static string CreateXml(List<Folder> list)
        {
            var xml = string.Empty;

            foreach (var item in list)
            {
                var xmlDoc = CreateXml(item);
                xml += XmlToString(xmlDoc);
            }

            return xml;
        }

        static string XmlToString(XmlDocument doc)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (StringWriter stringWriter = new StringWriter(stringBuilder))
            {
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    xmlTextWriter.Formatting = Formatting.Indented;
                    xmlTextWriter.Indentation = 4;
                    xmlTextWriter.IndentChar = ' ';
                    doc.WriteContentTo(xmlTextWriter);
                }
            }

            return stringBuilder.ToString();

        }

        static XmlDocument CreateXml(Folder folder)
        {
            if (folder == null)
            {
                return null;
            }

            var doc = new XmlDocument();
            var folderElem = doc.CreateElement("Folder");
            folderElem.SetAttribute("Name", folder.Name);
            doc.AppendChild(folderElem);
            
            foreach (var item in folder.ProjectItems)
            {
                var proItemElem = doc.CreateElement("ProjectItem");
                proItemElem.InnerText = item.Name;
                if (item.ReplaceParameters)
                {
                    proItemElem.SetAttribute("ReplaceParameters", "true");
                }

                folderElem.AppendChild(proItemElem);
            }

            foreach (var sub in folder.SubFolders)
            {
                var subXml = CreateXml(sub);
                if (subXml == null || subXml.DocumentElement == null)
                {
                    continue;
                }

                folderElem.AppendChild(doc.ImportNode(subXml.DocumentElement, true));
            }

            return doc;
        }
    }
}
