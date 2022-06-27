namespace API_Restaurants.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpPost("register")]
        public ActionResult RegisterUser([FromBody] RegisterUserDto dto)
        {
            _accountService.SRegisterUser(dto);
            return Ok($"Account for email: {dto.Email} created succesfully.");
        }

        [HttpPost("login")]
        public ActionResult LoginUser([FromBody] LoginUserDto dto)
        {
            var jwtToken = _accountService.SGenerateJwtToken(dto);
            return Ok(jwtToken); // Ok("Login Succesfully.)"
        }
    }
}
