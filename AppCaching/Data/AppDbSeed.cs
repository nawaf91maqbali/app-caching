using AppCaching.Models;

namespace AppCaching.Data
{
    public class AppDbSeed
    {
        public AppDbSeed(IServiceProvider service)
        {
            using(var scope = service.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                SeedData(_context);
            }
        }
        private void SeedData(AppDbContext _context)
        {
            if(_context.Users.Any())
                return; // Database has been seeded

            var faker = new Bogus.Faker<User>()
                .RuleFor(u => u.Name, f => f.Name.FullName())
                .RuleFor(u => u.Email, f => f.Internet.Email());

            var users = faker.Generate(1000); // Generate 1000 fake users

            _context.Users.AddRange(users);
            _context.SaveChanges();
        }
    }
}
