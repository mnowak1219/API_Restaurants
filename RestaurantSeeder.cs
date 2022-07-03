public class RestaurantSeeder
{
    private readonly RestaurantDbContext _dbContext;
    public RestaurantSeeder(RestaurantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void SeedRolesAndRestaurants()
    {
        if (_dbContext.Database.CanConnect())
        {
            var pendingMigrations = _dbContext.Database.GetPendingMigrations();
            if (pendingMigrations != null && pendingMigrations.Any())
            {
                _dbContext.Database.Migrate();
            }

            if (!_dbContext.Roles.Any())
            {
                _dbContext.Roles.AddRange(GetRoles());
                _dbContext.SaveChanges();
            }

            if (!_dbContext.Users.Any())
            {
                _dbContext.Users.AddRange(GetUsers());
                _dbContext.SaveChanges();
            }

            if (!_dbContext.Addresses.Any())
            {
                _dbContext.Addresses.AddRange(GetAddresses());
                _dbContext.SaveChanges();
            }

            if (!_dbContext.Restaurants.Any())
            {
                _dbContext.Restaurants.AddRange(GetRestaurants());
                _dbContext.SaveChanges();

                var admin = _dbContext.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.Id == 1);
                admin.NumberOfCreatedRestaurants = _dbContext.Restaurants.Where(r => r.CreatedById == 1).Count();

                var manager = _dbContext.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.Id == 2);
                manager.NumberOfCreatedRestaurants = _dbContext.Restaurants.Where(r => r.CreatedById == 2).Count();

                _dbContext.SaveChanges();
            }

            if (!_dbContext.Dishes.Any())
            {
                _dbContext.AddRange(GetDishes());
                _dbContext.SaveChanges();
            }
        }
    }
    private IEnumerable<Role> GetRoles()
    {
        var roles = new List<Role>()
        {
            new Role()
            {
                 Name = "User"
            },
            new Role()
            {
                 Name = "Manager"
            },
            new Role()
            {
                Name = "Administrator"
            }
        };
        return roles;
    }
    private IEnumerable<User> GetUsers()
    {
        var users = new List<User>()
        {
            new User()
            {
                 Email = "admin@restaurants.pl",
                 FirstName = "Jan",
                 LastName = "Kowalski",
                 DateOfBirth = DateTime.Parse("1993-12-19"),
                 Nationality = "Polish",
                 PasswordHash = "AQAAAAEAACcQAAAAEI0b/a+n1EIc9urFKSwNmotx9KXwAmBDJR/JSKQxZe/weWJ7ZGg2IMTSSjABiACGnw==", //restauranty
                 RoleId = 3,
            },
            new User()
            {
                 Email = "manager@restaurants.pl",
                 FirstName = "Tom",
                 LastName = "Cruise",
                 DateOfBirth = DateTime.Parse("1999-02-04"),
                 Nationality = "English",
                 PasswordHash = "AQAAAAEAACcQAAAAEPm9P5LoMxqiV6qvMW2uREj8qPivLGlTDxygqYnHszH/gNOFjx7W1h32gZxa61MS9g==", //restauranty
                 RoleId = 2,
            },
            new User()
            {
                 Email = "user@restaurants.pl",
                 FirstName = "Til",
                 LastName = "Schweiger",
                 DateOfBirth = DateTime.Parse("2005-07-22"),
                 Nationality = "German",
                 PasswordHash = "AQAAAAEAACcQAAAAECGCMdv/7oAOtWJ6jMaQCFKAktWtvMBf3UE9/jizxDj2J9cgZa0kkWa9CzaQWzgG6A==", //restauranty
                 RoleId = 1,
            }
        };
        return users;
    }
    private IEnumerable<Address> GetAddresses()
    {
        Randomizer.Seed = new Random(0);

        var addressGenerator = new Faker<Address>("pl")
            .RuleFor(a => a.City, f => f.Address.City())
            .RuleFor(a => a.Street, f => f.Address.StreetAddress())
            .RuleFor(a => a.PostalCode, f => f.Address.ZipCode());
        var addresses = addressGenerator.Generate(500);

        return addresses;
    }
    private IEnumerable<Restaurant> GetRestaurants()
    {
        Randomizer.Seed = new Random(0);

        var restaurantGenerator = new Faker<Restaurant>("pl")
            .RuleFor(r => r.Name, f => f.Company.CompanyName().ClampLength(1, 25))
            .RuleFor(r => r.Description, f => f.Random.String2(200, "abcde fghijkl mnopq rstu vwxyz żźćń łąśęó "))
            .RuleFor(r => r.Category, f => f.Commerce.Categories(1)[0])
            .RuleFor(r => r.HasDelivery, f => f.Random.Bool())
            .RuleFor(r => r.ContactEmail, f => f.Internet.Email())
            .RuleFor(r => r.ContactNumber, f => f.Phone.PhoneNumber("###-###-###"))
            .RuleFor(r => r.CreatedById, f => f.Random.ArrayElement(new[] { 1, 2 }))
            .RuleFor(r => r.AddressId, f => f.IndexFaker+1);

        var restaurants = restaurantGenerator.Generate(500);

        return restaurants;
    }
    private IEnumerable<Dish> GetDishes()
    {
        Randomizer.Seed = new Random(0);

        var dishGenerator = new Faker<Dish>("pl")
            .RuleFor(d => d.Name, f => f.Name.Random.String2(20, "abcde fghijkl mnopq rstu vwxyz żźćń łąśęó "))
            .RuleFor(d => d.Description, f => f.Name.Random.String2(80, "abcde fghijkl mnopq rstu vwxyz żźćń łąśęó "))
            .RuleFor(d => d.Price, f => f.Random.Decimal(1.00M, 30.00M))
            .RuleFor(d => d.RestaurantId, f => f.Random.Int(1, _dbContext.Restaurants.Count()));
        var dishes = dishGenerator.Generate(2500);

        return dishes;
    }
}