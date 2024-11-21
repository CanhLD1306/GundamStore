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

    public class ScalesController : BaseController
    {
        private readonly IScaleService _scaleService;

        public ScalesController(IScaleService scaleService)
        {
            _scaleService = scaleService ?? throw new ArgumentNullException(nameof(scaleService));
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListAllScales()
        {
            var scales = await _scaleService.ListAllScalesAsync();
            return PartialView("_ListScales", scales);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name, string description)
        {
            try
            {
                await _scaleService.InsertScaleAsync(name, description);
                return Json(new Result { Success = true, Message = "Scale created successfully." });
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
                var scale = await _scaleService.GetScaleByIdAsync(id);
                return PartialView("_EditScaleModal", scale);
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
                await _scaleService.UpdateScaleAsync(id, name, description);
                return Json(new Result { Success = true, Message = "Scale updated successfully." });
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
                var scale = await _scaleService.GetScaleByIdAsync(id);
                return PartialView("_DeleteScaleModal", scale);
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
                await _scaleService.DeleteScaleAsync(id);
                return Json(new Result { Success = true, Message = "Scale delete successfully" });
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
