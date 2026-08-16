using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.User.Commands;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.User
{
    public sealed class DeactivateUserHandler
    {
        private readonly IUserRepository _userRepository;

        public DeactivateUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(DeactivateUserCommand command)
        {
            Domain.Entities.User user = await _userRepository.GetByIdAsync(command.UserId) ?? throw new NotFoundException("User not found.");
            user.Deactivate();
            if (!await _userRepository.DeactivateUserAsync(command.UserId)) throw new Exception("Failed to deactivate user.");
        }
    }
}
