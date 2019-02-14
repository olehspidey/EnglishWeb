using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EnglishWeb.Core.Models.DomainModels.Abstractions;
using EnglishWeb.Data;
using Microsoft.EntityFrameworkCore;

namespace EnglishWeb.DAL
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly ApplicationDbContext _context;
        private DbSet<TEntity> _entities;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<TEntity> Table => Entities;

        protected virtual DbSet<TEntity> Entities => _entities ?? (_entities = _context.Set<TEntity>());

        public virtual async Task<TEntity> GetByIdAsync(object id)
            => await Entities.FindAsync(id);

        public virtual async Task<int> InsertAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                entities.ToList().ForEach(async entity => await Entities.AddAsync(entity));

                return await _context.SaveChangesAsync();
            }
            catch (Exception er)
            {
                Debug.WriteLine(er.Message);

                return -1;
            }
        }

        public virtual async Task<int> UpdateAsync(ICollection<TEntity> entities)
        {
            try
            {
                Entities.UpdateRange(entities);

                return await _context.SaveChangesAsync();
            }
            catch (Exception er)
            {
                Debug.Write(er.Message);

                return -1;
            }
        }

        public virtual async Task<int> DeleteAsync(ICollection<TEntity> entities)
        {
            var deleted = false;

            while (!deleted)
            {
                try
                {
                    foreach (var entity in entities)
                    {
                        _context.Entry(entity).State = EntityState.Deleted;
                    }

                    var result = await _context.SaveChangesAsync();
                    deleted = true;

                    return result;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        if (entry.Entity is TEntity)
                        {
                            var databaseValues = entry.GetDatabaseValues();

                            entry.OriginalValues.SetValues(databaseValues);
                        }
                    }
                }
                catch (Exception er)
                {
                    Debug.Write(er.Message);
                    return -1;
                }
            }

            return -1;
        }

        public async Task<int> InsertAsync(TEntity entity) => await InsertAsync(new List<TEntity>() { entity });

        public async Task<int> UpdateAsync(TEntity entity) => await UpdateAsync(new List<TEntity>() { entity });

        public async Task<int> DeleteAsync(TEntity entity) => await DeleteAsync(new List<TEntity>() { entity });
    }
}
