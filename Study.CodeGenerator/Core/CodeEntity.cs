using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;

namespace Study.CodeGenerator.Core
{
    /// <summary>
    /// 实体所需要的参数
    /// </summary>
    public class CodeEntityParameter
    {
        /// <summary>
        /// 添加或删除表名前缀
        /// true:添加, false:删除
        /// </summary>
        public bool IsAddOrRemovePrefix { get; set; }
        /// <summary>
        /// 表名前缀
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// 是否添加后缀
        /// </summary>
        public bool IsAddSuffix { get; set; }
        /// <summary>
        /// 表名后缀
        /// </summary>
        public string Suffix { get; set; }
        /// <summary>
        /// 命名空间
        /// </summary>
        public string NameSpace { get; set; }
        /// <summary>
        /// 保存路径
        /// </summary>
        public string SavePath { get; set; }
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// 服务器用户名
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 服务器密码
        /// </summary>
        public string UserPwd { get; set; }
        /// <summary>
        /// 获取或设置一个布尔值，该值指示是否在连接中指定用户 ID 和密码（值为 false 时），或者是否使用当前的 Windows 帐户凭据进行身份验证（值为true 时）
        /// </summary>
        public bool IntegratedSecurity { get; set; }
        /// <summary>
        /// 数据库名
        /// </summary>
        public string DataBaseName { get; set; }

        public void Save()
        {
            XmlSerializer xs = new XmlSerializer(typeof(CodeEntityParameter));
            string xmlPath = Application.ExecutablePath + ".Config.xml"; 
            using (Stream stream = new FileStream(xmlPath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                xs.Serialize(stream, this);
                stream.Close();
            }
        }

        public static CodeEntityParameter Get()
        {
            CodeEntityParameter entity = null;
            XmlSerializer xs = new XmlSerializer(typeof(CodeEntityParameter));
            string xmlPath = Application.ExecutablePath + ".Config.xml";
            try
            {
                using (Stream stream = new FileStream(xmlPath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                {
                    entity = xs.Deserialize(stream) as CodeEntityParameter;
                    stream.Close();
                }
            }
            catch 
            { 
                entity = new CodeEntityParameter();
                entity.Server = "(local)";
                entity.IntegratedSecurity = true;
                entity.UserId = "sa";
                entity.UserPwd = "123456";
                entity.SavePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                entity.IsAddSuffix = true;
                entity.Suffix = "Entity";
                entity.NameSpace = "Example.Entity";
            }

            return entity;
        }
    }

    /// <summary>
    /// 表
    /// </summary>
    public class Table
    {
        public Table()
        {
            Columns = new List<Column>();
        }

        /// <summary>
        /// 存放表的唯一ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 表名注释
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Table下面的列集合
        /// </summary>
        public IList<Column> Columns { get; set; }
    }

    /// <summary>
    /// 列
    /// </summary>
    public class Column
    {
        private Type type;
        private string typeName;
        /// <summary>
        /// 列ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数据列类型 int,char,varchar,nvarchar...
        /// </summary>
        public string DBType { get; set; }
        /// <summary>
        /// .Net 支持的类型
        /// </summary>
        public Type Type
        {
            get
            {
                if (type == null)
                {
                    if (!string.IsNullOrWhiteSpace(DBType))
                    {
                        type = DBTypeMapper.DBTypeToCodeType(DBType);
                    }
                    if (IsNullAble)
                    {
                        if (type.IsValueType)
                        {
                            type = typeof(Nullable<>).MakeGenericType(type);
                        }                    
                    }
                }                
                
                return type;
            }
            set { type = value; }
        }

        /// <summary>
        /// .Net 支持的类型名
        /// </summary>
        public string TypeName
        {
            get {
                if (string.IsNullOrWhiteSpace(typeName))
                {
                    if (!string.IsNullOrWhiteSpace(DBType))
                    {
                        typeName = DBTypeMapper.DBTypeToCodeTypeName(DBType);
                    }
                    if (IsNullAble)
                    {
                        if (Type.IsValueType)
                        {
                            typeName = string.Format("{0}?", typeName);
                        }
                    }
                }
                
                return typeName;
            }

            set { typeName = value; }
        }

        /// <summary>
        /// 列注释
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// 表 所属表
        /// </summary>
        public Table Table { get; set; }
        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// 是否外键
        /// </summary>
        public bool IsForeignKey { get; set; }
        /// <summary>
        /// 是否允许为空
        /// </summary>
        public bool IsNullAble { get; set; }
    }
}