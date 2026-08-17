using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Shipping.Mappings;
using OnlineStore.Application.Handlers.Shipping.Queries;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.Shipping
{
    public sealed class GetShippingHander
    {
        private readonly IShippingRepository _shippingRepository;

        public GetShippingHander(IShippingRepository shippingRepository)
        {
            _shippingRepository = shippingRepository;
        }

        public async Task<ShippingDto> ExecuteAsync(GetShippingQuery query)
        {
            var shipping = await _shippingRepository.GetByIdAsync(query.ShippingId) ?? throw new NotFoundException($"Shipping {query.ShippingId} not found.");
            return shipping.ToDto();
        }
    }
}
