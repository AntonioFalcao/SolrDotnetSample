using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using SolrDotnetSample.Domain.Abstractions;
using SolrDotnetSample.Repositories.Abstractions;

namespace SolrDotnetSample.Services.Abstractions
{
    public abstract class Service<TEntity, TModel, TId> : IService<TEntity, TId>
        where TEntity : Entity<TId>
        where TModel : Model<TId>
        where TId : struct
    {
        private readonly IMapper _mapper;
        private readonly IRepository<TModel, TId> _noSqlRepository;

        protected Service(IRepository<TModel, TId> noSqlRepository, IMapper mapper)
        {
            _noSqlRepository = noSqlRepository;
            _mapper = mapper;
        }

        public void Delete(TId id)
        {
            if (Equals(id, default)) return;
            _noSqlRepository.Delete(id);
        }

        public async Task DeleteAsync(TId id, CancellationToken cancellationToken)
        {
            if (Equals(id, default)) return;
            await _noSqlRepository.DeleteAsync(id, cancellationToken);
        }

        public TEntity Edit(TEntity entity)
        {
            if (entity.Valid == false) return entity;
            var model = _mapper.Map<TModel>(entity);
            _noSqlRepository.Update(model);
            return entity;
        }

        public async Task<TEntity> EditAsync(TEntity entity, CancellationToken cancellationToken)
        {
            if (entity.Valid == false) return entity;
            var model = _mapper.Map<TModel>(entity);
            await _noSqlRepository.UpdateAsync(model, cancellationToken);
            return entity;
        }

        public bool Exists(TId id)
            => Equals(id, default) ? default : _noSqlRepository.Exists(id);

        public async Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken)
            => Equals(id, default) ? default : await _noSqlRepository.ExistsAsync(id, cancellationToken);

        public TEntity GetById(TId id)
        {
            if (Equals(id, default)) return default;
            var model = _noSqlRepository.SelectById(id);
            return _mapper.Map<TEntity>(model);
        }

        public async Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken)
        {
            if (Equals(id, default)) return default;
            var models = await _noSqlRepository.SelectByIdAsync(id, cancellationToken);
            return _mapper.Map<TEntity>(models);
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate is null) return default;
            var mapPredicate = _mapper.MapExpression<Expression<Func<TModel, bool>>>(predicate);
            var models = _noSqlRepository.SelectAll(mapPredicate);
            return _mapper.Map<IEnumerable<TEntity>>(models);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            if (predicate is null) return default;
            var mapPredicate = _mapper.MapExpression<Expression<Func<TModel, bool>>>(predicate);
            var models = await _noSqlRepository.SelectAllAsync(mapPredicate, cancellationToken);
            return _mapper.Map<IEnumerable<TEntity>>(models);
        }

        public TEntity Save(TEntity entity)
        {
            if (entity.Valid == false) return entity;
            var model = _mapper.Map<TModel>(entity);
            _noSqlRepository.Insert(model);
            return entity;
        }

        public async Task<TEntity> SaveAsync(TEntity entity, CancellationToken cancellationToken)
        {
            if (entity.Valid == false) return entity;
            var model = _mapper.Map<TModel>(entity);
            if (entity.Valid) await _noSqlRepository.InsertAsync(model, cancellationToken);
            return entity;
        }

        public IEnumerable<TEntity> SaveMany(IEnumerable<TEntity> entities)
        {
            entities = entities as TEntity[] ?? entities.ToArray();
            if (entities.Any() == false || entities.Any(x => x.Valid) == false) return entities;
            var models = _mapper.Map<IEnumerable<TModel>>(entities);
            _noSqlRepository.InsertMany(models);
            return entities;
        }

        public async Task<IEnumerable<TEntity>> SaveManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            entities = entities as TEntity[] ?? entities.ToArray();
            if (entities.Any() == false || entities.Any(x => x.Valid) == false) return entities;
            var models = _mapper.Map<IEnumerable<TModel>>(entities);
            await _noSqlRepository.InsertManyAsync(models, cancellationToken);
            return entities;
        }
    }
}