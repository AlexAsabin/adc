using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EFCore.BulkExtensions;
using adc.Core.Interfaces;
using adc.Dal.Interfaces;

namespace adc.Dal.Implementation {
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity {

        #region Includes
        /// <see cref="IRepository{TEntity}.CreateIncludes"/>
        public List<Expression<Func<TEntity, object>>> CreateIncludes(params Expression<Func<TEntity, object>>[] includes) {
            return includes?.ToList();
        }

        #endregion

        #region OrderBy
        /// <see cref="IRepository{TEntity}.CreateOrderBys"/>
        public List<IOrderBy<TEntity>> CreateOrderBys(params IOrderBy<TEntity>[] orderBys) {
            return orderBys?.ToList();
        }

        #endregion

        #region Get

        /// <see cref="IRepository{TEntity}.GetSingle" /> 
        public TEntity GetSingle(
            Expression<Func<TEntity, bool>> predicate,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            IEnumerable<IOrderBy<TEntity>> orderBys = null
        ) {
            return Query(predicate, includes, orderBys).FirstOrDefault();
        }

        /// <see cref="IRepository{TEntity}.GetSingleNoTracking" /> 
        public abstract TEntity GetSingleNoTracking(Expression<Func<TEntity, bool>> predicate,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            IEnumerable<IOrderBy<TEntity>> orderBys = null);

        /// <see cref="IRepository{TEntity}.GetSingle" /> 
        public TRequestedType GetSingle<TRequestedType>(
            Expression<Func<TEntity, bool>> predicate,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            IEnumerable<IOrderBy<TEntity>> orderBys = null,
            Expression<Func<TEntity, TRequestedType>> @select = null
        ) {
            return SelectRequestedTypes(Query(predicate, includes, orderBys), select).FirstOrDefault();
        }

        /// <see cref="IRepository{TEntity}.GetSingleNoTracking" /> 
        public abstract TRequestedType GetSingleNoTracking<TRequestedType>(Expression<Func<TEntity, bool>> predicate,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            IEnumerable<IOrderBy<TEntity>> orderBys = null, Expression<Func<TEntity, TRequestedType>> @select = null);

        /// <see cref="IRepository{TEntity}.GetList"/>
        public List<TEntity> GetList(
            Expression<Func<TEntity, bool>> predicate,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            IEnumerable<IOrderBy<TEntity>> orderBys = null,
            int? skip = null,
            int? take = null
        ) {
            return Query(predicate, includes, orderBys, skip, take).ToList();
        }

        /// <see cref="IRepository{TEntity}.GetListNoTracking"/>
        public abstract List<TEntity> GetListNoTracking(Expression<Func<TEntity, bool>> predicate,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            IEnumerable<IOrderBy<TEntity>> orderBys = null, int? skip = null,
            int? take = null);

        /// <see cref="IRepository{TEntity}.GetList"/>
        public List<TRequestedType> GetList<TRequestedType>(
            Expression<Func<TEntity, bool>> predicate,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            IEnumerable<IOrderBy<TEntity>> orderBys = null,
            Expression<Func<TEntity, TRequestedType>> @select = null,
            int? skip = null,
            int? take = null
        ) {
            return SelectRequestedTypes(Query(predicate, includes, orderBys, skip, take), select).ToList();
        }

        /// <see cref="IRepository{TEntity}.GetListNoTracking"/>
        public abstract List<TRequestedType> GetListNoTracking<TRequestedType>(Expression<Func<TEntity, bool>> predicate,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            IEnumerable<IOrderBy<TEntity>> orderBys = null,
            Expression<Func<TEntity, TRequestedType>> @select = null, int? skip = null, int? take = null);

        /// <see cref="IRepository{TEntity}.GetGroupList{TRequestedType, TKey}"/>
        public List<TRequestedType> GetGroupList<TRequestedType, TKey>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TKey>> keySelector,
            Expression<Func<IGrouping<TKey, TEntity>, TRequestedType>> @select,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null
        ) {
            var groupQuery = Query(predicate, includes, null).GroupBy(keySelector);
            return SelectRequestedTypes(groupQuery, select).ToList();
        }

