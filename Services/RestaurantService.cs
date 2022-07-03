namespace API_Restaurants.Services
{
    public interface IRestaurantService
    {
        List<RestaurantDto> SGetAllRestaurants();
        PagedResult<RestaurantDto> SGetResponseFromQuery(RestaurantQuery query);
        RestaurantDto SGetOneRestaurantsById(int id);
        int SCreateRestaurant(CreateRestaurantDto dto);
        void SUpdateRestaurantById(UpdateRestaurantDto dto, int id);
        void SDeleteRestaurantById(int id);
    }
    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger, IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper;
            _authorizationService = authorizationService;
           _userContextService = userContextService;
        }

        public List<RestaurantDto> SGetAllRestaurants()
        {
            var restaurants = _dbContext
                .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .ToList();

            if (restaurants == null) return null;
            var restaurantDtos = _mapper.Map<List<RestaurantDto>>(restaurants);
            return restaurantDtos;
        }

        public PagedResult<RestaurantDto> SGetResponseFromQuery(RestaurantQuery query)
        {
            var baseQuery = _dbContext.Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .Where(r => (query.SearchPhrase == null) || (r.Name.ToLower().Contains(query.SearchPhrase.ToLower()) || r.Description.ToLower().Contains(query.SearchPhrase.ToLower())));
            
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                var columnsSelectors = new Dictionary<string, Expression<Func<Restaurant, object>>>
                {
                    { nameof(Restaurant.Name), r => r.Name },
                    { nameof(Restaurant.Description), r => r.Description},
                    { nameof(Restaurant.Category), r => r.Category},
                };

                var selectedColumn = columnsSelectors[query.SortBy];
                            
                baseQuery = query.SortDirection == SortDirection.Ascending
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }

            var restaurants = baseQuery
                .Skip(query.PageSize*(query.PageNumber -1))
                .Take(query.PageSize)
                .ToList();

            var totalItemsCount = baseQuery
                .Count();

            if (restaurants == null) return null;
            var restaurantDtos = _mapper.Map<List<RestaurantDto>>(restaurants);
            var result = new PagedResult<RestaurantDto>(restaurantDtos, totalItemsCount, query.PageSize, query.PageNumber);
            return result;
        }

        public RestaurantDto SGetOneRestaurantsById(int id)
        {
            var restaurant = _dbContext
               .Restaurants
               .Include(r => r.Address)
               .Include(r => r.Dishes)
               .FirstOrDefault<Restaurant>(r => r.Id == id);

            if (restaurant == null)
                throw new NotFoundException($"Restaurant with id: {id} not found.");
            var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);
            return restaurantDto;
        }

        public int SCreateRestaurant(CreateRestaurantDto dto)
        {           
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = _userContextService.GetUserId;

            var user = _dbContext.Users
               .FirstOrDefault(u => u.Id == restaurant.CreatedById);

            user.NumberOfCreatedRestaurants++;

            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();

            return restaurant.Id;
        }

        public void SUpdateRestaurantById(UpdateRestaurantDto dto, int id)
        {
            var restaurant = _dbContext
             .Restaurants
             .FirstOrDefault<Restaurant>(r => r.Id == id);

            if (restaurant == null)
                throw new NotFoundException($"Restaurant with id: {id} not found.");

            var authoriztionResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!authoriztionResult.Succeeded)
            {
                throw new ForbidException($"Action is forbidden. You didn't create this restaurant.");
            }
            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.HasDelivery = dto.HasDelivery;
            _dbContext.SaveChanges();
        }

        public void SDeleteRestaurantById(int id)
        {
            _logger.LogError($"Restaurant with id: {id} - DELETE action invoked.");
            var restaurant = _dbContext
              .Restaurants
              .FirstOrDefault<Restaurant>(r => r.Id == id);

            if (restaurant == null)
                throw new NotFoundException($"Restaurant with id: {id} not found.");

            var authoriztionResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authoriztionResult.Succeeded)
            {
                throw new ForbidException($"Action is forbidden.");
            }
            _dbContext.Remove(restaurant);
            _dbContext.SaveChanges();
        }
    }
}