using System.Linq;
using System.Linq.Expressions;

namespace adc.Core.PredicateCombinators {
    public class ReplaceParameterVisitor<TResult> : ExpressionVisitor {
        private readonly ParameterExpression _parameter;
        private readonly Expression _replacement;

        public ReplaceParameterVisitor(ParameterExpression parameter, Expression replacement) {
            this._parameter = parameter;
            this._replacement = replacement;
        }

        public Expression<TResult> Visit<T>(Expression<T> node) {
            var parameters = node.Parameters.Where(p => p != _parameter);
            return Expression.Lambda<TResult>(Visit(node.Body), parameters);
        }

        protected override Expression VisitParameter(ParameterExpression node) {
            return node == _parameter ? _replacement : base.VisitParameter(node);
        }
    }
}