using Bi.Domain;
using Bi.Utility;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Bi.Biz.Security
{
    public class BiSignInManager : SignInManager<IdentityUser, string>
    {
        public BiSignInManager(BiUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
            uManager = userManager;
            AuthManager = authenticationManager;
        }

        public BiUserManager uManager { get; private set; }
        public IAuthenticationManager AuthManager { get; private set; }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(IdentityUser user)
        {
            return user.GenerateUserIdentityAsync((BiUserManager)UserManager);
        }

        public static BiSignInManager Create(IdentityFactoryOptions<BiSignInManager> options, IOwinContext context)
        {
            return new BiSignInManager(context.GetUserManager<BiUserManager>(), context.Authentication);
        }
        public async Task<BiSignInStatus> PasswordSignIn(string userName, string password, bool isPersistent, string logId, bool shouldLockout)
        {
            var user = await UserManager.FindByNameAsync(userName);

            if (user.Id == "")
            {
                return BiSignInStatus.Failure;
            }

            if (user.Status == (short)UserStatus.DISABLE)
            {
                return BiSignInStatus.LockedOut;
            }

            if (await UserManager.CheckPasswordAsync(user, password))
            {
                TB_SYS_LOG log = new TB_SYS_LOG();
                log.LOGID = logId;
                log.LOGTIME = Pub.GetLocalTime();
                //log.IP = WebHelper.GetUserIP();
                log.URL = HttpContext.Current.Request.Url.ToString();
                log.MEMO1 += "浏览器:" + HttpContext.Current.Request.Browser.Type;

                user.SecurityStamp = await uManager.ChangeSecurityStampBySQLAsync(user, log);

                return await BiSignInAsync(user, isPersistent, false);
            }

            if (shouldLockout)
            {
                // If lockout is requested, increment access failed count which might lock out the user
                await UserManager.AccessFailedAsync(user.Id);
                if (await UserManager.IsLockedOutAsync(user.Id))
                {
                    return BiSignInStatus.LockedOut;
                }
            }
            return BiSignInStatus.Failure;
        }


        public async Task<BiSignInStatus> BiSignInAsync(IdentityUser user, bool isPersistent, bool rememberBrowser)
        {
            if (await ConfigUserPermission(user))
            {
                HttpContext.Current.Session[SessionKey.SYS_USER_INFO] = user;

                // Clear any partial cookies from external or two factor partial sign ins
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
                var userIdentity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

                if (rememberBrowser)
                {
                    var rememberBrowserIdentity = AuthenticationManager.CreateTwoFactorRememberBrowserIdentity(user.Id);
                    AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, userIdentity, rememberBrowserIdentity);
                }
                else
                {
                    AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, userIdentity);
                }

                return BiSignInStatus.Success;
            }
            else { return BiSignInStatus.Failure; }
        }

        /// <summary>
        /// 配置用户权限
        /// </summary>
        /// <param name="user"></param>
        public async Task<bool> ConfigUserPermission(IdentityUser user)
        {
            List<TB_SYS_DIR> dirs = new List<TB_SYS_DIR>();

            //SysCaching sysCac = new SysCaching();

            //dirs = sysCac.GetUsableDirs().ToList();

            string roles = "";

            var userRights = await uManager.GetRightsBySQLAsync(user.Id, dirs, ref roles);

            if (userRights.Keys.Contains("IsAdmin")) { user.IsAdmin = true; userRights.Remove("IsAdmin"); }

            user.Rights = userRights;
            user.UserRoles = roles;

            return true;
        }
    }
}
