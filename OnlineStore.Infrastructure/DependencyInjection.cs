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
using OnlineStore.Infrastructure.Services.Storage;
using OnlineStore.Application.Interfaces.Services.Images;
using OnlineStore.Application.Interfaces.Services.Payments;
using OnlineStore.Infrastructure.Services.Payments;
using OnlineStore.Application.Authorization;

namespace OnlineStore.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddScoped<IEncryptionService, BCryptEncryptionService>();

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddScoped<ICurrentUser, CurrentUserAccessor>();

            services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();

            services.AddScoped<IAuthorizationHandler, ActiveUserAuthorizationHandler>();

            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddHttpContextAccessor();

            services.AddScoped<IImageStorageService, LocalImageStorageService>();

            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddScoped<ICustomerRepository, CustomerRepository>();

            services.AddScoped<IPaymentGateway, StripePaymentGateway>();

            services.AddScoped<IPaymentGateway, PayPalPaymentGateway>();

            services.AddScoped<IPaymentGatewayFactory, PaymentGatewayFactory>();

            services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();

            services.AddScoped<IShippingRepository, ShippingRepository>();

            services.AddScoped<IAuthorizationHandler, ActiveUserAuthorizationHandler>();

            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            services.AddScoped<IAuthorizationHandler, ShippingViewAuthorizationHandler>();

            services.AddScoped<IAuthorizationHandler, CustomerViewAuthorizationHandler>();

            services.AddScoped<IRoleRepository, RoleRepository>();

            services.AddScoped<IPermissionRepository, PermissionRepository>();

            services.AddScoped<IAuthorizationHandler, SuperAdminAuthorizationHandler>();

            services.AddScoped<IPaymentRepository, PaymentRepository>();

            services.AddScoped<IReviewRepository, ReviewRepository>();

            services.AddScoped<IAuthorizationHandler, CustomerOnlyAuthorizationHandler>();

            return services;
        }
    }
}