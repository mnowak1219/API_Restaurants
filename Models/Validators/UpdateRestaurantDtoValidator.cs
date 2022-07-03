namespace API_Restaurants.Models.Validators
{
    public class UpdateRestaurantDtoValidator : AbstractValidator<UpdateRestaurantDto>
    {
        public UpdateRestaurantDtoValidator(RestaurantDbContext dbContext)
        {
            RuleFor(f => f.Name)
                .NotEmpty()
                .MaximumLength(25);

            RuleFor(f => f.HasDelivery.ToString())
                .Matches("^([Tt][Rr][Uu][Ee]|[Ff][Aa][Ll][Ss][Ee])$");
        }
    }
}