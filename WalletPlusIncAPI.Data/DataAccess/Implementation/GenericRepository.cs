using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WalletPlusIncAPI.Data.Data;
using WalletPlusIncAPI.Data.DataAccess.Interfaces;

namespace WalletPlusIncAPI.Data.DataAccess.Implementation
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _ctx;
        private readonly DbSet<TEntity> _entities;
        public int TotalNumberOfItems { get; set; }
        public int TotalNumberOfPages { get; set; }

        public GenericRepository(ApplicationDbContext ctx)
        {
            _ctx = ctx;
            _entities = ctx.Set<TEntity>();
        }
        
        public IQueryable<TEntity> GetAll()
        {
            var result = _entities.AsNoTracking();

            return result;
        }


        public async Task<bool> Add(TEntity model)
        {
            _entities.Add(model);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<TEntity> GetById(object id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task<bool> Modify(TEntity entity)
        {
            _entities.Update(entity);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteById(object id)
        {
            var result = await GetById(id);
            _entities.Remove(result);
            return await _ctx.SaveChangesAsync() > 0;
        }

    }
}
