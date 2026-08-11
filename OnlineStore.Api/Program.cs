
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using OnlineStore.Api.Middlewares;
using OnlineStore.Api.Services;
using OnlineStore.Application.Handlers.Category;
using OnlineStore.Application.Handlers.Customer;
using OnlineStore.Application.Handlers.Order;
using OnlineStore.Application.Handlers.Product;
using OnlineStore.Application.Handlers.User;
using OnlineStore.Application.Security;
using OnlineStore.Domain.Enums;
using OnlineStore.Infrastructure;
using OnlineStore.Infrastructure.Authorization;
using Serilog;

namespace OnlineStore.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter: Bearer {your token}"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddInfrastructure();

            builder.Services.AddScoped<LoginHandler>();

            builder.Services.AddScoped<GetUserHandler>();

            builder.Services.AddScoped<CreateProductHandler>();

            builder.Services.AddScoped<FileUrlGenerator>();

            builder.Services.AddScoped<GetProductHandler>();

            builder.Services.AddScoped<GetCategoryHandler>();

            builder.Services.AddScoped<GetProductsHandler>();

            builder.Services.AddScoped<GetCategoriesHandler>();

            builder.Services.AddScoped<CreateOrderHandler>();

            builder.Services.AddScoped<GetOrderHandler>();

            builder.Services.AddScoped<GetCustomerHandler>();

            builder.Services.AddScoped<GetCurrentUserHandler>();

            builder.Services.AddScoped<PayOrderHandler>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

                    ValidateIssuer = true,

                    ValidIssuer = builder.Configuration["Jwt:Issuer"],

                    ValidateAudience = true,

                    ValidAudience = builder.Configuration["Jwt:Audience"],

                    ValidateLifetime = true,

                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        Console.WriteLine($"Authorization: {context.Request.Headers.Authorization}");
                        return Task.CompletedTask;
                    },

                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine(context.Exception);
                        return Task.CompletedTask;
                    },

                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token Valid");
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new ActiveUserRequirement())
                    .Build();

                foreach (string permission in Permissions.GetAll())
                {
                    options.AddPolicy(permission, policy =>
                    {
                        policy.RequireAuthenticatedUser();

                        policy.AddRequirements(
                            new ActiveUserRequirement(),
                            new PermissionRequirement(permission));
                    });
                }

                foreach (RoleType role in Enum.GetValues<RoleType>())
                {
                    options.AddPolicy(role.ToString(), policy =>
                    {
                        policy.RequireAuthenticatedUser();
                        policy.RequireRole(role.ToString());

                        policy.AddRequirements(
                            new ActiveUserRequirement());
                    });
                }
            });

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

            builder.Host.UseSerilog();

            var app = builder.Build();

            app.UseGlobalExceptionHandling();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
