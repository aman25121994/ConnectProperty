using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyConnect.Data;

namespace PropertyConnect.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET /  and  GET /Home/Index?search=...
        public async Task<IActionResult> Index(string? search)
        {
            var query = _db.Properties.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(p =>
                    EF.Functions.Like(p.Title, $"%{term}%") ||
                    EF.Functions.Like(p.Location, $"%{term}%") ||
                    EF.Functions.Like(p.Description, $"%{term}%"));
            }

            var listings = await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            ViewData["Search"] = search;
            ViewData["Total"] = listings.Count;
            return View(listings);
        }

        // GET /Home/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var property = await _db.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            return View(property);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
