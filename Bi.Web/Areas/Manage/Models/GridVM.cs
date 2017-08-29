using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Bi.Web.Areas.Manage.Models
{
    public class GridVM
    {
        public string RowPrimaryKey { get; set; }
        public string ViewName { get; set; }
        public string AjaxCallUrl { get; set; }

        /// <summary>
        /// 当前视图列集合
        /// </summary>
        public DataRow[] dsRows { get; set; }
    }
}