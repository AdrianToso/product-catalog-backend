using ADR_T.ProductCatalog.Core.Domain.Entities;

namespace ADR_T.ProductCatalog.Core.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    /// <summary>
    /// Obtiene un producto por su ID, incluyendo su categoría asociada.
    /// </summary>
    /// <param name="id">El ID del producto.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El producto encontrado o null si no existe.</returns>
    Task<Product?> GetByIdWithCategoriesAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una lista paginada de productos, incluyendo sus categorías asociadas.
    /// </summary>
    /// <param name="pageNumber">Número de página (base 1).</param>
    /// <param name="pageSize">Tamaño de la página.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Una colección de productos paginados con sus categorías.</returns>
    Task<IEnumerable<Product>> GetAllWithCategoriesPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cuenta el número total de productos.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El número total de productos.</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
  
}