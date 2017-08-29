using Bi.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bi.Models
{
    public class QueryParams
    {
        public string ViewName { get; set; }
        public string PrimaryKeyCol { get; set; }
        public int PageNum { get; set; }
        public int RowsPerPage { get; set; }
        public int TotolRecordCount { get; set; }
        public int TotolPage { get; set; }

        /// <summary>
        /// 自定义过滤条件（写在程序里）
        /// </summary>
        public string CustomerFilter { get; set; }

        public Dictionary<string, string> columnArr { get; set; }
        public Dictionary<string, string> sortingArr { get; set; }
        public Dictionary<string, string> filterRulesArr { get; set; }

        public List<Columns> columns { get; set; }
        public List<Sorting> sorting { get; set; }
        public List<FilterRules> filterRules { get; set; }

        public QueryParams(string form)
        {
            columns = new List<Columns>();
            sorting = new List<Sorting>();
            filterRules = new List<FilterRules>();

            columnArr = new Dictionary<string, string>();
            sortingArr = new Dictionary<string, string>();
            filterRulesArr = new Dictionary<string, string>();

            BuildQuery(form);
        }

        private void BuildQuery(string form)
        {
            string[] fromKeyValue = form.Split('&');

            foreach (string kv in fromKeyValue)
            {
                if (kv.IndexOf("page_num") > -1)
                {
                    string[] keyValues = kv.Split('=');
                    PageNum = Pub.ConvertToInt32(keyValues[1]);
                }
                if (kv.IndexOf("rows_per_page") > -1)
                {
                    string[] keyValues = kv.Split('=');
                    RowsPerPage = Pub.ConvertToInt32(keyValues[1]);
                }
                if (kv.IndexOf("columns") > -1)
                {
                    string[] keyValues = kv.Split('=');
                    columnArr.Add(keyValues[0], keyValues[1]);
                }
                if (kv.IndexOf("sorting") > -1)
                {
                    string[] keyValues = kv.Split('=');
                    sortingArr.Add(keyValues[0], keyValues[1]);
                }
                if (kv.IndexOf("filter_rules") > -1)
                {
                    string[] keyValues = kv.Split('=');
                    filterRulesArr.Add(keyValues[0], keyValues[1]);
                }
            }

            int maxidx = 0;

            FilterRules frules = new FilterRules();
            int count = filterRulesArr.Count;
            int curIndex = 0;

            foreach (KeyValuePair<string, string> dr in filterRulesArr)
            {
                curIndex++;
                int start = dr.Key.IndexOf("[") + 1;
                int end = dr.Key.IndexOf("]");
                int idx = int.Parse(dr.Key.Substring(start, end - start));

                if (maxidx != idx)
                {
                    filterRules.Add(frules);
                    frules = new FilterRules();
                    maxidx = idx;
                }

                if (dr.Key.IndexOf("element_rule_id]") > 0) { frules.element_rule_id = dr.Value; }
                else if (dr.Key.IndexOf("logical_operator") > 0) { frules.logical_operator = dr.Value; }

                else if (dr.Key.IndexOf("condition") > 0)
                {
                    if (dr.Key.IndexOf("filterType") > 0) { frules.condi.filterType = dr.Value; }
                    else if (dr.Key.IndexOf("numberType") > 0) { frules.condi.numberType = dr.Value; }
                    else if (dr.Key.IndexOf("field") > 0) { frules.condi.field = dr.Value; }
                    else if (dr.Key.IndexOf("operator") > 0) { frules.condi.operater = dr.Value; }
                    else if (dr.Key.IndexOf("filterValue") > 0) { frules.condi.filterValue = dr.Value; }
                }

                if (count == curIndex) { filterRules.Add(frules); }
            }
        }

        public class Columns
        {
            string field { get; set; }
            string header { get; set; }
            string visible { get; set; }
            string sortable { get; set; }
        }

        public class Sorting
        {
            string sortingName { get; set; }
            string field { get; set; }
            string order { get; set; }
        }

        public class FilterRules
        {
            public FilterRules() { condi = new Condition(); }

            public string element_rule_id { get; set; }
            public string logical_operator { get; set; }

            public Condition condi { get; set; }

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
}
