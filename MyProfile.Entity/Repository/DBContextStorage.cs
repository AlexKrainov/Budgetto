using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.Repository
{
	public static class DBContextStorage
	{
		public static MyProfile_DBContext dbContext
		{
			get
			{
				if (_dbContext == null)
				{
					//try to not use
					_dbContext = new MyProfile_DBContext((DbContextOptions<MyProfile_DBContext>)_accessor.HttpContext.RequestServices.GetService(typeof(DbContextOptions<MyProfile_DBContext>)));
				}

				return _dbContext;
			}
			set
			{
				_dbContext = value;
			}
		}
		public static BaseRepository repository
		{
			get
			{
				if (_repository == null)
				{
					//try to not use
					_repository = (BaseRepository)_accessor.HttpContext.RequestServices.GetService(typeof(IBaseRepository));
				}

				return _repository;
			}
			set
			{
				_repository = value;
			}
		}

		private static MyProfile_DBContext _dbContext;
		private static BaseRepository _repository;
		private static IHttpContextAccessor _accessor;

		public static void Configure(IHttpContextAccessor httpContextAccessor)
		{
			_accessor = httpContextAccessor;

		}
	}
}
