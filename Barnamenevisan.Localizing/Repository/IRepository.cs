using Barnamenevisan.Localizing.Generator;
using Barnamenevisan.Localizing.Shared;
using System.Linq.Expressions;

namespace Barnamenevisan.Localizing.Repository
{
    public interface IRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        void Delete(TEntity entity);

        void DeleteRange(List<TEntity> entities);

        void Delete(TKey id);

        Task<List<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, object>>? orderBy = null,
            Expression<Func<TEntity, object>>? orderByDesc = null,
            params string[] includeProperties);

        Task<TEntity?> GetByIdAsync(TKey id, params string[] includeProperties);

        Task<List<TEntity>> GetByIdsAsync(List<TKey> ids);

        Task InsertAsync(TEntity entity);

        Task InsertRangeAsync(List<TEntity> entities);

        Task SaveAsync();

        void Update(TEntity entityToUpdate);

        void UpdateRange(List<TEntity> entitiesToUpdate);

        Task FilterAsync<TModel>(
            BasePaging<TModel> filterModel,
            FilterConditions<TEntity> filterConditions,
            Expression<Func<TEntity, TModel>> mapping,
            Expression<Func<TEntity, object>>? orderBy = null,
            Expression<Func<TEntity, object>>? orderByDesc = null);

        Task FilterAsync<TModel>(
            BasePaging<TModel> filterModel,
            FilterConditions<TEntity> filterConditions,
            Expression<Func<TEntity, TModel>> mapping,
            FilterOrder<TEntity> orderBy);

        Task<TEntity?> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, object>>? orderBy = null,
            Expression<Func<TEntity, object>>? orderByDesc = null,
            params string[] includeProperties);

        Task<TEntity?> LastOrDefaultAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, object>>? orderBy = null,
            Expression<Func<TEntity, object>>? orderByDesc = null,
            params string[] includeProperties);
    }
}
