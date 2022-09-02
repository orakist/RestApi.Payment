using System.Linq.Expressions;
using Acme.Foundation.Domain.Entities;

namespace Acme.Foundation.Domain.Repositories;

public abstract class BasicRepositoryBase<TEntity> :
    IBasicRepository<TEntity>
    where TEntity : class, IEntity
{

    public abstract Task<IQueryable<TEntity>> WithDetailsAsync();

    public abstract Task<IQueryable<TEntity>> WithDetailsAsync(params Expression<Func<TEntity, object>>[] propertySelectors);

    public abstract Task<IQueryable<TEntity>> GetQueryableAsync();

    public abstract Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

    public virtual async Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await InsertAsync(entity, cancellationToken: cancellationToken);
        }

        if (autoSave)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    protected abstract Task SaveChangesAsync(CancellationToken cancellationToken);

    public abstract Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

    public virtual async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await UpdateAsync(entity, cancellationToken: cancellationToken);
        }

        if (autoSave)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    public abstract Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

    public virtual async Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await DeleteAsync(entity, cancellationToken: cancellationToken);
        }

        if (autoSave)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    public abstract Task<List<TEntity>> GetListAsync(
        bool includeDetails = false, CancellationToken cancellationToken = default);

    public abstract Task<List<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate, bool includeDetails = false, CancellationToken cancellationToken = default);

    public abstract Task<long> GetCountAsync(CancellationToken cancellationToken = default);

    public abstract Task<List<TEntity>> GetPagedListAsync(
        int skipCount, int maxResultCount, string sorting, bool includeDetails = false, CancellationToken cancellationToken = default);
    
    public abstract Task<int> CountAsync(
        Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default);

    public abstract Task<List<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default);

    public abstract Task<List<TEntity>> GetListAsync(
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, 
        Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default);

    public abstract Task<List<TEntity>> GetListAsync(
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, int skipCount, int maxResultCount, 
        Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default);

    public abstract Task<List<TResult>> GetSelectListAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector, 
        Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default);

    public abstract Task<List<TResult>> GetSelectListAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector, 
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, 
        Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default);

    public abstract Task<List<TResult>> GetSelectListAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector, 
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, int skipCount, int maxResultCount,
        Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default);
}

public abstract class BasicRepositoryBase<TEntity, TKey> : BasicRepositoryBase<TEntity>, IBasicRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    public virtual async Task<TEntity> GetAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, includeDetails, cancellationToken);

        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(TEntity), id);
        }

        return entity;
    }

    public abstract Task<TEntity> FindAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default);

    public virtual async Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, cancellationToken: cancellationToken);
        if (entity == null)
        {
            return;
        }

        await DeleteAsync(entity, autoSave, cancellationToken);
    }

    public async Task DeleteManyAsync(IEnumerable<TKey> ids, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        foreach (var id in ids)
        {
            await DeleteAsync(id, cancellationToken: cancellationToken);
        }

        if (autoSave)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }
}
