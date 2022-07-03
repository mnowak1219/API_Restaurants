namespace API_Restaurants.Authorization
{
    public class MinimumAgeRequirementHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        private readonly ILogger<MinimumAgeRequirementHandler> _logger;

        public MinimumAgeRequirementHandler(ILogger<MinimumAgeRequirementHandler> logger)
        {
            _logger = logger;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            if (context.User.Claims.ToList().Count != 0)
            {
                var userEmail = context.User.FindFirst(c => c.Type == ClaimTypes.Name).Value;
                var dateOfBirth = DateTime.Parse(context.User.FindFirst(c => c.Type == "DateOfBirth").Value);

                _logger.LogInformation($"User {userEmail} with date of birth: [{dateOfBirth}] ");

                if (dateOfBirth.AddYears(requirement.MinimumAge) <= DateTime.Today)
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
