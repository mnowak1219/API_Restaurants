namespace API_Restaurants.Models.Validators
{
    public class RestaurantDtoValidator : AbstractValidator<RestaurantDto>
    {
        public RestaurantDtoValidator(RestaurantDbContext dbContext)
        {
            RuleFor(f => f.Name)
                .NotEmpty()
                .MaximumLength(25);

            RuleFor(f => f.City)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(f => f.Street)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}