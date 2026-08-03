using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Application.Interfaces;
using OnlineStore.Application.Handlers.User.Commands;
using OnlineStore.Application.Exceptions;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Application.Handlers.User
{
    public class LoginHandler
    {
        private readonly IUserRepository _repo;

        private readonly IEncryptionService _encryption;

        private readonly IJwtTokenGenerator _jwt;

        public LoginHandler(IUserRepository repo, IEncryptionService encryption, IJwtTokenGenerator jwt)
        {
            _repo = repo;
            _encryption = encryption;
            _jwt = jwt;
        }

        public async Task<LoginResponseDto> ExecuteAsync(LoginCommand command)
        {
            var user = await _repo.GetByUsernameAsync(command.Username);

            if (user == null || !_encryption.Verify(command.Password, user.PasswordHash)) throw new UnauthorizedException("Invalid credentials.");

            if (!user.IsActive) throw new ForbiddenException("Your account is inactive.");

            return new LoginResponseDto
            {
                Token = _jwt.Generate(user),

                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    Name = user.Name,
                    RoleType = (RoleType)user.RoleId,
                    Permissions = [.. user.Permissions.Select(p => new PermissionDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Code = p.Code
                    })],
                }
            };
        }
    }
}
