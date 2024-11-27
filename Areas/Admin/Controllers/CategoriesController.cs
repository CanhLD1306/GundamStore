using GundamStore.Models;
using GundamStore.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace GundamStore.Areas.Admin.Controllers
{
    public class CategoriesController : BaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return PartialView("_ListCategories", categories);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name, string description)
        {
            try
            {
                await _categoryService.InsertCategoryAsync(name, description);
                return Json(new Result { Success = true, Message = "Category created successfully." });
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
                var category = await _categoryService.GetCategoryByIdAsync(id);
                return PartialView("_EditCategoryModal", category);
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
        public async Task<IActionResult> Edit(long id, string name, string description)
        {
            try
            {
                await _categoryService.UpdateCategoryAsync(id, name, description);
                return Json(new Result { Success = true, Message = "Category updated successfully." });
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

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                return PartialView("_DeleteCategoryModal", category);
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
                await _categoryService.DeleteCategoryAsync(id);
                return Json(new Result { Success = true, Message = "Category delete successfully" });
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