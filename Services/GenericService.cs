using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using CompanyManagement.Models;
using CompanyManagement.Constrains;
using CompanyManagement.Helper;

namespace CompanyManagement.Services
{
    public class GenericService<TEntity> where TEntity : class
    {
        private readonly CMScontext _context;

        public GenericService(CMScontext context)
        {
            _context = context;
        }

        #region ======== Aysnc functions ============

        public async Task<TEntity> GetByIdAsync(long id)
        {
            var dbSet = _context.Set<TEntity>();
            return await dbSet.FindAsync(id);
        }

        public async Task<List<TEntity>> GetAllAsync(string includeProperties = "")
        {
            var dbSet = _context.Set<TEntity>();
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                dbSet.Include(includeProperty);
            }
            return await dbSet.ToListAsync();
        }

        public async Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, string includeProperties = "")
        {
            var dbSet = _context.Set<TEntity>();
            foreach (var includeProperty in includeProperties.Split
               (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                dbSet.Include(includeProperty);
            }
            var z = await dbSet.Where(predicate).ToListAsync();
            return z;
        }

        public async Task<List<TEntity>> FindActiveListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var dbSet = _context.Set<TEntity>();
            var prop = typeof(TEntity).GetProperty("ActiveStatus");
            var entities = await dbSet.Where(predicate).ToListAsync();

            entities = entities.Where(i => (int?)prop?.GetValue(i) == 1).ToList();
            return entities;
        }

        #endregion=====================

        #region sync functions

        public TEntity GetById(long id)
        {
            var dbSet = _context.Set<TEntity>();
            return dbSet.Find(id);
        }

        public List<TEntity> GetAll(string includeProperties = "")
        {
            var dbSet = _context.Set<TEntity>();

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                dbSet.Include(includeProperty);
            }
            return dbSet.ToList();
        }

        public List<TEntity> GetAllGlobal(string includeProperties = "")
        {
            var dbSet = _context.Set<TEntity>();

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                dbSet.Include(includeProperty);
            }
            var entities = dbSet.ToList();
            return entities;
        }
        public List<TEntity> GetAllActive(string includeProperties = "")
        {
            var dbSet = _context.Set<TEntity>();
            var prop = typeof(TEntity).GetProperty("ActiveStatus");
            var entities = dbSet.ToList();
            return entities.Where(i => (int?)prop?.GetValue(i) == PropertyConstant.Active).ToList();
        }
        public List<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate, string includeProperties = "")
        {
            var dbSet = _context.Set<TEntity>();
            foreach (var includeProperty in includeProperties.Split
               (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                dbSet.Include(includeProperty);
            }
            var z = dbSet.Where(predicate).ToList();
            return z;
        }

        public List<TEntity> FindActiveList(Expression<Func<TEntity, bool>> predicate)
        {
            var dbSet = _context.Set<TEntity>();
            var prop = typeof(TEntity).GetProperty("ActiveStatus");
            var entities = dbSet.Where(predicate).ToList();
            entities = entities.Where(i => (int)prop.GetValue(i) == PropertyConstant.Active).ToList();
            return entities;
        }

        #endregion ============

        #region=========Add=========

        public void Add(TEntity entiry)
        {
            _context.Set<TEntity>().Add(entiry);
            _context.SaveChanges();
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().AddRange(entities);
            _context.SaveChanges();
        }

        #endregion==================

        #region=========Update===========

        public void Update(TEntity entityToUpdate)
        {
            _context.Set<TEntity>().Update(entityToUpdate);
            _context.SaveChanges();
        }

        public void UpdateRange(IEnumerable<TEntity> entitiesToUpdate)
        {
            foreach (var entityToUpdate in entitiesToUpdate.ToList())
            {
                _context.Set<TEntity>().Update(entityToUpdate);
            }
            _context.SaveChanges();
        }

        #endregion=======================

        #region==========Delete========


        public void DeleteRange(IEnumerable<TEntity> entitiesToDelete)
        {
            foreach (var entityToDelete in entitiesToDelete)
            {
                var property = entityToDelete.GetType().GetProperty("ActiveStatus");
                property?.SetValue(entityToDelete, PropertyConstant.Deleted);
                _context.Set<TEntity>().Update(entityToDelete);
            }
            _context.SaveChanges();
        }

        #endregion=====================

        public void Remove(object id)
        {
            TEntity entityToDelete = _context.Set<TEntity>().Find(id);
            if (entityToDelete == null)
            {
                throw new Exception("Entry not found");
            }
            Remove(entityToDelete);
        }

        public void Remove(TEntity entiry)
        {
            _context.Set<TEntity>().Remove(entiry);
            _context.SaveChanges();
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                _context.Set<TEntity>().Remove(entity);
            }
            _context.SaveChanges();
        }

        public async Task CommitAsync()
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(() =>
            {
                using (var dbContextTransaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.SaveChanges();
                        // Your code here                    
                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
                return Task.CompletedTask;
            });
        }

        public ResponseMessage Commit()
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            try
            {
                strategy.Execute(() =>
                {
                    using (var dbContextTransaction = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            _context.SaveChanges();
                            // Your code here                    
                            dbContextTransaction.Commit();
                        }
                        catch (Exception e)
                        {
                            dbContextTransaction.Rollback();                            
                        }
                    }                    
                });
                return AutoResponse.SuccessMessage("Transaction completed");
            }
            catch(Exception e)
            {
                return AutoResponse.ExceptionOccuredMessage(e.Message);
            }
            
        }
    }
}
