﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WalletPlusIncAPI.Data.DataAccess.Interfaces;
using WalletPlusIncAPI.Helpers.Rates;
using WalletPlusIncAPI.Models.Dtos.Wallet;
using WalletPlusIncAPI.Models.Entities;
using WalletPlusIncAPI.Services.Interfaces;

namespace WalletPlusIncAPI.Services.Implementation
{
    public class WalletService : IWalletService
    {
        private readonly IMapper _mapper;
        private readonly ITransactionService _transactionService;
        private readonly IFundRepository _fundRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ICurrencyService _currencyService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
       
        


        public WalletService(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
        {
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _transactionService = serviceProvider.GetRequiredService<ITransactionService>();
            _fundRepository = serviceProvider.GetRequiredService<IFundRepository>();
            _walletRepository = serviceProvider.GetRequiredService<IWalletRepository>();
            _currencyService = serviceProvider.GetRequiredService<ICurrencyService>();
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
           
            
        }


        public string GetUserId()
        {
           return  _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<ServiceResponse<bool>> AddWallet(Wallet wallet)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                await _walletRepository.Add(wallet);
                response.Success = true;
                response.Message = "wallet created successfully";
                return response;
            }
            catch
            {
                response.Success = false;
                response.Message = "A problem occured";
                return response;
            }
        }

        public async Task<ServiceResponse<bool>> DeleteWallet(Guid id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                await _walletRepository.DeleteById(id);
                response.Success = true;
                response.Message = "wallet updated successfully";
                return response;
            }
            catch
            {
                response.Success = false;
                response.Message = "Unable to delete wallet, error encountered while deleting the wallet";
                return response;
            }
        }

