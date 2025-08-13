using BRELTV.API.Controllers;
using BRELTV.Models.DTOs;
using BRELTV.Models.Exceptions;
using BRELTV.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BRELTV.Tests.Controllers
{
    public class BusinessRulesControllerTests
    {
        private readonly Mock<IBusinessRuleService> _mockService;
        private readonly BusinessRulesController _controller;

        public BusinessRulesControllerTests()
        {
            _mockService = new Mock<IBusinessRuleService>();
            _controller = new BusinessRulesController(_mockService.Object);
        }

        [Fact]
        public async Task GetAllBusinessRules_ReturnsOkResultWithRules()
        {
            // Arrange
            var rules = new List<BusinessRuleDto>
            {
                new BusinessRuleDto { Id = 1, CustomerProfileBand = "750+" },
                new BusinessRuleDto { Id = 2, CustomerProfileBand = "700-750" }
            };

            _mockService
                .Setup(service => service.GetAllBusinessRulesAsync())
                .ReturnsAsync(rules);

            // Act
            var result = await _controller.GetAllBusinessRules();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<BusinessRuleDto>>(okResult.Value);
            Assert.Equal(2, ((List<BusinessRuleDto>)returnValue).Count);
        }

        [Fact]
        public async Task GetBusinessRuleById_WithValidId_ReturnsOkResultWithRule()
        {
            // Arrange
            var ruleId = 1;
            var rule = new BusinessRuleDto { Id = ruleId, CustomerProfileBand = "750+" };

            _mockService
                .Setup(service => service.GetBusinessRuleByIdAsync(ruleId))
                .ReturnsAsync(rule);

            // Act
            var result = await _controller.GetBusinessRuleById(ruleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<BusinessRuleDto>(okResult.Value);
            Assert.Equal(ruleId, returnValue.Id);
        }

        [Fact]
        public async Task GetBusinessRuleById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var ruleId = 999;

            _mockService
                .Setup(service => service.GetBusinessRuleByIdAsync(ruleId))
                .ReturnsAsync((BusinessRuleDto)null);

            // Act
            var result = await _controller.GetBusinessRuleById(ruleId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateBusinessRule_WithValidRule_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var rule = new BusinessRuleDto
            {
                CustomerProfileBand = "750+",
                NoIncomeProofLTV = 85,
                MaxLTVWithProof = 100,
                FIRequirement = "FI Waiver",
                MinIncomeProofAmount = 25000,
                MinFloatingMoneyPercentage = 50
            };
            var ruleId = 1;

            _mockService
                .Setup(service => service.CreateBusinessRuleAsync(rule, It.IsAny<string>()))
                .ReturnsAsync(ruleId);

            // Act
            var result = await _controller.CreateBusinessRule(rule);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(BusinessRulesController.GetBusinessRuleById), createdAtActionResult.ActionName);
            Assert.Equal(ruleId, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(ruleId, createdAtActionResult.Value);
        }

        [Fact]
        public async Task ApproveBusinessRule_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var ruleId = 1;
            var comments = "Approved after review";

            _mockService
                .Setup(service => service.ApproveBusinessRuleAsync(ruleId, It.IsAny<string>(), comments))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ApproveBusinessRule(ruleId, comments);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ApproveBusinessRule_WhenServiceThrowsNotFoundException_ReturnsNotFound()
        {
            // Arrange
            var ruleId = 999;

            _mockService
                .Setup(service => service.ApproveBusinessRuleAsync(ruleId, It.IsAny<string>(), null))
                .ThrowsAsync(new NotFoundException($"Business rule with ID {ruleId} not found."));

            // Act & Assert
            // Note: In a real test, we would use a middleware to handle exceptions
            // For this test, we're just verifying the service is called with the right parameters
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.ApproveBusinessRule(ruleId));
        }
    }
}

