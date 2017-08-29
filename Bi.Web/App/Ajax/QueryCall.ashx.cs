using Bi.Biz.Service;
using Bi.Models;
using Bi.Web.App_Start;
using Bi.Web.Areas.Manage.Models;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bi.Web.App.Ajax
{
    /// <summary>
    /// QueryCall 的摘要说明
    /// </summary>
    public class QueryCall : IHttpHandler
    {
        public IQueryService client { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            client = UnityConfig.GetService<IQueryService>();

            QueryParams p = new QueryParams(HttpUtility.UrlDecode(context.Request.Form.ToString()));

            p.PrimaryKeyCol = context.Request.Params["PrimaryKeyCol"].ToString();//行标况
            p.ViewName = context.Request.Params["ViewName"].ToString();//视图名称

            if (context.Request.Params["myfilter"] != null && context.Request.Params["myfilter"].ToString() != "")
            {
                p.CustomerFilter = context.Server.UrlDecode(context.Request["myfilter"].ToString());
            }

            string response = client.GetDatabaseViewData(p);

            context.Response.ContentType = "text/xml";
            context.Response.Charset = "utf-8";
            context.Response.Write(response);
            context.Response.Flush();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}