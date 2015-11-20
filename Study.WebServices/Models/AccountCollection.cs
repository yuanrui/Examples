//------------------------------------------------------------------------------
// <copyright file="AccountCollection.cs" company="CQ Ebos Co., Ltd.">
//    Copyright (c) 2015, CQ Ebos Co., Ltd. All rights reserved.
// </copyright>
// <author>Yuan Rui</author>
// <email>yuanrui@live.cn</email>
// <date>2015-11-20 15:08:34</date>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Study.WebServices.Models
{
    public class AccountCollection : ConfigurationElementCollection, ICollection<AccountModel>, IEnumerable<AccountModel>
    {
        protected override string ElementName
        {
            get { return "Account"; }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        public AccountModel this[int index]
        {
            get
            {
                return (AccountModel)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public new AccountModel this[string name]
        {
            get
            {
                return (AccountModel)base.BaseGet(name);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new AccountModel();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AccountModel)element).Id;
        }

        public new IEnumerator<AccountModel> GetEnumerator()
        {
            for (int i = 0; i < base.Count; i++)
            {
                yield return (AccountModel)base.BaseGet(i);
            }
        }

        public void Add(AccountModel item)
        {
            BaseAdd(item);
        }

        public void Clear()
        {
            BaseClear();
        }

        public bool Contains(AccountModel item)
        {
            return BaseIndexOf(item) > 0;
        }

        public bool Remove(AccountModel item)
        {
            BaseRemove(item.Id);
            return true;
        }

        public void CopyTo(AccountModel[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (array.Length < arrayIndex)
            {
                throw new ArgumentOutOfRangeException("array", "array数组范围小于arrayIndex");
            }

            for (int i = 0; i < arrayIndex; i++)
            {
                array[i] = (AccountModel)BaseGet(i);
            }
        }

        public new bool IsReadOnly
        {
            get { return false; }
        }
    }
}