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
    public class ThuonghieusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThuonghieusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Thuonghieus
        public async Task<IActionResult> Index()
        {
            return View(await _context.Thuonghieus.ToListAsync());
        }

        // GET: Thuonghieus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thuonghieu = await _context.Thuonghieus
                .FirstOrDefaultAsync(m => m.MaTh == id);
            if (thuonghieu == null)
            {
                return NotFound();
            }

            return View(thuonghieu);
        }

        // GET: Thuonghieus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Thuonghieus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaTh,Ten")] Thuonghieu thuonghieu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(thuonghieu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(thuonghieu);
        }

        // GET: Thuonghieus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thuonghieu = await _context.Thuonghieus.FindAsync(id);
            if (thuonghieu == null)
            {
                return NotFound();
            }
            return View(thuonghieu);
        }

        // POST: Thuonghieus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaTh,Ten")] Thuonghieu thuonghieu)
        {
            if (id != thuonghieu.MaTh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thuonghieu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThuonghieuExists(thuonghieu.MaTh))
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
            return View(thuonghieu);
        }

        // GET: Thuonghieus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thuonghieu = await _context.Thuonghieus
                .FirstOrDefaultAsync(m => m.MaTh == id);
            if (thuonghieu == null)
            {
                return NotFound();
            }

            return View(thuonghieu);
        }

        // POST: Thuonghieus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var thuonghieu = await _context.Thuonghieus.FindAsync(id);
            if (thuonghieu != null)
            {
                _context.Thuonghieus.Remove(thuonghieu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThuonghieuExists(int id)
        {
            return _context.Thuonghieus.Any(e => e.MaTh == id);
        }
    }
}
