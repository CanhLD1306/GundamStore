using System.Security.Claims;
using GundamStore.Common;
using GundamStore.Data;
using GundamStore.Models;
using GundamStore.Services;
using GundamStore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace GundamStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }
        public async Task<ActionResult> Index(string searhString, int page = 1, int pagesize = 5)
        {
            var categories = await _categoryService.ListAllAsync(searhString, page, pagesize);
            ViewBag.CurrentPage = page;
            ViewBag.SearchString = searhString;
            return View(categories);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(Category category)
        {

            if (string.IsNullOrEmpty(category.Name))
            {
                ModelState.AddModelError("category", "Category name is required!");
                return View("Create");
            }

            if (ModelState.IsValid)
            {
                category.CreatedAt = DateTime.Now;
                category.UpdatedAt = DateTime.Now;
                category.IsDeleted = false;

                if (!await _categoryService.CheckCategoryAsync(category.Name))
                {
                    var result = await _categoryService.CreateCategoryAsync(category);
                    if (result > 0)
                    {
                        ModelState.AddModelError("categorySuccess", "Category added successfully.");
                    }
                    else
                    {
                        ModelState.AddModelError("category", "Failed to add category.");
                    }
                }
                else
                {
                    ModelState.AddModelError("category", "Category name already exists!");
                }
            }
            else
            {
                ModelState.AddModelError("category", "Invalid data. Please check again.");
            }
            return View("Create");
        }
        [HttpGet]
        public async Task<ActionResult> Edit(long id)
        {
            var category = await _categoryService.ViewDetailAsync(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Category category)
        {

            if (ModelState.IsValid)
            {
                category.UpdatedAt = DateTime.Now;
                var result = await _categoryService.UpdateCategoryAsync(category);
                if (result)
                {
                    ModelState.AddModelError("categorySuccess", "Category updated successfully.");
                }
                else
                {
                    ModelState.AddModelError("category", "Failed to update category.");
                }
            }
            else
            {
                ModelState.AddModelError("category", "Invalid data. Cannot update category.");
            }
            return View("Edit");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(long id, string searchString, int page = 1)
        {

            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToAction("Index", new { searchString, page });
            }

            category.UpdatedAt = DateTime.Now;
            category.IsDeleted = true;

            var result = await _categoryService.UpdateCategoryAsync(category);

            if (result)
            {
                TempData["SuccessMessage"] = "Category deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete category.";
            }

            return RedirectToAction("Index", new { searchString, page });
        }

    }
}