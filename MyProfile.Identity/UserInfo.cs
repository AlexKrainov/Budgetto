using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using MyProfile.Entity.ModelView;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyProfile.Identity
{
	public static class UserInfo
	{
		/// <summary>
		/// tmp
		/// </summary>
		public static Guid UserID { get { return Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"); } }


		public static readonly string USER_INFO = "UserInfo";
		public static readonly string LAST_USER_LOG_ID = "LastUserLogID";
		public static Microsoft.AspNetCore.Http.HttpContext HttpContext => _accessor.HttpContext;

		/// <summary>
		/// Authorized user, if user not authorized than return NULL.
		/// </summary>
		public static UserInfoModel Current
		{
			get
			{
				//return new UserInfoModel();
				if (_accessor.HttpContext == null)
				{
					return null;
				}
				Claim claim = _accessor.HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType);

				try
				{
					if (claim != null && claim.Properties[USER_INFO] != null)
					{
						var info = claim.Properties[USER_INFO];
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

		/// <summary>
		/// Authorized user, if user not authorized than return NULL.
		/// </summary>
		public static int LastUserLogID
		{
			get
			{
				if (_accessor.HttpContext == null)
				{
					return 0;
				}
				Claim claim = _accessor.HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType);

				try
				{
					if (claim != null && claim.Properties[LAST_USER_LOG_ID] != null)
					{
						var info = claim.Properties[LAST_USER_LOG_ID];
						return int.Parse(info);
					}
				}
				catch (Exception ex)
				{
					Log.Error(ex, "ERROR, when try to get user!");
				}
				return 0;
			}
			set
			{
				var claim = HttpContext.User.FindFirst(x => x.Value == Current.Email);

				if (claim != null)
				{
					claim.Properties.Remove(LAST_USER_LOG_ID);
					claim.Properties.Add(LAST_USER_LOG_ID, value.ToString());
				}
			}
		}

		private static IHttpContextAccessor _accessor;

		public static void Configure(IHttpContextAccessor httpContextAccessor)
		{
			_accessor = httpContextAccessor;
		}


		public static async Task AddOrUpdate_Authenticate(UserInfoModel user)
		{
			var claim = HttpContext.User.FindFirst(x => x.Value == user.Email);

			if (claim == null)
			{
				claim = new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email);
			}
			else
			{
				claim.Properties.Remove(USER_INFO);
			}
			claim.Properties.Add(USER_INFO, JsonConvert.SerializeObject(user));

			// создаем один claim
			var claims = new List<Claim>
			{
			   claim// new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
            };
			// создаем объект ClaimsIdentity
			ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie",
				ClaimsIdentity.DefaultNameClaimType,
				ClaimsIdentity.DefaultRoleClaimType);

			// установка аутентификационных куки
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
		}

	}
}
