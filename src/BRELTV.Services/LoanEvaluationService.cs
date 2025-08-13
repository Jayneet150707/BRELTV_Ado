using BRELTV.DataAccess.Repositories;
using BRELTV.Models.DTOs;
using BRELTV.Models.Entities;
using BRELTV.Models.Exceptions;
using BRELTV.Models.ValueObjects;
using BRELTV.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BRELTV.Services
{
    public class LoanEvaluationService : ILoanEvaluationService
    {
        private readonly BusinessRuleRepository _businessRuleRepository;
        private readonly CustomerProfileRepository _customerProfileRepository;
        private readonly LoanEvaluationRepository _loanEvaluationRepository;
        private readonly ILogger<LoanEvaluationService> _logger;

        public LoanEvaluationService(
            BusinessRuleRepository businessRuleRepository,
            CustomerProfileRepository customerProfileRepository,
            LoanEvaluationRepository loanEvaluationRepository,
            ILogger<LoanEvaluationService> logger)
        {
            _businessRuleRepository = businessRuleRepository ?? throw new ArgumentNullException(nameof(businessRuleRepository));
            _customerProfileRepository = customerProfileRepository ?? throw new ArgumentNullException(nameof(customerProfileRepository));
            _loanEvaluationRepository = loanEvaluationRepository ?? throw new ArgumentNullException(nameof(loanEvaluationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LoanEvaluationResponse> EvaluateLoanAsync(LoanEvaluationRequest request)
        {
            // Evaluate loan
            var result = await GetLtvAndFiAsync(
                request.CustomerProfile,
                request.IncomeProofAvailable,
                request.IncomeProofAmount,
                request.FloatingMoneyAfterExpensesAndEMI);

            // Create evaluation record
            var evaluation = new LoanEvaluation
            {
                CustomerProfileBand = request.CustomerProfile,
                IncomeProofAvailable = request.IncomeProofAvailable,
                IncomeProofAmount = request.IncomeProofAmount,
                FloatingMoneyAfterExpensesAndEMI = request.FloatingMoneyAfterExpensesAndEMI,
                AssignedLTV = result.LTV,
                FIRequirement = result.FIRequirement,
                Reason = result.Reason,
                EvaluatedBy = request.EvaluatedBy ?? "System"
            };

            var evaluationId = await _loanEvaluationRepository.CreateAsync(evaluation);

            // Return response
            return new LoanEvaluationResponse
            {
                LTV = result.LTV,
                FIRequirement = result.FIRequirement,
                Reason = result.Reason,
                EvaluationId = evaluationId
            };
        }

        public async Task<LoanEvaluationResult> GetLtvAndFiAsync(
            string customerProfile,
            bool incomeProofAvailable,
            decimal incomeProofAmount,
            decimal floatingMoneyAfterExpensesAndEmi)
        {
            // Normalize customer profile
            var normalizedProfile = NormalizeProfileBand(customerProfile);

            // Get customer profile
            var profile = await _customerProfileRepository.GetByProfileBandAsync(normalizedProfile);
            if (profile == null)
            {
                throw new NotFoundException($"Customer profile with band '{normalizedProfile}' not found.");
            }

            // Get business rule for profile
            var rule = await _businessRuleRepository.GetByProfileBandAsync(normalizedProfile);
            if (rule == null)
            {
                throw new NotFoundException($"Business rule for profile band '{normalizedProfile}' not found.");
            }

            // Check if income proof meets conditions
            bool meetsIncomeCondition = incomeProofAvailable &&
                                       incomeProofAmount >= rule.MinIncomeProofAmount &&
                                       floatingMoneyAfterExpensesAndEmi > rule.MinFloatingMoneyPercentage;

            decimal ltv;
            string reason;

            if (!incomeProofAvailable)
            {
                ltv = rule.NoIncomeProofLTV;
                reason = $"No income proof provided. Default LTV of {ltv}% assigned for profile {normalizedProfile}.";
            }
            else if (meetsIncomeCondition)
            {
                ltv = rule.MaxLTVWithProof;
                reason = $"Income proof meets conditions. Maximum LTV of {ltv}% assigned for profile {normalizedProfile}.";
            }
            else
            {
                ltv = rule.NoIncomeProofLTV;
                reason = $"Income proof does not meet conditions. Default LTV of {ltv}% assigned for profile {normalizedProfile}.";
            }

            return new LoanEvaluationResult(ltv, rule.FIRequirement, reason);
        }

        private string NormalizeProfileBand(string profileBand)
        {
            // Trim and normalize profile band
            return profileBand.Trim().ToUpperInvariant() switch
            {
                "0" => "0",
                "-1" => "-1",
                "650-700" or "650 - 700" or "650- 700" or "650 -700" => "650-700",
                "700-750" or "700 - 750" or "700- 750" or "700 -750" => "700-750",
                "750+" or "750 +" => "750+",
                _ => profileBand.Trim()
            };
        }
    }
}

