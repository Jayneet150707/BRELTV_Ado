using BRELTV.Domain.Interfaces;
using BRELTV.Infrastructure.BusinessRules;
using BRELTV.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BRELTV.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IBusinessRuleRepository, BusinessRuleRepository>();
            services.AddScoped<IBusinessRuleEngine, BusinessRuleEngine>();

            return services;
        }
    }
}

