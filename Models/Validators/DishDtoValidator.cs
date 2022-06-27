namespace API_Restaurants.Models.Validators
{
    public class DishDtoValidator : AbstractValidator<DishDto>
    {
        public DishDtoValidator(RestaurantDbContext dbContext)
        {
            RuleFor(f => f.Name)
                .NotEmpty();
        }
    }
}