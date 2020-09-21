using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Http;

namespace BulkyBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            //then we will add to cart
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCartRepository.
                    GetAll(x => x.UserId == claim.Value).ToList().Count();

                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);
            }
            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = _unitOfWork.Product.
                GetFirstOrDefault(u => u.Id == id, includeProperties: "Category,CoverType");
            var cart = new ShoppingCart
            {
                Product = product,
                ProductId = product.Id,

            };
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart cartModel)
        {

            cartModel.Id = 0;
            if (ModelState.IsValid)
            {
                //then we will add to cart
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cartModel.UserId = claim.Value;

                ShoppingCart entity = _unitOfWork.ShoppingCartRepository.
                    GetFirstOrDefault(x => x.UserId == cartModel.UserId &&
                   x.ProductId == cartModel.ProductId, includeProperties: "Product");
                if (entity == null)
                {
                    //no records exists in database for that product for that user
                    _unitOfWork.ShoppingCartRepository.Insert(cartModel);
                }
                else
                {
                    entity.Count += cartModel.Count;
                    //  _unitOfWork.ShoppingCartRepository.Update(entity);
                }
                _unitOfWork.Save();
                var count = _unitOfWork.ShoppingCartRepository.
                    GetAll(x => x.UserId == cartModel.UserId).ToList().Count();

                //HttpContext.Session.SetObject(SD.ssShoppingCart, cartModel);
                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var product = _unitOfWork.Product.
                GetFirstOrDefault(u => u.Id == cartModel.Id, includeProperties: "Category,CoverType");
                var cart = new ShoppingCart
                {
                    Product = product,
                    ProductId = product.Id,

                };
                return View(cart);
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
