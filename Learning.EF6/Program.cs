using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Learning.EF6
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = new List<Category>();
            var ctx = new LocalContext();

            for (int i = 0; i < 10; i++)
            {
                var category = new Category();
                category.Id = Guid.NewGuid();
                category.Name = "Test" + i + category.Id;
                category.SortId = i;
                ctx.Categories.Add(category);
                
            }
            ctx.SaveChanges();

            //ctx.Categories.RemoveRange(ctx.Categories);
            //ctx.SaveChanges();
            foreach (var item in ctx.Categories)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }
    }
}
