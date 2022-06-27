namespace API_Restaurants.Models.Validators
{
    public class CreateDishDtoValidator : AbstractValidator<CreateDishDto>
    {
        public CreateDishDtoValidator(RestaurantDbContext dbContext)
        {
            RuleFor(f => f.Name)
                .NotEmpty();
        }
    }
}