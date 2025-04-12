using AutoMapper;
using Sanayii.Core.Entities;
using Sanayii.DTOs.AccountDTO;

namespace Sanayii.MapperConfig
{
    public class mappConfig:Profile
    {
        public mappConfig()
        {
            CreateMap<Customer, RegisterDTO>().AfterMap((src,dst) =>
            {
                src.UserName = dst.Username;
            }).ReverseMap();
        }
    }
}
