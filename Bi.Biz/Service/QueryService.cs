using Bi.Domain;
using Bi.Models;
using Bi.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Bi.Models.QueryParams;

namespace Bi.Biz.Service
{
    public class QueryService : IQueryService
    {
        public StringBuilder Sql = new StringBuilder();

        QueryParams queryParams;

        #region 数据库视图核心查询

        /// <summary>
        /// 从数据库视图获取数据
        /// </summary>
        /// <param name="p.CustomerFilter"></param>
        /// <param name="pageParams"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public string GetDatabaseViewData(QueryParams param)
        {
            queryParams = param;

            if (queryParams.filterRulesArr.Count == 0)
            {
                Sql.Append("select * from " + queryParams.ViewName + " v ");
            }
            else
            {
                Sql.Append("select * from " + queryParams.ViewName + " v" + " where 1=1");

                if (queryParams.filterRules.Count > 0)
                {
                    foreach (var rule in queryParams.filterRules)
                    {
                        if (rule.logical_operator.CompareTo("AND") == 0)
                        {//用 AND 连接
                            CreateWhere(rule.condi, "and");
                        }
                        else
                        {
                            CreateWhere(rule.condi, "or");
                        }
                    }
                }
            }

            if (queryParams.CustomerFilter != null && !string.IsNullOrEmpty(queryParams.CustomerFilter))
            {
                Sql = Sql.Replace("1=1", "(1=1");
                Sql.Append(") and " + queryParams.CustomerFilter);
            }

            string sortingDir = string.Empty;

            string execSql = "";

            Sql.Append(" order by ");

            if (queryParams.sortingArr.Count > 0)
            {
                for (int i = 0; i < queryParams.sortingArr.Count / 3; i++)
                {
                    if (queryParams.sortingArr["sorting[" + i + "][order]"] != "none")
                    {
                        if (queryParams.sortingArr["sorting[" + i + "][order]"] == "ascending")
                        {
                            Sql.Append(" " + queryParams.sortingArr["sorting[" + i + "][field]"] + ",");
                        }
                        else
                        {
                            Sql.Append(" " + queryParams.sortingArr["sorting[" + i + "][field]"] + " desc,");
                        }
                    }
                    else
                    {
                        Sql.Append(" " + queryParams.PrimaryKeyCol + ",");
                    }
                }
            }
            else
            {
                Sql.Append(" " + queryParams.PrimaryKeyCol + ",");
            }

            execSql = Sql.ToString().Substring(0, Sql.Length - 1).Replace("1=1 or", "");

            DataSet ds = new DataSet();

            using (OraDb104 db = new OraDb104())
            {
                if (queryParams.RowsPerPage == 0)
                {
                    //导出时，不分页
                    execSql = execSql.ToString();

                    ds = db.ExecuteSql(execSql);
                }
                else
                {
                    //分页
                    execSql = GetPagedSql(execSql.ToString(), queryParams.PageNum - 1, queryParams.RowsPerPage);

                    ds = db.ExecuteSql(execSql);

                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        CreatePagerInfo(Pub.ConvertToInt32(ds.Tables[0].Rows[0][0].ToString()));
                    else
                        CreatePagerInfo(0);
                }
            }

            return CreateResponseData(ds);
        }

        /// <summary>
        /// 生成查询Where
        /// </summary>
        /// <param name="condi"></param>
        /// <param name="group"></param>
        private void CreateWhere(FilterRules.Condition condi, string group)
        {

            if (condi.field.IndexOf("时间") > -1 || condi.field.IndexOf("日期") > -1 || condi.filterType == "date")
            {
                condi.filterValue = "TO_DATE(" + condi.filterValue + ",'yyyy-MM-dd HH24:mi:ss')";

                //匹配相应的操作
                switch (condi.operater)
                {
                    case "equal":
                        Sql.Append(" " + group + " " + condi.field + " = " + condi.filterValue);
                        break;
                    case "less":
                        Sql.Append(" " + group + " " + condi.field + " < " + condi.filterValue);
                        break;
                    case "less_or_equal":
                        Sql.Append(" " + group + " " + condi.field + " <= " + condi.filterValue);
                        break;
                    case "greater":
                        Sql.Append(" " + group + " " + condi.field + " > " + condi.filterValue);
                        break;
                    case "greater_or_equal":
                        Sql.Append(" " + group + " " + condi.field + " >= " + condi.filterValue);
                        break;
                }
            }
            else
            {
                //匹配相应的操作
                switch (condi.operater)
                {
                    case "equal":
                        Sql.Append(" " + group + " " + condi.field + " = '" + condi.filterValue + "' ");
                        break;
                    case "not_equal":
                        Sql.Append(" " + group + " " + condi.field + " <> '" + condi.filterValue + "' ");
                        break;
                    case "less":
                        Sql.Append(" " + group + " " + condi.field + " < '" + condi.filterValue + "' ");
                        break;
                    case "less_or_equal":
                        Sql.Append(" " + group + " " + condi.field + " <= '" + condi.filterValue + "' ");
                        break;
                    case "greater":
                        Sql.Append(" " + group + " " + condi.field + " > '" + condi.filterValue + "' ");
                        break;
                    case "greater_or_equal":
                        Sql.Append(" " + group + " " + condi.field + " >= '" + condi.filterValue + "' ");
                        break;
                    case "contains":
                        Sql.Append(" " + group + " " + condi.field + " like '%" + condi.filterValue + "%' ");
                        break;
                    case "in":
                        Sql.Append(" " + group + " " + condi.field + " in ('" + condi.filterValue + "') ");
                        break;
                    case "not_in":
                        Sql.Append(" " + group + " " + condi.field + " not in ('" + condi.filterValue + "') ");
                        break;
                    case "is_null":
                        Sql.Append(" " + group + " " + condi.field + " is null ");
                        break;
                    case "is_not_null":
                        Sql.Append(" " + group + " " + condi.field + " is not null ");
                        break;
                }
            }


        }

