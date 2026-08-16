using System;
using System.Collections.Generic;
using System.Linq;using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.User.Commands;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.User
{
    public sealed class ActivateUserHandler
    {
        private readonly IUserRepository _userRepository;

        public ActivateUserHandler(IUserRepository userRepository) => _userRepository = userRepository;

        public async Task ExecuteAsync(ActivateUserCommand command)
        {
            Domain.Entities.User user = await _userRepository.GetByIdAsync(command.UserId) ?? throw new NotFoundException("User not found.");
            user.Activate();
            if (!await _userRepository.ActivateUserAsync(command.UserId)) throw new Exception("Failed to Activate user.");
        }
    }
}
