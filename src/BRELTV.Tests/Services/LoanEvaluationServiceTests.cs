using BRELTV.DataAccess.Repositories;
using BRELTV.Models.Entities;
using BRELTV.Models.Exceptions;
using BRELTV.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace BRELTV.Tests.Services
{
    public class LoanEvaluationServiceTests
    {
        private readonly Mock<BusinessRuleRepository> _mockBusinessRuleRepo;
        private readonly Mock<CustomerProfileRepository> _mockCustomerProfileRepo;
        private readonly Mock<LoanEvaluationRepository> _mockLoanEvaluationRepo;
        private readonly Mock<ILogger<LoanEvaluationService>> _mockLogger;
        private readonly LoanEvaluationService _service;

        public LoanEvaluationServiceTests()
        {
            _mockBusinessRuleRepo = new Mock<BusinessRuleRepository>(null, null);
            _mockCustomerProfileRepo = new Mock<CustomerProfileRepository>(null, null);
            _mockLoanEvaluationRepo = new Mock<LoanEvaluationRepository>(null, null);
            _mockLogger = new Mock<ILogger<LoanEvaluationService>>();

            _service = new LoanEvaluationService(
                _mockBusinessRuleRepo.Object,
                _mockCustomerProfileRepo.Object,
                _mockLoanEvaluationRepo.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task GetLtvAndFiAsync_WithNoIncomeProof_ReturnsDefaultLTV()
        {
            // Arrange
            var profileBand = "750+";
            var profile = new CustomerProfile { Id = 1, ProfileBand = profileBand };
            var rule = new BusinessRule 
            { 
                CustomerProfileId = 1, 
                NoIncomeProofLTV = 85, 
                MaxLTVWithProof = 100, 
                FIRequirement = "FI Waiver",
                MinIncomeProofAmount = 25000,
                MinFloatingMoneyPercentage = 50
            };

            _mockCustomerProfileRepo
                .Setup(repo => repo.GetByProfileBandAsync(profileBand))
                .ReturnsAsync(profile);

            _mockBusinessRuleRepo
                .Setup(repo => repo.GetByProfileBandAsync(profileBand))
                .ReturnsAsync(rule);

            // Act
            var result = await _service.GetLtvAndFiAsync(profileBand, false, 0, 0);

            // Assert
            Assert.Equal(85, result.LTV);
            Assert.Equal("FI Waiver", result.FIRequirement);
            Assert.Contains("No income proof provided", result.Reason);
        }

        [Fact]
        public async Task GetLtvAndFiAsync_WithIncomeProofMeetingConditions_ReturnsMaxLTV()
        {
            // Arrange
            var profileBand = "750+";
            var profile = new CustomerProfile { Id = 1, ProfileBand = profileBand };
            var rule = new BusinessRule 
            { 
                CustomerProfileId = 1, 
                NoIncomeProofLTV = 85, 
                MaxLTVWithProof = 100, 
                FIRequirement = "FI Waiver",
                MinIncomeProofAmount = 25000,
                MinFloatingMoneyPercentage = 50
            };

            _mockCustomerProfileRepo
                .Setup(repo => repo.GetByProfileBandAsync(profileBand))
                .ReturnsAsync(profile);

            _mockBusinessRuleRepo
                .Setup(repo => repo.GetByProfileBandAsync(profileBand))
                .ReturnsAsync(rule);

            // Act
            var result = await _service.GetLtvAndFiAsync(profileBand, true, 30000, 60);

            // Assert
            Assert.Equal(100, result.LTV);
            Assert.Equal("FI Waiver", result.FIRequirement);
            Assert.Contains("Income proof meets conditions", result.Reason);
        }

        [Fact]
        public async Task GetLtvAndFiAsync_WithIncomeProofNotMeetingConditions_ReturnsDefaultLTV()
        {
            // Arrange
            var profileBand = "750+";
            var profile = new CustomerProfile { Id = 1, ProfileBand = profileBand };
            var rule = new BusinessRule 
            { 
                CustomerProfileId = 1, 
                NoIncomeProofLTV = 85, 
                MaxLTVWithProof = 100, 
                FIRequirement = "FI Waiver",
                MinIncomeProofAmount = 25000,
                MinFloatingMoneyPercentage = 50
            };

            _mockCustomerProfileRepo
                .Setup(repo => repo.GetByProfileBandAsync(profileBand))
                .ReturnsAsync(profile);

            _mockBusinessRuleRepo
                .Setup(repo => repo.GetByProfileBandAsync(profileBand))
                .ReturnsAsync(rule);

            // Act
            var result = await _service.GetLtvAndFiAsync(profileBand, true, 20000, 40);

            // Assert
            Assert.Equal(85, result.LTV);
            Assert.Equal("FI Waiver", result.FIRequirement);
            Assert.Contains("Income proof does not meet conditions", result.Reason);
        }

        [Fact]
        public async Task GetLtvAndFiAsync_WithInvalidProfile_ThrowsNotFoundException()
        {
            // Arrange
            var profileBand = "Invalid";

            _mockCustomerProfileRepo
                .Setup(repo => repo.GetByProfileBandAsync(profileBand))
                .ReturnsAsync((CustomerProfile)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => 
                _service.GetLtvAndFiAsync(profileBand, true, 30000, 60));
        }

        [Fact]
        public async Task GetLtvAndFiAsync_WithNoBusinessRule_ThrowsNotFoundException()
        {
            // Arrange
            var profileBand = "750+";
            var profile = new CustomerProfile { Id = 1, ProfileBand = profileBand };

            _mockCustomerProfileRepo
                .Setup(repo => repo.GetByProfileBandAsync(profileBand))
                .ReturnsAsync(profile);

            _mockBusinessRuleRepo
                .Setup(repo => repo.GetByProfileBandAsync(profileBand))
                .ReturnsAsync((BusinessRule)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => 
                _service.GetLtvAndFiAsync(profileBand, true, 30000, 60));
        }

        [Theory]
        [InlineData("750+", "750+")]
        [InlineData(" 750+ ", "750+")]
        [InlineData("750 +", "750+")]
        [InlineData("700-750", "700-750")]
        [InlineData("700 - 750", "700-750")]
        [InlineData("700- 750", "700-750")]
        [InlineData("700 -750", "700-750")]
        public async Task GetLtvAndFiAsync_NormalizesProfileBand(string input, string expected)
        {
            // Arrange
            var profile = new CustomerProfile { Id = 1, ProfileBand = expected };
            var rule = new BusinessRule 
            { 
                CustomerProfileId = 1, 
                NoIncomeProofLTV = 85, 
                MaxLTVWithProof = 100, 
                FIRequirement = "FI Waiver",
                MinIncomeProofAmount = 25000,
                MinFloatingMoneyPercentage = 50
            };

            _mockCustomerProfileRepo
                .Setup(repo => repo.GetByProfileBandAsync(expected))
                .ReturnsAsync(profile);

            _mockBusinessRuleRepo
                .Setup(repo => repo.GetByProfileBandAsync(expected))
                .ReturnsAsync(rule);

            // Act
            var result = await _service.GetLtvAndFiAsync(input, false, 0, 0);

            // Assert
            Assert.Equal(85, result.LTV);
            Assert.Equal("FI Waiver", result.FIRequirement);
        }
    }
}

