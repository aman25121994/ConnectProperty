using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyConnect.Data;
using PropertyConnect.Models;

namespace PropertyConnect.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PropertiesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public PropertiesController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET /Properties/Manage
        public async Task<IActionResult> Manage()
        {
            var listings = await _db.Properties
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(listings);
        }

        // GET /Properties/Create
        public IActionResult Create()
        {
            return View(new PropertyFormViewModel());
        }

        // POST /Properties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PropertyFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? imagePath = null;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var ext = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError(nameof(model.ImageFile), "Only image files (jpg, jpeg, png, gif, webp) are allowed.");
                    return View(model);
                }

                var uploadsRoot = PropertyConnect.Data.StoragePaths.UploadsDirectory;
                Directory.CreateDirectory(uploadsRoot);

                var uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(model.ImageFile.FileName)}";
                var fullPath = Path.Combine(uploadsRoot, uniqueName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                imagePath = $"/uploads/{uniqueName}";
            }

            var property = new Property
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                Location = model.Location,
                Bedrooms = model.Bedrooms,
                Bathrooms = model.Bathrooms,
                ContactName = model.ContactName,
                ContactPhone = model.ContactPhone,
                ContactEmail = model.ContactEmail,
                ImagePath = imagePath,
                CreatedAt = DateTime.Now
            };

            _db.Properties.Add(property);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"\"{property.Title}\" was published.";
            return RedirectToAction(nameof(Manage));
        }

        // POST /Properties/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var property = await _db.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(property.ImagePath))
            {
                // ImagePath is stored as "/uploads/xxxx.jpg" — the file itself
                // lives in persistent storage, not under wwwroot
                var fileName = Path.GetFileName(property.ImagePath);
                var fullPath = Path.Combine(PropertyConnect.Data.StoragePaths.UploadsDirectory, fileName);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            _db.Properties.Remove(property);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"\"{property.Title}\" was deleted.";
            return RedirectToAction(nameof(Manage));
        }
    }
}
