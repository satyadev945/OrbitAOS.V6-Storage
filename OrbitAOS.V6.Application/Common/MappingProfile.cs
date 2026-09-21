using AutoMapper;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Domain.Entities;

namespace OrbitAOS.V6.Application.Common;

/// <summary>
/// AutoMapper profile for mapping between domain entities and DTOs.
/// Centralizes all mapping configuration in one place.
/// Replaces manual property-by-property mapping in legacy controllers.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // SampleEntity <-> SampleDto bidirectional mapping
        CreateMap<SampleEntity, SampleDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ReverseMap()
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
    }
}
