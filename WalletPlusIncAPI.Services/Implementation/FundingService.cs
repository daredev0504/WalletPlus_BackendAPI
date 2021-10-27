using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using WalletPlusIncAPI.Data.DataAccess.Interfaces;
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

        public FundingService(IFundRepository fundingRepository, IMapper iMapper)
        {
            _fundingRepository = fundingRepository;
            _iMapper = iMapper;
        }


         public Funding GetFundingById(Guid id)
        {
            var fund =  _fundingRepository.GetFundingById(id);
            return fund;

        }


        public async Task<bool> CreateFunding(FundFreeDto fundFreeDto, Guid walletId)
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
              await _fundingRepository.Add(funding);
                return true;
            }
            catch
            {
                return false;
            }
          
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
    }
}
