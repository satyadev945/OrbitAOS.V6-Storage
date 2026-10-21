// =============================================================================
// Application/Common/Mappings/MappingProfile.cs
// Clean Architecture: Application Layer - AutoMapper Profile
//
// Migration Notes:
//   - Centralizes object-to-object mapping configuration
//   - Replaces manual property assignment in legacy controllers
//   - Maps between Domain Entities and Application DTOs/ViewModels
// =============================================================================

using AutoMapper;
using OrbitAOS.V6.Domain.Entities;

namespace OrbitAOS.V6.Application.Common.Mappings
{
    /// <summary>
    /// AutoMapper profile for the Application layer.
    /// Defines mappings between Domain entities and Application DTOs.
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ---------------------------------------------------------------
            // Add entity-to-DTO mappings here as domain entities are created.
            // Example:
            //   CreateMap<Product, ProductDto>().ReverseMap();
            //   CreateMap<Order, OrderDto>()
            //       .ForMember(dest => dest.CustomerName,
            //                  opt => opt.MapFrom(src => src.Customer.Name));
            // ---------------------------------------------------------------

            // BaseEntity mapping (common audit fields)
            // Specific entity mappings are added as the domain grows
        }
    }
}
