using System.Linq;
using System.Threading.Tasks;

namespace WalletPlusIncAPI.Data.DataAccess.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
      
        IQueryable<TEntity> GetAll();
        Task<bool> Add(TEntity model);
        Task<TEntity> GetById(object id);
        Task<bool> Modify(TEntity entity);
        Task<bool> DeleteById(object id);
          
    }
    
}
