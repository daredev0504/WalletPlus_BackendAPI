using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WalletPlusIncAPI.Controllers;
using WalletPlusIncAPI.Data.DataAccess.Interfaces;
using WalletPlusIncAPI.Helpers;
using WalletPlusIncAPI.Models.Dtos.Wallet;
using WalletPlusIncAPI.Models.Entities;
using WalletPlusIncAPI.Services.Implementation;
using WalletPlusIncAPI.Services.Interfaces;
using Xunit;

namespace WalletPlusIncAPI.Tests.WalletTests
{
    public class WalletControllerShould
    {
        private readonly WalletService _sut;
        private IServiceProvider _serviceProvider;
        public Mock<IWalletService> mockWalletService { get; set; } = new Mock<IWalletService>();
        public Mock<ICurrencyService> mockCurrencyService { get; set; } = new Mock<ICurrencyService>();
        public Mock<IFundingService> mockFundingService { get; set; } = new Mock<IFundingService>();
        public Mock<IAppUserService> mockAppUserService { get; set; } = new Mock<IAppUserService>();
        //public Mock<IWalletRepository> mockWalletRepository { get; set; } = new Mock<IWalletRepository>();

        public WalletControllerShould()
        {
            var store = new Mock<IUserStore<AppUser>>();
            var userManager = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider.Setup(provide => provide.GetService(typeof(IAppUserService))).Returns(mockAppUserService.Object).Verifiable();
            mockServiceProvider.Setup(provide => provide.GetService(typeof(IWalletService))).Returns(mockWalletService.Object).Verifiable();
            mockServiceProvider.Setup(provide => provide.GetService(typeof(ICurrencyService))).Returns(mockCurrencyService.Object).Verifiable();
            mockServiceProvider.Setup(provide => provide.GetService(typeof(IFundingService))).Returns(mockFundingService.Object).Verifiable();
          
            mockServiceProvider.Setup(injector => injector.GetService(typeof(UserManager<AppUser>)))
                .Returns(userManager.Object).Verifiable();
            _serviceProvider = mockServiceProvider.Object;
        }

        [Fact]
        public void GetById_ShouldReturnWallet_IfExist()
        {
            var walletController = new WalletController(_serviceProvider);
            var id = Guid.NewGuid().ToString();
            var expected = 200;

            //ACT
            var actual = walletController.GetWalletsByUserId(id) as OkObjectResult;

            //Assert
            Assert.Equal(expected, actual.StatusCode);
        }

        [Fact]
        public async Task Return_Notfound_FundWallet_If_Not_MeetLimitValidation()
        {
            MockUp(false);
            var walletController = new WalletController(_serviceProvider);
            var funding = new FundPremiumDto();

            //ACT
            var actual = await walletController.FundPremiumWallet(funding) as BadRequestObjectResult;

            //Assert
            Assert.Equal(StatusCodes.Status400BadRequest, actual.StatusCode);
        }
          [Fact]
        public async Task Return_Ok_WithdrawWallet_If_MeetLimitValidation()
        {
            MockUp(true);
            var walletController = new WalletController(_serviceProvider);
            var withdraw = new WithdrawalDto();

            //ACT
            var actual = await walletController.WithdrawFromWallet(withdraw) as OkObjectResult;

            //Assert
            Assert.Equal(StatusCodes.Status200OK, actual.StatusCode);
        }
          [Fact]
        public async Task Return_BadRequet_WithdrawWallet_If_Not_MeetLimitValidation()
        {
            MockUp(false);
            var walletController = new WalletController(_serviceProvider);
            var withdraw = new WithdrawalDto();

            //ACT
            var actual = await walletController.WithdrawFromWallet(withdraw) as BadRequestObjectResult;

            //Assert
            Assert.Equal(StatusCodes.Status400BadRequest, actual.StatusCode);
        }
        [Fact]
        public async Task FundWallet_IfMeetLimitValidation()
        {
            MockUp(true);
            var walletController = new WalletController(_serviceProvider);
            var funding = new FundPremiumDto();

            //ACT
            var actual = await walletController.FundPremiumWallet(funding) as OkObjectResult;

            //Assert
            Assert.Equal(StatusCodes.Status200OK, actual.StatusCode);
        }
          [Fact]
        public async Task FundOthers_ReturnOk_IfMeetLimitValidation()
        {
            MockUp(true);
            var walletController = new WalletController(_serviceProvider);
            var funding = new FundOthersDto();

            //ACT
            var actual = await walletController.FundOthersWallet(funding) as OkObjectResult;

            //Assert
            Assert.Equal(StatusCodes.Status200OK, actual.StatusCode);
        }
        private void MockUp(bool state)
        {
            mockFundingService.Setup(service => service.CreateFundingAsync(It.IsAny<FundPremiumDto>()))
                .ReturnsAsync((new ServiceResponse<bool>(){ Success = state }));
            mockCurrencyService.Setup(service => service.CurrencyExist(It.IsAny<int>())).Returns(Task.FromResult(new ServiceResponse<bool>(){Success = state}));
            mockAppUserService.Setup(service => service.GetUserAsync(It.IsAny<string>()))
                .ReturnsAsync((new ServiceResponse<AppUser>() {Success = state, Data = new AppUser()}));
            mockWalletService.Setup(service => service.GetFiatWalletByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((new Wallet()));
             mockWalletService.Setup(service => service.WithdrawFromWalletAsync(It.IsAny<WithdrawalDto>()))
                .ReturnsAsync((new ServiceResponse<bool>(){ Success = state }));
              mockWalletService.Setup(service => service.FundOthersAsync(It.IsAny<FundOthersDto>()))
                .ReturnsAsync((new ServiceResponse<bool>(){ Success = state }));
        }
    }
}