        #endregion

        #region Create

        /// <see cref="IRepository{TEntity}.Create(TEntity)"/>
        public TEntity Create(TEntity model) {
            return Create(new List<TEntity> { model }).FirstOrDefault();
        }

        /// <see cref="IRepository{TEntity}.Create(List{TEntity})"/>
        public List<TEntity> Create(List<TEntity> models) {
            if (models == null) throw new ArgumentNullException("models", $"Repository<{typeof(TEntity).FullName}>.Create ");
            try {
                if (!IsInTransaction()) BeginTransaction();
                var result = CreateNoTransaction(models);
                if (IsTransactionOwner()) CommitTransaction();
                return result;
            } catch {
                if (IsTransactionOwner()) RollbackTransaction();
                throw;
            }
        }

        /// <see cref="IRepository{TEntity}.CreateNoTransaction(TEntity)"/>
        public TEntity CreateNoTransaction(TEntity model) {
            return CreateNoTransaction(new List<TEntity> { model }).FirstOrDefault();
        }

        /// <see cref="IRepository{TEntity}.CreateNoTransaction(List{TEntity})"/>
        public abstract List<TEntity> CreateNoTransaction(List<TEntity> models);

        /// <see cref="IRepository{TEntity}.CreateBulk(List{TEntity}, BulkConfig)"/>
        public abstract void CreateBulk(List<TEntity> models, BulkConfig bulkConfig = null);

        #endregion

        #region Update

        /// <see cref="IRepository{TEntity}.Update(TEntity)"/>
        public TEntity Update(TEntity model) {
            return Update(new List<TEntity> { model }).FirstOrDefault();
        }

        /// <see cref="IRepository{TEntity}.Update(List{TEntity})"/>
        public List<TEntity> Update(List<TEntity> models) {
            if (models == null) throw new ArgumentNullException("models", $"Repository<{typeof(TEntity).FullName}>.Update ");
            try {
                if (!IsInTransaction()) BeginTransaction();
                var result = UpdateNoTransaction(models);
                if (IsTransactionOwner()) CommitTransaction();
                return result;
            } catch {
                if (IsTransactionOwner()) RollbackTransaction();
                throw;
            }
        }

        /// <see cref="IRepository{TEntity}.UpdateBulk(List{TEntity}, BulkConfig)"/>
        public abstract void UpdateBulk(List<TEntity> models, BulkConfig bulkConfig = null);

        /// <see cref="IRepository{TEntity}.Update(Expression{Func{TEntity, bool}}, IEnumerable{Expression{Func{TEntity, object}}}, Action{TEntity})"/>
        public List<TEntity> Update(
            Expression<Func<TEntity, bool>> predicate,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            Action<TEntity> action = null) {
            if (predicate == null) throw new ArgumentNullException("predicate", $"Repository<{typeof(TEntity).FullName}>.Update ");
            try {
                if (!IsInTransaction()) BeginTransaction();
                var result = UpdateNoTransaction(predicate, includes, action);
                if (IsTransactionOwner()) CommitTransaction();
                return result;
            } catch {
                if (IsTransactionOwner()) RollbackTransaction();
                throw;
            }

        }

        /// <see cref="IRepository{TEntity}.UpdateNoTransaction(TEntity)"/>
        public TEntity UpdateNoTransaction(TEntity model) {
            return UpdateNoTransaction(new List<TEntity> { model }).FirstOrDefault();
        }

        /// <see cref="IRepository{TEntity}.UpdateNoTransaction(List{TEntity})"/>
        public abstract List<TEntity> UpdateNoTransaction(List<TEntity> models);

        /// <see cref="IRepository{TEntity}.UpdateNoTransaction(Expression{Func{TEntity, bool}}, IEnumerable{Expression{Func{TEntity, object}}}, Action{TEntity})"/>
        public abstract List<TEntity> UpdateNoTransaction(
            Expression<Func<TEntity, bool>> predicate,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            Action<TEntity> action = null
        );

