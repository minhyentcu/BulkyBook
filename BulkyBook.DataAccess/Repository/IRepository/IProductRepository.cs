using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
   public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);
        Task UpdateAsync(Product product);
    }
}
