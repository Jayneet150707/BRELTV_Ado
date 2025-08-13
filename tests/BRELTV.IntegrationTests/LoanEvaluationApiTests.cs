using BRELTV.Application.LoanEvaluations.Commands.EvaluateLoanApplication;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace BRELTV.IntegrationTests
{
    public class LoanEvaluationApiTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public LoanEvaluationApiTests(TestWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task EvaluateLoanApplication_WithValidData_ReturnsSuccessResult()
        {
            // Arrange
            var command = new EvaluateLoanApplicationCommand
            {
                CustomerProfile = "750+",
                IncomeProofAvailable = true,
                IncomeProofAmount = 30000,
                FloatingMoneyAfterExpensesAndEMI = 60,
                EvaluatedBy = "TestUser"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/LoanEvaluations", command);
            
            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<LoanEvaluationResultDto>();
            
            Assert.NotNull(result);
            Assert.Equal(100, result.LTV);
            Assert.Equal("FI Waiver", result.FIRequirement);
            Assert.NotNull(result.Reason);
        }

        [Fact]
        public async Task EvaluateLoanApplication_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var command = new EvaluateLoanApplicationCommand
            {
                CustomerProfile = "", // Invalid - empty
                IncomeProofAvailable = true,
                IncomeProofAmount = 30000,
                FloatingMoneyAfterExpensesAndEMI = 60,
                EvaluatedBy = "TestUser"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/LoanEvaluations", command);
            
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}

