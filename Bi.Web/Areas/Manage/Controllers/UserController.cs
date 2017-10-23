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
    public class UserController : BaseCtrl
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
            gv.ViewName = "BI_V_USERS";
            gv.PK = "主键";

            gv.Sorting.Add("建立时间", SortingKey.DESC);
            gv.Sorting.Add("用户姓名", SortingKey.ASC);

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
        public async Task<string> Create(FormCollection form, TB_ADMIN_USER model)
        {

            UserFacade userHelper = new UserFacade();

            userHelper.BuildNewUser(form, model);

            TB_SYS_LOG log = await Log("用户管理", "创建系统用户", "", "", "新增系统用户动作");

            bool result = SysService.CreateUser(model, log);

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
            UserFacade userHelper = new UserFacade(SysService);

            var user = userHelper.FillEditPage(id);

            return View(user);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxException]
        public async Task<string> Edit(FormCollection form, TB_ADMIN_USER model)
        {
            UserFacade userHelper = new UserFacade();

            userHelper.BuildUpdateUser(form, model);

            TB_SYS_LOG log = await Log("用户管理", "更新用户", model.USER_ID.ToString(), model.USER_NAME, "更新用户信息");

            return await Submit(SysService.UpdateUser(model, log));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [AjaxException]
        public async Task<string> Delete(string id)
        {
            TB_SYS_LOG log = await Log("用户管理", "更新用户", id, "", "更新用户信息");

            SysService.DeleteUser(id, log);

            return "ok";
        }
    }
}