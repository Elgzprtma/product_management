using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NawatechTest.Models;
using NawatechTest.Data;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace NawatechTest.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => !p.IsDeleted)
                .ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                var categories = await _context.ProductCategories
                    .Where(c => !c.IsDeleted)
                    .ToListAsync();

                ViewBag.Categories = categories;
                Console.WriteLine($"Categories loaded: {categories.Count}");

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading categories: {ex.Message}");
                ViewBag.Categories = new List<ProductCategory>();
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
        {
            try
            {
                Console.WriteLine("Create method called");
                Console.WriteLine($"CategoryId received: {product.CategoryId}");

                if (product.CategoryId > 0)
                {
                    ModelState.Remove("CategoryId");
                    ModelState.Remove("Category");
                }

                if (product.CategoryId <= 0)
                {
                    ModelState.AddModelError("CategoryId", "Please select a valid category.");
                }
                else
                {
                    var categoryExists = await _context.ProductCategories
                        .AnyAsync(c => c.Id == product.CategoryId && !c.IsDeleted);
                    if (!categoryExists)
                    {
                        ModelState.AddModelError("CategoryId", "Selected category does not exist.");
                    }
                }

                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }

                    ViewBag.Categories = await _context.ProductCategories
                        .Where(c => !c.IsDeleted)
                        .ToListAsync();
                    return View(product);
                }

                product.CreatedAt = DateTime.UtcNow;
                product.UpdatedAt = DateTime.UtcNow;
                product.IsDeleted = false;

                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "products");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    product.Image = "/uploads/products/" + fileName;
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                Console.WriteLine("Product saved successfully");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Create: {ex.Message}");
                ModelState.AddModelError("", "Terjadi kesalahan saat menyimpan produk.");

                ViewBag.Categories = await _context.ProductCategories
                    .Where(c => !c.IsDeleted)
                    .ToListAsync();
                return View(product);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || product.IsDeleted) return NotFound();

            ViewBag.Categories = await _context.ProductCategories
     .Where(c => !c.IsDeleted)
     .Select(c => new SelectListItem
     {
         Value = c.Id.ToString(),
         Text = c.Name
     })
     .ToListAsync();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product, IFormFile? imageFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = await _context.ProductCategories
    .Where(c => !c.IsDeleted)
    .Select(c => new SelectListItem
    {
        Value = c.Id.ToString(),
        Text = c.Name
    })
    .ToListAsync();

                    return View(product);
                }

                var existingProduct = await _context.Products.FindAsync(product.Id);
                if (existingProduct == null || existingProduct.IsDeleted)
                {
                    return NotFound();
                }

                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Stock = product.Stock;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.UpdatedAt = DateTime.UtcNow;

                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "products");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    existingProduct.Image = "/uploads/products/" + fileName;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Edit: {ex.Message}");
                ModelState.AddModelError("", "Terjadi kesalahan saat mengupdate produk.");

                ViewBag.Categories = await _context.ProductCategories
    .Where(c => !c.IsDeleted)
    .Select(c => new SelectListItem
    {
        Value = c.Id.ToString(),
        Text = c.Name
    })
    .ToListAsync();

                return View(product);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null) return NotFound();

                product.IsDeleted = true;
                product.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Delete: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}