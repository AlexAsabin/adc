using System;
using System.Linq;
using System.Linq.Expressions;

using adc.Dal.Interfaces;

namespace adc.Dal.Implementation {
    public class OrderByDescending<TEntity, TKey> : IOrderBy<TEntity> where TEntity : class {
        private Expression<Func<TEntity, TKey>> _expression;

        public OrderByDescending(Expression<Func<TEntity, TKey>> expression) {
            if (expression == null) throw new ArgumentNullException();
            _expression = expression;
        }

        public IOrderedQueryable<TEntity> Apply(IQueryable<TEntity> query) {
            return query.OrderByDescending(_expression);
        }

        public IOrderedQueryable<TEntity> Apply(IOrderedQueryable<TEntity> query) {
            return query.ThenByDescending(_expression);
        }

    }
}