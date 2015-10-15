using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace Study.CodeGenerator.Core
{
    /// <summary>
    /// object扩展方法
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// 将泛型对象序列化为xml文件
        /// </summary>
        /// <typeparam name="T">类型T</typeparam>
        /// <param name="t"></param>
        /// <param name="filePath">要保存的xml文件路径</param>
        public static void SaveAsXml<T>(this T t, string filePath) where T : class, new()
        {
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }
            if (filePath == null)
            {
                throw new ArgumentException("filePath不能为空");
            }
            
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                xs.Serialize(stream, t);
            }
        }

        /// <summary>
        /// 将泛型对象序列化为xml格式的字符串
        /// </summary>
        /// <typeparam name="T">类型T</typeparam>
        /// <param name="t"></param>
        /// <returns>xml格式字符串</returns>
        public static string AsXml<T>(this T t) where T : class, new()
        {
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }

            XmlSerializer xs = new XmlSerializer(typeof(T));

            using (TextWriter writer = new StringWriter(System.Globalization.CultureInfo.CurrentCulture))
            {
                xs.Serialize(writer, t);
                return writer.ToString();
            }
        }

        /// <summary>
        /// 加载xml文件返回泛型对象
        /// </summary>
        /// <typeparam name="T">类型T</typeparam>
        /// <param name="t"></param>
        /// <param name="filePath">要保存的xml文件路径</param>
        /// <returns>泛型对象</returns>
        public static T LoadFromXml<T>(this T t, string filePath) where T : class, new()
        {
            if (filePath == null)
            {
                throw new ArgumentException("filePath不能为空");
            }

            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (Stream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
            {
                return xs.Deserialize(stream) as T;
            }
        }
    }
}
