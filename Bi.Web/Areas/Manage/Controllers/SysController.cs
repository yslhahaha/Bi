using Bi.Biz.Service;
using Bi.Domain;
using Bi.Models;
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
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Bi.Web.Areas.Manage.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class SysController : BaseCtrl
    {
        [Dependency]
        public ISysService SysService { get; set; }

        [Dependency]
        public IQueryService querySerrvice { get; set; }
        #region User

        public ActionResult Users()
        {
            GridVM gv = new GridVM();
            gv.ViewName = "BI_V_USERS";
            gv.RowPrimaryKey = "主键";
            new ViewCaching().FillViewData(gv, querySerrvice);

            return View(gv);
        }

        public Task<string> GetUsers()
        {
            QueryParams p = new QueryParams(Server.UrlDecode(Request.Form.ToString()));

            string jsonword = "{\"total_rows\":\"200\",\"page_data\":["
            + "{\"customer_id\":\"85\",\"lastname\":\"<a href=\\\"\\/test\\/85\\\" rel=\\\"nofollow\\\">Albert<\\/a>\",\"firstname\":\"Sean\",\"email\":\"Mauris.blandit@vestibulumMauris.com\",\"gender\":\"male\",\"date_updated\":\"03\\/07\\/2007 10:01:51\"},"
            + "{\"customer_id\":\"49\",\"lastname\":\"<a href=\\\"\\/test\\/49\\\" rel=\\\"nofollow\\\">Allison<\\/a>\",\"firstname\":\"Trevor\",\"email\":\"arcu@non.edu\",\"gender\":\"male\",\"date_updated\":\"19\\/12\\/2008 17:53:43\"},"
            + "{\"customer_id\":\"152\",\"lastname\":\"<a href=\\\"\\/test\\/152\\\" rel=\\\"nofollow\\\">Andrews<\\/a>\",\"firstname\":\"Rhea\",\"email\":\"id.magna.et@scelerisquemollisPhasellus.org\",\"gender\":\"female\",\"date_updated\":\"16\\/03\\/1991 04:53:13\"},"
            + "{\"customer_id\":\"53\",\"lastname\":\"<a href=\\\"\\/test\\/53\\\" rel=\\\"nofollow\\\">Atkins<\\/a>\",\"firstname\":\"Lawrence\",\"email\":\"egestas.Aliquam.nec@convallis.com\",\"gender\":\"male\",\"date_updated\":\"25\\/07\\/1995 09:14:37\"},"
            + "{\"customer_id\":\"41\",\"lastname\":\"<a href=\\\"\\/test\\/41\\\" rel=\\\"nofollow\\\">Austin<\\/a>\",\"firstname\":\"Harper\",\"email\":\"Aliquam@feugiatnon.org\",\"gender\":\"male\",\"date_updated\":\"30\\/12\\/2008 22:35:55\"},"
            + "{\"customer_id\":\"3\",\"lastname\":\"<a href=\\\"\\/test\\/3\\\" rel=\\\"nofollow\\\">Bailey<\\/a>\",\"firstname\":\"Benjamin\",\"email\":null,\"gender\":\"male\",\"date_updated\":\"17\\/06\\/2012 03:17:19\"},"
            + "{\"customer_id\":\"19\",\"lastname\":\"<a href=\\\"\\/test\\/19\\\" rel=\\\"nofollow\\\">Barker<\\/a>\",\"firstname\":\"Uriel\",\"email\":\"dictum.eu.placerat@enim.ca\",\"gender\":\"male\",\"date_updated\":\"30\\/06\\/1999 09:48:11\"},"
            + "{\"customer_id\":\"172\",\"lastname\":\"<a href=\\\"\\/test\\/172\\\" rel=\\\"nofollow\\\">Barr<\\/a>\",\"firstname\":\"Kelly\",\"email\":\"fringilla.purus.mauris@pedenonummy.com\",\"gender\":\"female\",\"date_updated\":\"23\\/12\\/2013 03:35:01\"},"
            + "{\"customer_id\":\"131\",\"lastname\":\"<a href=\\\"\\/test\\/131\\\" rel=\\\"nofollow\\\">Bates<\\/a>\",\"firstname\":\"Uta\",\"email\":\"consectetuer@nec.com\",\"gender\":\"female\",\"date_updated\":\"27\\/08\\/1998 00:58:49\"},"
            + "{\"customer_id\":\"65\",\"lastname\":\"<a href=\\\"\\/test\\/65\\\" rel=\\\"nofollow\\\">Bean<\\/a>\",\"firstname\":\"Nash\",\"email\":\"tincidunt.Donec@magnaa.ca\",\"gender\":\"male\",\"date_updated\":\"31\\/05\\/1992 20:54:57\"}],"
            + "\"error\":null,\"filter_error\":[],\"debug_message\":[]}";

            return Task.FromResult<string>(jsonword);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public ActionResult NewUser()
        {
            return View();
        }

        [HttpPost]
        [AjaxException]
        public async Task<string> NewUser(FormCollection form, TB_ADMIN_USER model)
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
        public ActionResult EditUser(string id)
        {
            UserFacade userHelper = new UserFacade(SysService);

            var user = userHelper.FillEditPage(id);

            return View(user);
        }

        [HttpPost]
        [AjaxException]
        public async Task<string> EditUser(FormCollection form, TB_ADMIN_USER model)
        {
            UserFacade userHelper = new UserFacade();

            userHelper.BuildUpdateUser(form, model);

            TB_SYS_LOG log = await Log("用户管理", "更新用户", model.USER_ID.ToString(), model.USER_NAME, "更新用户信息");

            return await Submit(SysService.UpdateUser(model, log));
        }

        #endregion

        public ActionResult Roles()
        {
            return View();
        }

        public string GetRoles()
        {
            var roles = SysService.GetRoles().ToList();

            return JsonConvert.SerializeObject(roles);
        }

        public ActionResult Dirs()
        {
            return View();
        }
    }
}