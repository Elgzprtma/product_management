using Microsoft.AspNetCore.Mvc;
using NawatechTest.Models;
using Microsoft.EntityFrameworkCore;
using NawatechTest.Data;


namespace NawatechTest.Controllers
{
    public class ProductCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var categories = await _context.ProductCategories
                .Where(c => !c.IsDeleted)
                .ToListAsync();
            return View(categories);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(ProductCategory category)
        {
            if (!ModelState.IsValid) return View(category);
            _context.ProductCategories.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null || category.IsDeleted) return NotFound();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductCategory category)
        {
            if (!ModelState.IsValid) return View(category);
            category.UpdatedAt = DateTime.UtcNow;
            _context.Update(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null) return NotFound();
            category.IsDeleted = true;
            category.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
