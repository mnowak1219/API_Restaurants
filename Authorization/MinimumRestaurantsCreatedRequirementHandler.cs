namespace API_Restaurants.Authorization
{
    public class MinimumRestaurantsCreatedRequirementHandler : AuthorizationHandler<MinimumRestaurantsCreatedRequirement>
    {
        private readonly ILogger<MinimumRestaurantsCreatedRequirementHandler> _logger;

        public MinimumRestaurantsCreatedRequirementHandler(ILogger<MinimumRestaurantsCreatedRequirementHandler> logger)
        {
            _logger = logger;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumRestaurantsCreatedRequirement requirement)
        {
            if (context.User.Claims.ToList().Count != 0)
            {
                var userEmail = context.User.FindFirst(c => c.Type == ClaimTypes.Name).Value;
                var userNumberOfCreatedRestaurants = int.Parse((context.User.FindFirst(c => c.Type == "NumberOfCreatedRestaurants")).Value);

                _logger.LogInformation($"User {userEmail} with {userNumberOfCreatedRestaurants} created restaurants.");

                if (userNumberOfCreatedRestaurants >= requirement.MinimumRestaurantsCreated)
                {
                    _logger.LogInformation("Authorization succeded.");
                    context.Succeed(requirement);
                }
                else
                {
                    _logger.LogInformation("Authorization failed.");
                }
            }
            return Task.CompletedTask;
        }
    }
}
