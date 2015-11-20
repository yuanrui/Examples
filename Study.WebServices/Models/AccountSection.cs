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