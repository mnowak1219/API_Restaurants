namespace API_Restaurants.Services
{
    public interface IDishService
    {
        List<DishDto> SGetAllRestaurantDishes(int restaurantId);
        DishDto SGetOneRestaurantDishById(int restaurantId, int dishId);
        int SCreateRestaurantDish(int restaurantId, CreateDishDto dto);
        void SDeleteAllRestaurantDishes(int restaurantId);
        void SDeleteOneRestaurantDishById(int restaurantId, int dishId);
    }

    public class DishService : IDishService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;

        private Restaurant GetRestaurantById(int restaurantId)
        {
            var restaurant = _dbContext.Restaurants
               .Include(r => r.Dishes)
               .FirstOrDefault(r => r.Id == restaurantId);
            if (restaurant == null)
                throw new NotFoundException($"Restaurant with id: {restaurantId} not found.");
            return restaurant;
        }
        private Dish GetRestaurantDishById(int restaurantId, int dishId)
        {
            var dish = _dbContext.Dishes.FirstOrDefault(d => d.Id == dishId);
            if (dish == null || dish.RestaurantId != restaurantId)
                throw new NotFoundException($"Dish with id: {dishId} not found in restaurant with id: {restaurantId}.");
            return dish;
        }

        public DishService(RestaurantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public List<DishDto> SGetAllRestaurantDishes(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            if (restaurant.Dishes == null) return null;
            var dishDtos = _mapper.Map<List<DishDto>>(restaurant.Dishes);
            return dishDtos;
        }

        public DishDto SGetOneRestaurantDishById(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dish = GetRestaurantDishById(restaurantId, dishId);
            var dishDto = _mapper.Map<DishDto>(dish);
            return dishDto;
        }

        public int SCreateRestaurantDish(int restaurantId, CreateDishDto dto)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dishEntity = _mapper.Map<Dish>(dto);

            dishEntity.RestaurantId = restaurantId;
            _dbContext.Dishes.Add(dishEntity);
            _dbContext.SaveChanges();
            return dishEntity.Id;
        }

        public void SDeleteAllRestaurantDishes(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            _dbContext.RemoveRange(restaurant.Dishes);
            _dbContext.SaveChanges();
        }
        public void SDeleteOneRestaurantDishById(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            var dish = GetRestaurantDishById(restaurantId, dishId);

            _dbContext.Remove(dish);
            _dbContext.SaveChanges();
        }
    }
}
