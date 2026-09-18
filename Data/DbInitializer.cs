using PropertyConnect.Models;

namespace PropertyConnect.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Properties.Any())
            {
                return; // already seeded
            }

            var samples = new[]
            {
                new Property
                {
                    Title = "Modern 3-Bedroom Apartment in Bole",
                    Description = "Bright, newly renovated apartment close to Bole Road with secure parking, backup generator, and 24/7 water supply. Walking distance to cafes and Edna Mall.",
                    Price = 45000,
                    Location = "Bole, Addis Ababa",
                    Bedrooms = 3,
                    Bathrooms = 2,
                    ContactName = "Sara Tesfaye",
                    ContactPhone = "+251 91 234 5678",
                    ContactEmail = "sara.t@example.com",
                    ImagePath = null
                },
                new Property
                {
                    Title = "Cozy Studio near CMC",
                    Description = "Compact, well-lit studio ideal for a single professional. Includes built-in wardrobe, kitchenette, and shared rooftop terrace.",
                    Price = 15000,
                    Location = "CMC, Addis Ababa",
                    Bedrooms = 1,
                    Bathrooms = 1,
                    ContactName = "Daniel Bekele",
                    ContactPhone = "+251 92 345 6789",
                    ContactEmail = "daniel.b@example.com",
                    ImagePath = null
                },
                new Property
                {
                    Title = "Spacious Family Villa in Ayat",
                    Description = "Detached 4-bedroom villa with private compound, garden, and two-car garage. Quiet residential neighborhood near Ayat school district.",
                    Price = 80000,
                    Location = "Ayat, Addis Ababa",
                    Bedrooms = 4,
                    Bathrooms = 3,
                    ContactName = "Meron Alemu",
                    ContactPhone = "+251 93 456 7890",
                    ContactEmail = null,
                    ImagePath = null
                }
            };

            context.Properties.AddRange(samples);
            context.SaveChanges();
        }
    }
}
