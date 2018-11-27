using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using adc.Core.Interfaces;
using adc.Dal.Interfaces;

namespace adc.Dal.Implementation {
    public class DbRepository<TEntity> : Repository<TEntity> where TEntity : class, IEntity {

        #region Properties
        private readonly AdcDbContext _dbContext;
        private IDbContextTransaction _dbTransaction;
        #endregion

        #region Constructor
        public DbRepository(AdcDbContext dbContext) {
            _dbContext = dbContext ?? throw new ArgumentNullException("dbContext", $"DbRepository<{typeof(TEntity).FullName}> Constructor");
            _dbTransaction = null;
        }
        #endregion

        #region Implementation

        /// <see cref="IRepository{TEntity}.GetSingleNoTracking"/>
        public override TEntity GetSingleNoTracking(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> includes = null, IEnumerable<IOrderBy<TEntity>> orderBys = null) {
            return Query(predicate, includes, orderBys).AsNoTracking().FirstOrDefault();
        }

        /// <see cref="IRepository{TRequestedType}.GetSingleNoTracking"/>
        public override TRequestedType GetSingleNoTracking<TRequestedType>(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            IEnumerable<IOrderBy<TEntity>> orderBys = null, Expression<Func<TEntity, TRequestedType>> @select = null) {
            return SelectRequestedTypes(Query(predicate, includes, orderBys).AsNoTracking(), select).FirstOrDefault();
        }

        /// <see cref="IRepository{TEntity}.GetListNoTracking"/>
        public override List<TEntity> GetListNoTracking(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> includes = null, IEnumerable<IOrderBy<TEntity>> orderBys = null, int? skip = null,
            int? take = null) {
            return Query(predicate, includes, orderBys, skip, take).AsNoTracking().ToList();
        }

        /// <see cref="IRepository{TRequestedType}.GetListNoTracking"/>
        public override List<TRequestedType> GetListNoTracking<TRequestedType>(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> includes = null, IEnumerable<IOrderBy<TEntity>> orderBys = null,
            Expression<Func<TEntity, TRequestedType>> @select = null, int? skip = null, int? take = null) {
            return SelectRequestedTypes(Query(predicate, includes, orderBys, skip, take).AsNoTracking(), select).ToList();
        }

        /// <see cref="IRepository{TEntity}.CreateNoTransaction(List{TEntity})"/>
        public override List<TEntity> CreateNoTransaction(List<TEntity> models) {
            var dbSet = _dbContext.Set<TEntity>();
            dbSet.AddRange(models);
            SaveChanges();
            return models;
        }

        /// <see cref="IRepository{TEntity}.CreateBulk(List{TEntity}, BulkConfig)"/>
        public override void CreateBulk(List<TEntity> models, BulkConfig bulkConfig = null) {
            using (var transaction = _dbContext.Database.BeginTransaction()) {
                _dbContext.BulkInsert(models, bulkConfig);
                transaction.Commit();
            }
        }

        /// <see cref="IRepository{TEntity}.UpdateBulk(List{TEntity}, BulkConfig)"/>
        public override void UpdateBulk(List<TEntity> models, BulkConfig bulkConfig = null) {
            using (var transaction = _dbContext.Database.BeginTransaction()) {
                _dbContext.BulkUpdate(models, bulkConfig);
                transaction.Commit();
            }
        }

        /// <see cref="IRepository{TEntity}.UpdateNoTransaction(List{TEntity})"/>
        public override List<TEntity> UpdateNoTransaction(List<TEntity> models) {
            var dbSet = _dbContext.Set<TEntity>();
            foreach (var model in models) {
                dbSet.Attach(model);
                _dbContext.Entry(model).State = EntityState.Modified;
            }
            SaveChanges();
            return models;
        }

        /// <see cref="IRepository{TEntity}.UpdateNoTransaction(Expression{Func{TEntity, bool}}, IEnumerable{Expression{Func{TEntity, object}}}, Action{TEntity})"/>
        public override List<TEntity> UpdateNoTransaction(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> includes = null, Action<TEntity> action = null) {
            var items = Query(predicate, includes).ToList();
            if (action != null) {
                foreach (var entity in items) {
                    action(entity);
                    _dbContext.Entry(entity).State = EntityState.Modified;
                }
                SaveChanges();
            }
            return items;
        }

