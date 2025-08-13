using BRELTV.Application.LoanEvaluations.Commands.EvaluateLoanApplication;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace BRELTV.IntegrationTests
{
    public class LoanEvaluationsControllerTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public LoanEvaluationsControllerTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task EvaluateLoanApplication_WithValidData_ReturnsSuccessResult()
        {
            // Arrange
            var command = new EvaluateLoanApplicationCommand
            {
                CustomerProfileBand = "750+",
                IncomeProofAvailable = true,
                IncomeProofAmount = 30000,
                FloatingMoneyAfterExpensesAndEMI = 60,
                EvaluatedBy = "Test"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(command),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/LoanEvaluations", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<LoanEvaluationResultDto>(
                responseString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(result);
            Assert.Equal(100, result.LTV);
            Assert.Equal("FI Waiver", result.FIRequirement);
        }

        [Fact]
        public async Task EvaluateLoanApplication_WithNoIncomeProof_ReturnsDefaultLTV()
        {
            // Arrange
            var command = new EvaluateLoanApplicationCommand
            {
                CustomerProfileBand = "750+",
                IncomeProofAvailable = false,
                IncomeProofAmount = 0,
                FloatingMoneyAfterExpensesAndEMI = 0,
                EvaluatedBy = "Test"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(command),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/LoanEvaluations", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<LoanEvaluationResultDto>(
                responseString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(result);
            Assert.Equal(85, result.LTV);
            Assert.Equal("FI Waiver", result.FIRequirement);
        }

        [Fact]
        public async Task EvaluateLoanApplication_WithInvalidProfile_ReturnsBadRequest()
        {
            // Arrange
            var command = new EvaluateLoanApplicationCommand
            {
                CustomerProfileBand = "Invalid",
                IncomeProofAvailable = true,
                IncomeProofAmount = 30000,
                FloatingMoneyAfterExpensesAndEMI = 60,
                EvaluatedBy = "Test"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(command),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/LoanEvaluations", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}

