using AutoMapper;
using ADR_T.ProductCatalog.Application.DTOs;
using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Application.Features.Products.Commands.CreateProduct; 
using ADR_T.ProductCatalog.Application.Features.Products.Commands.UpdateProduct;

namespace ADR_T.ProductCatalog.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();

        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));

        CreateMap<CreateProductCommand, Product>()
            .ForCtorParam("name", opt => opt.MapFrom(src => src.Name))
            .ForCtorParam("description", opt => opt.MapFrom(src => src.Description))
            .ForCtorParam("categoryId", opt => opt.MapFrom(src => src.CategoryId))
            .ForCtorParam("imageUrl", opt => opt.MapFrom(src => src.ImageUrl));

        CreateMap<UpdateProductCommand, Product>(); 
        
    }
}