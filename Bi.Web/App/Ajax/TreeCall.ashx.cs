using Bi.Biz.Service;
using Bi.Domain;
using Bi.Models;
using Bi.Utility;
using Bi.Web.App.Caching;
using Bi.Web.App_Start;
using Bi.Web.Areas.Manage.Models;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Bi.Web.App.Ajax
{
    /// <summary>
    /// QueryCall 的摘要说明
    /// </summary>
    public class TreeCall : IHttpHandler
    {
        public ISysService client { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            client = UnityConfig.GetService<ISysService>();

            string type = context.Request.Params["type"].ToString();//tree类型

            StringBuilder response = new StringBuilder();

            switch (type)
            {
                case "dir":
                    CreateDirTree(context);
                    break;
                case "roledir":
                    CreateRoleDirTree(context);
                    break;
                case "menu":
                    break;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private void CreateRoleDirTree(HttpContext context)
        {
            TB_SYS_ROLE role = null;

            string id = Pub.GetSysConfig().SysParam.RootDirID;
            string roleId = "";

            if (context.Request.Params["roleid"] != null)
            {
                roleId = context.Request.Params["roleid"].ToString();
                role = client.GetRoleByKey(roleId);
            }

            IList<TB_SYS_DIR> dirs = new SysCaching().GetUsableDirs();
            IList<TB_SYS_DIR> levelDirs = dirs.Where(it => it.PARENT_ID == id).ToList();

            StringBuilder result = new StringBuilder();

            List<Node> dirNodes = new List<Node>();

            Node root = new Node();
            root.id = id;
            root.text = "根目录";
            root.level = 0;

            if (levelDirs.Count > 0)
            {
                root.nodes = new List<Node>();

                foreach (TB_SYS_DIR dir in levelDirs)
                {
                    int childNum = dirs.Where(it => it.PARENT_ID == dir.DIR_ID).Count();

                    Node node = CreateTreeNode(null, dir);

                    if (role != null && role.DirIds.Where(it => it.ToString() == dir.DIR_ID).FirstOrDefault() != null)
                    {
                        node.state = new NodeState(true,false,true);
                    }

                    if (childNum > 0)
                    {
                        CreateRoleSubDirTree(node, dirs, role);
                    }

                    root.nodes.Add(node);
                }
            }

            dirNodes.Add(root);

            if (dirNodes.Count > 0)
                context.Response.Write(JsonConvert.SerializeObject(dirNodes));
            else
                context.Response.Write("");

            context.Response.Flush();
        }


        private void CreateRoleSubDirTree(Node parentNode, IList<TB_SYS_DIR> dirs, TB_SYS_ROLE role)
        {
            parentNode.nodes = new List<Node>();

            IList<TB_SYS_DIR> levelDirs = dirs.Where(it => it.PARENT_ID == parentNode.id).ToList();

            if (levelDirs.Count <= 0)
                return;
            foreach (TB_SYS_DIR dir in levelDirs)
            {
                int childNum = dirs.Where(it => it.PARENT_ID == dir.DIR_ID).Count();

                Node node = CreateTreeNode(null, dir);

                if (role != null && role.DirIds.Where(it => it.ToString() == dir.DIR_ID).FirstOrDefault() != null)
                {
                    node.state = new NodeState(true, false, true);
                }

                if (childNum > 0)
                {
                    CreateRoleSubDirTree(node, dirs, role);
                }

                parentNode.nodes.Add(node);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        private void CreateDirTree(HttpContext context)
        {
            string id = Pub.GetSysConfig().SysParam.RootDirID;
            string selectedDirID = "0";//已经存在选定的DirID
            string curID = "0";//编辑时，所编辑的目录ID

            if (context.Request.Params["id"] != null) id = context.Request.Params["id"].ToString();
            if (context.Request.Params["dirId"] != null && context.Request.Params["dirId"].ToString() != "") selectedDirID = context.Request.Params["dirId"].ToString();
            if (context.Request.Params["curId"] != null && context.Request.Params["curId"].ToString() != "") curID = context.Request.Params["curId"].ToString();

            StringBuilder result = new StringBuilder();

            List<Node> dirNodes = new List<Node>();

            Node root = new Node();

            if (id == Pub.GetSysConfig().SysParam.RootDirID && context.Request.Params["id"] == null)
            {
                root.id = id;
                root.text = "根目录";
                root.level = 0;

                if (id == selectedDirID)
                {
                    NodeState state = new NodeState(true, true, true);
                    root.state = state;
                }
            }

            IList<TB_SYS_DIR> dirs = new SysCaching().GetUsableDirs();
            IList<TB_SYS_DIR> levelDirs = dirs.Where(it => it.PARENT_ID == id).ToList();
            if (levelDirs.Count > 0)
            {
                root.nodes = new List<Node>();

                foreach (TB_SYS_DIR dir in levelDirs)
                {
                    if (dir.DIR_ID == curID) continue;

                    int childNum = dirs.Where(it => it.PARENT_ID == dir.DIR_ID).Count();

                    Node node = CreateTreeNode(selectedDirID, dir);
                    node.level = root.level + 1;

                    if (childNum > 0)
                    {
                        CreateSubDirTree(node, dirs, selectedDirID, curID);
                    }

                    root.nodes.Add(node);
                }
            }

            dirNodes.Add(root);

            if (dirNodes.Count > 0)
                context.Response.Write(JsonConvert.SerializeObject(dirNodes));
            else
                context.Response.Write("");

            context.Response.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="dirs"></param>
        /// <param name="selectedDirID"></param>
        /// <param name="curID"></param>
        private void CreateSubDirTree(Node parentNode, IList<TB_SYS_DIR> dirs, string selectedDirID, string curID)
        {
            parentNode.nodes = new List<Node>();

            IList<TB_SYS_DIR> levelDirs = dirs.Where(it => it.PARENT_ID == parentNode.id).ToList();

            if (levelDirs.Count <= 0)
                return;
            foreach (TB_SYS_DIR dir in levelDirs)
            {
                if (dir.DIR_ID == curID) continue;

                int childNum = dirs.Where(it => it.PARENT_ID == dir.DIR_ID).Count();

                Node node = CreateTreeNode(selectedDirID, dir);
                node.level = parentNode.level + 1;

                if (childNum > 0)
                {
                    CreateSubDirTree(node, dirs, selectedDirID, curID);
                }

                parentNode.nodes.Add(node);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedDirID"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        private static Node CreateTreeNode(string selectedDirID, TB_SYS_DIR dir)
        {
            Node node = new Node();
            node.id = dir.DIR_ID;
            node.text = dir.DIR_NAME;

            if (dir.DIR_ID == selectedDirID)
            {
                NodeState state = new NodeState(true, true, true);
                node.state = state;
            }

            return node;
        }
    }
}