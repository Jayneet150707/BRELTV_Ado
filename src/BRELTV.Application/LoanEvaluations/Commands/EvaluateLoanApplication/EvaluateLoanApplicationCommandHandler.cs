using BRELTV.Domain.Entities;
using BRELTV.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BRELTV.Application.LoanEvaluations.Commands.EvaluateLoanApplication
{
    public class EvaluateLoanApplicationCommandHandler : IRequestHandler<EvaluateLoanApplicationCommand, LoanEvaluationResultDto>
    {
        private readonly IBusinessRuleEngine _ruleEngine;
        private readonly IBusinessRuleRepository _repository;

        public EvaluateLoanApplicationCommandHandler(
            IBusinessRuleEngine ruleEngine,
            IBusinessRuleRepository repository)
        {
            _ruleEngine = ruleEngine;
            _repository = repository;
        }

        public async Task<LoanEvaluationResultDto> Handle(EvaluateLoanApplicationCommand request, CancellationToken cancellationToken)
        {
            // Evaluate the loan application
            var result = _ruleEngine.EvaluateLoanApplication(
                request.CustomerProfile,
                request.IncomeProofAvailable,
                request.IncomeProofAmount,
                request.FloatingMoneyAfterExpensesAndEMI);

            // Log the evaluation
            var evaluation = new LoanEvaluation
            {
                CustomerProfileBand = request.CustomerProfile,
                IncomeProofAvailable = request.IncomeProofAvailable,
                IncomeProofAmount = request.IncomeProofAmount,
                FloatingMoneyAfterExpensesAndEMI = request.FloatingMoneyAfterExpensesAndEMI,
                AssignedLTV = result.LTV,
                FIRequirement = result.FIRequirement,
                Reason = result.Reason,
                EvaluatedAt = DateTime.UtcNow,
                EvaluatedBy = request.EvaluatedBy ?? "System"
            };

            await _repository.LogLoanEvaluationAsync(evaluation);

            // Return the result
            return new LoanEvaluationResultDto
            {
                LTV = result.LTV,
                FIRequirement = result.FIRequirement,
                Reason = result.Reason
            };
        }
    }
}

