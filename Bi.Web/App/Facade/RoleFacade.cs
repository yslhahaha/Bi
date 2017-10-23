using Bi.Biz.Security;
using Bi.Biz.Service;
using Bi.Domain;
using Bi.Utility;
using Bi.Web.App.Caching;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Bi.Web.App.Facade
{
    public class RoleFacade
    {
        public ISysService SysService { get; set; }

        public RoleFacade(ISysService service = null) { SysService = service; }

        internal void BuildSubmitData(FormCollection form, TB_SYS_ROLE model)
        {
            if (form["hidRoleDirIds"] == null || form["hidRoleDirIds"].ToString() == "") { throw new Exception("请选择权限目录。"); }

            model.STATUS = Pub.ConvertToByte(form["rbStatus"]);
            model.IS_ADMIN = Pub.ConvertToByte(form["rbIsAdmin"]);

            model.DirIds = form["hidRoleDirIds"].ToString().Substring(0, form["hidRoleDirIds"].ToString().Length - 1).Split(',').ToList();
        }

        internal void BuildNewUser(FormCollection form, TB_SYS_ROLE model)
        {
            model.ROLE_ID = Guid.NewGuid().ToString();

            BuildSubmitData(form, model);
        }

        internal void BuildUpdateUser(FormCollection form, TB_SYS_ROLE model)
        {
            BuildSubmitData(form, model);
        }
    }
}