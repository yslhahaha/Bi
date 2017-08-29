using Bi.Web.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Bi.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //在应用程序启动时运行的代码
            //加载log4net的配置文件
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(
                        AppDomain.CurrentDomain.SetupInformation.ApplicationBase +
                        "log4net.config"));
        }
    }
}
