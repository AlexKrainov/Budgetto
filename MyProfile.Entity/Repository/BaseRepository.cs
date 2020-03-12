﻿using LinqKit;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyProfile.Entity.Repository
{
	public class BaseRepository : IBaseRepository
	{
		private MyProfile_DBContext context;
		public BaseRepository(MyProfile_DBContext context)
		{
			this.context = context;
		}

		public DbConnection GetDBCommand()
		{
			return this.context.Database.GetDbConnection();
		}

		public virtual IQueryable<T> GetAll<T>() where T : class
		{
			return (GetDbSet<T>()).Cast<T>();
		}
		/// <summary>
		/// Get all by predicate 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public virtual IQueryable<T> GetAll<T>(Expression<Func<T, bool>> predicate) where T : class
		{
			return (GetDbSet<T>()).Where(predicate).Cast<T>();
		}

		//public async Task<T> GetFirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate) where T : class
		//{
		//	return await (GetDbSet<T>()).FirstOrDefaultAsync<T>(predicate);
		//}

		//public T GetFirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : class
		//{
		//	return (GetDbSet<T>()).FirstOrDefault(predicate);
		//}

		public void Create<T>(T entity, bool isSave = false) where T : class
		{
			context.Add<T>(entity);

			if (isSave) { this.Save(); }
		}

		public async void CreateAsync<T>(T entity, bool isSave = false) where T : class
		{
			await context.AddAsync<T>(entity);
			if (isSave) { await this.SaveAsync(); }
		}

		public void CreateRange<T>(IEnumerable<T> entity, bool isSave = false) where T : class
		{
			for (int i = 0; i < entity.Count(); i++)
			{
				this.Create(entity.ElementAt(i));
			}
			if (isSave) { this.Save(); }
		}

		public async void CreateRangeAsync<T>(IEnumerable<T> entity, bool isSave = false) where T : class
		{
			for (int i = 0; i < entity.Count(); i++)
			{
				this.CreateAsync(entity.ElementAt(i));
			}
			if (isSave) { await this.SaveAsync(); }
		}

		public void Update<T>(T entity, bool isSave = false) where T : class
		{
			context.Entry<T>(entity).State = EntityState.Modified;

			if (isSave) { this.Save(); }
		}

		public async void UpdateAsync<T>(T entity, bool isSave = false) where T : class
		{
			context.Update<T>(entity);

			if (isSave) { await this.SaveAsync(); }
		}


		[Obsolete("Not implemented")]
		public void CreateOrUpdate<T>(T entity, bool isSave = false) where T : class
		{
			throw new NotImplementedException();
		}

		[Obsolete("Not implemented")]
		public Task<int> CreateOrUpdateAsync<T>(T entity, bool isSave = false) where T : class
		{
			throw new NotImplementedException();
		}

		public void Delete<T>(int id, bool isSave = false) where T : class
		{
			context.Remove<T>(GetByID<T>(id));

			if (isSave) { this.Save(); }
		}
		public virtual void Delete<T>(T entity, bool isSave = false) where T : class
		{
			GetDbSet<T>().Remove(entity);
			if (isSave) { this.Save(); }
		}

		public void DeleteRange<T>(IEnumerable<T> entities, bool isSave = false) where T : class
		{
			context.RemoveRange(entities);

			if (isSave) { this.Save(); }
		}

		public int Save()
		{
			return context.SaveChanges();
		}

		public async Task<int> SaveAsync()
		{
			return await context.SaveChangesAsync();
		}

		public IQueryable<T> SearchFor<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes) where T : class
		{
			var result = GetAll<T>();
			foreach (var include in includes)
			{
				result = result.Include(include);
			}
			if (predicate != null)
			{
				result = result.AsExpandable().Where(predicate);
			}
			return result;
		}

		public T GetByID<T>(int id) where T : class
		{
			return GetDbSet<T>().Find(id);
		}

		public async Task<T> GetByIDAsync<T>(int id) where T : class
		{

			return await GetDbSet<T>().FindAsync(id);
		}


		private DbSet<T> GetDbSet<T>() where T : class
		{
			return context.Set<T>();
		}

		public bool Any<T>(int id) where T : class
		{
			return this.GetByID<T>(id) != null;
		}
		public async Task<bool> AnyAsync<T>(int id) where T : class
		{
			return await this.GetByIDAsync<T>(id) != null;
		}


	}
}