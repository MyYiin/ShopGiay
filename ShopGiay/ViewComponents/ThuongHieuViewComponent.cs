using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopGiay.Data;
using ShopGiay.Models;

public class ThuongHieuViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public ThuongHieuViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var brands = await _context.Thuonghieus.ToListAsync();
        return View(brands);
    }
}
