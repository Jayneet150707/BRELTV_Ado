using BRELTV.Domain.Entities;
using BRELTV.Domain.Interfaces;
using BRELTV.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace BRELTV.Infrastructure.Services
{
    public class BusinessRuleEngine : IBusinessRuleEngine
    {
        private readonly IBusinessRuleRepository _repository;

        public BusinessRuleEngine(IBusinessRuleRepository repository)
        {
            _repository = repository;
        }

        public LoanEvaluationResult EvaluateLoanApplication(
            string customerProfile, 
            bool incomeProofAvailable, 
            decimal incomeProofAmount, 
            decimal floatingMoneyAfterExpensesAndEMI)
        {
            // Normalize the profile input
            customerProfile = customerProfile.Trim();

            // Get the business rule for this profile
            var rule = _repository.GetActiveRuleForProfileAsync(customerProfile).GetAwaiter().GetResult();
            
            if (rule == null)
            {
                throw new ArgumentException($"No active business rule found for profile: {customerProfile}");
            }

            // Check if income proof meets conditions
            bool meetsIncomeCondition = incomeProofAvailable &&
                                       incomeProofAmount >= rule.MinIncomeProofAmount &&
                                       floatingMoneyAfterExpensesAndEMI > rule.MinFloatingMoneyPercentage;

            decimal ltv;
            string reason;

            // Determine LTV based on income proof and conditions
            if (!incomeProofAvailable)
            {
                ltv = rule.NoIncomeProofLTV;
                reason = $"No income proof provided. Assigned base LTV for profile {customerProfile}.";
            }
            else if (meetsIncomeCondition)
            {
                ltv = rule.MaxLTVWithProof;
                reason = $"Income proof meets all conditions. Assigned maximum LTV for profile {customerProfile}.";
            }
            else
            {
                ltv = rule.NoIncomeProofLTV;
                reason = $"Income proof provided but does not meet all conditions. Assigned base LTV for profile {customerProfile}.";
            }

            // Return the evaluation result
            return new LoanEvaluationResult(ltv, rule.FIRequirement, reason);
        }
    }
}

