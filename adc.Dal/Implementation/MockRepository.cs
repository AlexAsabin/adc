using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EFCore.BulkExtensions;
using adc.Core.Interfaces;
using adc.Dal.Interfaces;
using adc.Dal.Implementation;

namespace Start.Dal.Implementation {
    public class MockRepository<TEntity> : Repository<TEntity> where TEntity : class, IEntity {

        #region Fields/Properties

        private bool _transactionCreator = false;
        public List<TEntity> Data { get; protected set; }

        #endregion

        #region Constructor

        public MockRepository() : this(null) {
        }

        public MockRepository(List<TEntity> data = null) {
            InitData(data);
        }

        #endregion

        public void InitData(List<TEntity> data) {
            Data = data ?? new List<TEntity>();
        }

        #region Implementation

        /// <see cref="IRepository{TEntity}.GetSingleNoTracking"/>
        public override TEntity GetSingleNoTracking(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> includes = null, IEnumerable<IOrderBy<TEntity>> orderBys = null) {
            return GetSingle(predicate, includes, orderBys);
        }

        /// <see cref="IRepository{TRequestedType}.GetSingleNoTracking"/>
        public override TRequestedType GetSingleNoTracking<TRequestedType>(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            IEnumerable<IOrderBy<TEntity>> orderBys = null, Expression<Func<TEntity, TRequestedType>> @select = null) {
            return GetSingle(predicate, includes, orderBys, select);
        }

        /// <see cref="IRepository{TEntity}.GetListNoTracking"/>
        public override List<TEntity> GetListNoTracking(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> includes = null, IEnumerable<IOrderBy<TEntity>> orderBys = null, int? skip = null,
            int? take = null) {
            return GetList(predicate, includes, orderBys, skip, take);
        }

        /// <see cref="IRepository{TRequestedType}.GetListNoTracking"/>
        public override List<TRequestedType> GetListNoTracking<TRequestedType>(Expression<Func<TEntity, bool>> predicate, IEnumerable<Expression<Func<TEntity, object>>> includes = null, IEnumerable<IOrderBy<TEntity>> orderBys = null,
            Expression<Func<TEntity, TRequestedType>> @select = null, int? skip = null, int? take = null) {
            return GetList(predicate, includes, orderBys, select, skip, take);
        }

        /// <see cref="IRepository{TEntity}.CreateNoTransaction(List{TEntity})"/>
        public override List<TEntity> CreateNoTransaction(List<TEntity> models) {
            if (models == null) {
                throw new ArgumentNullException("models", $"MockRepository<{typeof(TEntity).FullName}>.{nameof(CreateNoTransaction)}");
            }

            for (var i = 0; i < models.Count; i++) {
                if (models[i] is IId<int>) {
                    (models[i] as IId<int>).Id = Data.Count + i + 1;
                }
            }
            Data.AddRange(models);
            return models;
        }

        /// <see cref="IRepository{TEntity}.UpdateNoTransaction(List{TEntity})"/>
        public override List<TEntity> UpdateNoTransaction(List<TEntity> models) {
            if (models == null) {
                throw new ArgumentNullException("models", $"MockRepository<{typeof(TEntity).FullName}>.{nameof(UpdateNoTransaction)}");
            }

            if (models.Any()) {
                if (models.FirstOrDefault() is IId<int>) {
                    foreach (var model in models) {
                        var found = Data.FindIndex(x => (model as IId<int>)?.Id == (x as IId<int>)?.Id);
                        if (found != -1) {
                            Data[found] = model;
                        } else {
                            throw new ArgumentException("Incorrect model.");
                        }
                    }
                }
            }

            return models;
        }

        /// <see cref="IRepository{TEntity}.UpdateNoTransaction(Expression{Func{TEntity, bool}}, IEnumerable{Expression{Func{TEntity, object}}}, Action{TEntity})"/>
        public override List<TEntity> UpdateNoTransaction(Expression<Func<TEntity, bool>> predicate,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            Action<TEntity> action = null) {
            var items = this.Query(predicate, includes, null).ToList();
            items.ForEach(x => action(x));
            return items;
        }

        /// <see cref="IRepository{TEntity}.UpdateNoTransaction(Expression{Func{TEntity, bool}}, int, IEnumerable{Expression{Func{TEntity, object}}}, Action{TEntity})"/>
        public override List<TEntity> UpdateNoTransaction(Expression<Func<TEntity, bool>> predicate, int groupSize, IEnumerable<Expression<Func<TEntity, object>>> includes = null, Action<TEntity> action = null) {
            var items = this.Query(predicate, includes, null).ToList();
            items.ForEach(x => action(x));
            return items;
        }

        /// <see cref="IRepository{TEntity}.DeleteNoTransaction"/>
        public override int DeleteNoTransaction(Expression<Func<TEntity, bool>> predicate) {
            if (predicate == null) {
                throw new ArgumentNullException("predicate", $"MockRepository<{typeof(TEntity).FullName}>.{nameof(DeleteNoTransaction)}");
            }

            var items = Data.AsQueryable().Where(predicate);
            int itemsCount = items.Count();

            if (items.Any()) {
                List<int> indexes = items.Select(x => Data.IndexOf(x)).Reverse().ToList();
                indexes.ForEach(x => Data.RemoveAt(x));
            }
            return itemsCount;
        }

        /// <see cref="Repository{TEntity}.Query"/>
        protected override IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate = null,
            IEnumerable<Expression<Func<TEntity, object>>> includeList = null, // 
            IEnumerable<IOrderBy<TEntity>> orderBys = null, int? skip = null, int? take = null) {
            var query = Data.AsQueryable();
            if (predicate != null) {
                query = query.Where(predicate);
            }
            if (orderBys != null) {
                IOrderedQueryable<TEntity> ordered = null;
                foreach (var current in orderBys) {
                    ordered = ordered == null ? current.Apply(query) : current.Apply(ordered);
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

        /// <see cref="IRepository{TEntity}.IsInTransaction"/>
        public override bool IsInTransaction() {
            return MockTransactionHolder.IsInTransaction;
        }

        /// <see cref="IRepository{TEntity}.IsTransactionOwner"/>
        public override bool IsTransactionOwner() {
            return _transactionCreator;
        }

        /// <see cref="IRepository{TEntity}.BeginTransaction"/>
        public override void BeginTransaction() {
            if (!MockTransactionHolder.IsInTransaction) {
                MockTransactionHolder.IsInTransaction = true;
                _transactionCreator = true;
            }
        }

        /// <see cref="IRepository{TEntity}.CommitTransaction"/>
        public override void CommitTransaction() {
            if (_transactionCreator) {
                MockTransactionHolder.IsInTransaction = false;
                _transactionCreator = false;
            }
        }

        /// <see cref="IRepository{TEntity}.RollbackTransaction"/>
        public override void RollbackTransaction() {
            MockTransactionHolder.IsInTransaction = false;
            _transactionCreator = false;
        }

        public override void CreateBulk(List<TEntity> models, BulkConfig bulkConfig = null) {
            Create(models);
        }

        public override void UpdateBulk(List<TEntity> models, BulkConfig bulkConfig = null) {
            Update(models);
        }

        #endregion
    }
}
