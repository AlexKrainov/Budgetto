using Microsoft.AspNetCore.Http;
using MyProfile.Entity.ModelView;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace MyProfile.Identity
{
	public static class UserInfo
	{
		/// <summary>
		/// tmp
		/// </summary>
		public static Guid PersonID { get { return Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"); } }

		
		public static readonly string CONNECT_USER = "ConnectUserInfo";
		public static Microsoft.AspNetCore.Http.HttpContext HttpContext => _accessor.HttpContext;

		/// <summary>
		/// Authorized user, if user not authorized than return NULL.
		/// </summary>
		public static UserInfoModel Current
		{
			get
			{
				return new UserInfoModel();
				if (_accessor.HttpContext == null)
				{
					return null;
				}
				Claim claim = _accessor.HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType);

				try
				{
					if (claim.Properties[CONNECT_USER] != null)
					{
						var info = claim.Properties[CONNECT_USER];
						return (UserInfoModel)JsonConvert.DeserializeObject(info, typeof(UserInfoModel)); //do we need it  ? (?? new ApplicationUser())
					}
				}
				catch (Exception ex)
				{
					Log.Error(ex, "ERROR, when try to get user!");
				}
				return null;
			}
		}

		private static IHttpContextAccessor _accessor;

		public static void Configure(IHttpContextAccessor httpContextAccessor)
		{
			_accessor = httpContextAccessor;
		}


	}
}
