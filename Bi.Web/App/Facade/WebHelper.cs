using Bi.Biz.Security;
using Bi.Models;
using Bi.Utility;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Bi.Web.App.Facade
{
    public class WebHelper
    {
        public static async Task<IdentityUser> GetIdentityUser()
        {
            //TODO:验证获取方式
            if (HttpContext.Current.Session[SessionKey.SYS_USER_INFO] != null)
                return HttpContext.Current.Session[SessionKey.SYS_USER_INFO] as IdentityUser;
            else
            {
                var curIdentity = (System.Security.Claims.ClaimsIdentity)HttpContext.Current.User.Identity;

                //重获授权目录
                if (curIdentity != null)
                {
                    var userData = curIdentity.FindFirst(System.Security.Claims.ClaimTypes.UserData);

                    if (userData == null || userData.Value == null) { return null; }

                    IdentityUser sysUser = new IdentityUser
                    {
                        Id = curIdentity.FindFirst(System.Security.Claims.ClaimTypes.Sid).Value,
                        UserName = curIdentity.FindFirst(System.Security.Claims.ClaimTypes.Name).Value,
                        CnName = curIdentity.FindFirst(System.Security.Claims.ClaimTypes.Surname).Value,
                        SecurityStamp = curIdentity.FindFirst("AspNet.Identity.SecurityStamp").Value,
                        UserRoles = curIdentity.FindFirst(System.Security.Claims.ClaimTypes.Role).Value,
                    };

                    Dictionary<string, string> jObs = (Dictionary<string, string>)JsonConvert.DeserializeObject(curIdentity.FindFirst(System.Security.Claims.ClaimTypes.UserData).Value, typeof(Dictionary<string, string>));

                    if (jObs.Count > 0)
                    {
                        sysUser.Rights = new Dictionary<string, string>();

                        foreach (KeyValuePair<string, string> kv in jObs)
                        {
                            sysUser.Rights.Add(kv.Key, kv.Value);
                        }
                    }

                    if (sysUser.Rights == null || string.IsNullOrEmpty(sysUser.UserRoles))
                        await new SignInHelper(HttpContext.Current.GetOwinContext().GetUserManager<BiUserManager>()
                            , HttpContext.Current.GetOwinContext().Authentication).ConfigUserPermission(sysUser);

                    HttpContext.Current.Session[SessionKey.SYS_USER_INFO] = sysUser;

                    return sysUser;
                }

                return null;
            }
        }
        public static IdentityUser IdentityUser
        {
            get
            {
                if (HttpContext.Current.Session[SessionKey.SYS_USER_INFO] != null)
                    return HttpContext.Current.Session[SessionKey.SYS_USER_INFO] as IdentityUser;
                else//TODO:验证一下，当Session超时后，这里的异步可以取到值
                    return GetIdentityUser().Result;
            }
            set
            {
                IdentityUser = value;
            }
        }
    }
}