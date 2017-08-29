using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bi.Utility
{
    /// <summary>
    /// 登录状态
    /// </summary>
    public enum BiSignInStatus
    {
        Success,
        LockedOut,
        RequiresTwoFactorAuthentication,
        Failure,
        Verify
    }

    /// <summary>
    /// 删除状态
    /// </summary>
    public enum DeleteStatus
    {
        /// <summary>
        /// 停用
        /// </summary>
        YES = 0,
        /// <summary>
        /// 启用
        /// </summary>
        NO = 1,
    }
    /// <summary>
    /// 用户状态
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// 停用
        /// </summary>
        DISABLE = 0,
        /// <summary>
        /// 正常
        /// </summary>
        NORMAL = 1,
    }
    /// <summary>
    /// 用户状态
    /// </summary>
    public enum RoleStatus
    {
        /// <summary>
        /// 停用
        /// </summary>
        DISABLE = 0,
        /// <summary>
        /// 正常1
        /// </summary>
        NORMAL = 1
    }
    /// <summary>
    /// 字典状态
    /// </summary>
    public enum DictStatus
    {
        /// <summary>
        /// 停用
        /// </summary>
        DISABLE = 0,
        /// <summary>
        /// 正常
        /// </summary>
        NORMAL = 1
    }
    public enum DirStatus
    {
        /// <summary>
        /// 停用
        /// </summary>
        DISABLE = 0,
        /// <summary>
        /// 正常
        /// </summary>
        NORMAL = 1
    }
    /// <summary>
    /// 系统用户分类
    /// </summary>
    public enum UserType
    {
        /// <summary>
        /// 管理者
        /// </summary>
        ADMIN = 1,
        /// <summary>
        /// 普通
        /// </summary>
        NORMAL = 2,
        /// <summary>
        /// KM用户（不需要密码）
        /// </summary>
        KM_USER = 0,
    }
}
