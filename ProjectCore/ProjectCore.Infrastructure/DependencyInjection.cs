using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectCore.Application.Common.Configuration;
using ProjectCore.Application.Common.Security;
using ProjectCore.Application.Interfaces;
using ProjectCore.Domain.Interfaces.PermissionRepository;
using ProjectCore.Domain.Interfaces.UserRepository;
using ProjectCore.Domain.Interfaces.RoleRepository;
using ProjectCore.Infrastructure.Configuration;
using ProjectCore.Infrastructure.Persistence;
using ProjectCore.Infrastructure.Repositories;
using ProjectCore.Infrastructure.Security;

namespace ProjectCore.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("ProjectCore.Infrastructure")
                    ));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPermissionQueryRepository, PermissionQueryRepository>();
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            // Đăng ký configuration từ appsettings/.env
            // Đọc từ environment variables (ưu tiên) hoặc appsettings
            services.AddSingleton<IAdminSeedConfig>(sp =>
            {
                var config = new AdminSeedConfig
                {
                    UserName = Environment.GetEnvironmentVariable("ADMIN_USERNAME") 
                        ?? configuration["AdminSeed:UserName"] 
                        ?? throw new InvalidOperationException("AdminSeed:UserName is required"),
                    Email = Environment.GetEnvironmentVariable("ADMIN_EMAIL") 
                        ?? configuration["AdminSeed:Email"] 
                        ?? throw new InvalidOperationException("AdminSeed:Email is required"),
                    Password = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") 
                        ?? configuration["AdminSeed:Password"] 
                        ?? throw new InvalidOperationException("AdminSeed:Password is required"),
                    FullName = Environment.GetEnvironmentVariable("ADMIN_FULLNAME") 
                        ?? configuration["AdminSeed:FullName"]
                };
                return config;
            });

            services.AddSingleton<IAdminRoleSeedConfig>(sp =>
            {
                var config = new AdminRoleSeedConfig
                {
                    Name = Environment.GetEnvironmentVariable("ADMIN_ROLE_NAME") 
                        ?? configuration["AdminRole:Name"] 
                        ?? "ADMIN",
                    Description = Environment.GetEnvironmentVariable("ADMIN_ROLE_DESCRIPTION") 
                        ?? configuration["AdminRole:Description"]
                };
                return config;
            });

            return services;
        }
    }
}
