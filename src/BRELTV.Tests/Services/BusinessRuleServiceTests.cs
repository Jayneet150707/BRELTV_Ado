using BRELTV.DataAccess.Repositories;
using BRELTV.Models.DTOs;
using BRELTV.Models.Entities;
using BRELTV.Models.Exceptions;
using BRELTV.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BRELTV.Tests.Services
{
    public class BusinessRuleServiceTests
    {
        private readonly Mock<BusinessRuleRepository> _mockBusinessRuleRepo;
        private readonly Mock<CustomerProfileRepository> _mockCustomerProfileRepo;
        private readonly Mock<RuleApprovalRepository> _mockRuleApprovalRepo;
        private readonly Mock<ILogger<BusinessRuleService>> _mockLogger;
        private readonly BusinessRuleService _service;

        public BusinessRuleServiceTests()
        {
            _mockBusinessRuleRepo = new Mock<BusinessRuleRepository>(null, null);
            _mockCustomerProfileRepo = new Mock<CustomerProfileRepository>(null, null);
            _mockRuleApprovalRepo = new Mock<RuleApprovalRepository>(null, null);
            _mockLogger = new Mock<ILogger<BusinessRuleService>>();

            _service = new BusinessRuleService(
                _mockBusinessRuleRepo.Object,
                _mockCustomerProfileRepo.Object,
                _mockRuleApprovalRepo.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task CreateBusinessRuleAsync_WithValidData_CreatesRuleAndApprovalRequest()
        {
            // Arrange
            var profileBand = "750+";
            var profile = new CustomerProfile { Id = 1, ProfileBand = profileBand };
            var ruleDto = new BusinessRuleDto
            {
                CustomerProfileBand = profileBand,
                NoIncomeProofLTV = 85,
                MaxLTVWithProof = 100,
                FIRequirement = "FI Waiver",
                MinIncomeProofAmount = 25000,
                MinFloatingMoneyPercentage = 50
            };
            var createdBy = "TestUser";
            var ruleId = 1;

            _mockCustomerProfileRepo
                .Setup(repo => repo.GetByProfileBandAsync(profileBand))
                .ReturnsAsync(profile);

            _mockBusinessRuleRepo
                .Setup(repo => repo.CreateAsync(It.IsAny<BusinessRule>()))
                .ReturnsAsync(ruleId);

            _mockRuleApprovalRepo
                .Setup(repo => repo.CreateAsync(It.IsAny<RuleApproval>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.CreateBusinessRuleAsync(ruleDto, createdBy);

            // Assert
            Assert.Equal(ruleId, result);
            
            _mockBusinessRuleRepo.Verify(
                repo => repo.CreateAsync(It.Is<BusinessRule>(r => 
                    r.CustomerProfileId == profile.Id &&
                    r.NoIncomeProofLTV == ruleDto.NoIncomeProofLTV &&
                    r.MaxLTVWithProof == ruleDto.MaxLTVWithProof &&
                    r.FIRequirement == ruleDto.FIRequirement &&
                    r.MinIncomeProofAmount == ruleDto.MinIncomeProofAmount &&
                    r.MinFloatingMoneyPercentage == ruleDto.MinFloatingMoneyPercentage &&
                    r.IsActive == false &&
                    r.IsApproved == false &&
                    r.CreatedBy == createdBy
                )),
                Times.Once);

            _mockRuleApprovalRepo.Verify(
                repo => repo.CreateAsync(It.Is<RuleApproval>(a => 
                    a.BusinessRuleId == ruleId &&
                    a.ApprovalStatus == "Pending" &&
                    a.RequestedBy == createdBy
                )),
                Times.Once);
        }

        [Fact]
        public async Task CreateBusinessRuleAsync_WithInvalidProfile_ThrowsNotFoundException()
        {
            // Arrange
            var profileBand = "Invalid";
            var ruleDto = new BusinessRuleDto { CustomerProfileBand = profileBand };
            var createdBy = "TestUser";

            _mockCustomerProfileRepo
                .Setup(repo => repo.GetByProfileBandAsync(profileBand))
                .ReturnsAsync((CustomerProfile)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => 
                _service.CreateBusinessRuleAsync(ruleDto, createdBy));
        }

        [Fact]
        public async Task ApproveBusinessRuleAsync_WithValidData_ApprovesRuleAndUpdatesStatus()
        {
            // Arrange
            var ruleId = 1;
            var approvedBy = "TestUser";
            var rule = new BusinessRule { Id = ruleId };
            var approval = new RuleApproval { Id = 1, BusinessRuleId = ruleId, ApprovalStatus = "Pending" };

            _mockBusinessRuleRepo
                .Setup(repo => repo.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _mockRuleApprovalRepo
                .Setup(repo => repo.GetByBusinessRuleIdAsync(ruleId))
                .ReturnsAsync(new List<RuleApproval> { approval });

            _mockRuleApprovalRepo
                .Setup(repo => repo.UpdateStatusAsync(approval.Id, "Approved", approvedBy, null))
                .ReturnsAsync(true);

            _mockBusinessRuleRepo
                .Setup(repo => repo.UpdateAsync(It.IsAny<BusinessRule>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.ApproveBusinessRuleAsync(ruleId, approvedBy);

            // Assert
            Assert.True(result);
            
            _mockBusinessRuleRepo.Verify(
                repo => repo.UpdateAsync(It.Is<BusinessRule>(r => 
                    r.Id == ruleId &&
                    r.IsActive == true &&
                    r.IsApproved == true &&
                    r.ApprovedBy == approvedBy
                )),
                Times.Once);

            _mockRuleApprovalRepo.Verify(
                repo => repo.UpdateStatusAsync(approval.Id, "Approved", approvedBy, null),
                Times.Once);
        }

        [Fact]
        public async Task ApproveBusinessRuleAsync_WithInvalidRule_ThrowsNotFoundException()
        {
            // Arrange
            var ruleId = 999;
            var approvedBy = "TestUser";

            _mockBusinessRuleRepo
                .Setup(repo => repo.GetByIdAsync(ruleId))
                .ReturnsAsync((BusinessRule)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => 
                _service.ApproveBusinessRuleAsync(ruleId, approvedBy));
        }

        [Fact]
        public async Task ApproveBusinessRuleAsync_WithNoPendingApprovals_ThrowsBusinessRuleException()
        {
            // Arrange
            var ruleId = 1;
            var approvedBy = "TestUser";
            var rule = new BusinessRule { Id = ruleId };

            _mockBusinessRuleRepo
                .Setup(repo => repo.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _mockRuleApprovalRepo
                .Setup(repo => repo.GetByBusinessRuleIdAsync(ruleId))
                .ReturnsAsync(new List<RuleApproval>());

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleException>(() => 
                _service.ApproveBusinessRuleAsync(ruleId, approvedBy));
        }

        [Fact]
        public async Task GetPendingApprovalsAsync_ReturnsAllPendingRules()
        {
            // Arrange
            var pendingRules = new List<BusinessRule>
            {
                new BusinessRule { Id = 1, IsApproved = false },
                new BusinessRule { Id = 2, IsApproved = false }
            };

            _mockBusinessRuleRepo
                .Setup(repo => repo.GetPendingApprovalsAsync())
                .ReturnsAsync(pendingRules);

            // Act
            var result = await _service.GetPendingApprovalsAsync();

            // Assert
            Assert.Equal(pendingRules.Count, result.Count());
            Assert.Equal(pendingRules.Select(r => r.Id), result.Select(r => r.Id));
        }
    }
}

