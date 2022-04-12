using AutoMapper;
using Manager.Domain.Entities;
using Manager.Services.DTO;

namespace Manager.Tests.Configuration;

public static class AutoMapperConfiguration
{
    public static IMapper GetConfiguration()
    {
        var autoMapperConfig = new MapperConfiguration(c =>
        {
            c.CreateMap<User, UserDTO>().ReverseMap();
        });

        return autoMapperConfig.CreateMapper();
    }
}
