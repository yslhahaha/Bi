using AutoMapper;
using Bi.Domain;
using Bi.Models;
using Bi.Utility;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Bi.Biz.Security
{
    public class BiUserManager : UserManager<IdentityUser>
    {
        public BiUserManager(IUserStore<IdentityUser> store)
            : base(store) { }

        #region web

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(IdentityUser user)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await this.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public static BiUserManager Create(IdentityFactoryOptions<BiUserManager> options, IOwinContext context)
        {
            var manager = new BiUserManager(new BiUserStore<IdentityUser>(new OraDb104()));
            // 配置用户名的验证逻辑
            manager.UserValidator = new UserValidator<IdentityUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // 配置密码的验证逻辑
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            // 注册双重身份验证提供程序。此应用程序使用手机和电子邮件作为接收用于验证用户的代码的一个步骤
            // 你可以编写自己的提供程序并在此处插入。
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<IdentityUser>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<IdentityUser>
            {
                Subject = "安全代码",
                BodyFormat = "Your security code is: {0}"
            });
            //manager.EmailService = new EmailService();
            //manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<IdentityUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public override async Task<IdentityUser> FindAsync(string userName, string password)
        {
            IdentityUser tUser = await this.FindByNameAsync(userName).ConfigureAwait(false);
            IdentityUser result;

            if (tUser == null)
            {
                result = default(IdentityUser);
            }
            else
            {
                result = ((await this.CheckPasswordAsync(tUser, password).ConfigureAwait(false)) ? tUser : default(IdentityUser));
            }

            return result;
        }

        public override Task<IdentityUser> FindByNameAsync(string userName)
        {
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }

            IUserLoginStore<IdentityUser> userLoginStore = this.Store as IUserLoginStore<IdentityUser>;

            var user = userLoginStore.FindByNameAsync(userName);

            if (user == null)
            {
                return Task.FromResult<IdentityUser>(new IdentityUser()
                {
                    Id = "",
                });
            }

            return user;
        }

        /// <summary>
        /// 校验密码
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override async Task<bool> CheckPasswordAsync(IdentityUser user, string password)
        {
            IUserPasswordStore<IdentityUser> passwordStore = this.Store as IUserPasswordStore<IdentityUser>;

            bool result;

            if (user == null)
            {
                result = false;
            }
            else
            {
                result = await this.VerifyPasswordAsync(passwordStore, user, password).ConfigureAwait(false);
            }

            return result;
        }

        protected async Task<bool> VerifyPasswordAsync(IUserPasswordStore<IdentityUser> store, IdentityUser user, string password)
        {
            string hashedPassword = await store.GetPasswordHashAsync(user).ConfigureAwait(false);

            return new PasswordHasher().VerifyHashedPassword(hashedPassword, password) != PasswordVerificationResult.Failed;
        }

        public override Task<ClaimsIdentity> CreateIdentityAsync(IdentityUser user, string authenticationType)
        {
            if (user.Rights == null)
            {
                string roles = "";

                Dictionary<string, string> rights = GetRightsBySQLAsync(user.Id, null, ref roles).Result;
                if (rights.Keys.Contains("IsAdmin")) { user.IsAdmin = true; rights.Remove("IsAdmin"); }

                user.Rights = rights;
                user.UserRoles = roles;
            }

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Sid, user.Id));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserName));
            claims.Add(new Claim(ClaimTypes.GivenName, user.CnName));
            claims.Add(new Claim(ClaimTypes.Surname, user.CnName));
            claims.Add(new Claim("AspNet.Identity.SecurityStamp", user.SecurityStamp));
            claims.Add(new Claim(ClaimTypes.UserData, (user.Rights == null ? "" : JsonConvert.SerializeObject(user.Rights))));
            claims.Add(new Claim(ClaimTypes.Role, user.UserRoles));

            var identity = new ClaimsIdentity(claims, authenticationType);

            return Task.FromResult<ClaimsIdentity>(identity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="dirs"></param>
        /// <returns></returns>
        public Task<Dictionary<string, string>> GetRightsBySQLAsync(string userId, List<TB_SYS_DIR> dirs, ref string roles)
        {
            Dictionary<string, string> dicRights = new Dictionary<string, string>();

            decimal key = Pub.ConvertToDecimal(userId);


            if (dirs == null || dirs.Count == 0)
            {
                dirs = new List<TB_SYS_DIR>();

                using (OraDb104 _db = new OraDb104())
                {
                    string sql = "SELECT * FROM TB_SYS_DIR WHERE ENABLED = 1 AND DELETED = 0";

                    DataSet ds = _db.ExecuteSql(sql);

                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            dirs.Add(new TB_SYS_DIR()
                            {
                                DIR_ID = dr["DIR_ID"].ToString(),
                                DIR_NAME = dr["DIR_NAME"].ToString(),
                                DIR_URL = dr["DIR_URL"].ToString()
                            });
                        }
                    }

                    string userDirSql = @"SELECT SR.ISADMIN, D.DIR_ID, D.DIR_URL, SR.ROLE_NAME
                                                          FROM TB_ADMIN_USER U
                                                          LEFT JOIN TB_USER_ROLE R
                                                            ON U.USER_ID = R.USER_ID
                                                          LEFT JOIN TB_SYS_ROLE SR
                                                            ON SR.ROLEID = R.ROLEID
                                                          LEFT JOIN TB_ROLE_DIR RD
                                                            ON RD.ROLEID = R.ROLEID
                                                          LEFT JOIN TB_SYS_DIR D
                                                            ON RD.DIR_ID = D.DIR_ID
                                                         WHERE U.USER_ID = '" + userId + "'";

                    DataSet dsUserDir = _db.ExecuteSql(userDirSql);

                    if (dsUserDir.Tables.Count == 0 || dsUserDir.Tables[0].Rows.Count == 0) { return Task.FromResult<Dictionary<string, string>>(dicRights); }

                    foreach (DataRow dr in dsUserDir.Tables[0].Rows)
                    {
                        if (!dicRights.Keys.Contains("IsAdmin") && dr["ISADMIN"].ToString() == "1") { dicRights["IsAdmin"] = "true"; }

                        if (!dicRights.Keys.Contains(dr["DIR_ID"].ToString()))
                            dicRights.Add(dr["DIR_ID"].ToString(), dr["DIR_URL"].ToString().ToLower());
                    }

                    var rolesList = dsUserDir.Tables[0].DefaultView.ToTable(true, "ROLE_NAME").Rows.Cast<DataRow>().Select(it => it["ROLE_NAME"]).ToList();

                    roles = string.Join(",", rolesList);
                }
            }

            return Task.FromResult<Dictionary<string, string>>(dicRights);
        }

        /// <summary>
        /// 重新登录，改变SecurityStamp，记录登录日志
        /// </summary>
        /// <param name="USER_ID"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public Task<string> ChangeSecurityStampBySQLAsync(IdentityUser user, TB_SYS_LOG log)
        {
            string newSecurityStamp = Guid.NewGuid().ToString();

            List<string> sqls = new List<string>();

            using (OraDb104 _db = new OraDb104())
            {
                log.MEMO2 = "SecurityStamp:{" + user.SecurityStamp;

                log.MEMO2 += "}|{" + user.SecurityStamp + "}";
                log.USERID = user.UserName.ToString();
                log.OPERATION = "登录";
                log.OPERATION = "登录";
                log.FUNNAME = "登录";
                log.BIZID = "";
                log.BIZCODE = "";
                log.MEMO1 = "登录成功。[ " + log.MEMO1 + " ]";

                sqls.Add("UPDATE TB_ADMIN_USER SET SECURITYSTAMP = '" + newSecurityStamp + @"', 
                                LASTLOGINDATE = SYSDATE,
                                LOGINCOUNT = LOGINCOUNT + 1
                            WHERE USER_ID = '" + user.Id + "'");

                sqls.Add(_db.CreateLogSql(log));

                bool flag = _db.ExecuteNonQueryTran(CommandType.Text, sqls);

                if (flag) return Task.FromResult<string>(newSecurityStamp);
                else return Task.FromResult<string>("");
            }
        }

        #endregion
    }
}