        /// <summary>
        /// 将SQL自动转化为分页SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private string GetPagedSql(string sql, int pageIndex, int pageSize)
        {
            if (String.IsNullOrEmpty(sql))
            {
                return null;
            }
            string start = "0";
            if (pageIndex > 0)
            {
                start = Convert.ToString(pageIndex * pageSize);
            }
            int select = sql.GetSymPosition("select");
            string end = Convert.ToString((pageIndex + 1) * pageSize);
            string orderSql = sql.GetStrBySym("order by");
            //fromwhere部分 含from、join及where部分group by 但不含order by
            string fromSql = sql.GetStrBySym("from");
            //select部分 含select
            string selectSql = "select " + sql.Substring(select, sql.Length - select - fromSql.Length);
            selectSql = selectSql.Replace("*", "v.*");
            if (!String.IsNullOrEmpty(orderSql))
            {
                fromSql = fromSql.Substring(0, fromSql.Length - orderSql.Length);
            }
            else
            {
                throw new Exception(" sql2005 怎么着也得弄个order by啊");
            }
            //合并成分页SQL
            string strSql = "select (totalCountSql) totalCount,data.* from (" + selectSql;
            //order部分 含 order by
            string rownum = "row_number()";

            strSql += "," + rownum + " over (" + orderSql + ") rn " + fromSql// + " v "
                   + ") data where rn>" + start + " and rn<=" + end;
            if (!String.IsNullOrEmpty(fromSql.GetStrBySym("group by")))
            {
                fromSql = " from (select count(*) totalCount " + fromSql + ") tbl";
            }

            string totalCountSql = "select count(*) as totalCount " + fromSql;

            return strSql.Replace("r\n", " ").Replace('\n', ' ').Replace("totalCountSql", totalCountSql);
        }

        /// <summary>
        /// 计算分页页数
        /// </summary>
        /// <param name="count">记录总条数</param>
        private void CreatePagerInfo(int count)
        {
            queryParams.TotolRecordCount = count;

            if (queryParams.PageNum > count)
                queryParams.PageNum = count;

            if (queryParams.TotolRecordCount % queryParams.RowsPerPage > 0)
            {
                queryParams.TotolPage = queryParams.TotolRecordCount / queryParams.RowsPerPage + 1;
            }
            else
            {
                queryParams.TotolPage = queryParams.TotolRecordCount / queryParams.RowsPerPage;
            }
        }

        /// <summary>
        /// 生成返回数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dsViewData"></param>
        /// <returns></returns>
        private string CreateResponseData(DataSet dsViewData)
        {
            StringBuilder data = new StringBuilder("{\"total_rows\":\"" + queryParams.TotolRecordCount + "\",\"page_data\":[");

            foreach (DataRow dr in dsViewData.Tables[0].Rows)
            {
                data.Append(CreateXmlRowData(dr, dsViewData));
            }

            return data.ToString().Substring(0, data.Length - 1) + "]," + "\"error\":null,\"filter_error\":[],\"debug_message\":[]}"; ;
        }


        /// <summary>
        /// 获得到每行的数据
        /// </summary>
        /// <param name="objType">视图列表</param>
        /// <param name="rowID"></param>
        /// <returns></returns>
        private string CreateXmlRowData(DataRow drData, DataSet dsViewData)
        {
            StringBuilder data = new StringBuilder();

            data.Append("{");
            //生成除主键外的其它列
            foreach (DataColumn dc in dsViewData.Tables[0].Columns)
            {
                if (!Pub.IsHidenColumn(dc.ColumnName))
                {
                    data.Append("\"" + dc.ColumnName + "\":\"" + FormatData(drData[dc.ColumnName].ToString(), dc) + "\",");
                }
            }

            return data.ToString().Substring(0, data.Length - 1) + "},";
        }

