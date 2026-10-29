using AutoMapper;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Domain.Entities;

namespace OrbitAOS.V6.Application.Common
{
    /// <summary>
    /// AutoMapper profile for mapping between entities and DTOs
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Sample entity mappings
            CreateMap<SampleEntity, SampleDto>();
            CreateMap<CreateSampleDto, SampleEntity>();
            CreateMap<UpdateSampleDto, SampleEntity>();
        }
    }
}
