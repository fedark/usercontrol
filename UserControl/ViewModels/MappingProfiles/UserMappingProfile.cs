using AutoMapper;
using Data.Models;

namespace UserControl.ViewModels.MappingProfiles;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserViewModel>()
            .AfterMap<UserMappingAction>();
    }
}
