using Barnamenevisan.Localizing.Statics;
using System.Linq.Expressions;

namespace Barnamenevisan.Localizing.Generator
{
    public class FilterConditions<TEntity> : List<Expression<Func<TEntity, bool>>>
    {
        public bool CurrentCultureIsDefault { get; set; }
    }

    public class FilterOrder<TEntity>
    {
        public bool IsAscending { get; private set; }
        public Expression<Func<TEntity, object>> OrderBy { get; private set; }

        public FilterOrder(Expression<Func<TEntity, object>> orderBy, bool isAscending)
        {
            IsAscending = isAscending;
            OrderBy = orderBy;
        }
    }

    public static class Filter
    {
        public static FilterOrder<TEntity> OrderBy<TEntity>(Expression<Func<TEntity, object>> orderBy, bool isAscending)
        {
            return new FilterOrder<TEntity>(orderBy, isAscending);
        }

        public static FilterConditions<TEntity> GenerateConditions<TEntity>()
        {
            var result = new FilterConditions<TEntity>();
            result.CurrentCultureIsDefault = LocalizingTools.DefaultCultureName == Thread.CurrentThread.CurrentCulture.Name;

            return result;
        }
    }
}
