using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
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

        public FundingService(IFundRepository fundingRepository, IMapper iMapper, IWalletService walletService)
        {
            _fundingRepository = fundingRepository;
            _iMapper = iMapper;
            _walletService = walletService;
        }


         public Funding GetFundingById(Guid id)
        {
            var fund =  _fundingRepository.GetFundingById(id);
            return fund;

        }


        public async Task<bool> CreateFunding(FundPremiumDto fundFreeDto, Guid walletId)
        {
            var userId = _walletService.GetUserId();
            var wallet  = await _walletService.GetFiatWalletById(userId);
            var result = WalletReachLimit(fundFreeDto.Amount, wallet.Balance);
            if (result)
            {
                Funding funding = new Funding()
                {
                    DestinationId = walletId,
                    CurrencyId = fundFreeDto.CurrencyId,
                    Amount = fundFreeDto.Amount,
                    IsApproved = false
                };

                try
                {
                
                    if ( await _fundingRepository.Add(funding))
                    {
                    
                        if (fundFreeDto.Amount >= 5000 && fundFreeDto.Amount <= 10000)
                        {
                            int points  = (int) ((PercentagesCalc.PointOne / 100) * (double) fundFreeDto.Amount);

                            await _walletService.AwardPremiumWalletPoint(points);
                        }

                        if (fundFreeDto.Amount >= 10001 && fundFreeDto.Amount <= 25000)
                        {
                            int points = (int)Math.Round((PercentagesCalc.PointTwo/100) * (double) fundFreeDto.Amount);
                            await _walletService.AwardPremiumWalletPoint(points);
                        }

                        if (fundFreeDto.Amount >= 25000)
                        {
                            int points = (int)Math.Round((decimal) (PercentagesCalc.PointThree/100)  * fundFreeDto.Amount);
                            await _walletService.AwardPremiumWalletPoint(points);
                        }
                    }
             

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;

        }

        public async Task<bool> DeleteFunding(Guid id)
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
        private bool WalletReachLimit(decimal deposit, decimal balance) => (deposit + balance) <= LimitCalc.LimitForDeposit; 
    }
}
