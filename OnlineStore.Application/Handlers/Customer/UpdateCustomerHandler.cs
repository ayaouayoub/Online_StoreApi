using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Customer.Commands;
using OnlineStore.Application.Handlers.Customer.Mappings;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Exceptions;
using static OnlineStore.Application.Security.Permissions;

namespace OnlineStore.Application.Handlers.Customer
{
    public sealed class UpdateCustomerHandler
    {
        private readonly ICustomerRepository _customerRepository;

        public UpdateCustomerHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerDto> ExecuteAsync(UpdateCustomerCommand command)
        {
            var customerDetails = await _customerRepository.GetByIdAsync(command.CustomerId) ?? throw new NotFoundException("Customer not found.");

            var existingCustomer = await _customerRepository.GetByEmailAsync(command.Email);

            if (existingCustomer is not null && existingCustomer.Id != customerDetails.Customer.Id) throw new ConflictException("Email already exists.");

            customerDetails.Customer.ChangePhone(command.Phone);
            customerDetails.Customer.ChangeEmail(command.Email);
            customerDetails.Customer.ChangeAddress(command.Address);

            if (!await _customerRepository.UpdateAsync(customerDetails.Customer)) throw new Exception("Failed to update customer.");

            return customerDetails.ToDto();
        }
    }
}