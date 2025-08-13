using BRELTV.Application.Common.Exceptions;
using BRELTV.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BRELTV.Application.BusinessRules.Commands.RejectBusinessRule
{
    public class RejectBusinessRuleCommandHandler : IRequestHandler<RejectBusinessRuleCommand>
    {
        private readonly IBusinessRuleRepository _repository;

        public RejectBusinessRuleCommandHandler(IBusinessRuleRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(RejectBusinessRuleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _repository.RejectRuleAsync(request.RuleApprovalId, request.RejectedBy, request.Comments);
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

