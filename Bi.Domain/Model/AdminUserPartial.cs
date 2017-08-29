using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bi.Domain
{
    public class TB_ADMIN_USER_Metadata { }

    [MetadataType(typeof(TB_ADMIN_USER_Metadata))]
    public partial class TB_ADMIN_USER
    {
        #region 辅助View显示

        /// <summary>
        /// 
        /// </summary>
        public string ComparePwd { get; set; }

        /// <summary>
        /// 原密码（只在密码修改界面使用）
        /// </summary>
        public string OldPwd { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public List<TB_SYS_ROLE> Roles { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public List<TB_USER_ROLE> UserRole{ get; set; }

        #endregion

    }
}
