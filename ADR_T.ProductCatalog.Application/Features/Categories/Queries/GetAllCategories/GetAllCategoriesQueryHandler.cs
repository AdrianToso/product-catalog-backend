using ADR_T.ProductCatalog.Application.DTOs;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace ADR_T.ProductCatalog.Application.Features.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork.CategoryRepository.ListAllAsync(cancellationToken);
        return _mapper.Map<List<CategoryDto>>(categories);
    }
}
