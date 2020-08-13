using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
   public interface ICoverTypeRepository : IRepository<CoverType>
    {
        void Update(CoverType coverType);
        Task UpdateAsync(CoverType coverType);
    }
}
