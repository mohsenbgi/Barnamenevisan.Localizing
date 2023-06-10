using System.Linq.Expressions;
using Barnamenevisan.Localizing.Shared;

namespace Barnamenevisan.Localizing.Extensions
{
    public static class ExpressionExtensions
    {
        public static LocalizedExpressionVisitor<TEntity> Visit<TEntity>(this Expression<Func<TEntity, bool>> ex)
        {
            var visitor = new LocalizedExpressionVisitor<TEntity>(ex);
            visitor.Visit(ex);

            return visitor;
        }
    }
}
