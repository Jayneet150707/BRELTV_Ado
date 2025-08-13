using BRELTV.Domain.Entities;
using BRELTV.Domain.Interfaces;
using BRELTV.Infrastructure.Services;
using Moq;
using Xunit;

namespace BRELTV.Application.Tests
{
    public class BusinessRuleEngineTests
    {
        private readonly Mock<IBusinessRuleRepository> _mockRepository;
        private readonly BusinessRuleEngine _ruleEngine;

        public BusinessRuleEngineTests()
        {
            _mockRepository = new Mock<IBusinessRuleRepository>();
            _ruleEngine = new BusinessRuleEngine(_mockRepository.Object);
        }

        [Fact]
        public void EvaluateLoanApplication_WithNoIncomeProof_ReturnsDefaultLTV()
        {
            // Arrange
            var profileBand = "750+";
            var rule = new BusinessRule
            {
                NoIncomeProofLTV = 85,
                MaxLTVWithProof = 100,
                FIRequirement = "FI Waiver",
                MinIncomeProofAmount = 25000,
                MinFloatingMoneyPercentage = 50
            };

            _mockRepository.Setup(r => r.GetActiveRuleForProfileAsync(profileBand))
                .ReturnsAsync(rule);

            // Act
            var result = _ruleEngine.EvaluateLoanApplication(profileBand, false, 0, 0);

            // Assert
            Assert.Equal(85, result.LTV);
            Assert.Equal("FI Waiver", result.FIRequirement);
            Assert.Contains("No income proof provided", result.Reason);
        }

        [Fact]
        public void EvaluateLoanApplication_WithIncomeProofMeetingConditions_ReturnsMaxLTV()
        {
            // Arrange
            var profileBand = "750+";
            var rule = new BusinessRule
            {
                NoIncomeProofLTV = 85,
                MaxLTVWithProof = 100,
                FIRequirement = "FI Waiver",
                MinIncomeProofAmount = 25000,
                MinFloatingMoneyPercentage = 50
            };

            _mockRepository.Setup(r => r.GetActiveRuleForProfileAsync(profileBand))
                .ReturnsAsync(rule);

            // Act
            var result = _ruleEngine.EvaluateLoanApplication(profileBand, true, 30000, 60);

            // Assert
            Assert.Equal(100, result.LTV);
            Assert.Equal("FI Waiver", result.FIRequirement);
            Assert.Contains("Income proof meets all conditions", result.Reason);
        }

        [Fact]
        public void EvaluateLoanApplication_WithIncomeProofNotMeetingConditions_ReturnsDefaultLTV()
        {
            // Arrange
            var profileBand = "750+";
            var rule = new BusinessRule
            {
                NoIncomeProofLTV = 85,
                MaxLTVWithProof = 100,
                FIRequirement = "FI Waiver",
                MinIncomeProofAmount = 25000,
                MinFloatingMoneyPercentage = 50
            };

            _mockRepository.Setup(r => r.GetActiveRuleForProfileAsync(profileBand))
                .ReturnsAsync(rule);

            // Act
            var result = _ruleEngine.EvaluateLoanApplication(profileBand, true, 20000, 40);

            // Assert
            Assert.Equal(85, result.LTV);
            Assert.Equal("FI Waiver", result.FIRequirement);
            Assert.Contains("Income proof provided but does not meet all conditions", result.Reason);
        }

        [Fact]
        public void EvaluateLoanApplication_WithUnknownProfile_ReturnsZeroLTV()
        {
            // Arrange
            var profileBand = "Unknown";

            _mockRepository.Setup(r => r.GetActiveRuleForProfileAsync(profileBand))
                .ReturnsAsync((BusinessRule)null);

            // Act
            var result = _ruleEngine.EvaluateLoanApplication(profileBand, true, 30000, 60);

            // Assert
            Assert.Equal(0, result.LTV);
            Assert.Equal("Unknown", result.FIRequirement);
            Assert.Contains("No business rule found for profile band", result.Reason);
        }
    }
}

