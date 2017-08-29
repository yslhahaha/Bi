using Bi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bi.Biz.Service
{
    public interface IQueryService
    {
        #region 数据库视图核心查询

        /// <summary>
        /// 从数据库视图获取数据
        /// </summary>
        /// <param name="queryFilters"></param>
        /// <param name="pageParams"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        string GetDatabaseViewData(QueryParams p);

        #endregion

        #region 全部数据库视图
        DataSet GetDatabaseViews();

        #endregion
    }
}
