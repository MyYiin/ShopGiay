using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopGiay.Data;
using ShopGiay.Models;

public class LoaiGiayViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public LoaiGiayViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var loais = await _context.Loaigiays.ToListAsync();
        return View(loais);
    }
}
