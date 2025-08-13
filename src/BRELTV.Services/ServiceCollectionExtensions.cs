using BRELTV.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BRELTV.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IBusinessRuleService, BusinessRuleService>();
            services.AddScoped<ILoanEvaluationService, LoanEvaluationService>();
            services.AddScoped<ICustomerProfileService, CustomerProfileService>();

            return services;
        }
    }
}

