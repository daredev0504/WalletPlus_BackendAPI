using System;
using System.Collections.Generic;
using WalletPlusIncAPI.Models.Entities;

namespace WalletPlusIncAPI.Data.DataAccess.Interfaces
{
    public interface IFundRepository : IGenericRepository<Funding>
    {
        Funding GetFundingById(Guid id);

        List<Funding> GetAllFundings();

        List<Funding> GetUnApprovedFundings();
        List<Funding> GetApprovedFundings();
    }
}