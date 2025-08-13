using BRELTV.DataAccess.Repositories;
using BRELTV.Models.DTOs;
using BRELTV.Models.Entities;
using BRELTV.Models.Exceptions;
using BRELTV.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRELTV.Services
{
    public class BusinessRuleService : IBusinessRuleService
    {
        private readonly BusinessRuleRepository _businessRuleRepository;
        private readonly CustomerProfileRepository _customerProfileRepository;
        private readonly RuleApprovalRepository _ruleApprovalRepository;
        private readonly ILogger<BusinessRuleService> _logger;

        public BusinessRuleService(
            BusinessRuleRepository businessRuleRepository,
            CustomerProfileRepository customerProfileRepository,
            RuleApprovalRepository ruleApprovalRepository,
            ILogger<BusinessRuleService> logger)
        {
            _businessRuleRepository = businessRuleRepository ?? throw new ArgumentNullException(nameof(businessRuleRepository));
            _customerProfileRepository = customerProfileRepository ?? throw new ArgumentNullException(nameof(customerProfileRepository));
            _ruleApprovalRepository = ruleApprovalRepository ?? throw new ArgumentNullException(nameof(ruleApprovalRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<BusinessRuleDto>> GetAllBusinessRulesAsync()
        {
            var rules = await _businessRuleRepository.GetAllAsync();
            return rules.Select(MapToDto);
        }

        public async Task<BusinessRuleDto?> GetBusinessRuleByIdAsync(int id)
        {
            var rule = await _businessRuleRepository.GetByIdAsync(id);
            if (rule == null)
            {
                return null;
            }

            return MapToDto(rule);
        }

        public async Task<int> CreateBusinessRuleAsync(BusinessRuleDto businessRuleDto, string createdBy)
        {
            // Validate customer profile exists
            var profile = await _customerProfileRepository.GetByProfileBandAsync(businessRuleDto.CustomerProfileBand);
            if (profile == null)
            {
                throw new NotFoundException($"Customer profile with band '{businessRuleDto.CustomerProfileBand}' not found.");
            }

            // Create business rule
            var businessRule = new BusinessRule
            {
                CustomerProfileId = profile.Id,
                NoIncomeProofLTV = businessRuleDto.NoIncomeProofLTV,
                MaxLTVWithProof = businessRuleDto.MaxLTVWithProof,
                FIRequirement = businessRuleDto.FIRequirement,
                MinIncomeProofAmount = businessRuleDto.MinIncomeProofAmount,
                MinFloatingMoneyPercentage = businessRuleDto.MinFloatingMoneyPercentage,
                IsActive = false, // New rules are inactive until approved
                IsApproved = false,
                CreatedBy = createdBy
            };

            var ruleId = await _businessRuleRepository.CreateAsync(businessRule);

            // Create approval request
            var approval = new RuleApproval
            {
                BusinessRuleId = ruleId,
                ApprovalStatus = "Pending",
                RequestedBy = createdBy
            };

            await _ruleApprovalRepository.CreateAsync(approval);

            return ruleId;
        }

        public async Task<bool> UpdateBusinessRuleAsync(BusinessRuleDto businessRuleDto, string updatedBy)
        {
            var existingRule = await _businessRuleRepository.GetByIdAsync(businessRuleDto.Id);
            if (existingRule == null)
            {
                throw new NotFoundException($"Business rule with ID {businessRuleDto.Id} not found.");
            }

            // Update rule properties
            existingRule.NoIncomeProofLTV = businessRuleDto.NoIncomeProofLTV;
            existingRule.MaxLTVWithProof = businessRuleDto.MaxLTVWithProof;
            existingRule.FIRequirement = businessRuleDto.FIRequirement;
            existingRule.MinIncomeProofAmount = businessRuleDto.MinIncomeProofAmount;
            existingRule.MinFloatingMoneyPercentage = businessRuleDto.MinFloatingMoneyPercentage;
            existingRule.IsActive = false; // Updated rules are inactive until approved
            existingRule.IsApproved = false;
            existingRule.UpdatedBy = updatedBy;

            var updated = await _businessRuleRepository.UpdateAsync(existingRule);
            if (updated)
            {
                // Create approval request
                var approval = new RuleApproval
                {
                    BusinessRuleId = existingRule.Id,
                    ApprovalStatus = "Pending",
                    RequestedBy = updatedBy
                };

                await _ruleApprovalRepository.CreateAsync(approval);
            }

            return updated;
        }

        public async Task<bool> ApproveBusinessRuleAsync(int id, string approvedBy, string? comments = null)
        {
            var rule = await _businessRuleRepository.GetByIdAsync(id);
            if (rule == null)
            {
                throw new NotFoundException($"Business rule with ID {id} not found.");
            }

            // Get pending approvals for this rule
            var pendingApprovals = (await _ruleApprovalRepository.GetByBusinessRuleIdAsync(id))
                .Where(a => a.ApprovalStatus == "Pending")
                .ToList();

            if (!pendingApprovals.Any())
            {
                throw new BusinessRuleException("No pending approval requests found for this business rule.");
            }

            // Update approval status
            var approval = pendingApprovals.First();
            await _ruleApprovalRepository.UpdateStatusAsync(approval.Id, "Approved", approvedBy, comments);

            // Update business rule
            rule.IsActive = true;
            rule.IsApproved = true;
            rule.ApprovedBy = approvedBy;
            rule.ApprovedAt = DateTime.UtcNow;

            return await _businessRuleRepository.UpdateAsync(rule);
        }

        public async Task<bool> RejectBusinessRuleAsync(int id, string rejectedBy, string? comments = null)
        {
            var rule = await _businessRuleRepository.GetByIdAsync(id);
            if (rule == null)
            {
                throw new NotFoundException($"Business rule with ID {id} not found.");
            }

            // Get pending approvals for this rule
            var pendingApprovals = (await _ruleApprovalRepository.GetByBusinessRuleIdAsync(id))
                .Where(a => a.ApprovalStatus == "Pending")
                .ToList();

            if (!pendingApprovals.Any())
            {
                throw new BusinessRuleException("No pending approval requests found for this business rule.");
            }

            // Update approval status
            var approval = pendingApprovals.First();
            await _ruleApprovalRepository.UpdateStatusAsync(approval.Id, "Rejected", rejectedBy, comments);

            return true;
        }

        public async Task<IEnumerable<BusinessRuleDto>> GetPendingApprovalsAsync()
        {
            var pendingRules = await _businessRuleRepository.GetPendingApprovalsAsync();
            return pendingRules.Select(MapToDto);
        }

        private BusinessRuleDto MapToDto(BusinessRule rule)
        {
            return new BusinessRuleDto
            {
                Id = rule.Id,
                CustomerProfileBand = "Unknown", // This would be populated from a join in a real implementation
                NoIncomeProofLTV = rule.NoIncomeProofLTV,
                MaxLTVWithProof = rule.MaxLTVWithProof,
                FIRequirement = rule.FIRequirement,
                MinIncomeProofAmount = rule.MinIncomeProofAmount,
                MinFloatingMoneyPercentage = rule.MinFloatingMoneyPercentage,
                IsActive = rule.IsActive,
                IsApproved = rule.IsApproved,
                CreatedAt = rule.CreatedAt
            };
        }
    }
}

