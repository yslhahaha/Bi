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
        /// 角色对应的目录ID
        /// </summary>
        public List<string> DirIds { get; set; }

        #endregion

    }
}
