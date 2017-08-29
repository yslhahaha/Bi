using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bi.Domain
{
    public class TB_SYS_ROLE_Metadata { }

    [MetadataType(typeof(TB_SYS_ROLE_Metadata))]
    public partial class TB_SYS_ROLE
    {
        #region 辅助View显示

        /// <summary>
        /// 角色列表（键值对）
        /// </summary>
        public List<TB_SYS_ROLE> Roles { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> RoleTypes { get; set; }

        #endregion

        /// <summary>
        ///为【编辑用户】页面准备角色数据。
        /// </summary>
        public void FillRoleTypes()
        {
            foreach (TB_SYS_ROLE role in Roles)
            {
                if (!RoleTypes.Contains(role.ROLE_TYPE))
                {
                    RoleTypes.Add(role.ROLE_TYPE);
                }
            }
        }


    }
}
