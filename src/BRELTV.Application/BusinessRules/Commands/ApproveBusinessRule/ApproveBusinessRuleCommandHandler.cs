using BRELTV.Application.Common.Exceptions;
using BRELTV.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BRELTV.Application.BusinessRules.Commands.ApproveBusinessRule
{
    public class ApproveBusinessRuleCommandHandler : IRequestHandler<ApproveBusinessRuleCommand>
    {
        private readonly IBusinessRuleRepository _repository;

        public ApproveBusinessRuleCommandHandler(IBusinessRuleRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(ApproveBusinessRuleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _repository.ApproveRuleAsync(request.RuleApprovalId, request.ApprovedBy);
            }
            catch (System.Collections.Generic.KeyNotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch (System.InvalidOperationException ex)
            {
                throw new ValidationException(new[] { new FluentValidation.Results.ValidationFailure("RuleApprovalId", ex.Message) });
            }
        }
    }
}

