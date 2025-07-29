namespace ADR_T.ProductCatalog.Core.Domain.Interfaces;
public interface IUnitOfWork : IDisposable
{
    IProductRepository ProductRepository { get; }
    ICategoryRepository CategoryRepository { get; }

    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
