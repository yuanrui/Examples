using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminLTE.WebUI.Models
{
    public class RoleEntity
    {
        public virtual string Id { get; set; }

        public virtual string Name { get; set; }

        public virtual bool Enabled { get; set; }
                
        public virtual string Remark { get; set; }

        public virtual string CreatedBy { get; set; }

        public virtual DateTime CreatedAt { get; set; }

        public virtual string ModifiedBy { get; set; }

        public virtual DateTime ModifiedAt { get; set; }

        public virtual ICollection<AccountEntity> Accounts { get; set; }

        public virtual ICollection<PermissionEntity> Permissions { get; set; }
    }
}