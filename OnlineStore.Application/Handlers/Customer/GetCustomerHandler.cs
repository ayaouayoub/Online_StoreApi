using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Customer.Mappings;
using OnlineStore.Application.Handlers.Customer.Queries;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.Customer
{
    public sealed class GetCustomerHandler
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomerHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerDto> ExecuteAsync(GetCustomerQuery query)
        {
            var customerDetails = await _customerRepository.GetByIdAsync(query.CustomerId) ?? throw new NotFoundException("Customer not found.");
            return customerDetails.ToDto();
        } 
    }
}
