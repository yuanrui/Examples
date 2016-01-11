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
        public string Id { get; set; }

        public string Name { get; set; }

        public string Remark { get; set; }

        public int SortId { get; set; }

        public Category()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
