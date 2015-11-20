//------------------------------------------------------------------------------
// <copyright file="AccountDto.cs" company="CQ Ebos Co., Ltd.">
//    Copyright (c) 2015, CQ Ebos Co., Ltd. All rights reserved.
// </copyright>
// <author>Yuan Rui</author>
// <email>yuanrui@live.cn</email>
// <date>2015-11-20 17:15:39</date>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Study.WebServices.Models
{
    public class AccountDto
    {
        private AccountModel model;

        public Guid Id
        {
            get { return model.Id; } 
            set { model.Id = value; }
        }

        public string Name
        {
            get { return model.Name; }
            set { model.Name = value; }
        }

        public string UserName
        {
            get { return model.UserName; }
            set { model.UserName = value; }
        }

        public string Password
        {
            get { return model.Password; }
            set { model.Password = value; }
        }

        public bool Enable
        {
            get { return model.Enable; }
            set { model.Enable = value; }
        }

        public string CreatedBy
        {
            get { return model.CreatedBy; }
            set { model.CreatedBy = value; }
        }

        public DateTime CreatedAt
        {
            get { return model.CreatedAt; }
            set { model.CreatedAt = value; }
        }

        public string ModifiedBy
        {
            get { return model.ModifiedBy; }
            set { model.ModifiedBy = value; }
        }

        public DateTime ModifiedAt
        {
            get { return model.ModifiedAt; }
            set { model.ModifiedAt = value; }
        }

        public int SortOrder
        {
            get { return model.SortOrder; }
            set { model.SortOrder = value; }
        }

        public AccountDto()
        {
            model = new AccountModel();
        }

        public AccountDto(AccountModel model)
        {
            this.model = model;
        }

        public AccountModel AsModel()
        {
            return model;
        }
    }
}