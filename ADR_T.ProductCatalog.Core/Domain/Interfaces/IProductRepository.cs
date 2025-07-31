using ADR_T.ProductCatalog.Core.Domain.Entities;

namespace ADR_T.ProductCatalog.Core.Domain.Interfaces;
public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByIdWithCategoriesAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Product>> GetAllWithCategoriesAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<Product>> GetAllWithCategoriesPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<int> CountAsync(CancellationToken cancellationToken = default);
}