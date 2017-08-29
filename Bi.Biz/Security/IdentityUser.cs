using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Bi.Biz.Security
{
    public class IdentityUser : Microsoft.AspNet.Identity.EntityFramework.IdentityUser
    {
        /// <summary>
        /// Default constructor 
        /// </summary>
        public IdentityUser()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Constructor that takes user name as argument
        /// </summary>
        /// <param name="userName"></param>
        public IdentityUser(string uName)
            : this()
        {
            UserName = uName;
        }

        #region 原始属性

        /// <summary>
        /// The salted/hashed form of the user password
        /// </summary>


        #endregion

        #region 新增属性

        /// <summary>
        /// 登录状态
        /// /// <summary>
        /// TB_ADMIN_USER.STATUS
        /// </summary>
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 授权店名
        /// TB_ADMIN_USER.NAME
        /// </summary>
        public string CnName { get; set; }


        /// <summary>
        /// 权限
        /// </summary>
        public Dictionary<string, string> Rights { get; set; }

        /// <summary>
        /// 是管理类型
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        public string UserType { get; set; }
        /// <summary>
        /// 角色列表名称，用","分隔，例如：积分管理员,管理员,专卖店
        /// </summary>
        public string UserRoles { get; set; }

        #endregion

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<IdentityUser> manager)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在此处添加自定义用户声明
            return userIdentity;
        }
    }
}
