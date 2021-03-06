using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WalletPlusIncAPI.Models.Dtos.Funding;
using WalletPlusIncAPI.Models.Dtos.Wallet;
using WalletPlusIncAPI.Models.Entities;

namespace WalletPlusIncAPI.Services.Interfaces
{
    public interface IFundingService
    {
        Funding GetFundingById(Guid id);

        Task<ServiceResponse<bool>> CreateFundingAsync(FundPremiumDto fundFreeDto);

        Task<bool> DeleteFundingAsync(Guid id);

        List<Funding> GetAllFundings();

        List<FundingReadDto> GetUnApprovedFundings();
        List<FundingReadDto> GetApprovedFundings();
    }
}
