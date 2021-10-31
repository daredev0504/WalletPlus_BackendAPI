using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletPlusIncAPI.Data.DataAccess.Interfaces;
using WalletPlusIncAPI.Models.Entities;
using WalletPlusIncAPI.Services.Implementation;
using WalletPlusIncAPI.Services.Interfaces;
using Xunit;

namespace WalletPlusIncAPI.Tests.FundingTests
{
    public class FundingServiceShould
    {
        private readonly FundingService _sut;
        private IServiceProvider _serviceProvider;
        public Mock<IWalletService> mockWalletService { get; set; } = new Mock<IWalletService>();
        public Mock<IFundRepository> mockFundingRepo { get; set; } = new Mock<IFundRepository>();
         public Mock<ICurrencyService> mockCurrencyService { get; set; } = new Mock<ICurrencyService>();
        public Mock<IMapper> mockMapper { get; set; } = new Mock<IMapper>();

        public FundingServiceShould()
        {
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider.Setup(provide => provide.GetService(typeof(IWalletService))).Returns(mockWalletService.Object).Verifiable();
            mockServiceProvider.Setup(provide => provide.GetService(typeof(IFundRepository))).Returns(mockFundingRepo.Object).Verifiable();
             mockServiceProvider.Setup(provide => provide.GetService(typeof(IMapper))).Returns(mockMapper.Object).Verifiable();
               mockServiceProvider.Setup(provide => provide.GetService(typeof(ICurrencyService))).Returns(mockCurrencyService.Object).Verifiable();
             
            _serviceProvider = mockServiceProvider.Object;
        }

          [Fact]
        public void DepositFunds_ShouldReturnTrue()
        {
            //Arrange
            var fundService = new FundingService(_serviceProvider);
            var balance = 5990;
            var amount = 2000; 
       

            //Act
            var actual = fundService.WalletReachLimit(balance, amount);

            //Assert
            Assert.True(actual);
        }
           [Fact]
        public void DepositFunds_ShouldReturnFalse()
        {
            //Arrange
            var fundService = new FundingService(_serviceProvider);
            var balance = 5990;
            var amount = 2000000; 
       

            //Act
            var actual = fundService.WalletReachLimit(balance, amount);

            //Assert
            Assert.False(actual);
        }
    }
}
