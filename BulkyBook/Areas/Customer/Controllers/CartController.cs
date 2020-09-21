using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace BulkyBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitofWork;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;

        public ShoppingCartViewModel shoppingCartViewModel { get; set; }

        public CartController(IUnitOfWork unitofWork, IEmailSender emailSender
            , UserManager<IdentityUser> userManager)
        {
            _unitofWork = unitofWork;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            shoppingCartViewModel = new ShoppingCartViewModel()
            {
                OrderHeader = new OrderHeader(),
                ListCart = _unitofWork.ShoppingCartRepository.
                GetAll(x => x.UserId == claim.Value, includeProperties: "Product")
            };
            shoppingCartViewModel.OrderHeader.OrderTotal = 0;
            shoppingCartViewModel.OrderHeader.User = _unitofWork.ApplicationUser.
                GetFirstOrDefault(x => x.Id == claim.Value, includeProperties: "Company");

            foreach (var item in shoppingCartViewModel.ListCart)
            {
                item.Price = SD.GetPriceBaseOnQuantity(item.Count, item.Product.Price,
                                item.Product.Price50, item.Product.Price100);
                shoppingCartViewModel.OrderHeader.OrderTotal += (item.Price * item.Count);
                item.Product.Description = SD.ConvertToRawHtml(item.Product.Description);
                if (item.Product.Description.Length > 100)
                {
                    item.Product.Description = item.Product.Description.Substring(0, 99) + "...";
                }
            }
            return View(shoppingCartViewModel);
        }

        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var user = _unitofWork.ApplicationUser.GetFirstOrDefault(x => x.Id == claim.Value);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Verification email is empty!");
            }
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Plus(int cartId)
        {
            var cart = _unitofWork.ShoppingCartRepository
                .GetFirstOrDefault(x => x.Id == cartId, includeProperties: "Product");
            cart.Count += 1;
            cart.Price = SD.GetPriceBaseOnQuantity(cart.Count, cart.Product.Price,
                cart.Product.Price50, cart.Product.Price100);
            _unitofWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cart = _unitofWork.ShoppingCartRepository
                 .GetFirstOrDefault(x => x.Id == cartId, includeProperties: "Product");
            if (cart.Count == 1)
            {
                var cnt = _unitofWork.ShoppingCartRepository
                    .GetAll(x => x.UserId == cart.UserId).ToList().Count();
                _unitofWork.ShoppingCartRepository.Remove(cart);
                _unitofWork.Save();
                HttpContext.Session.SetInt32(SD.ssShoppingCart, cnt - 1);
            }
            else
            {
                cart.Count -= 1;
                cart.Price = SD.GetPriceBaseOnQuantity(cart.Count, cart.Product.Price,
                    cart.Product.Price50, cart.Product.Price100);
                _unitofWork.Save();
            }
            return RedirectToAction(nameof(Index));
        }

       
        public IActionResult Remove(int cartId)
        {
            var cart = _unitofWork.ShoppingCartRepository
                .GetFirstOrDefault(x => x.Id == cartId, includeProperties: "Product");

            var cnt = _unitofWork.ShoppingCartRepository
                .GetAll(x => x.UserId == cart.UserId).ToList().Count();
            _unitofWork.ShoppingCartRepository.Remove(cart);
            _unitofWork.Save();
            HttpContext.Session.SetInt32(SD.ssShoppingCart, cnt - 1);

            return RedirectToAction(nameof(Index));
        }

    }
}
