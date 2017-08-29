using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Bi.Biz;
using Bi.Models;
using Bi.Utility;
using Bi.Web.Areas.Manage.Models;
using Bi.Biz.Service;
using Microsoft.Practices.Unity;

namespace Bi.Web.App.Caching
{
    public class ViewCaching : BaseCaching
    {
        ILog logger = LogManager.GetLogger("SystemError");

        /// <summary>
        /// 取得数据库所有视图
        /// </summary>
        /// <returns></returns>
        public void FillViewData(GridVM model, IQueryService querySerrvice)
        {
            DataSet sysViews = new DataSet();

            try
            {
                sysViews = (DataSet)GetCache(CachingKey.SQL_VIEW_KEY);

                if (sysViews == null)
                {
                    sysViews = querySerrvice.GetDatabaseViews();

                    if (sysViews != null && sysViews.Tables[0].Rows.Count > 0)
                    {
                        CreateCaching(CachingKey.SQL_VIEW_KEY, sysViews, Pub.GetSysConfig().Dependency.Views);
                    }
                }
            }
            catch (Exception ex)
            {
                sysViews = querySerrvice.GetDatabaseViews();
                logger.Error(ex);
            }

            model.dsRows = sysViews.Tables[0].Select("ViewName = '" + model.ViewName + "'");
        }
    }
}