using System;
using System.Linq;
using System.Linq.Expressions;
using adc.Dal.Interfaces;

namespace adc.Dal.Implementation {
    public class OrderBy<TEntity, TKey> : IOrderBy<TEntity> where TEntity : class {
        private Expression<Func<TEntity, TKey>> _expression;

        public OrderBy(Expression<Func<TEntity, TKey>> expression) {
            _expression = expression ?? throw new ArgumentNullException();
        }

        public IOrderedQueryable<TEntity> Apply(IQueryable<TEntity> query) {
            return query.OrderBy(_expression);
        }

        public IOrderedQueryable<TEntity> Apply(IOrderedQueryable<TEntity> query) {
            return query.ThenBy(_expression);
        }
    }
}
