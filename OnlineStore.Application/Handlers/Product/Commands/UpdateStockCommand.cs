namespace OnlineStore.Application.Handlers.Product.Commands
{
    public sealed record UpdateStockCommand(int ProductId, int QuantityChange);
}
