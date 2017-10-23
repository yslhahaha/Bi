using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bi.Web.Areas.Manage.Models
{
    /// <summary>
    /// tree节点
    /// </summary>
    public class Node
    {
        public Node() { }

        public Node(string pId, string noteText, List<Node> node)
        {
            id = pId;
            text = noteText;
            nodes = node;
        }

        public string id { get; set; }    //数据库保存的数据Id
        public string text { get; set; }   //节点名称

        public int level { get; set; }//层级
        public NodeState state { get; set; } //节点状态
        public List<Node> nodes { get; set; }
    }

    /// <summary>
    /// 节点状态
    /// </summary>
    public class NodeState
    {
        public NodeState(bool ck, bool sel, bool ex)
        {
            @checked = ck;
            selected = sel;
            expanded = ex;
        }

        public bool @checked;
        public bool selected;
        public bool expanded;
    }
}