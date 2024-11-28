using GudandStore.Interfaces;
using GundamStore.Interfaces;
using GundamStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GundamStore.Controllers
{
    [AllowAnonymous]
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

        public async Task<IActionResult> Details(long id)
        {

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var category = await _categoryService.GetCategoryByIdAsync(product.CategoryId);
            var scale = await _scaleService.GetScaleByIdAsync(product.ScaleId);
            var images = await _productImageService.GetProductImagesByProductIdAsync(id);


            ViewBag.Category = category;
            ViewBag.Scale = scale;
            ViewBag.Images = images;

            return View(product);
        }
    }
}