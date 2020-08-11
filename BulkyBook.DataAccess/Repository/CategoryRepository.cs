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
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Category category)
        {
            var entity = _context.Categories.FirstOrDefault(x => x.Id == category.Id);
            if (entity != null)
            {
                entity.Name = category.Name;
                _context.SaveChanges();
            }
        }

        public async Task UpdateAsync(Category category)
        {
            var entity = await _context.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
            if (entity != null)
            {
                entity.Name = category.Name;
                await _context.SaveChangesAsync();
            }
        }
    }
}
