//------------------------------------------------------------------------------
// <copyright file="Account.cs" company="CQ Ebos Co., Ltd.">
//    Copyright (c) 2015, CQ Ebos Co., Ltd. All rights reserved.
// </copyright>
// <author>Yuan Rui</author>
// <email>yuanrui@live.cn</email>
// <date>2015-11-20 14:51:19</date>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Study.WebServices.Models
{
    [Serializable]
    public class AccountModel : ConfigurationElement
    {
        [ConfigurationProperty("Id", IsRequired = true)]
        public Guid Id
        {
            get { return Guid.Parse(this["Id"].ToString()); }
            set { this["Id"] = value; }
        }

        [ConfigurationProperty("Name", IsRequired = true)]
        public string Name
        {
            get { return this["Name"].ToString(); }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("UserName", IsRequired = true)]
        public string UserName
        {
            get { return this["UserName"].ToString(); }
            set { this["UserName"] = value; }
        }

        [ConfigurationProperty("Password", IsRequired = true)]
        public string Password
        {
            get { return this["Password"].ToString(); }
            set { this["Password"] = value; }
        }

        [ConfigurationProperty("Enable", IsRequired = true)]
        public bool Enable
        {
            get { return bool.Parse(this["Enable"].ToString()); }
            set { this["Enable"] = value; }
        }

        [ConfigurationProperty("CreatedBy", IsRequired = true)]
        public string CreatedBy
        {
            get { return this["CreatedBy"].ToString(); }
            set { this["CreatedBy"] = value; }
        }

        [ConfigurationProperty("CreatedAt", IsRequired = true)]
        public DateTime CreatedAt
        {
            get { return DateTime.Parse(this["CreatedAt"].ToString()); }
            set { this["CreatedAt"] = value; }
        }

        [ConfigurationProperty("ModifiedBy", IsRequired = true)]
        public string ModifiedBy
        {
            get { return this["ModifiedBy"].ToString(); }
            set { this["ModifiedBy"] = value; }
        }

        [ConfigurationProperty("ModifiedAt", IsRequired = true)]
        public DateTime ModifiedAt
        {
            get { return DateTime.Parse(this["ModifiedAt"].ToString()); }
            set { this["ModifiedAt"] = value; }
        }

        [ConfigurationProperty("SortOrder", IsRequired = true)]
        public int SortOrder
        {
            get { return int.Parse(this["SortOrder"].ToString()); }
            set { this["SortOrder"] = value; }
        }

        public AccountModel()
        {
            Id = Guid.NewGuid();
            Enable = true;
            CreatedAt = DateTime.Now;
            ModifiedAt = CreatedAt;
        }
    }
}