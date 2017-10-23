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
    public class DirFacade
    {
        public ISysService SysService { get; set; }

        public DirFacade(ISysService service = null) { SysService = service; }

        internal void BuildSubmitData(FormCollection form, TB_SYS_DIR model)
        {
            if (form["hidParentDirId"] == null || form["hidParentDirId"].ToString() == "") { throw new Exception("[上级目录]未选定，请检查。"); }
            if (form["hidDirLevel"] == null || form["hidDirLevel"].ToString() == "") { throw new Exception("[上级目录]未选定，请检查。"); }

            model.PARENT_ID = form["hidParentDirId"].ToString();
            model.D_LEVEL = Pub.ConvertToDecimal(form["hidDirLevel"]);
            model.ENABLED = Pub.ConvertToByte(form["rbIsEnable"]);
            model.DIR_TYPE = Pub.ConvertToByte(form["rbDirType"]);
            model.DELETED = (byte)DeleteStatus.NO;

        }

        internal void BuildCreateDir(FormCollection form, TB_SYS_DIR model)
        {
            model.DIR_ID = Guid.NewGuid().ToString();

            BuildSubmitData(form, model);
        }

        internal void BuildUpdateDir(FormCollection form, TB_SYS_DIR model)
        {
            BuildSubmitData(form, model);
        }

        public TB_SYS_DIR FillEditPage(string id)
        {
            var dir = SysService.GetDirByKey(id);

            return dir;
        }
    }
}