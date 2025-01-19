using AutoMapper;
using Catalog.API.DTOs.Requests;
using Catalog.API.DTOs.Responses;
using Catalog.Domain;

namespace Catalog.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Plate, PlateDto>()
                .ForMember(dest => dest.Status,
                          opt => opt.MapFrom(src => (int)src.Status))
                .ForMember(dest => dest.StatusDisplay,
                          opt => opt.MapFrom(src => GetStatusDisplay(src.Status)));

            CreateMap<CreatePlateDto, Plate>()
                .ForMember(dest => dest.Status,
                          opt => opt.MapFrom(src => PlateStatus.ForSale));
        }

        private static string GetStatusDisplay(PlateStatus status) => status switch
        {
            PlateStatus.ForSale => "For Sale",
            PlateStatus.Reserved => "Reserved",
            PlateStatus.Sold => "Sold",
            _ => "Unknown"
        };
    }
}
