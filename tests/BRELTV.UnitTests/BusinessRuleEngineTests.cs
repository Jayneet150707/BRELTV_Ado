using BRELTV.Domain.Entities;
using BRELTV.Domain.Interfaces;
using BRELTV.Infrastructure.BusinessRules;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BRELTV.UnitTests
{
    public class BusinessRuleEngineTests
    {
        private readonly Mock<IBusinessRuleRepository> _mockRepository;
        private readonly BusinessRuleEngine _ruleEngine;
        private readonly List<CustomerProfile> _profiles;
        private readonly List<BusinessRule> _rules;

        public BusinessRuleEngineTests()
        {
            _mockRepository = new Mock<IBusinessRuleRepository>();
            _ruleEngine = new BusinessRuleEngine(_mockRepository.Object);

            // Setup test data
            _profiles = new List<CustomerProfile>
            {
                new CustomerProfile { Id = 1, ProfileBand = "0", IsActive = true },
                new CustomerProfile { Id = 2, ProfileBand = "-1", IsActive = true },
                new CustomerProfile { Id = 3, ProfileBand = "650-700", IsActive = true },
                new CustomerProfile { Id = 4, ProfileBand = "700-750", IsActive = true },
                new CustomerProfile { Id = 5, ProfileBand = "750+", IsActive = true }
            };

            _rules = new List<BusinessRule>
            {
                new BusinessRule 
                { 
                    Id = 1, 
                    CustomerProfileId = 1, 
                    NoIncomeProofLTV = 75, 
                    MaxLTVWithProof = 85, 
                    FIRequirement = "FI Mandatory", 
                    MinIncomeProofAmount = 25000, 
                    MinFloatingMoneyPercentage = 50, 
                    IsActive = true, 
                    IsApproved = true 
                },
                new BusinessRule 
                { 
                    Id = 2, 
                    CustomerProfileId = 2, 
                    NoIncomeProofLTV = 75, 
                    MaxLTVWithProof = 85, 
                    FIRequirement = "FI Mandatory", 
                    MinIncomeProofAmount = 25000, 
                    MinFloatingMoneyPercentage = 50, 
                    IsActive = true, 
                    IsApproved = true 
                },
                new BusinessRule 
                { 
                    Id = 3, 
                    CustomerProfileId = 3, 
                    NoIncomeProofLTV = 75, 
                    MaxLTVWithProof = 85, 
                    FIRequirement = "FI Mandatory", 
                    MinIncomeProofAmount = 25000, 
                    MinFloatingMoneyPercentage = 50, 
                    IsActive = true, 
                    IsApproved = true 
                },
                new BusinessRule 
                { 
                    Id = 4, 
                    CustomerProfileId = 4, 
                    NoIncomeProofLTV = 80, 
                    MaxLTVWithProof = 95, 
                    FIRequirement = "FI Waiver", 
                    MinIncomeProofAmount = 25000, 
                    MinFloatingMoneyPercentage = 50, 
                    IsActive = true, 
                    IsApproved = true 
                },
                new BusinessRule 
                { 
                    Id = 5, 
                    CustomerProfileId = 5, 
                    NoIncomeProofLTV = 85, 
                    MaxLTVWithProof = 100, 
                    FIRequirement = "FI Waiver", 
                    MinIncomeProofAmount = 25000, 
                    MinFloatingMoneyPercentage = 50, 
                    IsActive = true, 
                    IsApproved = true 
                }
            };

            // Setup repository mock
            _mockRepository.Setup(r => r.GetActiveRulesAsync())
                .ReturnsAsync(_rules);

            _mockRepository.Setup(r => r.GetAllCustomerProfilesAsync())
                .ReturnsAsync(_profiles);
        }

        [Theory]
        [InlineData("0", false, 0, 0, 75, "FI Mandatory")]
        [InlineData("-1", false, 0, 0, 75, "FI Mandatory")]
        [InlineData("650-700", false, 0, 0, 75, "FI Mandatory")]
        [InlineData("700-750", false, 0, 0, 80, "FI Waiver")]
        [InlineData("750+", false, 0, 0, 85, "FI Waiver")]
        public void EvaluateLoanApplication_NoIncomeProof_ReturnsCorrectLTV(
            string profile, bool incomeProofAvailable, decimal incomeAmount, 
            decimal floatingMoney, decimal expectedLTV, string expectedFI)
        {
            // Act
            var result = _ruleEngine.EvaluateLoanApplication(profile, incomeProofAvailable, incomeAmount, floatingMoney);

            // Assert
            Assert.Equal(expectedLTV, result.LTV);
            Assert.Equal(expectedFI, result.FIRequirement);
            Assert.Contains("No income proof provided", result.Reason);
        }

        [Theory]
        [InlineData("0", true, 30000, 60, 85, "FI Mandatory")]
        [InlineData("-1", true, 30000, 60, 85, "FI Mandatory")]
        [InlineData("650-700", true, 30000, 60, 85, "FI Mandatory")]
        [InlineData("700-750", true, 30000, 60, 95, "FI Waiver")]
        [InlineData("750+", true, 30000, 60, 100, "FI Waiver")]
        public void EvaluateLoanApplication_WithIncomeProofMeetingConditions_ReturnsMaxLTV(
            string profile, bool incomeProofAvailable, decimal incomeAmount, 
            decimal floatingMoney, decimal expectedLTV, string expectedFI)
        {
            // Act
            var result = _ruleEngine.EvaluateLoanApplication(profile, incomeProofAvailable, incomeAmount, floatingMoney);

            // Assert
            Assert.Equal(expectedLTV, result.LTV);
            Assert.Equal(expectedFI, result.FIRequirement);
            Assert.Contains("Income proof meets conditions", result.Reason);
        }

        [Theory]
        [InlineData("0", true, 20000, 60, 75, "FI Mandatory")]
        [InlineData("0", true, 30000, 40, 75, "FI Mandatory")]
        [InlineData("700-750", true, 20000, 60, 80, "FI Waiver")]
        [InlineData("750+", true, 30000, 40, 85, "FI Waiver")]
        public void EvaluateLoanApplication_WithIncomeProofNotMeetingConditions_ReturnsDefaultLTV(
            string profile, bool incomeProofAvailable, decimal incomeAmount, 
            decimal floatingMoney, decimal expectedLTV, string expectedFI)
        {
            // Act
            var result = _ruleEngine.EvaluateLoanApplication(profile, incomeProofAvailable, incomeAmount, floatingMoney);

            // Assert
            Assert.Equal(expectedLTV, result.LTV);
            Assert.Equal(expectedFI, result.FIRequirement);
            Assert.Contains("Income proof does not meet conditions", result.Reason);
        }

        [Fact]
        public void EvaluateLoanApplication_UnknownProfile_ReturnsZeroLTV()
        {
            // Act
            var result = _ruleEngine.EvaluateLoanApplication("Unknown", true, 30000, 60);

            // Assert
            Assert.Equal(0, result.LTV);
            Assert.Equal("Unknown", result.FIRequirement);
            Assert.Contains("not found", result.Reason);
        }

        [Fact]
        public void EvaluateLoanApplication_NormalizesCaseAndSpacing()
        {
            // Act
            var result = _ruleEngine.EvaluateLoanApplication(" 750+ ", true, 30000, 60);

            // Assert
            Assert.Equal(100, result.LTV);
            Assert.Equal("FI Waiver", result.FIRequirement);
        }
    }
}

