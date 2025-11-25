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
    public class TonkhosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TonkhosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tonkhos
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Tonkhos.Include(t => t.MaKcNavigation).Include(t => t.MaMhNavigation).Include(t => t.MaMsNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Tonkhos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tonkho = await _context.Tonkhos
                .Include(t => t.MaKcNavigation)
                .Include(t => t.MaMhNavigation)
                .Include(t => t.MaMsNavigation)
                .FirstOrDefaultAsync(m => m.MaK == id);
            if (tonkho == null)
            {
                return NotFound();
            }

            return View(tonkho);
        }

        // GET: Tonkhos/Create
        public IActionResult Create()
        {
            ViewData["MaKc"] = new SelectList(_context.Kichcos, "MaKc", "MaKc");
            ViewData["MaMh"] = new SelectList(_context.Mathangs, "MaMh", "MaMh");
            ViewData["MaMs"] = new SelectList(_context.Mausacs, "MaMs", "MaMs");
            return View();
        }

        // POST: Tonkhos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaK,MaMh,MaMs,MaKc,Sku,SoLuongTonKho,GiaGocBt,GiaBanBt")] Tonkho tonkho)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tonkho);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKc"] = new SelectList(_context.Kichcos, "MaKc", "MaKc", tonkho.MaKc);
            ViewData["MaMh"] = new SelectList(_context.Mathangs, "MaMh", "MaMh", tonkho.MaMh);
            ViewData["MaMs"] = new SelectList(_context.Mausacs, "MaMs", "MaMs", tonkho.MaMs);
            return View(tonkho);
        }

        // GET: Tonkhos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tonkho = await _context.Tonkhos.FindAsync(id);
            if (tonkho == null)
            {
                return NotFound();
            }
            ViewData["MaKc"] = new SelectList(_context.Kichcos, "MaKc", "MaKc", tonkho.MaKc);
            ViewData["MaMh"] = new SelectList(_context.Mathangs, "MaMh", "MaMh", tonkho.MaMh);
            ViewData["MaMs"] = new SelectList(_context.Mausacs, "MaMs", "MaMs", tonkho.MaMs);
            return View(tonkho);
        }

        // POST: Tonkhos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaK,MaMh,MaMs,MaKc,Sku,SoLuongTonKho,GiaGocBt,GiaBanBt")] Tonkho tonkho)
        {
            if (id != tonkho.MaK)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tonkho);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TonkhoExists(tonkho.MaK))
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
            ViewData["MaKc"] = new SelectList(_context.Kichcos, "MaKc", "MaKc", tonkho.MaKc);
            ViewData["MaMh"] = new SelectList(_context.Mathangs, "MaMh", "MaMh", tonkho.MaMh);
            ViewData["MaMs"] = new SelectList(_context.Mausacs, "MaMs", "MaMs", tonkho.MaMs);
            return View(tonkho);
        }

        // GET: Tonkhos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tonkho = await _context.Tonkhos
                .Include(t => t.MaKcNavigation)
                .Include(t => t.MaMhNavigation)
                .Include(t => t.MaMsNavigation)
                .FirstOrDefaultAsync(m => m.MaK == id);
            if (tonkho == null)
            {
                return NotFound();
            }

            return View(tonkho);
        }

        // POST: Tonkhos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tonkho = await _context.Tonkhos.FindAsync(id);
            if (tonkho != null)
            {
                _context.Tonkhos.Remove(tonkho);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TonkhoExists(int id)
        {
            return _context.Tonkhos.Any(e => e.MaK == id);
        }
    }
}
