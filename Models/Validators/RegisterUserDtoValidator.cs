namespace API_Restaurants.Models.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator(RestaurantDbContext dbContext)
        {
            RuleFor(f => f.Email)
                .NotEmpty()
                .EmailAddress();
            RuleFor(f => f.Email)
                .Custom((value, context) =>
                {
                    var emailInUse = dbContext.Users.Any(u => u.Email == value);
                    if (emailInUse)
                        context.AddFailure("Email", "That E-mail is already in use.");
                });

            RuleFor(f => f.Password)
                .NotEmpty()
                .MinimumLength(6);

            RuleFor(f => f.ConfirmPassword)
                .NotEmpty()
                .Equal(f => f.Password);
        }
    }
}