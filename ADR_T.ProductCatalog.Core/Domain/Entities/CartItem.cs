using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using System;

namespace ADR_T.ProductCatalog.Core.Domain.Entities
{
    public class CartItem : EntityBase
    {
        public Guid ShoppingCartId { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }

        // Navigation properties
        public Product Product { get; private set; } = null!;
        public ShoppingCart ShoppingCart { get; private set; } = null!;

        private CartItem() { }

        public CartItem(Guid shoppingCartId, Guid productId, int quantity)
        {
            if (shoppingCartId == Guid.Empty)
            {
                throw new DomainException("El CartItem debe estar asociado a un ShoppingCart.");
            }
            if (productId == Guid.Empty)
            {
                throw new DomainException("El CartItem debe estar asociado a un Product.");
            }

            ShoppingCartId = shoppingCartId;
            ProductId = productId;
            SetQuantity(quantity);
        }

        public void UpdateQuantity(int newQuantity)
        {
            SetQuantity(newQuantity);
            FechacActualizacion = DateTime.UtcNow;
        }

        private void SetQuantity(int quantity)
        {
            if (quantity <= 0)
            {
                throw new DomainException("La cantidad (Quantity) de un CartItem debe ser mayor que cero.");
            }
            Quantity = quantity;
        }
    }
}
