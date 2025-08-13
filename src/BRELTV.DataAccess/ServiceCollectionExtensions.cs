using BRELTV.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BRELTV.DataAccess
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services)
        {
            services.AddSingleton<DatabaseConnection>();
            services.AddScoped<BusinessRuleRepository>();
            services.AddScoped<CustomerProfileRepository>();
            services.AddScoped<LoanEvaluationRepository>();
            services.AddScoped<RuleApprovalRepository>();

            return services;
        }
    }
}

