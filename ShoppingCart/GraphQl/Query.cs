using Microsoft.EntityFrameworkCore;
using ShoppingCart.Models;

namespace ShoppingCart.GraphQl
{
    public class Query
    {
        public string GetMeessage() => "Hello from GraphQl";

        public List<Product>  ReadAll([Service] ShoppingDbContext context)
        {
            return context.Products.ToList();
        }

        public Product ReadDetail([Service] ShoppingDbContext context, int id)
        {
            var todo = context.Products.FirstOrDefault(o => o.Id == id);
            if (todo == null)
            {
                Console.WriteLine("Id not found");
            }
            return todo;
        }
    }
}
