using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

namespace WalletPlusIncAPI.Tests.WalletTests
{
    public class WalletServiceShould
    {
        private readonly WalletService _sut;
        private IServiceProvider _serviceProvider;
        public Mock<IWalletService> mockWalletService { get; set; } = new Mock<IWalletService>();
        public Mock<ICurrencyService> mockCurrencyService { get; set; } = new Mock<ICurrencyService>();
        public Mock<IFundingService> mockFundingService { get; set; } = new Mock<IFundingService>();
        public Mock<IAppUserService> mockAppUserService { get; set; } = new Mock<IAppUserService>();
        public Mock<IWalletRepository> mockWalletRepository { get; set; } = new Mock<IWalletRepository>();
        public Mock<IHttpContextAccessor> mockHttpContext { get; set; } = new Mock<IHttpContextAccessor>();
        public Mock<IMapper> mockMapper { get; set; } = new Mock<IMapper>();
        public Mock<ITransactionService> mockTransactionService { get; set; } = new Mock<ITransactionService>();
          public Mock<ILoggerService> mockLoggerService { get; set; } = new Mock<ILoggerService>();

        public WalletServiceShould()
        {
            var store = new Mock<IUserStore<AppUser>>();
            var userManager = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider.Setup(provide => provide.GetService(typeof(IAppUserService))).Returns(mockAppUserService.Object).Verifiable();
            mockServiceProvider.Setup(provide => provide.GetService(typeof(IWalletService))).Returns(mockWalletService.Object).Verifiable();
            mockServiceProvider.Setup(provide => provide.GetService(typeof(ICurrencyService))).Returns(mockCurrencyService.Object).Verifiable();
            mockServiceProvider.Setup(provide => provide.GetService(typeof(IFundingService))).Returns(mockFundingService.Object).Verifiable();
            mockServiceProvider.Setup(provide => provide.GetService(typeof(IWalletRepository))).Returns(mockWalletRepository.Object).Verifiable();
            mockServiceProvider.Setup(provide => provide.GetService(typeof(IHttpContextAccessor))).Returns(mockHttpContext.Object).Verifiable();
            mockServiceProvider.Setup(provide => provide.GetService(typeof(IMapper))).Returns(mockMapper.Object).Verifiable();
            mockServiceProvider.Setup(provide => provide.GetService(typeof(ITransactionService))).Returns(mockTransactionService.Object).Verifiable();
            mockServiceProvider.Setup(provide => provide.GetService(typeof(ILoggerService))).Returns(mockLoggerService.Object).Verifiable();

            mockServiceProvider.Setup(injector => injector.GetService(typeof(UserManager<AppUser>)))
                .Returns(userManager.Object).Verifiable();
            _serviceProvider = mockServiceProvider.Object;
        }

        [Fact]
        public void CanWithdrawFunds_ShouldReturnTrue()
        {
            //Arrange
            var walletService = new WalletService(_serviceProvider);
            var balance = 5990;
            var amount = 2000;


            //Act
            var actual = walletService.CanWithdrawFromWallet(balance, amount);

            //Assert
            Assert.True(actual);
        }
        [Fact]
        public void CanWithdrawFunds_ShouldReturnFalse()
        {
            //Arrange
            var walletService = new WalletService(_serviceProvider);
            var balance = 990;
            var amount = 2000;


            //Act
            var actual = walletService.CanWithdrawFromWallet(balance, amount);

            //Assert
            Assert.False(actual);
        }

       

        private void MockUp(bool state)
        {

        }
    }
}
