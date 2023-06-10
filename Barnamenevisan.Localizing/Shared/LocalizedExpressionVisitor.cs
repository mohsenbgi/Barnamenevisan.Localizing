using Barnamenevisan.Localizing.Attributes;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq.Expressions;
using System.Reflection;

namespace Barnamenevisan.Localizing.Shared
{

    public class LocalizedExpressionVisitor<TEntity> : ExpressionVisitor
    {
        private readonly Expression expression;

        public bool ShouldFilterByLocalizedValue { get; private set; }

        public string? EntityName { get; set; }

        public string? Key { get; set; }

        public string? ValueToSearch { get; set; }


        public LocalizedExpressionVisitor(Expression<Func<TEntity, bool>> ex)
        {
            expression = ex;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            //  stringifiedExpression: 'entity => entity.EntityProperty.Contains(model.ModelProperty)'
            var stringifiedExpression = expression.Print();

            //  splitedExpressionAsString: ['entity', 'EntityPropertyName', 'Contains(model', 'ModelPropertyName)']
            var splitedExpressionAsString = stringifiedExpression.Split(".").ToList();

            var containsPhrase = splitedExpressionAsString.FirstOrDefault(x => x.StartsWith("Contains"));

            if (containsPhrase is null)
            {
                ShouldFilterByLocalizedValue = false;
                return base.VisitConstant(node);
            }

            // index of EntityPropertyName is index of containsPhrase - 1:
            //
            //['entity', 'EntityPropertyName', 'Contains(model', 'ModelPropertyName)']
            //                                        ^
            //                                        |
            //                            Index of ContainsPhrase

            var entityPropertyName = splitedExpressionAsString[splitedExpressionAsString.IndexOf(containsPhrase) - 1];

            // ModelPropertyName is last item of splited array
            var itemOfModelPropertyName = splitedExpressionAsString.Last();

            // to get ModelPropertyName should remove ')'
            //
            // itemOfModelPropertyName: 'ModelPropertyName)'
            //
            // expected value: 'ModelPropertyName'
            var modelPropertyName = itemOfModelPropertyName.Replace(")", "");

            var model = (node.Value.GetType().GetRuntimeFields() as FieldInfo[])?[0]?.GetValue(node.Value);

            var property = model?.GetType()?.GetProperty(modelPropertyName);

            ShouldFilterByLocalizedValue = property?
                .CustomAttributes?
                .Any(attribute => attribute.AttributeType == typeof(FilterByLocalizedValueAttribute))
                ?? false;


            if (ShouldFilterByLocalizedValue)
            {
                ValueToSearch = property?.GetValue(model)?.ToString() ?? "";
                Key = entityPropertyName;
                EntityName = typeof(TEntity).Name;
            }

            return base.VisitConstant(node);
        }
    }
}
