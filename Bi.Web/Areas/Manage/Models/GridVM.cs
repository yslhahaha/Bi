using Bi.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace Bi.Web.Areas.Manage.Models
{
    public class GridVM
    {
        public GridVM()
        {
            Sorting = new Dictionary<string, string>();
        }

        public string PK { get; set; }
        public string ViewName { get; set; }
        public string AjaxCallUrl { get; set; }

        /// <summary>
        /// 当前视图列集合
        /// </summary>
        public DataRow[] dsRows { get; set; }
        /// <summary>
        /// 设置排序<字段名，ascending/descending>
        /// </summary>
        public Dictionary<string, string> Sorting { get; set; }
        /// <summary>
        /// Grid 列
        /// </summary>
        public StringBuilder Cols { get; set; }
        public StringBuilder DateCols { get; set; }
        public StringBuilder NumCols { get; set; }
        public StringBuilder StringCols { get; set; }
        public StringBuilder SortingCols { get; set; }

        public void BuildColsAndFilter()
        {
            Cols = new StringBuilder();
            DateCols = new StringBuilder();
            NumCols = new StringBuilder();
            StringCols = new StringBuilder();
            SortingCols = new StringBuilder();

            foreach (System.Data.DataRow dr in dsRows)
            {
                string colName = dr["ColName"] != null ? dr["ColName"].ToString() : "";

                if (colName.CompareTo(PK) == 0)
                {
                    Cols.Append("{ field: '" + PK + "', header: 'Code',  visible: 'no' },");
                }

                if (colName.CompareTo(PK) != 0 && !Pub.IsHidenColumn(colName))
                {
                    string colType = dr["ColType"] != null ? dr["ColType"].ToString() : "";

                    switch (colType)
                    {
                        case "DATE":
                            Cols.Append("{ field: '" + colName + "', header: '" + colName + "' },");
                            DateCols.Append(colName + ",");
                            break;
                        case "money":
                            Cols.Append("{ field: '" + colName + "', header: '" + colName + "', dataClass:'text-right' },");
                            NumCols.Append(colName + ",");
                            break;
                        case "smallint":
                            Cols.Append("{ field: '" + colName + "', header: '" + colName + "', dataClass:'text-right' },");
                            NumCols.Append(colName + ",");
                            break;
                        case "int":
                            Cols.Append("{ field: '" + colName + "', header: '" + colName + "', dataClass:'text-right' },");
                            NumCols.Append(colName + ",");
                            break;
                        case "float":
                            Cols.Append("{ field: '" + colName + "', header: '" + colName + "', dataClass:'text-right' },");
                            NumCols.Append(colName + ",");
                            break;
                        case "bit":
                            Cols.Append("{ field: '" + colName + "', header: '" + colName + "', dataClass:'text-right' },");
                            NumCols.Append(colName + ",");
                            break;
                        case "decimal":
                            Cols.Append("{ field: '" + colName + "', header: '" + colName + "', dataClass:'text-right' },");
                            NumCols.Append(colName + ",");
                            break;
                        case "NUMBER":
                            Cols.Append("{ field: '" + colName + "', header: '" + colName + "', dataClass:'text-right' },");
                            NumCols.Append(colName + ",");
                            break;
                        case "bigint":
                            Cols.Append("{ field: '" + colName + "', header: '" + colName + "', dataClass:'text-right' },");
                            NumCols.Append(colName + ",");
                            break;
                        case "CHAR":
                            Cols.Append("{ field: \"" + colName + "\", header: \'" + colName + "\' },");
                            StringCols.Append(colName + ",");
                            break;
                        case "VARCHAR2":
                            Cols.Append("{ field: '" + colName + "', header: '" + colName + "' },");
                            StringCols.Append(colName + ",");
                            break;
                        default:
                            Cols.Append("{ field: \"" + colName + "\", header: \'" + colName + "\' },");
                            StringCols.Append(colName + ",");
                            break;
                    }
                }
            }
            if (StringCols.Length > 0) StringCols.Remove(StringCols.Length - 1, 1);
            if (DateCols.Length > 0) DateCols.Remove(DateCols.Length - 1, 1);
            if (NumCols.Length > 0) NumCols.Remove(NumCols.Length - 1, 1);

            //生成排序
            if (Sorting.Count > 0)
            {
                foreach (var sort in Sorting)
                {
                    SortingCols.Append("{ sortingName: \"" + sort.Key + "\", field: \"" + sort.Key + "\", order: \"" + sort.Value + "\" },");
                }
            }
        }
    }

    public class FilterRules
    {
        public FilterRules() { condi = new List<Condition>(); }

        public List<Condition> condi { get; set; }

        public class Condition
        {
            public string filterType { get; set; }
            public string numberType { get; set; }
            public string field { get; set; }
            public string operater { get; set; }
            public string filterValue { get; set; }
        }
    }
}