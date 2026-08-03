using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Application.Interfaces.Data;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Application.Interfaces;
using OnlineStore.Infrastructure.Authentication;
using OnlineStore.Infrastructure.Persistence.Connection;
using OnlineStore.Infrastructure.Persistence.Repositories;
using OnlineStore.Infrastructure.Services.Security;
using Microsoft.AspNetCore.Authorization;
using OnlineStore.Infrastructure.Authorization;

namespace OnlineStore.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IEncryptionService, BCryptEncryptionService>();

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddScoped<ICurrentUser, CurrentUserAccessor>();

            services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();

            services.AddScoped<IAuthorizationHandler, ActiveUserAuthorizationHandler>();

            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            return services;
        }
    }
}