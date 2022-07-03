// Initialization
var builder = WebApplication.CreateBuilder();
var authenticationSettings = new AuthenticationSettings();

// Binding
builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);

// This method gets called by the runtime. Use this method to add services to the container.
builder.Services.AddSingleton(authenticationSettings);
builder.Services.AddAuthentication(option =>
{
	option.DefaultAuthenticateScheme = "Bearer";
	option.DefaultScheme = "Bearer";
	option.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(cfg =>
{
	cfg.RequireHttpsMetadata = false;
	cfg.SaveToken = true;
	cfg.TokenValidationParameters = new TokenValidationParameters()
	{
		ValidIssuer = authenticationSettings.JwtIssuer,
		ValidAudience = authenticationSettings.JwtIssuer,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))

	};
});
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality", "Polish", "English"));
	options.AddPolicy("MoreThan20Years", builder => builder.AddRequirements(new MinimumAgeRequirement(20)));
	options.AddPolicy("NumberOfCreatedRestaurants", builder => builder.AddRequirements(new MinimumRestaurantsCreatedRequirement(2)));
});
builder.Services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, MinimumRestaurantsCreatedRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
builder.Services.AddControllers().AddFluentValidation();
builder.Services.AddDbContext<RestaurantDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("RestaurantsDbConnection")));
builder.Services.AddScoped<RestaurantSeeder>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IValidator<CreateDishDto>, CreateDishDtoValidator>();
builder.Services.AddScoped<IValidator<CreateRestaurantDto>, CreateRestaurantDtoValidator>();
builder.Services.AddScoped<IValidator<DishDto>, DishDtoValidator>();
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
builder.Services.AddScoped<IValidator<RestaurantDto>, RestaurantDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateRestaurantDto>, UpdateRestaurantDtoValidator>();
builder.Services.AddScoped<IValidator<RestaurantQuery>, RestaurantQueryValidator>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<RequestTimeMiddleware>();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
	options.AddPolicy("FrontEndClient", policyBuilder =>
	{
		policyBuilder.AllowAnyMethod()
		.AllowAnyHeader()
        .WithOrigins(builder.Configuration["AllowedOrigins"]);
	});
});
builder.Host.UseNLog();
// Building application
var app = builder.Build();
var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<RestaurantSeeder>();

// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
app.UseResponseCaching();
app.UseStaticFiles(new StaticFileOptions
{
	FileProvider = new PhysicalFileProvider(
		   Path.Combine(builder.Environment.ContentRootPath, "wwwroot/Files/Public")),
	RequestPath = "/wwwroot/Files/Public"
});
app.UseCors("FrontEndClient");
seeder.SeedRolesAndRestaurants();
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeMiddleware>();
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
	options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Restaurants");
});
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
	endpoints.MapControllers();
});

// Running application
app.Run();