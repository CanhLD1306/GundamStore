using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GundamStore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GundamStore.Data;
using GundamStore.Models;
using GundamStore.Common;

namespace GundamStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class ScalesController : BaseController
    {
        private readonly IScaleService _scaleService;

        public ScalesController(IScaleService scaleService)
        {
            _scaleService = scaleService ?? throw new ArgumentNullException(nameof(scaleService));
        }
        public async Task<ActionResult> Index(string searhString, int page = 1, int pagesize = 5)
        {
            var scales = await _scaleService.ListAllAsync(searhString, page, pagesize);
            ViewBag.CurrentPage = page;
            ViewBag.SearchString = searhString;
            return View(scales);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(Scale scale)
        {


            if (string.IsNullOrEmpty(scale.Name))
            {
                ModelState.AddModelError("scale", "Scale name is required!");
                return View("Create");
            }

            if (ModelState.IsValid)
            {
                scale.CreatedAt = DateTime.Now;
                scale.UpdatedAt = DateTime.Now;
                scale.IsDeleted = false;

                if (!await _scaleService.CheckScaleAsync(scale.Name))
                {
                    var result = await _scaleService.InsertAsync(scale);
                    if (result > 0)
                    {
                        ModelState.AddModelError("scaleSuccess", "Scale added successfully.");
                    }
                    else
                    {
                        ModelState.AddModelError("scale", "Failed to add scale.");
                    }
                }
                else
                {
                    ModelState.AddModelError("scale", "Scale name already exists!");
                }
            }
            else
            {
                ModelState.AddModelError("scale", "Invalid data. Please check again.");
            }
            return View("Create");
        }
        [HttpGet]
        public async Task<ActionResult> Edit(long id)
        {
            var scale = await _scaleService.ViewDetailAsync(id);
            if (scale == null)
            {
                TempData["ErrorMessage"] = "Scale not found.";
                return RedirectToAction("Index");
            }
            return View(scale);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Scale scale)
        {


            if (ModelState.IsValid)
            {
                scale.UpdatedAt = DateTime.Now;
                var result = await _scaleService.UpdateAsync(scale);
                if (result)
                {
                    ModelState.AddModelError("scaleSuccess", "Scale updated successfully.");
                }
                else
                {
                    ModelState.AddModelError("scale", "Failed to update scale.");
                }
            }
            else
            {
                ModelState.AddModelError("scale", "Invalid data. Cannot update scale.");
            }
            return View("Edit");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(long id, string searchString, int page = 1)
        {

            var scale = await _scaleService.GetScaleByIdAsync(id);

            if (scale == null)
            {
                TempData["ErrorMessage"] = "Scale not found.";
                return RedirectToAction("Index", new { searchString, page });
            }

            scale.UpdatedAt = DateTime.Now;

            scale.IsDeleted = true;

            var result = await _scaleService.UpdateAsync(scale);

            if (result)
            {
                TempData["SuccessMessage"] = "Scale deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete scale.";
            }

            return RedirectToAction("Index", new { searchString, page });
        }

    }
}
