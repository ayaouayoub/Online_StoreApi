using OnlineStore.Application.Common.Models;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Customer.Mappings;
using OnlineStore.Application.Handlers.Customer.Queries;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Application.Handlers.Customer
{
    public sealed class GetCustomersHandler
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomersHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<PagedResultDto<CustomerDto>> ExecuteAsync(GetCustomersQuery query)
        {
            if (query.PageNumber < 1) throw new DomainException("Page number must be greater than zero.");

            if (query.PageSize < 1 || query.PageSize > 100) throw new DomainException("Page size must be between 1 and 100.");

            var result = await _customerRepository.GetPagedAsync(query);

            return new PagedResultDto<CustomerDto>
            {
                Items = [.. result.Items.Select(x => x.ToDto())],
                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }
    }
}