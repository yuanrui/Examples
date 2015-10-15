using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CSharp;

namespace Study.CodeGenerator.Core
{
    public class CodeGenerator
    {
        /// <summary>
        /// 创建类实体文件
        /// </summary>
        /// <param name="table">数据表实体</param>
        /// <param name="codeEntParam">实体参数</param>
        public void CreateCodeFile(Table table, CodeEntityParameter codeEntParam)
        {
            string className = string.Empty;
            CodeDomHelper codeDomHelper = new CodeDomHelper();
            
            className = GetClassName(table, codeEntParam);
            var codeCompileUnit = codeDomHelper.GetCodeCompileUnit(codeEntParam.NameSpace, className, string.IsNullOrWhiteSpace(table.Comment) ? table.Name : table.Comment);
            var codeTypeDeclaration = codeCompileUnit.Namespaces[0].Types[0];
            foreach (var column in table.Columns)
            {
                codeTypeDeclaration.Members.Add(codeDomHelper.CreateAutoProperty(column.Type, column.Name, column.Comment));
            }

            CodeDomProvider provider = new CSharpCodeProvider();
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            var stringBuilder = new StringBuilder();
            using (provider)
            {
                var stringWriter = new StringWriter(stringBuilder);
                provider.GenerateCodeFromCompileUnit(codeCompileUnit, stringWriter, new CodeGeneratorOptions());
            }
            string tmpPath = string.IsNullOrWhiteSpace(codeEntParam.Suffix) ? "Entity" : codeEntParam.Suffix;
            string path = codeEntParam.SavePath;
            if (! codeEntParam.SavePath.EndsWith(tmpPath))
            {
                path = Path.Combine(codeEntParam.SavePath, tmpPath);
            }

            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string filePath = Path.Combine(path, className + ".cs");
                File.WriteAllText(filePath, CleanupCode(stringBuilder.ToString()), Encoding.UTF8);
            }
            catch (IOException ioEx)
            {
                throw ioEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取数据表对应的类名
        /// </summary>
        /// <param name="table">数据表实体</param>
        /// <param name="codeEntParam">实体参数</param>
        /// <returns>类名</returns>
        private string GetClassName(Table table, CodeEntityParameter codeEntParam)
        {
            string className = table.Name;
            if (codeEntParam.IsAddOrRemovePrefix)
            {
                if (! table.Name.StartsWith(codeEntParam.Prefix))
                {
                    className = string.Format("{0}{1}", codeEntParam.Prefix, table.Name);
                }
            }
            else
            {
                if (table.Name.StartsWith(codeEntParam.Prefix))
                {
                    className = table.Name.Remove(0, codeEntParam.Prefix.Count());
                }
            }

            if (codeEntParam.IsAddSuffix)
            {
                className = string.Format("{0}{1}", className, codeEntParam.Suffix);
            }

            return className;
        }

        /// <summary>
        /// 清理整理多余部分代码
        /// </summary>
        /// <param name="code">代码内容</param>
        /// <returns></returns>
        private static string CleanupCode(string code)
        {
            code = RemoveComments(code);
            code = AddStandardHeader(code);
            code = FixAutoProperties(code);
            code = RemoveExcessLine(code);
            code = FixNullableSyntax(code);
            return code;
        }

        /// <summary>
        /// 修复自动属性
        /// </summary>
        /// <param name="code">代码内容</param>
        /// <returns></returns>
        private static string FixAutoProperties(string code)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("{");
            builder.Append("        }");
            code = code.Replace(builder.ToString(), "{ }");
            builder.Clear();
            builder.AppendLine("{");
            builder.AppendLine("            get {");
            builder.AppendLine("            }");
            builder.AppendLine("            set {");
            builder.AppendLine("            }");
            builder.Append("        }");

            code = code.Replace(builder.ToString(), "{ get; set; }");

            code = code.Replace(@"get {
            }", "get;");
            code = code.Replace(@"set {
            }", "set;");

            code = code.Replace(@"get { }", "get;");
            code = code.Replace(@"set { }", "set;");

            return code;
        }

        /// <summary>
        /// 添加头部引用命名空间
        /// </summary>
        /// <param name="code">代码内容</param>
        /// <returns></returns>
        private static string AddStandardHeader(string code)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("using System;");
            //builder.AppendLine("using System.Linq;"); 
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using System.Text;");
            builder.Append(code);
            return builder.ToString();
        }

        /// <summary>
        /// 去掉自动生成的注释
        /// </summary>
        /// <param name="code">代码内容</param>
        /// <returns></returns>
        private static string RemoveComments(string code)
        {
            int end = code.LastIndexOf("----------");
            code = code.Remove(0, end + 10);
            return code;
        }

        /// <summary>
        /// 将三行替换为两行
        /// </summary>
        /// <param name="code">代码内容</param>
        /// <returns></returns>
        private static string RemoveExcessLine(string code)
        {
            code = code.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);

            code = code.Replace("\r\n    \r\n    \r\n    ", "\r\n    \r\n    ");
            return code;
        }

        /// <summary>
        /// 将Nullable&lt;T&gt;替换为T?
        /// </summary>
        /// <param name="code">代码内容</param>
        /// <returns></returns>
        public static string FixNullableSyntax(string code)
        {
            string pattern1 = @"System.Nullable<System.(?<type1>\w+)>";            
            code = Regex.Replace(code, pattern1, "${type1}?");
            string pattern2 = @"System.Nullable<(?<type2>\w+)>";
            code = Regex.Replace(code, pattern2, "${type2}?");

            return code;
        }

    }
}
