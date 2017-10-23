using System;
using System.Collections.Generic;
using System.Linq;
using Bi.Utility;
using log4net;
using Microsoft.Practices.Unity;
using Bi.Biz.Service;
using Bi.Domain;
using Bi.Web.App_Start;

namespace Bi.Web.App.Caching
{
    public class SysCaching : BaseCaching
    {
        public ISysService SysService;

        public SysCaching() { SysService = UnityConfig.GetService<ISysService>(); }

        ILog logger = LogManager.GetLogger("SystemError");

        /// <summary>
        /// 取得所有可用菜单目录
        /// </summary>
        /// <returns></returns>
        public IList<TB_SYS_DIR> GetUsableDirs()
        {
            IList<TB_SYS_DIR> sysDirs = null;

            try
            {
                sysDirs = (IList<TB_SYS_DIR>)GetCache(CachingKey.DIRECTORY_KEY);

                if (sysDirs == null)
                {

                    sysDirs = SysService.GetUsableDirs().ToList();

                    if (sysDirs != null && sysDirs.Count > 0)
                    {
                        CreateCaching<TB_SYS_DIR>(CachingKey.DIRECTORY_KEY, sysDirs, Pub.GetSysConfig().Dependency.Directory);
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return sysDirs;
        }

        /// <summary>
        /// 取得所有可用机构
        /// </summary>
        /// <returns></returns>
        public IList<TB_SYS_ROLE> GetRoles()
        {
            IList<TB_SYS_ROLE> sysRoles = null;

            try
            {
                sysRoles = (IList<TB_SYS_ROLE>)GetCache(CachingKey.ROLES_KEY);

                if (sysRoles == null)
                {
                    sysRoles = SysService.GetRoles().ToList();

                    if (sysRoles != null && sysRoles.Count > 0)
                    {
                        CreateCaching<TB_SYS_ROLE>(CachingKey.ROLES_KEY, sysRoles, Pub.GetSysConfig().Dependency.Roles);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return sysRoles.ToList();
        }
    }
}
