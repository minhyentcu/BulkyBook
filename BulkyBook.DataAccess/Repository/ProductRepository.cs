using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            var entity = _context.Products.FirstOrDefault(x => x.Id == product.Id);
            if (entity != null)
            {
                if (product.ImageUrl != null)
                {
                    entity.ImageUrl = product.ImageUrl;
                    entity.ISBN = entity.ISBN;
                    entity.ListPrice = product.ListPrice;
                    entity.Price = product.Price;
                    entity.Price100 = product.Price100;
                    entity.Price50 = product.Price50;
                    entity.Title = product.Title;
                    entity.Description = product.Description;
                    entity.CategoryId = product.CategoryId;
                    entity.CoverTypeId = product.CoverTypeId;
                    entity.Author = product.Author;
                }
                //entity.Name = product.Name;
                //_context.SaveChanges();
            }
        }

        public async Task UpdateAsync(Product product)
        {
            var entity = await _context.Products.FirstOrDefaultAsync(x => x.Id == product.Id);
            if (entity != null)
            {

                // entity.Name = category.Name;
            }
        }
    }
}
