using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminLTE.WebUI.Models
{
    public class PermissionEntity
    {
        public virtual string Id { get; set; }

        public virtual string ParentId { get; set; }

        public virtual string Name { get; set; }

        public virtual string Url { get; set; }

        public virtual string Icon { get; set; }

        public virtual int Type { get; set; }

        public virtual bool Enabled { get; set; }

        public virtual int SortIndex { get; set; }

        public virtual int Level { get; set; }

        public virtual string Path { get; set; }

        public virtual string LinkTarget { get; set; }

        public virtual string Callback { get; set; }

        public virtual string Remark { get; set; }

        public virtual string CreatedBy { get; set; }

        public virtual DateTime CreatedAt { get; set; }

        public virtual string ModifiedBy { get; set; }

        public virtual DateTime ModifiedAt { get; set; }

        public virtual ICollection<RoleEntity> Roles { get; set; }
    }
}