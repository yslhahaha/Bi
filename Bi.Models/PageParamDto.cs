using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bi.Models
{
    /// <summary>
    /// 复合查询对象
    /// </summary>
    public class QueryFiltersDto
    {
        public string GroupOp { get; set; }

        public IList<QueryRulesDto> Rules { get; set; }

        public IList<QueryFiltersDto> Groups { get; set; }

        public string ViewFilter { get; set; }
    }

    /// <summary>
    /// 查询规则
    /// </summary>
    public class QueryRulesDto
    {
        private bool dataIsCol = false;

        public string Field { get; set; }

        public string Op { get; set; }

        public string Data { get; set; }

        public bool DataIsCol
        {
            get { return dataIsCol; }
            set { dataIsCol = value; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PageParamDto
    {
        /// <summary>
        /// 页数
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页记录条数
        /// </summary>
        public int RecordCount { get; set; }
        /// <summary>
        /// 记录总条数
        /// </summary>
        public int TotolRecordCount { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotolPage { get; set; }
        /// <summary>
        /// 排序列名
        /// </summary>
        public string SortColumn { get; set; }
        /// <summary>
        /// 升、降序
        /// </summary>
        public string SortOrder { get; set; }
        /// <summary>
        /// 标识列
        /// </summary>
        public string IdCol { get; set; }
    }
}