        /// <see cref="IRepository{TEntity}.UpdateNoTransaction(Expression{Func{TEntity, bool}}, int, IEnumerable{Expression{Func{TEntity, object}}}, Action{TEntity})"/>
        public override List<TEntity> UpdateNoTransaction(Expression<Func<TEntity, bool>> predicate, int groupSize,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null, Action<TEntity> action = null) {
            var items = new List<TEntity>();
            var start = 0;
            var groupItems = Query(predicate, includes, skip: start * groupSize, take: groupSize).ToList();
            while (groupItems.Count > 0) {
                foreach (var entity in groupItems) {
                    action?.Invoke(entity);
                    _dbContext.Entry(entity).State = EntityState.Modified;
                }
                SaveChanges();
                start++;
                items.AddRange(groupItems);
                groupItems = Query(predicate, includes, skip: start * groupSize, take: groupSize).ToList();
            }
            return items;
        }

        /// <see cref="IRepository{TEntity}.DeleteNoTransaction"/>
        public override int DeleteNoTransaction(Expression<Func<TEntity, bool>> predicate) {
            var itemCount = 0;
            var dbSet = _dbContext.Set<TEntity>();
            var items = dbSet.Where(predicate);
            if (items.Any()) {
                dbSet.RemoveRange(items);
                itemCount = items.Count();
                SaveChanges();
            }
            return itemCount;
        }

        /// <see cref="Repository{TEntity}.Query"/>
        protected override IQueryable<TEntity> Query(
            Expression<Func<TEntity, bool>> predicate = null,
            IEnumerable<Expression<Func<TEntity, object>>> includeList = null,
            IEnumerable<IOrderBy<TEntity>> orderBys = null,
            int? skip = null,
            int? take = null
        ) {
            return Query(_dbContext, predicate, includeList, orderBys, skip, take);
        }

        /// <see cref="IRepository{TEntity}.IsInTransaction"/>
        public override bool IsInTransaction() {
            return _dbContext.Database.CurrentTransaction != null;
        }

        /// <see cref="IRepository{TEntity}.IsTransactionOwner"/>
        public override bool IsTransactionOwner() {
            return _dbTransaction != null;
        }

        /// <see cref="IRepository{TEntity}.BeginTransaction"/>
        public override void BeginTransaction() {
            if (!IsInTransaction()) {
                _dbTransaction = _dbContext.Database.BeginTransaction();
            }
        }

        /// <see cref="Repository{TEntity}.CommitTransaction"/>
        public override void CommitTransaction() {
            if (_dbTransaction != null) {
                _dbTransaction.Commit();
                _dbTransaction.Dispose();
            }
            _dbTransaction = null;
        }

        /// <see cref="IRepository{TEntity}.RollbackTransaction"/>
        public override void RollbackTransaction() {
            if (_dbTransaction != null) {
                _dbTransaction.Rollback();
                _dbTransaction.Dispose();
            }
            _dbTransaction = null;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Query builder in specified DB context
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="predicate"></param>
        /// <param name="includeList"></param>
        /// <param name="orderBys"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        private IQueryable<TEntity> Query(
            AdcDbContext dbContext,
            Expression<Func<TEntity, bool>> predicate = null,
            IEnumerable<Expression<Func<TEntity, object>>> includeList = null,
            IEnumerable<IOrderBy<TEntity>> orderBys = null,
            int? skip = null,
            int? take = null
        ) {
            var query = dbContext.Set<TEntity>().AsQueryable();
            if (includeList != null) {
                foreach (var expression in includeList) {
                    if (IncludesBuilder.TryParsePath(expression.Body, out string path) && !string.IsNullOrEmpty(path)) {
                        query = query.Include(path);
                    }
                }
            }
            if (predicate != null) {
                query = query.Where(predicate);
            }
            if (orderBys != null) {
                IOrderedQueryable<TEntity> ordered = null;
                foreach (var orderBy in orderBys) {
                    ordered = ordered == null ? orderBy.Apply(query) : orderBy.Apply(ordered);
                }
                query = ordered ?? query;
            }
            if (skip.HasValue) {
                query = query.Skip(skip.Value);
            }
            if (take.HasValue) {
                query = query.Take(take.Value);
            }
            return query;
        }

        private void SaveChanges() {
            _dbContext.SaveChanges();
        }

        #endregion
    }
}
