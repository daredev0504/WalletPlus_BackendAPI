using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WalletPlusIncAPI.Data.Data;
using WalletPlusIncAPI.Data.DataAccess.Interfaces;
using WalletPlusIncAPI.Models.Entities;

namespace WalletPlusIncAPI.Data.DataAccess.Implementation
{
    
    public class FundRepository : GenericRepository<Funding>, IFundRepository
    {
        private readonly ApplicationDbContext _context;

        public FundRepository(ApplicationDbContext context) : base(context) => _context = context;

       
        public Funding GetFundingById(Guid id) => _context.Fundings.Include(f => f.Destination).FirstOrDefault(f => f.Id == id);

        public List<Funding> GetAllFundings() => _context.Fundings.Include(f => f.Currency).ToList();

        public List<Funding> GetUnApprovedFundings() => _context.Fundings.Include(f => f.Currency).Where(f => !f.IsApproved).ToList();

        public List<Funding> GetApprovedFundings() => _context.Fundings.Include(f => f.Currency).Where(f => f.IsApproved).ToList();
    }
}