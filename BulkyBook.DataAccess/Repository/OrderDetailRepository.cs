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
    public class OrderDetailRepository : Repository<OrderDetails>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderDetailRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderDetails order)
        {
            _context.Update(order);
        }

        //public async Task UpdateAsync(Category category)
        //{
        //    var entity = await _context.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
        //    if (entity != null)
        //    {
        //        entity.Name = category.Name;
        //    }
        //}
    }
}
