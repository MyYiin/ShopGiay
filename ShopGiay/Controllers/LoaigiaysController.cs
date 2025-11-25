using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopGiay.Data;
using ShopGiay.Models;

namespace ShopGiay.Controllers
{
    public class LoaigiaysController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoaigiaysController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Loaigiays
        public async Task<IActionResult> Index()
        {
            return View(await _context.Loaigiays.ToListAsync());
        }

        // GET: Loaigiays/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaigiay = await _context.Loaigiays
                .FirstOrDefaultAsync(m => m.MaLg == id);
            if (loaigiay == null)
            {
                return NotFound();
            }

            return View(loaigiay);
        }

        // GET: Loaigiays/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Loaigiays/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaLg,Ten")] Loaigiay loaigiay)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loaigiay);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loaigiay);
        }

        // GET: Loaigiays/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaigiay = await _context.Loaigiays.FindAsync(id);
            if (loaigiay == null)
            {
                return NotFound();
            }
            return View(loaigiay);
        }

        // POST: Loaigiays/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaLg,Ten")] Loaigiay loaigiay)
        {
            if (id != loaigiay.MaLg)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaigiay);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaigiayExists(loaigiay.MaLg))
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
            return View(loaigiay);
        }

        // GET: Loaigiays/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaigiay = await _context.Loaigiays
                .FirstOrDefaultAsync(m => m.MaLg == id);
            if (loaigiay == null)
            {
                return NotFound();
            }

            return View(loaigiay);
        }

        // POST: Loaigiays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loaigiay = await _context.Loaigiays.FindAsync(id);
            if (loaigiay != null)
            {
                _context.Loaigiays.Remove(loaigiay);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoaigiayExists(int id)
        {
            return _context.Loaigiays.Any(e => e.MaLg == id);
        }
    }
}