        /// <see cref="IRepository{TEntity}.Update(Expression{Func{TEntity, bool}}, int, IEnumerable{Expression{Func{TEntity, object}}}, Action{TEntity})"/>
        public List<TEntity> Update(Expression<Func<TEntity, bool>> predicate, int groupSize, IEnumerable<Expression<Func<TEntity, object>>> includes = null, Action<TEntity> action = null) {
            if (predicate == null)
                throw new ArgumentNullException("predicate", $"Repository<{typeof(TEntity).FullName}>.Update ");
            try {
                if (!IsInTransaction()) BeginTransaction();
                var result = UpdateNoTransaction(predicate, groupSize, includes, action);
                if (IsTransactionOwner()) CommitTransaction();
                return result;
            } catch {
                if (IsTransactionOwner()) RollbackTransaction();
                throw;
            }
        }

        /// <see cref="IRepository{TEntity}"/>
        public abstract List<TEntity> UpdateNoTransaction(
            Expression<Func<TEntity, bool>> predicate,
            int groupSize,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            Action<TEntity> action = null
        );

        #endregion

        #region Delete

        /// <see cref="IRepository{TEntity}.Delete"/>
        public int Delete(Expression<Func<TEntity, bool>> predicate) {
            var count = 0;
            try {
                if (!IsInTransaction()) BeginTransaction();
                count = DeleteNoTransaction(predicate);
                if (IsTransactionOwner()) CommitTransaction();
            } catch {
                if (IsTransactionOwner()) RollbackTransaction();
                throw;
            }
            return count;
        }

        /// <see cref="IRepository{TEntity}.DeleteNoTransaction"/>
        public abstract int DeleteNoTransaction(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region Count

        /// <see cref="IRepository{TEntity}.Count"/>
        public int Count(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> includes = null) {
            return Query(predicate, includes).Count();
        }

        #endregion

        #region Query
        /// <summary>
        /// query builder
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includeList"></param>
        /// <param name="orderBys"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        protected abstract IQueryable<TEntity> Query(
            Expression<Func<TEntity, bool>> predicate = null,
            IEnumerable<Expression<Func<TEntity, object>>> includeList = null,
            IEnumerable<IOrderBy<TEntity>> orderBys = null,
            int? skip = null,
            int? take = null
        );

        #endregion

        #region Transaction

        /// <see cref="IRepository{TEntity}.IsInTransaction"/>
        public abstract bool IsInTransaction();

        /// <see cref="IRepository{TEntity}.IsTransactionOwner"/>
        public abstract bool IsTransactionOwner();

        /// <see cref="IRepository{TEntity}.BeginTransaction"/>
        ///
        public abstract void BeginTransaction();

        /// <see cref="IRepository{TEntity}.CommitTransaction"/>
        public abstract void CommitTransaction();

        /// <see cref="IRepository{TEntity}.RollbackTransaction"/>
        public abstract void RollbackTransaction();

        #endregion

        #region Helpers

        /// <summary>
        /// Converting to TRequestedType
        /// </summary>
        /// <typeparam name="TRequestedType"></typeparam>
        /// <param name="query"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        protected IQueryable<TRequestedType> SelectRequestedTypes<TRequestedType>(
            IQueryable<TEntity> query,
            Expression<Func<TEntity, TRequestedType>> select
        ) {
            if (select == null) {
                throw new NullReferenceException("The variable select cannot be null!");
            }
            return query.Select(select);
        }

        /// <summary>
        /// Converting to TRequestedType
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TRequestedType"></typeparam>
        /// <param name="query"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        private IQueryable<TRequestedType> SelectRequestedTypes<TKey, TRequestedType>(
            IQueryable<IGrouping<TKey, TEntity>> query,
            Expression<Func<IGrouping<TKey, TEntity>, TRequestedType>> select
        ) {
            if (select == null) {
                throw new NullReferenceException("The variable select cannot be null!");
            }
            return query.Select(select);
        }

        #endregion
    }
}
