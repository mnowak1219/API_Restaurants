namespace API_Restaurants.Services
{
    public interface IAccountService
    {
        void SRegisterUser(RegisterUserDto dto);
        string SGenerateJwtToken(LoginUserDto dto);
    }

    public class AccountService : IAccountService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;

        public AccountService(RestaurantDbContext dbContext, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
        }
        public void SRegisterUser(RegisterUserDto dto)
        {
            var newUser = new User()
            {
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                Nationality = dto.Nationality.CapitalizeFirstWord(),
                RoleId = dto.RoleId
            };

            var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
            newUser.PasswordHash = hashedPassword;

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();
        }
        public string SGenerateJwtToken(LoginUserDto dto)
        {
            var user = _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == dto.Email);
            if (user == null)
                throw new BadRequestException($"Invalid user name or password");

            var isPasswordValid = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (isPasswordValid == PasswordVerificationResult.Failed)
                throw new BadRequestException($"Invalid user name or password");

            var jwtClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, $"{user.Role.Name}")
            };
            if (!string.IsNullOrEmpty(user.DateOfBirth.ToString()))
            {
                jwtClaims.Add(
                    new Claim("DateOfBirth", $"{user.DateOfBirth.Value.ToString("yyyy-MM-dd")}")
                    );
            }
            if (!string.IsNullOrEmpty(user.Nationality))
            {
                jwtClaims.Add(
                    new Claim("Nationality", $"{user.Nationality}")
                    );
            }
            if (!string.IsNullOrEmpty(user.NumberOfCreatedRestaurants.ToString()))
            {
                jwtClaims.Add(
                    new Claim("NumberOfCreatedRestaurants", $"{user.NumberOfCreatedRestaurants.ToString()}")
                    );
            }
            var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));

            var jwtCredentials = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256);

            var jwtExpires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

            var jwtToken = new JwtSecurityToken(
                _authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                jwtClaims,
                expires: jwtExpires,
                signingCredentials: jwtCredentials);

            var jwtTokenHandler = new JwtSecurityTokenHandler();

            return jwtTokenHandler.WriteToken(jwtToken);
        }
    }
}