        public async Task<ServiceResponse<bool>> UpdateWallet(Wallet wallet)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                await _walletRepository.Modify(wallet);
                response.Success = true;
                response.Message = "wallet updated successfully";
                return response;
            }
            catch
            {
                response.Success = false;
                response.Message = "A problem occured";
                return response;
            }
        }
        public async Task<ServiceResponse<bool>> UpdateWallet2(WalletUpdateDto updateWalletDto)
        {
            var response = new ServiceResponse<bool>();
            var walletToUpdate = await _walletRepository.GetById(updateWalletDto.WalletId);

            if (walletToUpdate == null)
            {
                response.Success = false;
                response.Message = "A problem occured";
                return response;
            }
            else
            {
                if (walletToUpdate.CurrencyId != updateWalletDto.CurrencyId)
                {
                    var targetCode = await _currencyService.GetCurrencyCode(updateWalletDto.CurrencyId);
                    var sourceCode =await _currencyService.GetCurrencyCode(walletToUpdate.CurrencyId);

                    var newAmount = await CurrencyRate.ConvertCurrency(sourceCode, targetCode, walletToUpdate.Balance);

                    walletToUpdate.CurrencyId = updateWalletDto.CurrencyId;
                    walletToUpdate.Balance = newAmount ?? 0;
                }
                var result = await _walletRepository.Modify(walletToUpdate);
                if (result)
                {
                    response.Success = true;
                    response.Message = "wallet updated successfully";
                    return response;
                }
                response.Success = false;
                response.Message = "A problem occured";
                return response;
            }
            
        }

        public ServiceResponse<Wallet> GetUserMainCurrencyWallet(string userId)
        {
            var response = new ServiceResponse<Wallet>();
            var wallet = _walletRepository.GetUserMainCurrencyWallet(userId);
            if (wallet != null)
            {
                response.Message = "main wallet returned";
                response.Success = true;
                return response;
            }
            response.Message = "main wallet not found";
            response.Success = false;

            return response;
        }


    

        public ServiceResponse<bool> CheckWallet(Guid walletId)
        {
            var response = new ServiceResponse<bool>();
            var wallet = _walletRepository.CheckWallet(walletId);
            if (wallet)
            {
                response.Message = "wallet found";
                response.Success = true;
                return response;
            }
            response.Message = "wallet not found";
            response.Success = false;
            return response;
        }

        public ServiceResponse<List<WalletReadDto>> GetAllMyWallets()
        {
            var response = new ServiceResponse<List<WalletReadDto>>();
            var wallets = _walletRepository.GetAllMyWallets();
            var walletToReturn = _mapper.Map<List<WalletReadDto>>(wallets);

            if (wallets != null)
            {
                response.Message = "main wallet returned";
                response.Success = true;
                response.Data = walletToReturn;
                return response;
            }
            response.Message = "main wallet not found";
            response.Success = false;
            return response;
        }

        public ServiceResponse<Wallet> GetWalletById(Guid? id)
        {
            var response = new ServiceResponse<Wallet>();
            var wallet = _walletRepository.GetWalletById(id);
            if (wallet != null)
            {
                response.Message = "main wallet returned";
                response.Success = true;
                return response;
            }
            response.Message = "main wallet not found";
            response.Success = false;
            return response;
        }

        public ServiceResponse<List<Wallet>> GetWalletsById(Guid id)
        {
            var response = new ServiceResponse<List<Wallet>>();
            var wallets = _walletRepository.GetWalletsById(id);
            if (wallets != null)
            {
                response.Message = "wallets returned";
                response.Success = true;
                return response;
            }
            response.Message = "wallets not found";
            response.Success = false;
            response.Data = wallets;
            return response;
        }

        public ServiceResponse<List<WalletReadDto>> GetWalletsByUserId(string ownerId)
        {
            var response = new ServiceResponse<List<WalletReadDto>>();
            var wallets = _walletRepository.GetWalletsByUserId(ownerId);
            var walletsToReturn = _mapper.Map<List<WalletReadDto>>(wallets);
            if (wallets != null)
            {
                response.Message = "wallet returned";
                response.Data = walletsToReturn;
                response.Success = true;

                return response;
            }
            response.Message = "wallet not found";
            response.Success = false;
            return response;
        }


        public ServiceResponse<List<WalletReadDto>> GetAllWallets()
        {
            var response = new ServiceResponse<List<WalletReadDto>>();
            var wallets = _walletRepository.GetAll();
            if (wallets != null)
            {
                var walletsRead = _mapper.Map<IEnumerable<WalletReadDto>>(wallets);

                response.Message = "main wallet returned";
                response.Success = true;
                return response;
            }
            response.Message = "main wallet not found";
            response.Success = false;
            return response;
        }

        public ServiceResponse<bool> UserHasWalletWithCurrency(FundingDto fundingDto)
        {
            var response = new ServiceResponse<bool>();
            var wallet = _walletRepository.UserHasWalletWithCurrency(fundingDto);
           
            if (wallet)
            {
                response.Message = "main wallet returned";
                response.Success = true;
                return response;
            }
            response.Message = "main wallet not found";
            response.Success = false;
            return response;
           
        }

        public ServiceResponse<List<Wallet>> GetUserWalletsByCurrencyId(string userId, int currencyId)
        {
            var response = new ServiceResponse<List<Wallet>>();
            var wallet = _walletRepository.GetUserWalletsByCurrencyId(userId, currencyId);
            if (wallet != null)
            {
                response.Message = "main wallet returned";
                response.Success = true;
                return response;
            }
            response.Message = "main wallet not found";
            response.Success = false;
            return response;
        }

         public async Task<ServiceResponse<bool>> MergeAllWalletsToMain(AppUser user)
        {
            var response = new ServiceResponse<bool>();

            var mainWallet = _walletRepository.GetUserMainCurrencyWallet(user.Id);
            var userWallets = _walletRepository.GetWalletsByUserId(user.Id);

            if (mainWallet != null && userWallets != null)
            {
                foreach (var wallet in userWallets)
                {
                    await FundWallet(mainWallet, wallet);
                    await _walletRepository.DeleteById(wallet.Id);
                }
               
                response.Success = true;
                response.Message = "wallet merged";
                return response;
            }
            response.Success = false;
            response.Message = "A problem occured";
            return response;
        }

      
        public async Task<ServiceResponse<bool>> FundWallet(Wallet wallet, decimal amount)
        {
            var response = new ServiceResponse<bool>();
            wallet.Balance += amount;

            var result = await _transactionService.CreateTransaction(TransactionType.Credit, amount, wallet.Id,
                wallet.CurrencyId);
            if (result.Data)
            {
                await _walletRepository.Modify(wallet);
                response.Message = "transaction created";
                response.Success = true;
                return response;
            }
            response.Message = "transaction failed";
            response.Success = false;
            return response;
        }

        public async Task<ServiceResponse<bool>> FundWallet(Wallet main, Wallet source)
        {
            //var wallet = GetWalletById(funding.DestinationId);
            var response = new ServiceResponse<bool>();
            if (main == null)
            {
                response.Success = false;
                response.Message = "an error occured";
                return response;

            }
            else
            {
                if (main.CurrencyId == source.CurrencyId)
                {
                    main.Balance += source.Balance;

                    await _transactionService.CreateTransaction(TransactionType.Credit, source.Balance, main.Id,
                        source.CurrencyId);
                }
                else
                {
                    var targetCode = await _currencyService.GetCurrencyCode(main.CurrencyId);
                    var sourceCode =await _currencyService.GetCurrencyCode(source.CurrencyId);

                    var newAmount = await CurrencyRate.ConvertCurrency(sourceCode, targetCode, source.Balance);

                    main.Balance += newAmount ?? 0;

                    await _transactionService.CreateTransaction(TransactionType.Credit, newAmount ?? 0, main.Id,
                        source.CurrencyId);
                }

                await UpdateWallet(main);
                
                response.Success = true;
                response.Message = "wallet funded successfully";
                return response;
            }
          
        }

        public async Task<ServiceResponse<bool>> FundFreeWallet(Funding funding)
        {
            var response = new ServiceResponse<bool>();
            var wallet = _walletRepository.GetWalletById(funding.DestinationId);
         

            if (wallet == null)
            {
                response.Success = false;
                response.Message = "wallet not found";
                return response;
            }
            else
            {
                var user = await _userManager.FindByIdAsync(wallet.OwnerId);

                if (funding.CurrencyId == wallet.CurrencyId)
                {
                    wallet.Balance += funding.Amount;

                    var transactioResult = await _transactionService.CreateTransaction(TransactionType.Credit, funding.Amount, wallet.Id,
                        wallet.CurrencyId);

                   
                 
                }
                else
                {
                    var targetCode = await _currencyService.GetCurrencyCode(wallet.CurrencyId);
                    var sourceCode = await _currencyService.GetCurrencyCode(funding.CurrencyId);

                    var newAmount = await CurrencyRate.ConvertCurrency(sourceCode, targetCode, funding.Amount);

                    wallet.Balance += newAmount ?? 0;

                    await _transactionService.CreateTransaction(TransactionType.Credit, newAmount ?? 0, wallet.Id, funding.CurrencyId);
                }

                await UpdateWallet(wallet);
                response.Success = true;
                response.Message = "free wallet funded successfully";
                return response;
            }
          
        }

        public bool CanWithdrawFromWallet(decimal balance, decimal? amount) => (balance - amount) >= 0;

        public async Task<ServiceResponse<bool>> WithdrawFromWallet(WithdrawalDto withdrawalDto)
        {
            var response = new ServiceResponse<bool>();

            var wallet = _walletRepository.GetWalletById(withdrawalDto.WalletId);

            if (wallet == null)
            {
                response.Success = false;
                response.Message = "wallet not found";
            }
            else
            {
                var user = await _userManager.FindByIdAsync(wallet.OwnerId);

                if (wallet.CurrencyId == withdrawalDto.CurrencyId)
                {
                    if (!CanWithdrawFromWallet(wallet.Balance, withdrawalDto.Amount))
                    {
                        response.Message = "you are not eligible to withdraw";
                        response.Success = false;
                        return response;
                    }
                    wallet.Balance -= withdrawalDto.Amount;

                    await _transactionService.CreateTransaction(TransactionType.Debit, withdrawalDto.Amount, withdrawalDto.WalletId,
                        withdrawalDto.CurrencyId);

                 

                    response.Message = "transaction success";
                    response.Success = true;
                    return response;
                }
                else
                {
                    var targetCode = await _currencyService.GetCurrencyCode(wallet.CurrencyId);
                    var sourceCode = await _currencyService.GetCurrencyCode(withdrawalDto.CurrencyId);

                    var newAmount = await CurrencyRate.ConvertCurrency(sourceCode, targetCode, withdrawalDto.Amount);

                    if (!CanWithdrawFromWallet(wallet.Balance, newAmount))
                    {
                        response.Message = "you are not eligible to withdraw";
                        response.Success = false;
                        return response;
                    }

                    wallet.Balance -= newAmount ?? 0;

                    await _transactionService.CreateTransaction(TransactionType.Debit, newAmount ?? 0, withdrawalDto.WalletId, withdrawalDto.CurrencyId);
                }

                await UpdateWallet(wallet);

            }
            response.Message = "transaction success";
            response.Success = true;
            return response;

        }             

        public async Task<ServiceResponse<bool>> WithdrawFromMain(string userId, decimal amount)
        {
            var response = new ServiceResponse<bool>();
            var mainWallet = GetUserMainCurrencyWallet(userId);

            if (!CanWithdrawFromWallet(mainWallet.Data.Balance, amount))
            {
                response.Message = "you are not eligible to withdraw";
                response.Success = false;
                return response;
            }

            mainWallet.Data.Balance -= amount;

            await UpdateWallet(mainWallet.Data);
            response.Message = "withdraw success";
            response.Success = true;
            return response;
        }
            

        public async Task<ServiceResponse<bool>> ChangeMainCurrency(Wallet oldWallet, Wallet newWallet)
        {
            var response = new ServiceResponse<bool>();
            oldWallet.IsMain = false;
            newWallet.IsMain = true;

            await UpdateWallet(oldWallet);
            await UpdateWallet(newWallet);

            response.Success = true;
            response.Message = "Main wallet changed";

            return response;
        }
    }
}
