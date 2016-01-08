using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Learning.EF6
{
    public class Category
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Remark { get; set; }

        public int SortId { get; set; }

        public override string ToString()
        {
            return string.Format("Id:{0} Name:{1} Remark:{2} SortId:{3}", Id, Name, Remark, SortId);
        }
    }
}
