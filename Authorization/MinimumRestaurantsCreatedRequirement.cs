namespace API_Restaurants.Authorization
{
    public class MinimumRestaurantsCreatedRequirement : IAuthorizationRequirement
    {
        public int MinimumRestaurantsCreated { get; }

        public MinimumRestaurantsCreatedRequirement(int minimumRestaurantsCreated)
        {
            MinimumRestaurantsCreated = minimumRestaurantsCreated;
        }
    }
}


