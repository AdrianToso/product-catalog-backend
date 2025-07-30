using AutoMapper; 
using ADR_T.ProductCatalog.Application.DTOs; 
using ADR_T.ProductCatalog.Core.Domain.Entities;

namespace ADR_T.ProductCatalog.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories));

        CreateMap<Category, CategoryDto>();
    }
}