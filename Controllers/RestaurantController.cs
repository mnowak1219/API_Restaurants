namespace API_Restaurants.Controllers
{
    [Route("api/restaurant")]
    [ApiController]
    //[Authorize]
    public class RestaurantController : ControllerBase
    {
        private IRestaurantService _restaurantService;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }
        
        [HttpGet]
        [Authorize(Policy = "NumberOfCreatedRestaurants")]
        [Authorize(Policy = "HasNationality")]
        [Authorize(Policy = "MoreThan20Years")]
        [Authorize(Roles = "Administrator")]
        public ActionResult<IEnumerable<RestaurantDto>> GetAllRestaurants()
        {
            var restaurantsDtos = _restaurantService.SGetAllRestaurants();
            return Ok(restaurantsDtos);
        }

        [HttpGet("query")]
        public ActionResult<IEnumerable<RestaurantDto>> GetResponseFromQuery([FromQuery] RestaurantQuery query)
        {
            var queryModel = _restaurantService.SGetResponseFromQuery(query);
            return Ok(queryModel);
        }

        [HttpGet("{id}")]
        public ActionResult<RestaurantDto> GetOneRestaurantsById([FromRoute] int id)
        {
            var restaurant = _restaurantService.SGetOneRestaurantsById(id);

            return Ok(restaurant);
        }

        [HttpPost]
        public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {   
            var userId = int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var id = _restaurantService.SCreateRestaurant(dto);
            return Created($"/api/restaurant/{id}", null); // Ok($"Restaurant created succesfully.");
        }

        [HttpPut("{id}")]
        public ActionResult UpdateRestaurantById([FromBody] UpdateRestaurantDto dto, [FromRoute] int id)
        {
            _restaurantService.SUpdateRestaurantById(dto, id);
            return Ok($"Restaurant with id: {id} updated succesfully.");
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            _restaurantService.SDeleteRestaurantById(id);
            return NoContent(); // Ok($"Restaurant with {id} deleted succesfully.");
        }

    }
}