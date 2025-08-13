using BRELTV.Domain.Entities;
using BRELTV.Domain.Interfaces;
using BRELTV.Domain.ValueObjects;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BRELTV.Infrastructure.BusinessRules
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
            // Normalize the customer profile input
            var normalizedProfile = customerProfile.Trim();

            // Get all active rules
            var rules = _repository.GetActiveRulesAsync().GetAwaiter().GetResult();
            
            // Get all customer profiles
            var profiles = _repository.GetAllCustomerProfilesAsync().GetAwaiter().GetResult();
            
            // Find the matching profile
            var profile = profiles.FirstOrDefault(p => p.ProfileBand.Equals(normalizedProfile, StringComparison.OrdinalIgnoreCase));
            
            if (profile == null)
            {
                return new LoanEvaluationResult(
                    0, 
                    "Unknown", 
                    $"Customer profile '{customerProfile}' not found.");
            }

            // Find the rule for this profile
            var rule = rules.FirstOrDefault(r => r.CustomerProfileId == profile.Id);
            
            if (rule == null)
            {
                return new LoanEvaluationResult(
                    0, 
                    "Unknown", 
                    $"No active rule found for customer profile '{customerProfile}'.");
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
                reason = $"No income proof provided. Default LTV of {ltv}% assigned.";
            }
            else if (meetsIncomeCondition)
            {
                ltv = rule.MaxLTVWithProof;
                reason = $"Income proof meets conditions (Amount: {incomeProofAmount}, Floating Money: {floatingMoneyAfterExpensesAndEMI}%). Maximum LTV of {ltv}% assigned.";
            }
            else
            {
                ltv = rule.NoIncomeProofLTV;
                reason = $"Income proof does not meet conditions (Amount: {incomeProofAmount}, Floating Money: {floatingMoneyAfterExpensesAndEMI}%). Default LTV of {ltv}% assigned.";
            }

            return new LoanEvaluationResult(ltv, rule.FIRequirement, reason);
        }
    }
}

