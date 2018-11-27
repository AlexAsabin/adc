using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EFCore.BulkExtensions;
using adc.Core.Interfaces;

namespace adc.Dal.Interfaces {
    public interface IRepository<TEntity> where TEntity : class, IEntity {
        #region Includes

        /// <summary>
        /// Create includes for query
        /// </summary>
        /// <example>
        /// 1. one->to-one
        /// CreateIncludes(x => x.IncludedProperty)
        /// 2. one->to-many-to>-one
        /// CreateIncludes(x => x.IncludedProperty.Select(y=>y.InnerProperty))
        /// </example>
        /// <param name="includes"></param>
        /// <returns></returns>
        List<Expression<Func<TEntity, object>>> CreateIncludes(params Expression<Func<TEntity, object>>[] includes);

        #endregion

        #region OrderBy

        /// <summary>
        /// create sorting for query
        /// SAMPLES:
        /// 1. ascending
        /// CreateOrderBys(new OrderBy<TEntity, TypeOf SortField>(x => x.SortField))
        /// 2. descending
        /// OrderByDescending(new OrderBy<TEntity, TypeOf SortField>(x => x.SortField))
        /// </summary>
        /// <param name="orderBys"></param>
        /// <returns></returns>
        List<IOrderBy<TEntity>> CreateOrderBys(params IOrderBy<TEntity>[] orderBys);

        #endregion

        #region Get

        /// <summary>
        /// get single record
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <param name="orderBys"></param>
        /// <returns></returns>
        TEntity GetSingle(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> includes = null, IEnumerable<IOrderBy<TEntity>> orderBys = null);

        /// <summary>
        /// get single record without tracking
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <param name="orderBys"></param>
        /// <returns></returns>
        TEntity GetSingleNoTracking(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> includes = null, IEnumerable<IOrderBy<TEntity>> orderBys = null);

        /// <summary>
        /// get single record of return type
        /// </summary>
        /// <typeparam name="TRequestedType"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <param name="orderBys"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        TRequestedType GetSingle<TRequestedType>(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> includes = null, IEnumerable<IOrderBy<TEntity>> orderBys = null, Expression<Func<TEntity, TRequestedType>> select = null);

        /// <summary>
        /// get single record of return type without tracking
        /// </summary>
        /// <typeparam name="TRequestedType"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <param name="orderBys"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        TRequestedType GetSingleNoTracking<TRequestedType>(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> includes = null, IEnumerable<IOrderBy<TEntity>> orderBys = null, Expression<Func<TEntity, TRequestedType>> select = null);

        /// <summary>
        /// get list of records
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <param name="orderBys"></param>
        /// <returns></returns>
        List<TEntity> GetList(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> includes = null, IEnumerable<IOrderBy<TEntity>> orderBys = null, int? skip = null, int? take = null);

        /// <summary>
        /// get list of records without tracking
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <param name="orderBys"></param>
        /// <returns></returns>
        List<TEntity> GetListNoTracking(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> includes = null, IEnumerable<IOrderBy<TEntity>> orderBys = null, int? skip = null, int? take = null);

        /// <summary>
        /// get list of records of return type
        /// </summary>
        /// <typeparam name="TRequestedType"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <param name="orderBys"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        List<TRequestedType> GetList<TRequestedType>(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> includes = null, IEnumerable<IOrderBy<TEntity>> orderBys = null, Expression<Func<TEntity, TRequestedType>> select = null, int? skip = null, int? take = null);

        /// <summary>
        /// get list of records of return type without tracking
        /// </summary>
        /// <typeparam name="TRequestedType"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <param name="orderBys"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        List<TRequestedType> GetListNoTracking<TRequestedType>(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> includes = null, IEnumerable<IOrderBy<TEntity>> orderBys = null, Expression<Func<TEntity, TRequestedType>> select = null, int? skip = null, int? take = null);

        /// <summary>
        /// Get grouped list
        /// </summary>
        /// <typeparam name="TRequestedType"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="keySelector"></param>
        /// <param name="select"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        List<TRequestedType> GetGroupList<TRequestedType, TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> keySelector, Expression<Func<IGrouping<TKey, TEntity>, TRequestedType>> @select, IEnumerable<Expression<Func<TEntity, object>>> includes = null);

        #endregion

        #region Create

        /// <summary>
        /// Create new item
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        TEntity Create(TEntity model);

        /// <summary>
        /// Create new item in exising transaction
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        TEntity CreateNoTransaction(TEntity model);

        /// <summary>
        /// Create new items
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        List<TEntity> Create(List<TEntity> models);

        /// <summary>
        /// Create new items use Bulk
        /// </summary>
        /// <param name="models"></param>
        /// <param name="bulkConfig"></param>
        void CreateBulk(List<TEntity> models, BulkConfig bulkConfig = null);

        /// <summary>
        /// create new items in exising transaction
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        List<TEntity> CreateNoTransaction(List<TEntity> models);

        #endregion

        #region Update

        /// <summary>
        /// update item
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        TEntity Update(TEntity model);

        /// <summary>
        /// update item in exising transaction
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        TEntity UpdateNoTransaction(TEntity model);

        /// <summary>
        /// update items
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        List<TEntity> Update(List<TEntity> models);

        /// <summary>
        /// Update records use Bulk operations
        /// </summary>
        /// <param name="models"></param>
        /// <param name="bulkConfig"></param>
        void UpdateBulk(List<TEntity> models, BulkConfig bulkConfig = null);

        /// <summary>
        /// update items in exising transaction
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        List<TEntity> UpdateNoTransaction(List<TEntity> models);

        /// <summary>
        /// update items found by peredicate applying action
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        List<TEntity> Update(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> includes = null, Action<TEntity> action = null);

        /// <summary>
        /// update items found by peredicate applying action in exising transaction
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        List<TEntity> UpdateNoTransaction(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> includes = null, Action<TEntity> action = null);

        /// <summary>
        /// update items found by predicate applying action in exising transaction with grouping
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="groupSize"></param>
        /// <param name="includes"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        List<TEntity> Update(Expression<Func<TEntity, bool>> predicate, int groupSize, IEnumerable<Expression<Func<TEntity, object>>> includes = null, Action<TEntity> action = null);

        /// <summary>
        /// update items found by predicate applying action in exising transaction with grouping
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="groupSize"></param>
        /// <param name="includes"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        List<TEntity> UpdateNoTransaction(Expression<Func<TEntity, bool>> predicate, int groupSize, IEnumerable<Expression<Func<TEntity, object>>> includes = null, Action<TEntity> action = null);

        #endregion

        #region Delete

        /// <summary>
        /// delete items
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// delete items in exising transaction
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int DeleteNoTransaction(Expression<Func<TEntity, bool>> predicate);

        #endregion

        /// <summary>
        /// count items
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        int Count(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> includes = null);

        /// <summary>
        /// Is transaction started
        /// </summary>
        /// <returns></returns>
        bool IsInTransaction();

        /// <summary>
        /// Is current repository creator of transaction
        /// </summary>
        /// <returns></returns>
        bool IsTransactionOwner();

        /// <summary>
        /// Begin transaction
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Commit transaction
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// Rollback transaction
        /// </summary>
        void RollbackTransaction();
    }
}