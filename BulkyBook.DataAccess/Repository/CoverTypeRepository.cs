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
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _context;
        public CoverTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(CoverType coverTye)
        {
            var entity = _context.CoverTypes.FirstOrDefault(x => x.Id == coverTye.Id);
            if (entity != null)
            {
                entity.Name = coverTye.Name;
                _context.SaveChanges();
            }
        }

        public async Task UpdateAsync(CoverType coverType)
        {
            var entity = await _context.CoverTypes.FirstOrDefaultAsync(x => x.Id == coverType.Id);
            if (entity != null)
            {
                entity.Name = coverType.Name;
            }
        }
    }
}
