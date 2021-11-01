using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using WalletPlusIncAPI.Data.DataAccess.Interfaces;
using WalletPlusIncAPI.Helpers;
using WalletPlusIncAPI.Models.Dtos.Funding;
using WalletPlusIncAPI.Models.Dtos.Wallet;
using WalletPlusIncAPI.Models.Entities;
using WalletPlusIncAPI.Services.Interfaces;

namespace WalletPlusIncAPI.Services.Implementation
{
    public class FundingService : IFundingService
    {
        private readonly IFundRepository _fundingRepository;
        private readonly IMapper _iMapper;
        private readonly IWalletService _walletService;
        private readonly ICurrencyService _currencyService;

        public FundingService(IServiceProvider serviceProvider)
        {
            _fundingRepository = serviceProvider.GetRequiredService<IFundRepository>();
            _iMapper = serviceProvider.GetRequiredService<IMapper>();
            _walletService = serviceProvider.GetRequiredService<IWalletService>();
            _currencyService = serviceProvider.GetRequiredService<ICurrencyService>();
        }


         public Funding GetFundingById(Guid id)
        {
            var fund =  _fundingRepository.GetFundingById(id);
            return fund;

        }


        public async Task<ServiceResponse<bool>> CreateFundingAsync(FundPremiumDto fundFreeDto)
        {
            var response = new ServiceResponse<bool>();
              var currencyExist = await _currencyService.CurrencyExist(fundFreeDto.CurrencyId);

            if (!currencyExist.Success)
            {
                response.Data = false;
                response.Success = false;
                response.Message = "Currency Not found, currency id provided is invalid";
                  return response;
            }

            var wallet = await _walletService.GetFiatWalletByIdAsync(_walletService.GetUserId());
               var walletPoint = await _walletService.GetPointWalletByIdAsync(_walletService.GetUserId());

            if (wallet == null)
            {
                 response.Success = false;
                response.Message = "Wallet Not found, userid provided is invalid";
                  return response;
            }

            var result = WalletReachLimit(fundFreeDto.Amount, wallet.Balance);
            if (result)
            {
                Funding funding = new Funding()
                {
                    DestinationId = wallet.Id,
                    CurrencyId = fundFreeDto.CurrencyId,
                    Amount = fundFreeDto.Amount,
                    IsApproved = false
                };

                try
                {
                    int points = 0;
                    if ( await _fundingRepository.Add(funding))
                    {
                    
                        if (fundFreeDto.Amount >= PercentagesCalc.Min && fundFreeDto.Amount <= PercentagesCalc.Max)
                        {
                            points  = (int) ((PercentagesCalc.PointOne / 100) * (double) fundFreeDto.Amount);

                            await _walletService.AwardPremiumWalletPointAsync(points);
                        }

                        if (fundFreeDto.Amount >= PercentagesCalc.Min2 && fundFreeDto.Amount <= PercentagesCalc.Max2)
                        {
                            points = (int)Math.Round((PercentagesCalc.PointTwo/100) * (double) fundFreeDto.Amount);
                            await _walletService.AwardPremiumWalletPointAsync(points);
                        }

                        if (fundFreeDto.Amount >= PercentagesCalc.Max2)
                        {
                            points = (int)Math.Round((decimal) (PercentagesCalc.PointThree/100)  * fundFreeDto.Amount);
                            var res = await _walletService.AwardPremiumWalletPointAsync(points);
                            if ( res == LimitTypes.Reached)
                            {
                                 response.Data = true;
                                response.Success = true;
                                response.Message = $"you have succesfully funded your account, waiting for approval from admin. You have reached your point limit, you cannot receive more points";
                                return response;
                            }
                             else if (res == LimitTypes.NotReached)
                            {
                                 response.Data = true;
                                response.Success = true;
                                response.Message = $"you have succesfully funded your account, waiting for approval from admin. , your point balance is fully reached at {walletPoint.Balance}";
                                return response;
                            }
                            else
                            {
                                response.Data = true;
                                response.Success = true;
                                response.Message = $"you have succesfully funded your account, waiting for approval from admin. , you have received {points}";
                                return response;
                            }
                                
                        }
                    }
                    
                    response.Data = true;
                    response.Success = true;
                    response.Message = $"you have succesfully funded your account, waiting for approval from admin";
                    return response;
                }
                catch
                {
                    response.Message= "An error occured";
                    response.Success = false;
                    return response;
                }
            }

            response.Message= $"An error occured, The Deposit limit is {LimitCalc.LimitForDeposit}";
             response.Success = false;
             return response;

        }

        public async Task<bool> DeleteFundingAsync(Guid id)
        {
            try
            {
               await _fundingRepository.DeleteById(id);
             
                return true;
            }
            catch
            {
                return false;
            }
        
        }

        public List<Funding> GetAllFundings()
        {
           var funds = _fundingRepository.GetAllFundings();
            if (funds == null)
            {
                return null;
            }
            return funds;
        }

        public List<FundingReadDto> GetApprovedFundings()
        {
           var funds = _fundingRepository.GetApprovedFundings();
           var fundsRead = _iMapper.Map<List<FundingReadDto>>(funds);
            if (funds != null)
            {
                return fundsRead;
            }
            return null;
        }

        public List<FundingReadDto> GetUnApprovedFundings()
        {
             var funds = _fundingRepository.GetUnApprovedFundings();
             var fundsRead = _iMapper.Map<List<FundingReadDto>>(funds);
            if (funds != null)
            {
                return fundsRead;
            }
            return null;
        }
        public bool WalletReachLimit(decimal deposit, decimal balance) => (deposit + balance) <= LimitCalc.LimitForDeposit; 
    }
}
