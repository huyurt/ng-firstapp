using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using postgre.Models;

namespace postgre.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<MyTableDomainModel, MyTableDtoModel>().ReverseMap();
        }
    }
}
