using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.User.Commands;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Enums;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Application.Handlers.User
{
    public sealed class DeactivateUserHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;

        public DeactivateUserHandler(IUserRepository userRepository, ICustomerRepository customerRepository)
        {
            _userRepository = userRepository;
            _customerRepository = customerRepository;
        }

        public async Task ExecuteAsync(DeactivateUserCommand command)
        {
            Domain.Entities.User user = await _userRepository.GetByIdAsync(command.UserId) ?? throw new NotFoundException("User not found.");

            if (user.RoleId == (int)RoleType.Customer)
            {
                var details = await _customerRepository.GetByUserIdAsync(user.Id) ?? throw new NotFoundException("Customer profile not found.");
                if (await _customerRepository.HasPendingOrdersAsync(details.Customer.Id)) throw new DomainException("Cannot deactivate user who has pending orders");
            }

            user.Deactivate();

            if (!await _userRepository.DeactivateUserAsync(command.UserId)) throw new Exception("Failed to deactivate user.");
        }
    }
}
