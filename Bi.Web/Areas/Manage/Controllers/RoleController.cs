using Bi.Biz.Service;
using Bi.Domain;
using Bi.Web.App;
using Bi.Web.App.Attribute;
using Bi.Web.App.Caching;
using Bi.Web.App.Facade;
using Bi.Web.Areas.Manage.Models;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Bi.Web.Areas.Manage.Controllers
{
    public class RoleController : BaseCtrl
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
            gv.ViewName = "BI_V_ROLES";
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
        public async Task<string> Create(FormCollection form, TB_SYS_ROLE model)
        {
            TB_SYS_LOG log;
            bool result;

            RoleFacade userHelper = new RoleFacade();

            if (form["ROLE_ID"] == null || form["ROLE_ID"]=="")
            {
                userHelper.BuildNewUser(form, model);

                log = await Log("角色管理", "创建系统角色", "", "", "新增系统角色动作");

                result = SysService.CreateRole(model, log);
            }
            else
            {
                userHelper.BuildUpdateUser(form, model);

                log = await Log("角色管理", "更新系统角色", "", "", "更新系统角色动作");

                result = SysService.UpdateRole(model, log);
            }

            if (result)
                return "ok";
            else return "保存失败。";
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [AjaxException]
        public async Task<string> Delete(string id)
        {
            TB_SYS_LOG log = await Log("角色管理", "更新角色", id, "", "删除角色信息");

            SysService.DeleteUser(id, log);

            return "ok";
        }


        /// <summary>
        /// 查询角色
        /// </summary>
        /// <returns></returns>
        public string GetList()
        {
            var roles = SysService.GetRoles().ToList();

            return JsonConvert.SerializeObject(roles);
        }

    }
}