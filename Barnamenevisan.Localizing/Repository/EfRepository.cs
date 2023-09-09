using Barnamenevisan.Localizing.Entity;
using Barnamenevisan.Localizing.Extensions;
using Barnamenevisan.Localizing.Generator;
using Barnamenevisan.Localizing.Shared;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Barnamenevisan.Localizing.Repository
{
    public class EfRepository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        #region Fields

        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        #endregion

        #region Constructor

        public EfRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        #endregion

        #region Methods

        public virtual async Task<List<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, object>>? orderBy = null,
            Expression<Func<TEntity, object>>? orderByDesc = null,
            params string[] includeProperties)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            if (orderBy is not null)
            {
                query = query.OrderBy(orderBy);
            }

            if (orderByDesc is not null)
            {
                query = query.OrderByDescending(orderByDesc);
            }

            return await query.ToListAsync();
        }

        public virtual Task<List<TEntity>> GetByIdsAsync(List<TKey> ids)
        {
            return _dbSet.Where(e => ids.Contains(e.Id))
                .ToListAsync();
        }

        public virtual Task FilterAsync<TModel>(
            BasePaging<TModel> filterModel,
            FilterConditions<TEntity> filterConditions,
            Expression<Func<TEntity, TModel>> mapping,
            FilterOrder<TEntity> orderBy)
        {
            var orderByAsc = orderBy.IsAscending ? orderBy.OrderBy : null;
            var orderByDesc = !orderBy.IsAscending ? orderBy.OrderBy : null;

            return FilterAsync(filterModel, filterConditions, mapping, orderByAsc, orderByDesc);
        }

        public virtual async Task FilterAsync<TModel>(
            BasePaging<TModel> filterModel,
            FilterConditions<TEntity> filterConditions,
            Expression<Func<TEntity, TModel>> mapping,
            Expression<Func<TEntity, object>>? orderBy = null,
            Expression<Func<TEntity, object>>? orderByDesc = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filterConditions.CurrentCultureIsDefault)
            {
                foreach (var filter in filterConditions)
                {
                    query = query.Where(filter);
                }
            }
            else
            {
                foreach (var filter in filterConditions)
                {
                    var visitor = filter.Visit();

                    if (visitor.ShouldFilterByLocalizedValue)
                    {
                        var localizedProperties = _context
                            .Set<LocalizedProperty>()
                            .Where(lp => lp.EntityName == visitor.EntityName
                            && lp.Key == visitor.Key
                            && lp.Value.Contains(visitor.ValueToSearch)
                            && lp.CultureName == Thread.CurrentThread.CurrentCulture.Name)
                            .ToList();

                        var selectedEntityIds = localizedProperties
                            .Select(lp => Convert.ChangeType(lp.EntityId, typeof(TKey)))
                            .ToList();

                        // apply condition
                        query = query.Where(e => selectedEntityIds.Contains(e.Id));

                        continue;
                    }

                    query = query.Where(filter);
                }
            }


            if (orderBy is not null)
            {
                query = query.OrderBy(orderBy);
            }
            else if (orderByDesc is not null)
            {
                query = query.OrderByDescending(orderByDesc);
            }
            else if (typeof(TEntity).IsAssignableTo(typeof(AuditBaseEntity<TKey>)))
            {
                query = query.OrderByDescending(entity => (entity as AuditBaseEntity<TKey>).CreatedDateOnUtc);
            }

            await filterModel.Paging(query.Select(mapping));
        }

        public virtual async Task<TEntity?> GetByIdAsync(TKey id, params string[] includeProperties)
        {
            if (id is null) return null;

            var query = _dbSet.AsQueryable();

            foreach (var inlcudeProperty in includeProperties)
            {
                query = query.Include(inlcudeProperty);
            }

            return await query.FirstOrDefaultAsync(entity => entity.Id.Equals(id));
        }

        public virtual async Task<TEntity?> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, object>>? orderBy = null,
            Expression<Func<TEntity, object>>? orderByDesc = null,
            params string[] includeProperties)
        {
            var query = _dbSet.AsQueryable();

            if (filter is not null)
            {
                query = query.Where(filter);
            }

            foreach (var inlcudeProperty in includeProperties)
            {
                query = query.Include(inlcudeProperty);
            }

            if (orderBy is not null)
            {
                query = query.OrderBy(orderBy);
            }

            if (orderByDesc is not null)
            {
                query = query.OrderByDescending(orderByDesc);
            }

            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<TEntity?> LastOrDefaultAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, object>>? orderBy = null,
            Expression<Func<TEntity, object>>? orderByDesc = null,
            params string[] includeProperties)
        {
            var query = _dbSet.AsQueryable();

            if (filter is not null)
            {
                query = query.Where(filter);
            }

            foreach (var inlcudeProperty in includeProperties)
            {
                query = query.Include(inlcudeProperty);
            }

            if (orderBy is not null)
            {
                query = query.OrderBy(orderBy);
            }

            if (orderByDesc is not null)
            {
                query = query.OrderByDescending(orderByDesc);
            }

            return await query.LastOrDefaultAsync();
        }

        public virtual async Task InsertAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task InsertRangeAsync(List<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public virtual void Delete(TKey id)
        {
            TEntity entityToDelete = _dbSet.Find(id);
            if (entityToDelete != null)
            {
                Delete(entityToDelete);
            }
        }

        public virtual void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void DeleteRange(List<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);

        }

        public virtual void Update(TEntity entityToUpdate)
        {
            _dbSet.Update(entityToUpdate);
        }

        public virtual void UpdateRange(List<TEntity> entitiesToUpdatee)
        {
            _dbSet.UpdateRange(entitiesToUpdatee);
        }

        public virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter)
        {
            return _dbSet.AnyAsync(filter);
        }

        public virtual async Task<bool> SoftDelete(TKey id)
        {
            var result = 0;

            if (typeof(TEntity).IsAssignableTo(typeof(AuditBaseEntity<TKey>)))
            {
                result = await _dbSet
                    .Where(e => e.Id.Equals(id))
                    .ExecuteUpdateAsync(s => s.SetProperty(e => e.IsDeleted, true)
                                              .SetProperty(e => (e as AuditBaseEntity<TKey>).UpdatedDateOnUtc, DateTime.UtcNow));
            }
            else
            {
                result = await _dbSet
                    .Where(e => e.Id.Equals(id))
                    .ExecuteUpdateAsync(s => s.SetProperty(e => e.IsDeleted, true));
            }

            return result > 0;
        }

        public virtual async Task SaveAsync() => await _context.SaveChangesAsync();

        #endregion
    }
}