        /// <summary>
        /// 格式化数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string FormatData(string value, DataColumn column)
        {
            string returnValue = "";

            string valueType = column.DataType.FullName;

            #region format data

            switch (valueType)
            {
                case "System.String":
                    returnValue = value;
                    break;
                case "System.Int32":
                    if (value != null && value != "")
                        returnValue = Int32.Parse(value).ToString("D");
                    break;
                case "System.Boolean":
                    if (value != null && value != "")
                        returnValue = Boolean.Parse(value).ToString();
                    break;
                case "System.DateTime":
                    if (value != null && value != "")
                    {
                        if (column.ColumnName.IndexOf("日期") != -1)
                            returnValue = DateTime.Parse(value).ToString("yyyy-MM-dd");
                        else
                            returnValue = DateTime.Parse(value).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    break;
                case "System.Decimal":
                    if (value != null && value != "")
                    {
                        if (column.ColumnName.IndexOf("数") != -1) { returnValue = Decimal.Parse(value).ToString("F0"); }
                        else if (column.ColumnName.IndexOf("价") != -1) { returnValue = Decimal.Parse(value).ToString("N4"); }
                        else if (column.ColumnName.IndexOf("额") != -1) { returnValue = Decimal.Parse(value).ToString("N2"); }
                        else if (column.ColumnName.IndexOf("序") != -1) { returnValue = Decimal.Parse(value).ToString("F0"); }
                        else { returnValue = Decimal.Parse(value).ToString("N2"); }
                    }

                    break;
                case "System.Double":
                    if (value != null && value != "")
                    {

                        if (column.ColumnName.IndexOf("数") != -1) { returnValue = Decimal.Parse(value).ToString("F0"); }
                        else if (column.ColumnName.IndexOf("价") != -1) { returnValue = Decimal.Parse(value).ToString("N4"); }
                        else if (column.ColumnName.IndexOf("额") != -1) { returnValue = Decimal.Parse(value).ToString("N2"); }
                        else if (column.ColumnName.IndexOf("序") != -1) { returnValue = Decimal.Parse(value).ToString("F0"); }
                        else { returnValue = Decimal.Parse(value).ToString("N2"); }
                    }
                    break;
                case "System.Int16":
                    if (value != null && value != "")
                        returnValue = Int16.Parse(value).ToString("D");
                    break;
                case "System.Int64":
                    if (value != null && value != "")
                        returnValue = Int64.Parse(value).ToString("D");
                    break;
                case "System.Single":
                    if (value != null && value != "")
                        returnValue = Single.Parse(value).ToString("D");
                    break;
                default:
                    returnValue = value;
                    break;
            }

            #endregion

            return returnValue;
        }

        #endregion

        #region 全部数据库视图
        public DataSet GetDatabaseViews()
        {
            string sql = @"SELECT COLUMN_NAME COLNAME, TABLE_NAME VIEWNAME, DATA_TYPE COLTYPE
                                  FROM USER_TAB_COLUMNS TC
                                  LEFT JOIN USER_OBJECTS UO
                                    ON TC.TABLE_NAME = UO.OBJECT_NAME
                                 WHERE UO.OBJECT_TYPE = 'VIEW'
                                   AND UO.OBJECT_NAME LIKE 'BI%'
                                 ORDER BY TC.COLUMN_ID";

            using (OraDb104 db = new OraDb104())
            {
                DataSet ds = db.ExecuteSql(sql);

                return ds;
            }
        }

        #endregion
    }

    public static class ExtMethod
    {
        public static int GetCount(this string str, string sym)
        {
            if (String.IsNullOrEmpty(str) || String.IsNullOrEmpty(sym))
            {
                return 0;
            }
            else
            {
                return str.Length - str.Replace(sym, "").Length;
            }
        }

        public static int GetSymPosition(this string str, string sym)
        {
            if (String.IsNullOrEmpty(str) || String.IsNullOrEmpty(sym))
            {
                return 0;
            }
            else
            {
                Regex regex = new Regex(@"^[\s\n]*?(?<key>" + sym + @")[\[\(\s\n]+?.*?$", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
                if (regex.IsMatch(str))
                {
                    return regex.Match(str).Groups["key"].Index + sym.Length;
                }
                else
                {
                    return 0;
                }
            }
        }

        public static string GetStrBySym(this string str, string sym)
        {
            if (String.IsNullOrEmpty(str) || String.IsNullOrEmpty(sym))
            {
                return null;
            }
            else
            {
                string result = null;
                Regex regex = new Regex(@"^.*?[\]\)\s\n\*]+?(?<key>" + sym + @")[\[\(\s\n\*]+?.*?$", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
                if (regex.IsMatch(str))
                {
                    bool isHas = false;
                    result = str;
                    while (regex.IsMatch(result))
                    {
                        result = result.Substring(regex.Match(result).Groups["key"].Index);
                        if (GetCount(result, "(") == GetCount(result, ")"))
                        {
                            isHas = true;
                            break;
                        }
                    }
                    if (!isHas)
                    {
                        result = null;
                    }
                }
                return result;
            }
        }
    }
}
