using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Interfaces;
using OnlineStore.Application.Handlers.User.Mappings;

namespace OnlineStore.Application.Handlers.User
{
    public sealed class GetCurrentUserHandler
    {
        private readonly ICurrentUser _currentUser;

        public GetCurrentUserHandler(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

        public UserDto Execute()
        {
            var user = _currentUser.User ?? throw new NotFoundException("User not found.");
            return user.ToDto();
        }
    }
}
