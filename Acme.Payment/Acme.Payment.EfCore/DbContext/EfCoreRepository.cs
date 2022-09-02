using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Acme.Foundation.Domain.Entities;
using Acme.Foundation.Domain.Repositories;

namespace Acme.Payment.EfCore.DbContext;

public class EfCoreRepository<TEntity, TKey> : RepositoryBase<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
{
    private readonly Microsoft.EntityFrameworkCore.DbContext _dbContext;

    public EfCoreRepository(Microsoft.EntityFrameworkCore.DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected Task<Microsoft.EntityFrameworkCore.DbContext> GetDbContextAsync()
    {
        return Task.FromResult(_dbContext);
    }

    protected Task<DbSet<TEntity>> GetDbSetAsync()
    {
        return Task.FromResult(_dbContext.Set<TEntity>());
    }

    public override async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        CheckAndSetId(entity);

        var dbContext = await GetDbContextAsync();

        var savedEntity = (await dbContext.Set<TEntity>().AddAsync(entity, cancellationToken)).Entity;

        if (autoSave)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return savedEntity;
    }

    public override async Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var entityArray = entities.ToArray();
        var dbContext = await GetDbContextAsync();

        foreach (var entity in entityArray)
        {
            CheckAndSetId(entity);
        }

        await dbContext.Set<TEntity>().AddRangeAsync(entityArray, cancellationToken);

        if (autoSave)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public override async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        dbContext.Attach(entity);
        var updatedEntity = dbContext.Update(entity).Entity;

        if (autoSave)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return updatedEntity;
    }

    public override async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        dbContext.Set<TEntity>().UpdateRange(entities);

        if (autoSave)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public override async Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        dbContext.Set<TEntity>().Remove(entity);

        if (autoSave)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public override async Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        dbContext.RemoveRange(entities);

        if (autoSave)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public override async Task<List<TEntity>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        return includeDetails
            ? await (await WithDetailsAsync()).ToListAsync(cancellationToken)
            : await (await GetDbSetAsync()).ToListAsync(cancellationToken);
    }

    public override async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        return includeDetails
            ? await (await WithDetailsAsync()).Where(predicate).ToListAsync(cancellationToken)
            : await (await GetDbSetAsync()).Where(predicate).ToListAsync(cancellationToken);
    }

    public override async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync()).LongCountAsync(cancellationToken);
    }

    public override async Task<List<TEntity>> GetPagedListAsync(
        int skipCount,
        int maxResultCount,
        string sorting,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var queryable = includeDetails
            ? await WithDetailsAsync()
            : await GetDbSetAsync();

        return await queryable
            .OrderBy(sorting)
            .Skip(skipCount).Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public override async Task<IQueryable<TEntity>> GetQueryableAsync()
    {
        return (await GetDbSetAsync()).AsQueryable();
    }

    protected override async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await (await GetDbContextAsync()).SaveChangesAsync(cancellationToken);
    }

    public override async Task<TEntity> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool includeDetails = true,
        CancellationToken cancellationToken = default)
    {
        return includeDetails
            ? await (await WithDetailsAsync())
                .Where(predicate)
                .SingleOrDefaultAsync(cancellationToken)
            : await (await GetDbSetAsync())
                .Where(predicate)
                .SingleOrDefaultAsync(cancellationToken);
    }

    public override async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        var dbSet = dbContext.Set<TEntity>();

        var entities = await dbSet
            .Where(predicate)
            .ToListAsync(cancellationToken);

        await DeleteManyAsync(entities, autoSave, cancellationToken);

        if (autoSave)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public override async Task<IQueryable<TEntity>> WithDetailsAsync()
    {
        return await GetQueryableAsync();
    }

    public override async Task<IQueryable<TEntity>> WithDetailsAsync(params Expression<Func<TEntity, object>>[] propertySelectors)
    {
        return IncludeDetails(
            await GetQueryableAsync(),
            propertySelectors
        );
    }

    private static IQueryable<TEntity> IncludeDetails(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, object>>[] propertySelectors)
    {
        if (propertySelectors is { Length: > 0 })
        {
            foreach (var propertySelector in propertySelectors)
            {
                query = query.Include(propertySelector);
            }
        }

        return query;
    }

    protected virtual void CheckAndSetId(TEntity entity)
    {
        if (entity is IEntity<Guid> entityWithGuidId)
        {
            TrySetGuidId(entityWithGuidId);
        }
    }

    protected virtual void TrySetGuidId(IEntity<Guid> entity)
    {
        if (entity.Id != default)
        {
            return;
        }

        entity.Id = Guid.NewGuid();
    }

    public override async Task<int> CountAsync(
        Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default)
    {
        var query = await GetQueryableAsync();
        return await query.Where(predicate).CountAsync(cancellation);
    }

    public override async Task<List<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default)
    {
        var query = await GetQueryableAsync();

        return await query.Where(predicate).AsNoTracking()
            .ToListAsync(cancellation);
    }

    public override async Task<List<TEntity>> GetListAsync(
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellation = default)
    {
        var query = await GetQueryableAsync();

        query = query.Where(predicate);

        return await orderBy(query)
            .AsNoTracking()
            .ToListAsync(cancellation);
    }

    public override async Task<List<TEntity>> GetListAsync(
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
        int skipCount, int maxResultCount, Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellation = default)
    {
        var query = await GetQueryableAsync();

        query = query.Where(predicate);

        return await orderBy(query)
            .Skip(skipCount).Take(maxResultCount)
            .AsNoTracking()
            .ToListAsync(cancellation);
    }

    public override async Task<List<TResult>> GetSelectListAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellation = default)
    {
        var query = await GetQueryableAsync();

        return await query.Where(predicate).Select(selector).ToListAsync(cancellation);
    }

    public override async Task<List<TResult>> GetSelectListAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellation = default)
    {
        var query = await GetQueryableAsync();
        query = query.Where(predicate);

        return await orderBy(query).Select(selector).ToListAsync(cancellation);
    }

    public override async Task<List<TResult>> GetSelectListAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
        int skipCount, int maxResultCount, Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellation = default)
    {
        var query = await GetQueryableAsync();
        query = query.Where(predicate);

        return await orderBy(query).Select(selector)
            .Skip(skipCount).Take(maxResultCount)
            .ToListAsync(cancellation);
    }

    public override async Task<TEntity> GetAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, includeDetails, cancellationToken);

        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(TEntity), id);
        }

        return entity;
    }

    public override async Task<TEntity> FindAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        return includeDetails
            ? await (await WithDetailsAsync()).OrderBy(e => e.Id).FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken)
            : await (await GetDbSetAsync()).FindAsync(new object[] { id }, cancellationToken);
    }
}
