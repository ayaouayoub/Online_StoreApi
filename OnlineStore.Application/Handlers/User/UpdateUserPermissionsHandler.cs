using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.User.Commands;
using OnlineStore.Application.Handlers.User.Mappings;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Application.Handlers.User
{
    public sealed class UpdateUserPermissionsHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;

        public UpdateUserPermissionsHandler(IUserRepository userRepository, IPermissionRepository permissionRepository)
        {
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<UserDto> ExecuteAsync(UpdateUserPermissionsCommand command)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId) ?? throw new NotFoundException("User not found.");

            var permissionIds = command.PermissionIds?.Distinct().ToList() ?? [];

            await SetPermissionsAsync(permissionIds, user);

            await _userRepository.UpdateUserPermissionsAsync(user);

            return user.ToDto();
        }

        private async Task SetPermissionsAsync(IReadOnlyCollection<int> permissionIds, Domain.Entities.User user)
        {
            if (permissionIds.Count == 0)
            {
                user.SetPermissions([]);
                return;
            }

            var permissions = await _permissionRepository.GetByIdsAsync(permissionIds);

            if (permissions.Count != permissionIds.Count)throw new DomainException("One or more permissions do not exist.");

            user.SetPermissions(permissions);
        }
    }
}
