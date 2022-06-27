namespace API_Restaurants.Models.Validators
{
    public class RestaurantQueryValidator : AbstractValidator<RestaurantQuery>
    {
        private int[] allowedPageSizes = new[] { 5, 10, 15 };
        private string[] allowedSortByColumnNames = new[] { nameof(Restaurant.Name), nameof(Restaurant.Description), nameof(Restaurant.Category), };
        public RestaurantQueryValidator(RestaurantDbContext dbContext)
        {
            RuleFor(f => f.SearchPhrase)
                .NotEmpty();    


            RuleFor(f => f.PageSize)
                .Custom((value, context) =>
                {
                    if (!allowedPageSizes.Contains(value))
                    {
                        context.AddFailure("PageSize", $"That value is inappropriated. Try with one of these: {string.Join(',', allowedPageSizes)}");
                    }
                });

            RuleFor(f => f.PageNumber)
                .GreaterThanOrEqualTo(1);

            RuleFor(r => r.SortBy).Must(value => string.IsNullOrEmpty(value) || allowedSortByColumnNames.Contains(value))
                .WithMessage($"SortBy is optional, but if exist it must be one of these: {string.Join(',', allowedSortByColumnNames)}");
        }
    }
}
