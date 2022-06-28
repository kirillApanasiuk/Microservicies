﻿using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Profiles
{
    public class PlatformProfile : Profile
    {
        public PlatformProfile()
        {
            // Source -> Target

            CreateMap<Platform, PlatformReadDto>();

            CreateMap<PlatformCreateDto, Platform>();

            CreateMap<PlatformReadDto, PlatformPublishedDto>();
            CreateMap<Platform, GrpcPlatformModel>()
            .ForMember(dest => dest.PlatformId,
                        opt => opt.MapFrom(source => source.Id)
                    )
             .ForMember(dest => dest.Name,
                        opt => opt.MapFrom(source => source.Name)
                    )
             .ForMember(dest => dest.Publisher,
                        opt => opt.MapFrom(source => source.Publisher)
                    );
        }
    }
}
