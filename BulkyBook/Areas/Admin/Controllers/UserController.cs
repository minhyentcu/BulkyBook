using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = _context.ApplicationUsers.Include(x => x.Company).ToList();
            var userRoles = _context.UserRoles.ToList();
            var role = _context.Roles.ToList();
            foreach (var user in userList)
            {
                var roleId = userRoles.Where(x => x.UserId == user.Id).FirstOrDefault()?.RoleId;
                user.Role = role.Where(x => x.Id == roleId).FirstOrDefault()?.Name;
                if (user.Company == null)
                {
                    user.Company = new Company
                    {
                        Name = string.Empty
                    };
                }
            }
            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _context.ApplicationUsers.Find(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd >DateTime.Now)
            {
                //user is currently loked , we will unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(500);
            }
            _context.SaveChanges();

            return Json(new { success = true, message = "Operation Successfully!" });
        }
    }
}
