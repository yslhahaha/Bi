using Bi.Biz.Service;
using Bi.Domain;
using Bi.Utility;
using Bi.Web.App;
using Bi.Web.App.Attribute;
using Bi.Web.App.Caching;
using Bi.Web.App.Facade;
using Bi.Web.Areas.Manage.Models;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Bi.Web.Areas.Manage.Controllers
{
    public class DirController : BaseCtrl
    {
        [Dependency]
        public ISysService SysService { get; set; }

        [Dependency]
        public IQueryService QuerySerrvice { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Query()
        {
            GridVM gv = new GridVM();
            gv.ViewName = "BI_V_DIRS";
            gv.PK = "主键";

            new ViewCaching().FillViewData(gv, QuerySerrvice);

            return View(gv);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxException]
        public async Task<string> Create(FormCollection form, TB_SYS_DIR model)
        {
            DirFacade dirHelper = new DirFacade();

            dirHelper.BuildCreateDir(form, model);

            TB_SYS_LOG log = await Log("目录管理", "创建系统目录", "", "", "新增系统目录动作");

            bool result = SysService.CreateDir(model, log);

            Pub.ModifyDependencyFile(TisConfig.Dependency.Directory);

            if (result)
                return "ok";
            else return "保存失败。";
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(string id)
        {
            DirFacade dirHelper = new DirFacade(SysService);

            var dir = dirHelper.FillEditPage(id);

            return View(dir);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxException]
        public async Task<string> Edit(FormCollection form, TB_SYS_DIR model)
        {
            DirFacade dirHelper = new DirFacade();

            dirHelper.BuildUpdateDir(form, model);

            TB_SYS_LOG log = await Log("目录管理", "更新目录", model.DIR_ID.ToString(), model.DIR_NAME, "更新目录信息");

            return await Submit(SysService.UpdateDir(model, log));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [AjaxException]
        public async Task<string> Delete(string id)
        {
            TB_SYS_LOG log = await Log("目录管理", "更新目录", id, "", "更新目录信息");

            SysService.DeleteDir(id, log);

            return "ok";
        }
    }
}