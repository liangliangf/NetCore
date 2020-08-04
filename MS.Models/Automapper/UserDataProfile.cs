using AutoMapper;
using MS.Component.Jwt.UserClaim;
using MS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MS.Models.Automapper
{
    public class UserDataProfile : Profile
    {
        public UserDataProfile()
        {
            CreateMap<User,UserData>()//把TSource中的数据映射到TDestination中
                .ForMember(x=>x.Id,y=>y.MapFrom(y=>y.Id))
                //.ForMember(x=>x.Account,y=>y.MapFrom(y=>y.Account)) //相似字段自动映射
                //.ForMember(x=>x.Name,y=>y.MapFrom(y=>y.Name))
                //.ForMember(x => x.Email, y => y.MapFrom(y => y.Email))
                //.ForMember(x => x.Phone, y => y.MapFrom(y => y.Phone))
                .ForMember(x => x.RoleName, y => y.MapFrom(y => y.Role.Name))
                .ForMember(x => x.RoleDisplayName, y => y.MapFrom(y => y.Role.DisplayName))
                ;
        }
    }
}
