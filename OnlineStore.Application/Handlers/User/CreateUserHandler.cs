using OnlineStore.Application.Dtos;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Application.Interfaces;
using OnlineStore.Domain.Exceptions;
using System;
using OnlineStore.Application.Handlers.User.Commands;
using OnlineStore.Domain.Enums;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.User.Mappings;

namespace OnlineStore.Application.Handlers.User
{
    public class CreateUserHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IPermissionRepository _permissionRepository;


        public CreateUserHandler(IUserRepository userRepository, IEncryptionService encryptionService, IRoleRepository roleRepository, IPermissionRepository permissionRepository)
        {
            _userRepository = userRepository;
            _encryptionService = encryptionService;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<UserDto> ExecuteAsync(CreateUserCommand command)
        {
            if (await _userRepository.GetByUsernameAsync(command.Username) is not null) throw new ConflictException("Username already exists.");
            
            var role = await _roleRepository.GetByIdAsync((int)RoleType.User) ?? throw new NotFoundException("Role not found.");

            var user = CreateUserObject(command, role);

            await SetPermissionsAsync([.. command.PermissionIds.Distinct()], user);

            int newUserId = await _userRepository.CreateUserAsync(user);

            return user.ToDto(newUserId);
        }

        private Domain.Entities.User CreateUserObject(CreateUserCommand command, Domain.Entities.Role role)
        {
            return Domain.Entities.User.Create(command.Name, command.Username, _encryptionService.Hash(command.Password), role);
        }

        private async Task SetPermissionsAsync(IReadOnlyCollection<int>? permissionIds, Domain.Entities.User user)
        {
            if (permissionIds is null || permissionIds.Count == 0) return;

            var permissions = await _permissionRepository.GetByIdsAsync(permissionIds);

            if (permissions.Count != permissionIds.Count) throw new DomainException("One or more permissions do not exist.");

            user.SetPermissions(permissions);
        }
    }
}
