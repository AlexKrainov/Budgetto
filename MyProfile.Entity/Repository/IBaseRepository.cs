using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyProfile.Entity.Repository
{
    public interface IBaseRepository
    {
        void Create<T>(T entity, bool isSave = false) where T : class;
        Task<int> CreateAsync<T>(T entity, bool isSave = false) where T : class;
        void CreateRange<T>(IEnumerable<T> entity, bool isSave = false) where T : class;
        Task<int> CreateRangeAsync<T>(IEnumerable<T> entity, bool isSave = false) where T : class;


        void Update<T>(T entity, bool isSave = false) where T : class;
        Task<int> UpdateAsync<T>(T entity, bool isSave = false) where T : class;


        //void CreateOrUpdate<T>(T entity, bool isSave = false) where T : class;
        //Task<int> CreateOrUpdateAsync<T>(T entity, bool isSave = false) where T : class;

        void Delete<T>(int id, bool isSave = false) where T : class;
        void Delete<T>(T entity, bool isSave = false) where T : class;
        Task DeleteAsync<T>(int id, bool isSave = false) where T : class;
        Task DeleteAsync<T>(T entity, bool isSave = false) where T : class;
        void DeleteRange<T>(IEnumerable<T> entities, bool isSave = false) where T : class;
        Task DeleteRangeAsync<T>(IEnumerable<T> entities, bool isSave = false) where T : class;

        int Save();
        Task<int> SaveAsync();

        IQueryable<T> SearchFor<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes) where T : class;

        IQueryable<T> GetAll<T>() where T : class;
        IQueryable<T> GetAll<T>(Expression<Func<T, bool>> predicate) where T : class;

        T GetByID<T>(int id) where T : class;
        Task<T> GetByIDAsync<T>(int id) where T : class;

        //Task<T> GetFirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate);
        //T GetFirstOrDefault<T>(Expression<Func<T, bool>> predicate);

        bool Any<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task<bool> AnyAsync<T>(int id) where T : class;
        Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// Remove all save change state
        /// Problem: I am logging the exceptions to the database.Till now everything works fine but problem is that when the DbConext.SaveChanges in try block throws an exception, cannot log the exception to the database as DbConext.SaveChanges in catch block also throws the same exception.
        /// https://stackoverflow.com/questions/48880719/log-dbcontext-savechanges-exception-to-the-database-in-entity-framework-entity-f
        /// </summary>
        void ResetContextState();
    }
}
