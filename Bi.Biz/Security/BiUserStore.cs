using Bi.Domain;
using Bi.Utility;
using Microsoft.AspNet.Identity;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bi.Biz.Security
{
    public class BiUserStore<TUser> : IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        IUserRoleStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserEmailStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserTwoFactorStore<TUser, string>,
        IUserLockoutStore<TUser, string>,
        IUserStore<TUser>
        where TUser : IdentityUser
    {
        public OraDb104 _db { get; private set; }

        /// <summary>
        /// Default constructor that initializes a new MySQLDatabase
        /// instance using the Default Connection string
        /// </summary>
        public BiUserStore()
        {
            new BiUserStore<TUser>(new OraDb104());
        }

        /// <summary>
        /// Constructor that takes a MySQLDatabase as argument 
        /// </summary>
        /// <param name="database"></param>
        public BiUserStore(OraDb104 database)
        {
            _db = database;
        }

        #region IUserLoginStore
        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("userId");
            }

            string sql = @"SELECT *
                                  FROM TB_ADMIN_USER U
                                 WHERE U.DELETED = 0 AND USER_ID = :USER_ID";

            var para = new OracleParameter
            {
                ParameterName = ":USER_ID",
                Value = userId,
                OracleDbType = OracleDbType.Decimal,
                Direction = ParameterDirection.Input
            };

            var result = _db.ExecuteQuery(sql, para);

            var row = result.Rows.Cast<DataRow>().SingleOrDefault();

            if (row != null)
            {
                return Task.FromResult<TUser>(new IdentityUser
                {
                    Id = row["USER_ID"].ToString(),
                    UserName = row["USER_NAME"].ToString(),
                    CnName = row["CN_NAME"].ToString(),
                    PasswordHash = string.IsNullOrEmpty(row["PASSWORD"].ToString()) ? null : row["PASSWORD"].ToString(),
                    Status = string.IsNullOrEmpty(row["STATUS"].ToString()) ? 0 : Pub.ConvertToInt32(row["STATUS"].ToString()),
                    SecurityStamp = row["SECURITYSTAMP"].ToString(),
                    UserType = row["USER_TYPE"].ToString()
                } as TUser);
            }

            return null;
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("userName");
            }

            string sql = @"SELECT *  FROM TB_ADMIN_USER U
                                 WHERE U.DELETED = 0 AND USER_NAME = :USER_NAME";

            var para = new OracleParameter
            {
                ParameterName = ":USER_NAME",
                Value = userName,
                OracleDbType = OracleDbType.Varchar2,
                Direction = ParameterDirection.Input
            };

            var result = _db.ExecuteQuery(sql, para);

            var row = result.Rows.Cast<DataRow>().SingleOrDefault();

            if (row != null)
            {
                return Task.FromResult<TUser>(new IdentityUser
                {
                    Id = row["USER_ID"].ToString(),
                    UserName = row["USER_NAME"].ToString(),
                    CnName = row["CN_NAME"].ToString(),
                    PasswordHash = string.IsNullOrEmpty(row["PASSWORD"].ToString()) ? null : row["PASSWORD"].ToString(),
                    Status = string.IsNullOrEmpty(row["STATUS"].ToString()) ? 0 : Pub.ConvertToInt32(row["STATUS"].ToString()),
                    SecurityStamp = row["SECURITYSTAMP"].ToString(),
                    UserType = row["USER_TYPE"].ToString()
                } as TUser);
            }

            return null;
        }

        public Task UpdateAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }

        #endregion

        #region IUserClaimStore
        public Task AddClaimAsync(TUser user, System.Security.Claims.Claim claim)
        {
            throw new NotImplementedException();
        }

        public Task<IList<System.Security.Claims.Claim>> GetClaimsAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task RemoveClaimAsync(TUser user, System.Security.Claims.Claim claim)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserPasswordStore
        public Task<string> GetPasswordHashAsync(TUser user)
        {
            return Task.FromResult<string>(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserSecurityStampStore
        public Task<string> GetSecurityStampAsync(TUser user)
        {
            var tUser = FindByIdAsync(user.Id);

            return Task.FromResult<string>(tUser.Result.SecurityStamp);
        }

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IQueryableUserStore
        public IQueryable<TUser> Users
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IUserEmailStore
        public Task<TUser> FindByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(TUser user, string email)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserPhoneNumberStore
        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserTwoFactorStore
        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserLockoutStore
        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserRoleStore
        public Task AddToRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
