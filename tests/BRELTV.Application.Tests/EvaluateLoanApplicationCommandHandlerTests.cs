using AutoMapper;
using BRELTV.Application.Common.Mappings;
using BRELTV.Application.LoanEvaluations.Commands.EvaluateLoanApplication;
using BRELTV.Domain.Entities;
using BRELTV.Domain.Interfaces;
using BRELTV.Domain.ValueObjects;
using BRELTV.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BRELTV.Application.Tests
{
    public class EvaluateLoanApplicationCommandHandlerTests
    {
        private readonly Mock<IBusinessRuleEngine> _mockRuleEngine;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public EvaluateLoanApplicationCommandHandlerTests()
        {
            _mockRuleEngine = new Mock<IBusinessRuleEngine>();

            // Configure AutoMapper
            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = configurationProvider.CreateMapper();

            // Configure in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Handle_ShouldReturnCorrectLoanEvaluationResult()
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

            var evaluationResult = new LoanEvaluationResult(100, "FI Waiver", "Income proof meets all conditions");

            _mockRuleEngine.Setup(x => x.EvaluateLoanApplication(
                command.CustomerProfileBand,
                command.IncomeProofAvailable,
                command.IncomeProofAmount,
                command.FloatingMoneyAfterExpensesAndEMI))
                .Returns(evaluationResult);

            var handler = new EvaluateLoanApplicationCommandHandler(_mockRuleEngine.Object, _context, _mapper);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(100, result.LTV);
            Assert.Equal("FI Waiver", result.FIRequirement);
            Assert.Equal("Income proof meets all conditions", result.Reason);

            // Verify that the evaluation was saved to the database
            var savedEvaluation = await _context.LoanEvaluations.FirstOrDefaultAsync();
            Assert.NotNull(savedEvaluation);
            Assert.Equal(command.CustomerProfileBand, savedEvaluation.CustomerProfileBand);
            Assert.Equal(command.IncomeProofAvailable, savedEvaluation.IncomeProofAvailable);
            Assert.Equal(command.IncomeProofAmount, savedEvaluation.IncomeProofAmount);
            Assert.Equal(command.FloatingMoneyAfterExpensesAndEMI, savedEvaluation.FloatingMoneyAfterExpensesAndEMI);
            Assert.Equal(evaluationResult.LTV, savedEvaluation.AssignedLTV);
            Assert.Equal(evaluationResult.FIRequirement, savedEvaluation.FIRequirement);
            Assert.Equal(evaluationResult.Reason, savedEvaluation.Reason);
            Assert.Equal(command.EvaluatedBy, savedEvaluation.EvaluatedBy);
        }
    }
}

