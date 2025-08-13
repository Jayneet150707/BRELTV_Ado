using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BRELTV.Domain.Interfaces;
using MediatR;

namespace BRELTV.Application.CustomerProfiles.Queries.GetCustomerProfilesList
{
    public class GetCustomerProfilesListQuery : IRequest<List<CustomerProfileDto>>
    {
    }

    public class GetCustomerProfilesListQueryHandler : IRequestHandler<GetCustomerProfilesListQuery, List<CustomerProfileDto>>
    {
        private readonly IBusinessRuleRepository _repository;
        private readonly IMapper _mapper;

        public GetCustomerProfilesListQueryHandler(IBusinessRuleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<CustomerProfileDto>> Handle(GetCustomerProfilesListQuery request, CancellationToken cancellationToken)
        {
            var profiles = await _repository.GetAllCustomerProfilesAsync();
            return _mapper.Map<List<CustomerProfileDto>>(profiles);
        }
    }
}

