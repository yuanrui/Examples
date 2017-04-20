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

        public string RawId { get; set; }

        public string[] RawIds
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RawId))
                {
                    return new string[0];
                }

                return RawId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public string Remark { get; set; }

        public int SortId { get; set; }

        public Category()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
