using Bi.Biz.Security;
using Bi.Domain;
using Bi.Models;
using Bi.Utility;
using Bi.Web.App.Caching;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Bi.Web.App.Facade
{
    public class SignInHelper
    {
        public SignInHelper(BiUserManager userManager, IAuthenticationManager authManager)
        {
            UserManager = userManager;
            AuthenticationManager = authManager;
        }

        public BiUserManager UserManager { get; private set; }
        public IAuthenticationManager AuthenticationManager { get; private set; }

        public async Task<BiSignInStatus> SignInAsync(IdentityUser user, bool isPersistent, bool rememberBrowser)
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

            SysCaching sysCac = new SysCaching();

            dirs = sysCac.GetUsableDirs().ToList();

            string roles = "";

            var userRights = await UserManager.GetRightsBySQLAsync(user.Id, dirs, ref roles);

            if (userRights.Keys.Contains("IsAdmin")) { user.IsAdmin = true; userRights.Remove("IsAdmin"); }

            user.Rights = userRights;
            user.UserRoles = roles;

            return true;
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

                user.SecurityStamp = await UserManager.ChangeSecurityStampBySQLAsync(user, log);

                return await SignInAsync(user, isPersistent, false);
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
    }
}