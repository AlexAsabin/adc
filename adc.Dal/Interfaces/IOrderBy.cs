using System.Linq;

namespace adc.Dal.Interfaces {
    public interface IOrderBy<TEntity> where TEntity : class {
        IOrderedQueryable<TEntity> Apply(IQueryable<TEntity> query);
        IOrderedQueryable<TEntity> Apply(IOrderedQueryable<TEntity> query);
    }
}
