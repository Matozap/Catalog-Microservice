using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using CatalogService.Infrastructure.Database.Context;
using CatalogService.Infrastructure.Extensions;
using CatalogService.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Database.Repositories;

public class EfRepository : IRepository
{
    private readonly DatabaseContext _applicationContext;
    private readonly DatabaseOptions _databaseOptions;

    public EfRepository(DatabaseContext applicationContext, DatabaseOptions databaseOptions)
    {
        _applicationContext = applicationContext;
        _databaseOptions = databaseOptions;
    }

    public async Task<T> AddAsync<T>(T entity) where T: EntityBase
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        entity.Id = UniqueIdGenerator.GenerateSequentialId();
        await CreateOutboxMessage(entity, nameof(AddAsync));
        await _applicationContext.AddAsync(entity);
        await _applicationContext.SaveChangesAsync();
    
        return entity;
    }

    public async Task<T> UpdateAsync<T>(T entity) where T: EntityBase
    {
        ArgumentNullException.ThrowIfNull(entity);

        _applicationContext.Update(entity);
        await CreateOutboxMessage(entity, nameof(UpdateAsync));
        await _applicationContext.SaveChangesAsync();
    
        return entity;
    }
    
    public async Task<T> DeleteAsync<T>(T entity, bool skipOutbox = false) where T: EntityBase
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        _applicationContext.Remove(entity);

        if (!skipOutbox)
        {
            await CreateOutboxMessage(entity, nameof(DeleteAsync));
        }

        await _applicationContext.SaveChangesAsync();
    
        return entity;
    }
    
    public async Task<List<T>> GetAsListAsync<T, TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderAscending = null,
        Expression<Func<T, TKey>> orderDescending = null, Expression<Func<T, T>> selectExpression = null, bool includeNavigationalProperties = false) where T: EntityBase
    {
        var result = ApplySpec(predicate, orderAscending, orderDescending, selectExpression);
        return includeNavigationalProperties switch
        {
            true when typeof(T) == typeof(Product) => (await LoadAllNavigationalPropertiesAsync(result as IQueryable<Product>)).ToList() as List<T>,
            true when typeof(T) == typeof(ProductImage) => (await LoadAllNavigationalPropertiesAsync(result as IQueryable<ProductImage>)).ToList() as List<T>,
            true when typeof(T) == typeof(ProductStock) => (await LoadAllNavigationalPropertiesAsync(result as IQueryable<ProductStock>)).ToList() as List<T>,
            _ => await result.ToListAsync()
        };
    }
    
    public async Task<T> GetAsSingleAsync<T, TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderAscending = null,
        Expression<Func<T, TKey>> orderDescending = null, Expression<Func<T, T>> selectExpression = null, bool includeNavigationalProperties = false) where T: EntityBase
    {
        var result = ApplySpec(predicate, orderAscending, orderDescending, selectExpression);

        return includeNavigationalProperties switch
        {
            true when typeof(T) == typeof(Product) => (await LoadAllNavigationalPropertiesAsync(result as IQueryable<Product>)).FirstOrDefault() as T,
            true when typeof(T) == typeof(ProductImage) => (await LoadAllNavigationalPropertiesAsync(result as IQueryable<ProductImage>)).FirstOrDefault() as T,
            true when typeof(T) == typeof(ProductStock) => (await LoadAllNavigationalPropertiesAsync(result as IQueryable<ProductStock>)).FirstOrDefault() as T,
            _ => await result.FirstOrDefaultAsync()
        };
    }
    
    private IQueryable<T> ApplySpec<T, TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderAscending,
        Expression<Func<T, TKey>> orderDescending, Expression<Func<T, T>> selectExpression) where T: EntityBase
    {
        var result = _applicationContext.Set<T>().AsQueryable();

        if (predicate != null)
        {
            result = result.Where(predicate);
        }

        if (orderAscending != null)
        {
            result = result.OrderBy(orderAscending);
        }
        
        if (orderDescending != null)
        {
            result = result.OrderBy(orderDescending);
        }
        
        if (orderDescending != null)
        {
            result = result.Select(selectExpression);
        }

        return result;
    }
    
    private async Task<List<Product>> LoadAllNavigationalPropertiesAsync(IQueryable<Product> source)
    {
        switch (_databaseOptions.EngineType)
        {
            case EngineType.NonRelational:
            {
                var partialResult = await source.AsNoTracking().ToListAsync();
                return partialResult.Select(product =>
                {
                    product.ProductImages = _applicationContext.Set<ProductImage>().Where(productImage => productImage.ProductId == product.Id).OrderBy(productImage => productImage.Name)
                        .AsNoTracking()
                        .ToList();

                    product.ProductImages = product.ProductImages.Select(f =>
                    {
                        f.ProductStock = _applicationContext.Set<ProductStock>().Where(productStock => productStock.ProductImageId == f.Id).OrderBy(productStock => productStock.Name)
                            .AsNoTracking()
                            .ToList();
                        return f;
                    }).ToList();

                    return product;
                }).ToList();
            }
            case EngineType.Relational:
            default:
                return source.Include(product => product.ProductImages.Where(productImage => !productImage.Disabled).OrderBy(productImage => productImage.Name))
                    .ThenInclude(s => s.ProductStock.Where(productStock => !productStock.Disabled).OrderBy(productStock => productStock.Name))
                    .AsSplitQuery()
                    .AsNoTracking()
                    .ToList();
        }
    }
    
    private async Task<List<ProductImage>> LoadAllNavigationalPropertiesAsync(IQueryable<ProductImage> source)
    {
        switch (_databaseOptions.EngineType)
        {
            case EngineType.NonRelational:
            {
                var partialResult = await source.AsNoTracking().ToListAsync();
                return partialResult.Select(productImage =>
                {
                    productImage.ProductStock = _applicationContext.Set<ProductStock>().Where(productStock => productStock.ProductImageId == productImage.Id).OrderBy(productStock => productStock.Name)
                        .AsNoTracking()
                        .ToList();
                    return productImage;
                }).ToList();
            }
            case EngineType.Relational:
            default:
                return source.Include(productImage => productImage.ProductStock.Where(c => !c.Disabled).OrderBy(productStock => productStock.Name))
                    .Include(productImage => productImage.Product)
                    .AsSplitQuery()
                    .AsNoTracking()
                    .ToList();
        }
    }
    
    private async Task<List<ProductStock>> LoadAllNavigationalPropertiesAsync(IQueryable<ProductStock> source)
    {
        switch (_databaseOptions.EngineType)
        {
            case EngineType.NonRelational:
                return await source.AsNoTracking().ToListAsync();
            default:
                return source.AsSplitQuery().Include(productStock => productStock.ProductImage).AsNoTracking().ToList();
        }
    }
    
    private async Task CreateOutboxMessage<T>(T entity, string sourceMethod) where T: EntityBase
    {
        if(typeof(T) == typeof(Outbox)) return;
        
        var outbox = new Outbox
        {
            Id = UniqueIdGenerator.GenerateSequentialId(),
            JsonObject = entity.Serialize(),
            LastUpdateDate = entity.LastUpdateDate,
            LastUpdateUserId = entity.LastUpdateUserId,
            ObjectType = typeof(T).Name,
            Operation = sourceMethod switch
            {
                nameof(AddAsync) => Operation.Create,
                nameof(UpdateAsync) => Operation.Update,
                nameof(DeleteAsync) => Operation.Delete,
                _ => Operation.None
            }
        };

        await _applicationContext.AddAsync(outbox);
    }
}
