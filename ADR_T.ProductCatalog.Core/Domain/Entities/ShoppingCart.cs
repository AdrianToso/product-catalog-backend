using ADR_T.ProductCatalog.Core.Domain.Exceptions;

namespace ADR_T.ProductCatalog.Core.Domain.Entities;

public class ShoppingCart : EntityBase
{
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;

    private readonly List<CartItem> _items = new List<CartItem>();
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    private ShoppingCart() { }

    public ShoppingCart(Guid customerId)
    {
        if (customerId == Guid.Empty)
        {
            throw new DomainException("Un ShoppingCart debe pertenecer a un Customer.");
        }
        CustomerId = customerId;
    }

    public void AddItem(Product product, int quantity)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        if (quantity <= 0)
        {
            throw new DomainException("La cantidad debe ser mayor que cero.");
        }

        var existingItem = _items.FirstOrDefault(i => i.ProductId == product.Id);

        if (existingItem != null)
        {
            int newQuantity = existingItem.Quantity + quantity;
            if (product.StockQuantity < newQuantity)
            {
                throw new DomainException("No hay suficiente stock para la cantidad solicitada.");
            }
            existingItem.UpdateQuantity(newQuantity);
        }
        else
        {
            if (product.StockQuantity < quantity)
            {
                throw new DomainException("No hay suficiente stock para la cantidad solicitada.");
            }
            _items.Add(new CartItem(this.Id, product.Id, quantity));
        }
    }

    public void RemoveItem(Guid productId)
    {
        var itemToRemove = _items.FirstOrDefault(i => i.ProductId == productId);
        if (itemToRemove != null)
        {
            _items.Remove(itemToRemove);
        }
    }

    public void UpdateItemQuantity(Guid productId, int newQuantity, int stockAvailable)
    {
        var itemToUpdate = _items.FirstOrDefault(i => i.ProductId == productId);
        if (itemToUpdate == null)
        {
            throw new DomainException("El producto no se encuentra en el carrito.");
        }

        if (stockAvailable < newQuantity)
        {
            throw new DomainException("La cantidad solicitada excede el stock disponible.");
        }

        itemToUpdate.UpdateQuantity(newQuantity);
    }

    public void Clear()
    {
        _items.Clear();
    }
}
