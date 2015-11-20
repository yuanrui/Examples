//------------------------------------------------------------------------------
// <copyright file="AccountSection.cs" company="CQ Ebos Co., Ltd.">
//    Copyright (c) 2015, CQ Ebos Co., Ltd. All rights reserved.
// </copyright>
// <author>Yuan Rui</author>
// <email>yuanrui@live.cn</email>
// <date>2015-11-20 15:41:06</date>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Study.WebServices.Models
{
    public class AccountSection : ConfigurationSection
    {
        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public AccountCollection Accounts
        {
            get { return (AccountCollection)base[""]; }
        }
    }
}