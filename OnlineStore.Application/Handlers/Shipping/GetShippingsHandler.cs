using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Common.Models;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Shipping.Mappings;
using OnlineStore.Application.Handlers.Shipping.Queries;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Application.Handlers.Shipping
{
    public sealed class GetShippingsHandler
    {
        private readonly IShippingRepository _shippingRepository;

        public GetShippingsHandler(IShippingRepository shippingRepository)
        {
            _shippingRepository = shippingRepository;
        }

        public async Task<PagedResult<ShippingDto>> ExecuteAsync(GetShippingsQuery query)
        {
            if (query.PageNumber < 1) throw new DomainException("Page number must be greater than 0.");

            if (query.PageSize < 1 || query.PageSize > 100) throw new DomainException("Page size must be between 1 and 100.");

            var result = await _shippingRepository.GetAllAsync(query);

            return new PagedResult<ShippingDto>
            {
                Items = result.Items.Select(s => s.ToDto()).ToList(),
                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }
    }
}