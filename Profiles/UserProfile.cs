using AutoMapper;
using sampleapi.Models;

namespace sampleapi.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile() 
        { 
            CreateMap<Entities.User, Models.User>();
            CreateMap<UserCreationDto, Entities.User>();
            CreateMap<UserUpdateDto, Entities.User>();
            CreateMap<Entities.User, UserUpdateDto>();
        }
    }
}
