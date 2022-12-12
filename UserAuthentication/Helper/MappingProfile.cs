using AutoMapper;
using UserAuthentication.Dto;
using UserAuthentication.Models;

namespace UserAuthentication.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AdminDto, Admin>()
                .ForMember(src => src.PasswordHash, option => option.Ignore())
                .ForMember(src => src.PasswordSlot, option => option.Ignore())
                .ForMember(src => src.IPAddress, option => option.Ignore());
                
        }
    }
}
