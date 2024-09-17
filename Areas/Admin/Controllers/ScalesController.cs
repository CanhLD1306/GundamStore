using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GundamStore.Data;
using GundamStore.Models;

namespace GundamStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ScalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Scales
        public async Task<IActionResult> Index()
        {
              return _context.Scales != null ? 
                          View(await _context.Scales.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Scales'  is null.");
        }

        // GET: Admin/Scales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Scales == null)
            {
                return NotFound();
            }

            var scale = await _context.Scales
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scale == null)
            {
                return NotFound();
            }

            return View(scale);
        }

        // GET: Admin/Scales/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Scales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Created_At,Updated_At,Created_By,Updated_By,IsDeleted")] Scale scale)
        {
            if (ModelState.IsValid)
            {
                _context.Add(scale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(scale);
        }

        // GET: Admin/Scales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Scales == null)
            {
                return NotFound();
            }

            var scale = await _context.Scales.FindAsync(id);
            if (scale == null)
            {
                return NotFound();
            }
            return View(scale);
        }

        // POST: Admin/Scales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Created_At,Updated_At,Created_By,Updated_By,IsDeleted")] Scale scale)
        {
            if (id != scale.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scale);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScaleExists(scale.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(scale);
        }

        // GET: Admin/Scales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Scales == null)
            {
                return NotFound();
            }

            var scale = await _context.Scales
                .FirstOrDefaultAsync(m => m.Id == id);
            if (scale == null)
            {
                return NotFound();
            }

            return View(scale);
        }

        // POST: Admin/Scales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Scales == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Scales'  is null.");
            }
            var scale = await _context.Scales.FindAsync(id);
            if (scale != null)
            {
                _context.Scales.Remove(scale);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScaleExists(int id)
        {
          return (_context.Scales?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
