    public class RestaurantMappingProfile : Profile
    {
        public RestaurantMappingProfile()
        {
            CreateMap<Restaurant, RestaurantDto>()
                .ForMember(a => a.City, c => c.MapFrom(s => s.Address.City))
                .ForMember(a => a.Street, c => c.MapFrom(s => s.Address.Street))
                .ForMember(a => a.PostalCode, c => c.MapFrom(s => s.Address.PostalCode));

            CreateMap<Dish, DishDto>();

            CreateMap<CreateRestaurantDto, Restaurant>()
                .ForMember(a => a.Address, c => c.MapFrom(dto => new Address()
                {
                    City = dto.City,
                    Street = dto.Street,
                    PostalCode = dto.PostalCode
                }));

        CreateMap<CreateDishDto, Dish>();
        }
    }