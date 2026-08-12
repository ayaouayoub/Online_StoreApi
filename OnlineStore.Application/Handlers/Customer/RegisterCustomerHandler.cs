using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Handlers.Customer.Commands;
using OnlineStore.Application.Handlers.Customer.Models;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Application.Interfaces;
using OnlineStore.Domain.Enums;
using OnlineStore.Domain.Exceptions;
using OnlineStore.Application.Exceptions;

namespace OnlineStore.Application.Handlers.Customer
{
    public sealed class RegisterCustomerHandler
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IEncryptionService _encryptionService;

        public RegisterCustomerHandler(ICustomerRepository customerRepository, IUserRepository userRepository, IRoleRepository roleRepository, IEncryptionService encryptionService)
        {
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _encryptionService = encryptionService;
        }

        public async Task<CustomerDetails> ExecuteAsync(RegisterCustomerCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Password)) throw new DomainException("Password is required.");

            var role = await _roleRepository.GetByIdAsync((int)RoleType.Customer) ?? throw new DomainException("Customer role is not configured.");

            var existingUser = await _userRepository.GetByUsernameAsync(command.Username);

            if (existingUser is not null) throw new ConflictException("Username is already in use.");

            var existingEmail = await _customerRepository.GetByEmailAsync(command.Email);

            if (existingEmail is not null) throw new ConflictException("Email is already in use.");

            var passwordHash = _encryptionService.Hash(command.Password);

            var user = Domain.Entities.User.Create
            (
                name: command.Name.Trim(),
                username: command.Username.Trim(),
                passwordHash: passwordHash,
                role: role
            );

            var customer = Domain.Entities.Customer.Create
            (
                user: user,
                email: command.Email.Trim(),
                address: command.Address.Trim(),
                phone: command.Phone?.Trim()
            );

            return await _customerRepository.RegisterAsync(user, customer);
        }
    }
}
