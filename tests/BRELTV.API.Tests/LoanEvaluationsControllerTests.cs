using BRELTV.API.Controllers;
using BRELTV.Application.LoanEvaluations.Commands.EvaluateLoanApplication;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BRELTV.API.Tests
{
    public class LoanEvaluationsControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly LoanEvaluationsController _controller;

        public LoanEvaluationsControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new LoanEvaluationsController();

            // Use reflection to set the private _mediator field
            var mediatorField = typeof(ApiControllerBase).GetField("_mediator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            mediatorField?.SetValue(_controller, _mockMediator.Object);
        }

        [Fact]
        public async Task EvaluateLoan_ReturnsCorrectResult()
        {
            // Arrange
            var command = new EvaluateLoanApplicationCommand
            {
                CustomerProfileBand = "750+",
                IncomeProofAvailable = true,
                IncomeProofAmount = 30000,
                FloatingMoneyAfterExpensesAndEMI = 60,
                EvaluatedBy = "TestUser"
            };

            var expectedResult = new LoanEvaluationResultDto
            {
                LTV = 100,
                FIRequirement = "FI Waiver",
                Reason = "Income proof meets all conditions"
            };

            _mockMediator.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.EvaluateLoan(command);

            // Assert
            var okResult = Assert.IsType<ActionResult<LoanEvaluationResultDto>>(result);
            var returnValue = Assert.IsType<LoanEvaluationResultDto>(okResult.Value);
            
            Assert.Equal(expectedResult.LTV, returnValue.LTV);
            Assert.Equal(expectedResult.FIRequirement, returnValue.FIRequirement);
            Assert.Equal(expectedResult.Reason, returnValue.Reason);
            
            _mockMediator.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

