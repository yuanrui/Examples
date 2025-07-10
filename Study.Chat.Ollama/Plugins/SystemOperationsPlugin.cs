// Copyright (c) 2025 YuanRui
// GitHub: https://github.com/yuanrui
// License: Apache-2.0

using Microsoft.SemanticKernel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study.Chat.Ollama.Plugins
{
    /// <summary>
    /// 系统操作插件
    /// </summary>
    public class SystemOperationsPlugin
    {
        [KernelFunction]
        [Description("打开指定的应用程序")]
        public void OpenProgramAsync(
            [Description("应用程序名称（如：notepad, chrome）或完整路径")] string programName,
            [Description("启动参数")] string arguments = "")
        {
            string? executablePath = FindApplicationPath(programName);

            if (executablePath == null)
            {
                throw new FileNotFoundException($"找不到应用程序: {programName}");
            }

            ProcessStartInfo startInfo = new(executablePath)
            {
                Arguments = arguments,
                UseShellExecute = true
            };

            Process.Start(startInfo);
        }

        /// <summary>
        /// 根据程序名称查找其安装路径。
        /// </summary>
        /// <param name="programName">要查找的程序名称（例如 "notepad.exe", "chrome.exe"）。</param>
        /// <returns>找到的程序路径，如果未找到则返回 null。</returns>
        [KernelFunction]
        [Description("根据程序名称查找其安装路径。支持查找如 notepad.exe, chrome.exe, calculator.exe 等。")]
        [System.ComponentModel.DisplayName("FindApplicationPath")]
        public string? FindApplicationPath(
            [Description("要查找的程序的可执行文件名，例如 'notepad.exe' 或 'chrome.exe'。")] string programName)
        {
            if (string.IsNullOrEmpty(programName))
            {
                Console.WriteLine("错误：程序名称不能为空。");
                return null;
            }

            // 1. 检查 PATH 环境变量
            string? pathEnv = Environment.GetEnvironmentVariable("PATH");
            if (!string.IsNullOrEmpty(pathEnv))
            {
                string[] directories = pathEnv.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
                foreach (string dir in directories)
                {
                    string fullPath = Path.Combine(dir, programName);
                    if (File.Exists(fullPath))
                    {
                        Console.WriteLine($"在 PATH 中找到 '{programName}'：{fullPath}");
                        return fullPath;
                    }
                }
            }

            // 2. 检查常见安装目录 (Program Files, Program Files (x86))
            string? programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string? programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            List<string> searchPaths = new List<string>();
            if (!string.IsNullOrEmpty(programFiles) && Directory.Exists(programFiles))
            {
                searchPaths.Add(programFiles);
                // 尝试查找子目录（例如 C:\Program Files\Google\Chrome\Application\chrome.exe）
                try
                {
                    foreach (var dir in Directory.GetDirectories(programFiles, "*", SearchOption.AllDirectories))
                    {
                        searchPaths.Add(dir);
                    }
                }
                catch { } // 忽略权限问题等
            }
            if (!string.IsNullOrEmpty(programFilesX86) && Directory.Exists(programFilesX86))
            {
                searchPaths.Add(programFilesX86);
                try
                {
                    foreach (var dir in Directory.GetDirectories(programFilesX86, "*", SearchOption.AllDirectories))
                    {
                        searchPaths.Add(dir);
                    }
                }
                catch { } // 忽略权限问题等
            }

            foreach (var dir in searchPaths)
            {
                string fullPath = Path.Combine(dir, programName);
                if (File.Exists(fullPath))
                {
                    Console.WriteLine($"在常见安装目录中找到 '{programName}'：{fullPath}");
                    return fullPath;
                }
            }

            // 3. 尝试从注册表查找（Windows 特有）
            // 这个部分可以非常复杂，因为注册表结构因应用程序而异。
            // 这里提供一个简化的示例，查找常见位置。
            // 对于更通用的查找，可能需要更深入的注册表扫描或第三方库。

            // 示例：查找 HKEY_CLASSES_ROOT\Applications\[programName]\shell\open\command
            string? regPath = $"Applications\\{programName}\\shell\\open\\command";
            string? foundPath = GetRegistryValue(Registry.ClassesRoot, regPath);
            if (foundPath != null)
            {
                Console.WriteLine($"在注册表 HKEY_CLASSES_ROOT 中找到 '{programName}'：{foundPath}");
                return GetExecutablePathFromCommand(foundPath); // 从命令字符串中提取 .exe 路径
            }

            // 示例：查找 HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\[programName]
            regPath = $"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\{programName}";
            foundPath = GetRegistryValue(Registry.LocalMachine, regPath);
            if (foundPath != null)
            {
                Console.WriteLine($"在注册表 HKEY_LOCAL_MACHINE 中找到 '{programName}'：{foundPath}");
                return GetExecutablePathFromCommand(foundPath);
            }

            // 尝试 32 位注册表视图
            regPath = $"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\{programName}";
            foundPath = GetRegistryValue(Registry.LocalMachine, regPath, true); // true 表示使用 32 位视图
            if (foundPath != null)
            {
                Console.WriteLine($"在注册表 HKEY_LOCAL_MACHINE (32位) 中找到 '{programName}'：{foundPath}");
                return GetExecutablePathFromCommand(foundPath);
            }


            // 如果以上都未找到，返回 null
            Console.WriteLine($"未找到程序 '{programName}' 的安装路径。");
            return null;
        }

        // --- 辅助方法 ---

        private static string? GetRegistryValue(RegistryKey rootKey, string subKey, bool force32Bit = false)
        {
            try
            {
                // Registry.OpenBaseKey(rootKey, RegistryView.Default) is deprecated, use specific base key
                // Example: Registry.LocalMachine
                RegistryKey? key = null;
                if (force32Bit)
                {
                    key = rootKey.OpenSubKey(subKey, (System.Security.AccessControl.RegistryRights)RegistryView.Registry32);
                }
                else
                {
                    key = rootKey.OpenSubKey(subKey);
                }

                if (key != null)
                {
                    // "Path" is a common value name for the executable path
                    // "Command" is also common for the full command line
                    object? value = key.GetValue("Path") ?? key.GetValue("Command") ?? key.GetValue(null); // Try common value names or the default value
                    if (value != null)
                    {
                        return value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取注册表 '{subKey}' 时出错：{ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// 从一个完整的命令字符串中提取可执行文件的路径。
        /// 例如："'C:\Program Files\Google\Chrome\Application\chrome.exe' --profile-directory=..."
        /// 提取出 'C:\Program Files\Google\Chrome\Application\chrome.exe'
        /// </summary>
        private static string? GetExecutablePathFromCommand(string commandLine)
        {
            if (string.IsNullOrEmpty(commandLine)) return null;

            // 移除可能存在的引号
            commandLine = commandLine.Trim();
            if (commandLine.StartsWith("\"") && commandLine.EndsWith("\""))
            {
                commandLine = commandLine.Substring(1, commandLine.Length - 2);
            }

            // 查找第一个参数分隔符（通常是空格），但要小心路径中可能存在的空格
            // 最好的方法是找到第一个非引号的空格，或者引号结束后的第一个空格
            int firstSpaceIndex = -1;
            bool inQuotes = false;
            for (int i = 0; i < commandLine.Length; i++)
            {
                if (commandLine[i] == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (commandLine[i] == ' ' && !inQuotes)
                {
                    firstSpaceIndex = i;
                    break;
                }
            }

            string executablePath;
            if (firstSpaceIndex != -1)
            {
                executablePath = commandLine.Substring(0, firstSpaceIndex);
            }
            else
            {
                executablePath = commandLine; // 如果没有空格，整个字符串就是路径
            }

            // 确保提取的是一个文件路径
            if (File.Exists(executablePath))
            {
                return executablePath;
            }
            else
            {
                // 可能是相对路径或者其他情况，或者路径本身就错了
                Console.WriteLine($"从命令提取的路径 '{executablePath}' 无效或不存在。");
                return null;
            }
        }

        [KernelFunction]
        [Description("在默认浏览器中打开URL")]
        public void OpenUrl(
            [Description("需要打开的完整URL地址, 例如 https://www.gov.cn")] string url)
        {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "http://" + url;
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
            {
                throw new ArgumentException("无效的URL格式");
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        private async Task<string?> FindExecutablePathAsync(string programName)
        {
            // 检查是否已提供完整路径
            if (File.Exists(programName))
                return Path.GetFullPath(programName);

            // 添加.exe扩展名如果缺少
            string fileName = programName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                ? programName
                : $"{programName}.exe";

            // 1. 在PATH环境变量中查找
            string? pathResult = FindInEnvironmentPath(fileName);
            if (pathResult != null)
                return pathResult;

            // 2. 在注册表中查找安装位置
            string? registryResult = FindInRegistry(fileName);
            if (registryResult != null)
                return registryResult;

            // 3. 在常见程序目录中查找
            string? commonDirResult = FindInCommonDirectories(fileName);
            return commonDirResult;
        }

        private string? FindInEnvironmentPath(string fileName)
        {
            string[] paths = Environment.GetEnvironmentVariable("PATH")?.Split(';')
                              ?? Array.Empty<string>();

            foreach (string path in paths)
            {
                string fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }
            return null;
        }

        private string? FindInRegistry(string fileName)
        {
            string[] registryPaths = {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths"
        };

            foreach (var basePath in registryPaths)
            {
                // 尝试HKEY_LOCAL_MACHINE
                using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(Path.Combine(basePath, fileName)))
                {
                    if (key?.GetValue("") is string path && File.Exists(path))
                    {
                        return path;
                    }
                }

                // 尝试HKEY_CURRENT_USER
                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(Path.Combine(basePath, fileName)))
                {
                    if (key?.GetValue("") is string path && File.Exists(path))
                    {
                        return path;
                    }
                }
            }
            return null;
        }

        private string? FindInCommonDirectories(string fileName)
        {
            string[] commonDirs = {
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms)
        };

            foreach (string dir in commonDirs)
            {
                string fullPath = Path.Combine(dir, fileName);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }
            return null;
        }
    }

}
