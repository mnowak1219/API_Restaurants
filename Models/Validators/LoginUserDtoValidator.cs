namespace API_Restaurants.Models.Validators
{
    public class LoginUserDtoValidator : AbstractValidator<LoginUserDto>
    {
        public LoginUserDtoValidator(RestaurantDbContext dbContext)
        {
            RuleFor(f => f.Email)
                .NotEmpty();

            RuleFor(f => f.Password)
                .NotEmpty();
        }
    }
}