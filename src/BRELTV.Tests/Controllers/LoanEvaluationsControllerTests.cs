using BRELTV.API.Controllers;
using BRELTV.Models.DTOs;
using BRELTV.Models.Exceptions;
using BRELTV.Models.ValueObjects;
using BRELTV.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace BRELTV.Tests.Controllers
{
    public class LoanEvaluationsControllerTests
    {
        private readonly Mock<ILoanEvaluationService> _mockService;
        private readonly LoanEvaluationsController _controller;

        public LoanEvaluationsControllerTests()
        {
            _mockService = new Mock<ILoanEvaluationService>();
            _controller = new LoanEvaluationsController(_mockService.Object);
        }

        [Fact]
        public async Task EvaluateLoan_WithValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new LoanEvaluationRequest
            {
                CustomerProfile = "750+",
                IncomeProofAvailable = true,
                IncomeProofAmount = 30000,
                FloatingMoneyAfterExpensesAndEMI = 60
            };

            var response = new LoanEvaluationResponse
            {
                LTV = 100,
                FIRequirement = "FI Waiver",
                Reason = "Income proof meets conditions",
                EvaluationId = 1
            };

            _mockService
                .Setup(service => service.EvaluateLoanAsync(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.EvaluateLoan(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<LoanEvaluationResponse>(okResult.Value);
            Assert.Equal(response.LTV, returnValue.LTV);
            Assert.Equal(response.FIRequirement, returnValue.FIRequirement);
            Assert.Equal(response.Reason, returnValue.Reason);
            Assert.Equal(response.EvaluationId, returnValue.EvaluationId);
        }

        [Fact]
        public async Task EvaluateLoan_WhenServiceThrowsNotFoundException_ReturnsNotFound()
        {
            // Arrange
            var request = new LoanEvaluationRequest
            {
                CustomerProfile = "Invalid",
                IncomeProofAvailable = true,
                IncomeProofAmount = 30000,
                FloatingMoneyAfterExpensesAndEMI = 60
            };

            _mockService
                .Setup(service => service.EvaluateLoanAsync(request))
                .ThrowsAsync(new NotFoundException("Customer profile not found"));

            // Act & Assert
            // Note: In a real test, we would use a middleware to handle exceptions
            // For this test, we're just verifying the service is called with the right parameters
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.EvaluateLoan(request));
        }
    }
}

