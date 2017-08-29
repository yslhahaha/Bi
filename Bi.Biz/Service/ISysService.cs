using Bi.Domain;
using System.Collections.Generic;

namespace Bi.Biz.Service
{
    public interface ISysService
    {
        #region User

        bool CreateUser(TB_ADMIN_USER userDTO, TB_SYS_LOG log);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userDto"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        bool UpdateUser(TB_ADMIN_USER userDto, TB_SYS_LOG log);

        bool UpdatePwd(TB_ADMIN_USER userDto, TB_SYS_LOG log);

        bool DeleteUser(string userId, TB_SYS_LOG log);

        TB_ADMIN_USER GetUserByKey(string userId);

        TB_ADMIN_USER GetUserByLoginName(string userName);

        TB_ADMIN_USER GetUserByNameForValidate(string name, string userId);

        #endregion

        #region Role

        TB_SYS_ROLE GetRoleByKey(string id);

        IEnumerable<TB_SYS_ROLE> GetRoles();

        TB_SYS_ROLE GetRoleByNameForValidate(string name, string roleId);

        bool CreateRole(TB_SYS_ROLE roleDto, TB_SYS_LOG log);

        bool UpdateRole(TB_SYS_ROLE roleDto, TB_SYS_LOG log);

        bool DeleteRole(string id, TB_SYS_LOG log);


        bool UpdateRoleDirectory(string roleId, string[] dirs, TB_SYS_LOG log);

        List<TB_ADMIN_USER> GetUsersByROLE_ID(string roleId);

        #endregion

        #region Dir

        IEnumerable<TB_SYS_DIR> GetUsableDirs();

        TB_SYS_DIR GetDirByKey(string id);

        bool Create(TB_SYS_DIR dirDto, TB_SYS_LOG log);

        bool Update(TB_SYS_DIR dirDto, TB_SYS_LOG log);

        bool Delete(string id, TB_SYS_LOG log);

        TB_SYS_DIR GetDirByName(string name, string dirId);

        #endregion
    }
}