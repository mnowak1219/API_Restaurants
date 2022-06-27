namespace API_Restaurants.Controllers
{
    [Route("api/restaurant/{restaurantId}/dish")]
    [ApiController]

    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;

        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }

        [HttpGet]
        public ActionResult GetAllRestaurantDishes([FromRoute] int restaurantId)
        {
            var dishes = _dishService.SGetAllRestaurantDishes(restaurantId);
            return Ok(dishes);
        }

        [HttpGet("{dishId}")]
        public ActionResult GetOneRestaurantDishById([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            var dish = _dishService.SGetOneRestaurantDishById(restaurantId, dishId);
            return Ok(dish);
        }

        [HttpPost]
        public ActionResult CreateRestaurantDish([FromRoute]int restaurantId,  [FromBody]CreateDishDto dto)
        {
            var dishId = _dishService.SCreateRestaurantDish(restaurantId, dto);
            return Created($"/api/restaurant/{restaurantId}/dish/{dishId}", null); // Ok($"Dish created succesfully.");
        }

        [HttpDelete]
        public ActionResult DeleteAllRestaurantDishes([FromRoute] int restaurantId)
        {
            _dishService.SDeleteAllRestaurantDishes(restaurantId);
            return NoContent(); // Ok($"Restaurant with {id} deleted succesfully.");
        }

        [HttpDelete("{dishId}")]
        public ActionResult DeleteOneRestaurantDishById([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            _dishService.SDeleteOneRestaurantDishById(restaurantId, dishId);
            return NoContent(); // Ok($"Restaurant with {id} deleted succesfully.");
        }
    }
}
