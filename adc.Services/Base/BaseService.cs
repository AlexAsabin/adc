using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using EFCore.BulkExtensions;
using Microsoft.Extensions.DependencyInjection;
using adc.Core.Interfaces;
using adc.Dal.Interfaces;

namespace adc.Services.Base {
    public abstract class BaseService {
        protected IMapper Mapper;
        protected IServiceProvider ServiceProvider;

        protected BaseService(IServiceProvider serviceProvider) {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider), "BaseService Constructor");
            Mapper = serviceProvider.GetRequiredService<IMapper>();
        }
    }
    public class BaseService<TEntity, TDto> :
        BaseService,
        IBaseService<TEntity, TDto> where TEntity : class, IEntity where TDto : class, IDto {
        protected IRepository<TEntity> Repository { get; set; }

        #region Ctor

        public BaseService(IServiceProvider serviceProvider) : base(serviceProvider) {
            Repository = serviceProvider.GetRequiredService<IRepository<TEntity>>();
        }

        #endregion

        #region Implementation

        /// <see cref="IBaseService{TEntity,TDto}.CreateIncludes"/>
        public List<Expression<Func<TEntity, object>>> CreateIncludes(
            params Expression<Func<TEntity, object>>[] includes) {
            return Repository.CreateIncludes(includes);
        }

        /// <see cref="IBaseService{TEntity,TDto}.CreateOrderBys"/>
        public List<IOrderBy<TEntity>> CreateOrderBys(params IOrderBy<TEntity>[] orderBys) {
            return Repository.CreateOrderBys(orderBys);
        }

        #region Get

        /// <see cref="IBaseService{TEntity,TDto}.GetSingle"/>
        public virtual TDto GetSingle(
            Expression<Func<TEntity, bool>> predicate,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            IEnumerable<IOrderBy<TEntity>> orderBys = null
        ) {
            return Mapper.Map<TEntity, TDto>(Repository.GetSingle(predicate, includes, orderBys));
        }

        /// <see cref="IBaseService{TEntity,TDto}.GetSingleNoTracking"/>
        public TDto GetSingleNoTracking(Expression<Func<TEntity, bool>> predicate,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            IEnumerable<IOrderBy<TEntity>> orderBys = null) {
            return Mapper.Map<TEntity, TDto>(Repository.GetSingleNoTracking(predicate, includes, orderBys));
        }

        /// <see cref="IBaseService{TEntity,TDto}.GetSingle"/>
        public virtual TRequestedType GetSingle<TRequestedType>(
            Expression<Func<TEntity, bool>> predicate,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            IEnumerable<IOrderBy<TEntity>> orderBys = null,
            Expression<Func<TEntity, TRequestedType>> @select = null
        ) {
            return Repository.GetSingle(predicate, includes, orderBys, select);
        }

        /// <see cref="IBaseService{TEntity,TDto}.GetSingleNoTracking"/>
        public TRequestedType GetSingleNoTracking<TRequestedType>(Expression<Func<TEntity, bool>> predicate,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            IEnumerable<IOrderBy<TEntity>> orderBys = null, Expression<Func<TEntity, TRequestedType>> @select = null) {
            return Repository.GetSingleNoTracking(predicate, includes, orderBys, select);
        }

        /// <see cref="IBaseService{TEntity,TDto}.GetList"/>
        public virtual List<TDto> GetList(
            Expression<Func<TEntity, bool>> predicate,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            IEnumerable<IOrderBy<TEntity>> orderBys = null,
            int? skip = null,
            int? take = null
        ) {
            return Repository.GetList(predicate, includes, orderBys, skip, take).Select(Mapper.Map<TEntity, TDto>)
                .ToList();
        }

        /// <see cref="IBaseService{TEntity,TDto}.GetListNoTracking"/>
        public List<TDto> GetListNoTracking(
            Expression<Func<TEntity, bool>> predicate,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            IEnumerable<IOrderBy<TEntity>> orderBys = null,
            int? skip = null,
            int? take = null
        ) {
            return Repository.GetListNoTracking(predicate, includes, orderBys, skip, take)
                .Select(Mapper.Map<TEntity, TDto>)
                .ToList();
        }

        /// <see cref="IBaseService{TEntity,TDto}.GetList"/>
        public virtual List<TRequestedType> GetList<TRequestedType>(
            Expression<Func<TEntity, bool>> predicate,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            IEnumerable<IOrderBy<TEntity>> orderBys = null,
            Expression<Func<TEntity, TRequestedType>> @select = null,
            int? skip = null,
            int? take = null
        ) {
            return Repository.GetList(predicate, includes, orderBys, select, skip, take);
        }

        /// <see cref="IBaseService{TEntity,TDto}.GetListNoTracking"/>
        public List<TRequestedType> GetListNoTracking<TRequestedType>(
            Expression<Func<TEntity, bool>> predicate,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            IEnumerable<IOrderBy<TEntity>> orderBys = null,
            Expression<Func<TEntity, TRequestedType>> @select = null,
            int? skip = null,
            int? take = null
        ) {
            return Repository.GetListNoTracking(predicate, includes, orderBys, select, skip, take);
        }

        /// <see cref="IBaseService{TEntity,TDto}.GetGroupList{TRequestedType, TKey}(Expression{Func{TEntity, bool}}, Expression{Func{TEntity, TKey}}, Expression{Func{IGrouping{TKey, TEntity}, TRequestedType}} ,IEnumerable{Expression{Func{TEntity, object}}})"/>
        public virtual List<TRequestedType> GetGroupList<TRequestedType, TKey>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TKey>> keySelector,
            Expression<Func<IGrouping<TKey, TEntity>, TRequestedType>> @select,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null
        ) {
            return Repository.GetGroupList(predicate, keySelector, @select, includes);
        }

        #endregion

        #region Create

        /// <see cref="IBaseService{TEntity,TDto}.Create(TDto)"/>
        public virtual TDto Create(TDto model) {
            if (model == null)
                throw new ArgumentNullException("model",
                    $"BaseService<{typeof(TEntity).FullName}, {typeof(TDto).FullName}> Create");
            var entity = Mapper.Map<TDto, TEntity>(model);
            return Mapper.Map<TEntity, TDto>(Repository.Create(entity));
        }

        /// <see cref="IBaseService{TEntity,TDto}.Create(List{TDto})"/>
        public virtual List<TDto> Create(List<TDto> models) {
            if (models == null)
                throw new ArgumentNullException("models",
                    $"BaseService<{typeof(TEntity).FullName}, {typeof(TDto).FullName}> Create");
            var entities = models.Select(Mapper.Map<TDto, TEntity>).ToList();
            return Repository.Create(entities).Select(Mapper.Map<TEntity, TDto>).ToList();
        }

        /// <see cref="IBaseService{TEntity,TDto}.CreateBulk(List{TDto})"/>
        public virtual void CreateBulk(List<TDto> models, BulkConfig bulkConfig = null) {
            if (models == null)
                throw new ArgumentNullException("models", $"BaseService<{typeof(TEntity).FullName}, {typeof(TDto).FullName}> Create");
            var entities = models.Select(Mapper.Map<TDto, TEntity>).ToList();
            Repository.CreateBulk(entities, bulkConfig);
        }

        #endregion

        #region Update

        /// <see cref="IBaseService{TEntity,TDto}.Update(TDto)"/>
        public virtual TDto Update(TDto model) {
            if (model == null)
                throw new ArgumentNullException("model",
                    $"BaseService<{typeof(TEntity).FullName}, {typeof(TDto).FullName}> Update");
            var entity = Mapper.Map<TDto, TEntity>(model);
            return Mapper.Map<TEntity, TDto>(Repository.Update(entity));
        }

        /// <see cref="IBaseService{TEntity,TDto}.Update(List{TDto})"/>
        public virtual List<TDto> Update(List<TDto> models) {
            if (models == null)
                throw new ArgumentNullException("models",
                    $"BaseService<{typeof(TEntity).FullName}, {typeof(TDto).FullName}> Update");
            var entities = models.Select(Mapper.Map<TDto, TEntity>).ToList();
            return Repository.Update(entities).Select(Mapper.Map<TEntity, TDto>).ToList();
        }

        /// <see cref="IBaseService{TEntity,TDto}.Update(Expression{Func{TEntity, bool}}, IEnumerable{Expression{Func{TEntity, object}}}, Action{TEntity})"/>
        public virtual List<TDto> Update(Expression<Func<TEntity, bool>> predicate,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            Action<TEntity> action = null) {
            return Repository.Update(predicate, includes, action).Select(Mapper.Map<TEntity, TDto>).ToList();
        }

        /// <see cref="IBaseService{TEntity,TDto}.Update(Expression{Func{TEntity, bool}}, int, IEnumerable{Expression{Func{TEntity, object}}}, Action{TEntity})"/>
        public virtual List<TDto> Update(Expression<Func<TEntity, bool>> predicate,
            int groupSize,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null,
            Action<TEntity> action = null) {
            return Repository.Update(predicate, groupSize, includes, action).Select(Mapper.Map<TEntity, TDto>).ToList();
        }

        public virtual void UpdateBulk(List<TDto> models, BulkConfig bulkConfig = null) {
            if (models == null)
                throw new ArgumentNullException("models", $"BaseService<{typeof(TEntity).FullName}, {typeof(TDto).FullName}> Update");
            var entities = models.Select(Mapper.Map<TDto, TEntity>).ToList();
            Repository.UpdateBulk(entities, bulkConfig);
        }

        #endregion

        #region delete

        /// <see cref="IBaseService{TEntity,TDto}.Delete"/>
        public virtual int Delete(Expression<Func<TEntity, bool>> predicate) {
            return Repository.Delete(predicate);
        }

        #endregion

        #region count

        /// <see cref="IBaseService{TEntity,TDto}.Count"/>
        public virtual int Count(Expression<Func<TEntity, bool>> predicate,
            IEnumerable<Expression<Func<TEntity, object>>> includes = null) {
            return Repository.Count(predicate, includes);
        }

        #endregion

        #endregion

    }
}