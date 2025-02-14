using Microsoft.EntityFrameworkCore;
using zurafworks.Shared.Data.Abstract;
using zurafworks.Shared.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EFCore.BulkExtensions;

namespace zurafworks.Shared.Data.Concrete.EntityFramework
{
    public class EfEntityRepositoryBase<TEntity> : IEntityRepository<TEntity>
        where TEntity : class, IEntity, new()
    {
        private readonly DbContext _context;

        public EfEntityRepositoryBase(DbContext context)
        {
            _context = context;
        }

        public void Add(TEntity Entity)
        {
            _context.Add(Entity);
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            return entity;
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task BulkInsert(IList<TEntity> entities)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var bulkConfig = new BulkConfig
                {
                    PreserveInsertOrder = true,
                    UseTempDB = true,
                };
                await _context.BulkInsertAsync(entities, bulkConfig);
                transaction.Commit();
            }catch(Exception ex)
            {
                transaction.Rollback();
                throw new Exception($"Bir Hata oluştu: {ex.Message}");
            }
        }

        public async Task<IList<TEntity>> BulkInsertKeepIds(IList<TEntity> entities)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var bulkConfig = new BulkConfig
                {
                    PreserveInsertOrder = true,
                    SetOutputIdentity = true,
                    UseTempDB = true,
                };
                await _context.BulkInsertAsync(entities, bulkConfig);
                transaction.Commit();
                return entities;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw;
            }
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Delete(TEntity Entity)
        {
            _context.Entry(Entity).State = EntityState.Modified;
            _context.Set<TEntity>().Remove(Entity);
            _context.SaveChanges();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            await Task.Run(() => { _context.Set<TEntity>().Remove(entity); });
        }

        public TEntity Get(int id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        public async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (includeProperties.Any())
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            return await query.ToListAsync();
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (includeProperties.Any())
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            return await query.SingleOrDefaultAsync(); ;
        }

        public void Update(TEntity Entity)
        {
            _context.Attach(Entity);
            _context.Entry(Entity).State = EntityState.Modified;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            await Task.Run(() => { _context.Set<TEntity>().Update(entity); });
            return entity;
        }

        public TEntity UpdateWithStateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return entity;
        }
    }
}
