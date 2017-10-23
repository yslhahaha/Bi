using Bi.Biz.Security;
using Bi.Config;
using Bi.Domain;
using Bi.Utility;
using Bi.Web.App.Caching;
using Bi.Web.App.Facade;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Bi.Web.App
{
    public class BaseCtrl : Controller
    {/// <summary>
     /// web.config 自定义配置
     /// </summary>
        public SysConfigSection TisConfig
        {
            get
            {
                return (SysConfigSection)ConfigurationManager.GetSection("tiensConfiguration");
            }
        }

        /// <summary>
        /// long 类型 根目录ID
        /// </summary>
        public string RootDir
        {
            get
            {
                return TisConfig.SysParam.RootDirID;
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            BuildMenu();
            //BuildPathWord();
        }

        private void BuildPathWord()
        {
            if (Request.IsAjaxRequest()) return;

            string pathText = "";

            IList<TB_SYS_DIR> sysDirs = new SysCaching().GetUsableDirs();

            string url = Request.Url.AbsolutePath;

            var dir = sysDirs.Where(it => it.DIR_URL.Contains(url)).FirstOrDefault();

            if (dir == null) { ViewBag.Title = "授权店业务管理系统"; ViewBag.PathWord = "授权店业务管理系统"; }
            else
            {
                ViewBag.Title = dir.DIR_NAME;

                var root = sysDirs.Where(it => it.DIR_VIEW == "root").FirstOrDefault();

                while (dir.DIR_ID != root.DIR_ID)
                {
                    if (string.IsNullOrEmpty(pathText))
                        pathText = dir.DIR_NAME;
                    else
                        pathText = dir.DIR_NAME + " - " + pathText;

                    dir = sysDirs.Where(it => it.DIR_ID == dir.PARENT_ID).FirstOrDefault();
                }

                ViewBag.PathWord = pathText;
            }
        }


        private void BuildMenu()
        {
            var user = WebHelper.IdentityUser;

            //if (user.Rights.Count == 0)
            //{
            //    ViewData[ViewBagKey.Menu_Key] = "<div class=\"left\" id=\"cssmenu\"><ul class=\"menu\"><li class=\"has-sub\"><a href='#'><span>无权限访问</span></a></li></u></div>";
            //    return;
            //}

            IList<TB_SYS_DIR> sysDirs = new SysCaching().GetUsableDirs();

            //StringBuilder menuHtml = new StringBuilder("<div class=\"left\" id=\"cssmenu\"><ul class=\"menu\">");
            StringBuilder menuHtml = new StringBuilder();

            List<TB_SYS_DIR> topDirs = sysDirs.ToList().FindAll(it => it.PARENT_ID == RootDir).OrderBy(it => it.SORT_NO).ToList();

            if (topDirs.Count == 0)
                return;

            foreach (TB_SYS_DIR topDir in topDirs)
            {
                //如果用户权限目录中包括topDir所指目录
                //KeyValuePair<string, string> dir = new KeyValuePair<string, string>(topDir.DIR_ID.ToString(), topDir.DIR_URL.ToLower());
                //if (user.Rights.Contains(dir))
                //{
                //menuHtml.Append("<li class=\"has-sub\"><a href='#'><span>" + topDir.DIR_NAME + "</span></a>");


                menuHtml.Append(BuildSubMenus(sysDirs, topDir, user));

                //}
            }

            //menuHtml.Append("</ul></div>");

            ViewData[ViewBagKey.Menu_Key] = menuHtml.ToString();
        }

        /// <summary>
        /// 建立子目录
        /// </summary>
        /// <param name="sysDirs">所有目录数据</param>
        /// <param name="PARENT_ID">父目录ID</param>
        /// <param name="parentNode">父目录结点</param>
        private string BuildSubMenus(IList<TB_SYS_DIR> sysDirs, TB_SYS_DIR parentDir, IdentityUser user)
        {
            StringBuilder menuHtml = new StringBuilder();

            List<TB_SYS_DIR> subDirs = sysDirs.Where(it => it.PARENT_ID == parentDir.DIR_ID && (it.D_LEVEL < 3)).OrderBy(it => it.SORT_NO).ToList();

            if (subDirs.Count == 0)
            {
                if (parentDir.D_LEVEL == 1)
                {
                    menuHtml.Append("<li><a href=\"" + parentDir.DIR_URL + "\"><i class=\"" + parentDir.MEMO + "\"></i>" + parentDir.DIR_NAME + "</a></li>");
                    return menuHtml.ToString();
                }
                else
                {
                    menuHtml.Append("<li><a href=\"" + parentDir.DIR_URL + "\" >" + parentDir.DIR_NAME + "</a></li>");
                    return menuHtml.ToString();
                }
            }
            else
            {
                menuHtml.Append("<li class=\"sub open active\">" +
                                            "<a href = \"javascript:;\" >" +
                                                "<i class=\"" + parentDir.MEMO + "\"></i>" + parentDir.DIR_NAME + "<div class=\"pull-right\"><span class=\"caret\"></span></div>" +
                                            "</a>" +
                                            "<ul class=\"templatemo-submenu\">");

                foreach (TB_SYS_DIR dir in subDirs)
                {
                    //如果用户权限目录中包括dir所指目录
                    //KeyValuePair<string, string> d = new KeyValuePair<string, string>(dir.DIR_ID.ToString(), dir.DIR_URL.ToLower());
                    //if (user.Rights.Contains(d))
                    //{
                    //menuHtml.Append("<li><a href=\"" + dir.DIR_URL + "\" >" + dir.DIR_NAME + "</a>");

                    menuHtml.Append(BuildSubMenus(sysDirs, dir, user));

                    //menuHtml.Append("</li>");
                    //}
                }

                menuHtml.Append("</ul></li>");

                return menuHtml.ToString();
            }
        }

        /// <summary>
        /// 构造系统操作运行日志，
        /// </summary>
        /// <param name="operMan">操作人</param>
        /// <param name="operation">操作</param>
        /// <param name="objID">操作对象</param>
        /// <param name="objCode">操作对象代码</param>
        /// <param name="memo">操作描述</param>
        /// <returns></returns>
        public async Task<TB_SYS_LOG> Log(string funname, string operation, string objID, string objCode, string memo)
        {
            var user = await WebHelper.GetIdentityUser();

            TB_SYS_LOG log = new TB_SYS_LOG();

            log.LOGID = Guid.NewGuid().ToString();
            log.LOGTIME = Pub.GetLocalTime();
            //log.USERID = user.UserName;
            log.OPERATION = operation;
            log.FUNNAME = funname;
            log.BIZID = objID;
            log.BIZCODE = objCode;
            //log.IP = WebHelper.GetUserIP();
            log.MEMO1 = memo;

            if (Request.Url.ToString().Length > 100)
                log.URL = Request.Url.ToString().Substring(0, 100);
            else log.URL = Request.Url.ToString();

            return log;
        }

        /// <summary>
        /// 构造系统操作运行日志，
        /// </summary>
        /// <param name="operMan">操作人</param>
        /// <param name="operation">操作</param>
        /// <param name="objID">操作对象</param>
        /// <param name="objCode">操作对象代码</param>
        /// <param name="memo">操作描述</param>
        /// <returns></returns>
        public TB_SYS_LOG SyncLog(string funname, string operation, string objID, string objCode, string memo)
        {
            var user = WebHelper.IdentityUser;

            TB_SYS_LOG log = new TB_SYS_LOG();
            log.LOGID = Guid.NewGuid().ToString();
            log.LOGTIME = Pub.GetLocalTime();
            log.USERID = user.UserName;
            log.OPERATION = operation;
            log.FUNNAME = funname;
            log.BIZID = objID;
            log.BIZCODE = objCode;
            //log.IP = WebHelper.GetUserIP();
            log.MEMO1 = memo;
            if (Request.Url.ToString().Length > 100)
                log.URL = Request.Url.ToString().Substring(0, 100);
            else log.URL = Request.Url.ToString();

            return log;
        }

        /// <summary>
        /// 提交保存
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="successMsg"></param>
        /// <param name="failMsg"></param>
        public Task<string> Submit(bool flag)
        {
            if (flag)
            {
                return Task.FromResult<string>("ok");
            }
            else
            {
                return Task.FromResult<string>("");
            }
        }
    }
}