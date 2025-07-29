using ADR_T.ProductCatalog.Core.Domain.Entities;

namespace ADR_T.ProductCatalog.Core.Domain.Interfaces;
public interface IProductRepository : IRepository<Product>
{
    // Task<IReadOnlyList<Product>> GetProductsByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
}
