using AutoMapper;
using RCHub.Models.Entities;

namespace RCHub.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() : this("MappingProfile")
        {
        }

        protected MappingProfile(string profileName)
        : base(profileName)
        {
            CreateMap<RCHub.Pages.Account.MyTown.CreateModel, Town>().ReverseMap();
            CreateMap<RCHub.Pages.Account.MyTown.EditModel, Town>().ReverseMap();

        }
    }
}
