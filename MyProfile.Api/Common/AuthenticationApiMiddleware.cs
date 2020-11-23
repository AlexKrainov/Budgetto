using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.API.Common
{
    public class AuthenticationApiMiddleware
    {
//		private readonly RequestDelegate next;

//		public AuthenticationApiMiddleware(RequestDelegate _next)
//		{
//			next = _next;
//		}

//		public async Task Invoke(HttpContext context, IAuthProvider authProvider)
//		{
//			BaseAuthParams authParams = new BaseAuthParams();
//			ApplicationUser currentUser = null;
//			string authHeader = context.Request.Headers["Authorization"];

//			//log in via header
//			if (authHeader != null && (authHeader.StartsWith("Basic") || authHeader.StartsWith("Bearer ")))
//			{
//				try
//				{
//					GetAuthFromHeader(authHeader, ref authParams);

//					currentUser = await authProvider.CheckAndGetUserParametersAsync(authParams);
//				}
//				catch (Exception ex)
//				{
//					context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//					throw ex;
//				}
//			}
//			//log in via cookies
//			else if (context.Request.Cookies.ContainsKey("AuthorizeToken") && (context.Request.Path.Value.Contains("/token") || context.Request.Path.Value.Contains("swagger"))) //authorize token ( SessionID )
//			{
//				authParams.SessionID = context.Request.Cookies["AuthorizeToken"];
//				currentUser = await authProvider.Fast_CheckAndGetUserParametersAsync(authParams);
//			}

//			if (currentUser != null)
//			{
//				var identity = JsonConvert.SerializeObject(currentUser);

//				var claim = new Claim(ClaimsIdentity.DefaultNameClaimType, String.Empty);
//				claim.Properties[ApplicationUserInfo.APPLICATION_USER] = identity;
//				context.User = new ClaimsPrincipal(new List<ClaimsIdentity> { new ClaimsIdentity(new List<Claim> { claim }) });

//				if (context.Request.Path.Value.Contains("/token"))//if user work with swagger
//				{
//					context.Response.Cookies.Append("AuthorizeToken", currentUser.SessionID);
//				}

//				await next.Invoke(context);
//				return;
//			}
//			else
//			{
//				if (context.Request.Path.Value.Contains("swagger") && ApplicationUserInfo.CurrentUser == null)
//				{
//#if prod
//					context.Response.Redirect("/api/login.html");
//#else
//					context.Response.Redirect("/login.html");
//#endif
//					return;
//				}
//				if (context.Request.Path.Value.Contains("login.html"))
//				{
//					context.Response.StatusCode = StatusCodes.Status200OK;
//					await next.Invoke(context);
//					return;
//				}
//				context.Response.StatusCode = StatusCodes.Status401Unauthorized; //Unauthorized
//				return;
//			}
//		}

//		private void GetAuthFromHeader(string authHeader, ref BaseAuthParams authParams)
//		{
//			if (authHeader.ToLower().Contains("basic"))
//			{
//				string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();// authHeader.StartsWith("Basic") ? authHeader.Substring("Basic ".Length).Trim() : context.Request.Cookies["Basic"];
//				Encoding encoding = Encoding.GetEncoding("iso-8859-1");
//				string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

//				int seperatorIndex = usernamePassword.IndexOf(':');

//				authParams.Login = usernamePassword.Substring(0, seperatorIndex);
//				authParams.Password = usernamePassword.Substring(seperatorIndex + 1);
//			}
//			//else
//			//if (authHeader.ToLower().Contains("bearer "))//SessionID
//			//{
//			//	string encodedUsernamePassword = authHeader.Substring("Bearer  ".Length).Trim();
//			//	Encoding encoding = Encoding.GetEncoding("iso-8859-1");
//			//	string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

//			//	int seperatorIndex = usernamePassword.IndexOf(':');

//			//	authParams.SessionID = usernamePassword.Substring(0, seperatorIndex);
//			//}
//		}


//		public string ToBase64(string sData) // Encode
//		{
//			try
//			{
//				byte[] encData_byte = new byte[sData.Length];
//				encData_byte = System.Text.Encoding.UTF8.GetBytes(sData);
//				string encodedData = Convert.ToBase64String(encData_byte);
//				return encodedData;
//			}
//			catch (Exception ex)
//			{
//				throw new Exception("Error in base64Encode" + ex.Message);
//			}
//		}
	}
}
