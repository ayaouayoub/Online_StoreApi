using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.User.Commands;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Application.Interfaces;
using OnlineStore.Domain.Exceptions;
using OnlineStore.Application.Handlers.User.Mappings;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Application.Handlers.User
{
    public sealed class UpdateUserHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;

        public UpdateUserHandler(IUserRepository userRepository, ICurrentUser currentUser)
        {
            _userRepository = userRepository;
            _currentUser = currentUser;
        }

        public async Task<UserDto> ExecuteAsync(UpdateUserCommand command)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId) ?? throw new NotFoundException("User not found.");

            var currentUser = _currentUser.User;

            if (user.RoleId == (int)RoleType.SuperAdmin && currentUser.RoleId != (int)RoleType.SuperAdmin)  throw new ForbiddenException("Only a SuperAdmin can modify a SuperAdmin.");

            var existingUser = await _userRepository.GetByUsernameAsync(command.Username);

            if (existingUser is not null && existingUser.Id != user.Id) throw new ConflictException("Username already exists.");

            user.ChangeUsername(command.Username);
            user.ChangeName(command.Name);

            if (!await _userRepository.UpdateUserAsync(user)) throw new Exception("Failed to deactivate user.");

            return user.ToDto();
        }
    }
}
