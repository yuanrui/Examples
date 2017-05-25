using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminLTE.WebUI.Models
{
    public class AccountEntity
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string UserName { get; set; }

        public virtual string Password { get; set; }

        public virtual string Salt { get; set; }

        public virtual string Email { get; set; }

        public virtual string Telephone { get; set; }

        public virtual string Mobile { get; set; }

        public virtual string LockIp { get; set; }

        public virtual DateTime PasswordModifiedAt { get; set; }

        public virtual bool Enabled { get; set; }

        public virtual string CreatedBy { get; set; }

        public virtual DateTime CreatedAt { get; set; }

        public virtual string ModifiedBy { get; set; }

        public virtual DateTime ModifiedAt { get; set; }

        public virtual ICollection<RoleEntity> Roles { get; set; }
    }
}