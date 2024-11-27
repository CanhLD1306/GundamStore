using GundamStore.Data;
using GundamStore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GundamStore.Areas.Admin.ViewModels;
using GundamStore.Models;
using GundamStore.Admin.ViewModels;
using GudandStore.Interfaces;

namespace GundamStore.Areas.Admin.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly IProductService _productService;

        private readonly ICategoryService _categoryService;

        private readonly IScaleService _scaleService;

        private readonly IProductImageService _productImageService;

        public ProductsController(IProductService productService, ICategoryService categoryService, IScaleService scaleService, IProductImageService productImageService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _scaleService = scaleService ?? throw new ArgumentNullException(nameof(scaleService));
            _productImageService = productImageService ?? throw new ArgumentNullException(nameof(productImageService));
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ListAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return PartialView("_ListProducts", products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var scales = await _scaleService.GetAllScalesAsync();

            ViewBag.Categories = categories;
            ViewBag.Scales = scales;

            return View(new ProductViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel model, List<ImageViewModel> images)
        {
            if (!ModelState.IsValid)
            {
                return Json(new Result { Success = false, Message = "Invalid product details." });
            }
            if (images == null || images.Count == 0)
            {
                return Json(new Result { Success = false, Message = "At least one image is required." });
            }

            if (images.Count(img => img.IsDefault) != 1)
            {
                return Json(new Result { Success = false, Message = "Exactly one image must be marked as default." });
            }
            try
            {
                await _productService.InsertProductAsync(model, images);
                return Json(new Result { Success = true, Message = "Product created successfully", RedirectUrl = Url.Action("Index", "Products") });
            }
            catch (ArgumentException ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Json(new Result { Success = false, Message = "You must be logged in to perform this action." });
            }
            catch (Exception)
            {
                return Json(new Result { Success = false, Message = "An error occurred. Please try again later." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                var categories = await _categoryService.GetAllCategoriesAsync();
                var scales = await _scaleService.GetAllScalesAsync();
                var images = await _productImageService.GetProductImagesByProductIdAsync(id);


                ViewBag.Categories = categories;
                ViewBag.Scales = scales;
                ViewBag.Images = images;

                return View(product);
            }
            catch (KeyNotFoundException ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new Result { Success = false, Message = "An error occurred. Please try again later." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(ProductViewModel model, List<ImageViewModel> images)
        {
            if (!ModelState.IsValid)
            {
                return Json(new Result { Success = false, Message = "Invalid product details." });
            }
            try
            {
                await _productService.UpdateProductAsync(model, images);
                return Json(new Result { Success = true, Message = "Product updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Json(new Result { Success = false, Message = "You must be logged in to perform this action." });
            }
            catch (Exception)
            {
                return Json(new Result { Success = false, Message = "An error occurred. Please try again later." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return PartialView("_DeleteProductModel", product);
            }
            catch (KeyNotFoundException ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new Result { Success = false, Message = "An error occurred. Please try again later." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(long id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return Json(new Result { Success = true, Message = "Banner delete successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new Result { Success = false, Message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Json(new Result { Success = false, Message = "You must be logged in to perform this action." });
            }
            catch (Exception)
            {
                return Json(new Result { Success = false, Message = "An error occurred. Please try again later." });
            }
        }
    }
}