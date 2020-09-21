using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            ProductViewModel product = new ProductViewModel()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            if (id == null)
            {
                return View(product);
            }
            //this for update

            product.Product = _unitOfWork.Product.Get(id.GetValueOrDefault());
            if (product.Product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                string webRootpath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootpath, @"images/product");
                    var extension = Path.GetExtension(files[0].FileName);
                    if (model.Product.ImageUrl != null)
                    {
                        //edit product
                        var imagePath = Path.Combine(webRootpath, model.Product.ImageUrl.TrimStart('\\'));

                        //remove file image old product
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    using (var filesStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);

                    }
                    model.Product.ImageUrl = @"\images\product\" + fileName + extension;
                }
                else
                {
                    //update product when not change the image
                    if (model.Product?.Id != 0)
                    {
                        var entity = _unitOfWork.Product.Get(model.Product.Id);
                        model.Product.ImageUrl = entity.ImageUrl;
                    }
                }

                if (model.Product.Id == 0)
                {
                    _unitOfWork.Product.Insert(model.Product);

                }
                else
                {
                    _unitOfWork.Product.Update(model.Product);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            model.CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            model.CoverTypeList = _unitOfWork.CoverType.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            if (model.Product.Id != 0)
            {
                model.Product = _unitOfWork.Product.Get(model.Product.Id);
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data = products });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var entity = _unitOfWork.Product.Get(id);
            if (entity == null)
            {
                return Json(new { success = false, message = "Error while deleting!" });
            }
            string webRootpath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootpath, entity.ImageUrl.TrimStart('\\'));

            //remove file image old product
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _unitOfWork.Product.Remove(entity);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successfully!" });
        }
    }
}
