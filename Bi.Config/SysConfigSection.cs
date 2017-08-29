using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bi.Config
{
    public class SysConfigSection : ConfigurationSection
    {
        /// <summary>
        /// 缓存依赖参数
        /// </summary>
        [ConfigurationProperty("dependency")]
        public DependencyElement Dependency
        {
            get { return (DependencyElement)this["dependency"]; }
            set { this["dependency"] = value; }
        }

        /// <summary>
        /// 系统一般参数
        /// </summary>
        [ConfigurationProperty("sysparam")]
        public SysParamElement SysParam
        {
            get { return (SysParamElement)this["sysparam"]; }
            set { this["sysparam"] = value; }
        }

        /// <summary>
        /// 页面一般参数
        /// </summary>
        [ConfigurationProperty("webparam")]
        public WebParamElement WebParam
        {
            get { return (WebParamElement)this["webparam"]; }
            set { this["webparam"] = value; }
        }

        /// <summary>
        /// 数据字典表tbl_Sys_Dictionary查询,关键字 tbl_Sys_Dictionary.DictTypeName
        /// </summary>
        [ConfigurationProperty("dictionary")]
        public DictionaryElement DictionaryQuery
        {
            get { return (DictionaryElement)this["dictionary"]; }
            set { this["dictionary"] = value; }
        }

        [ConfigurationProperty("mail")]
        public MailElement Mail
        {
            get { return (MailElement)this["mail"]; }
            set { this["mail"] = value; }
        }
    }


    /// <summary>
    /// 分页
    /// </summary>
    public class MailElement : ConfigurationElement
    {
        /// <summary>
        /// 接收人
        /// </summary>
        [ConfigurationProperty("switch", DefaultValue = "", IsRequired = false)]
        public string Switch
        {
            get { return (string)this["switch"]; }
            set { this["switch"] = value; }
        }
        /// <summary>
        /// 接收人
        /// </summary>
        [ConfigurationProperty("to", DefaultValue = "", IsRequired = false)]
        public string To
        {
            get { return (string)this["to"]; }
            set { this["to"] = value; }
        }

        /// <summary>
        /// 发送人
        /// </summary>
        [ConfigurationProperty("from", DefaultValue = "", IsRequired = false)]
        public string From
        {
            get { return (string)this["from"]; }
            set { this["from"] = value; }
        }

        /// <summary>
        /// 发送人密码
        /// </summary>
        [ConfigurationProperty("frompwd", DefaultValue = "", IsRequired = false)]
        public string FromPwd
        {
            get { return (string)this["frompwd"]; }
            set { this["frompwd"] = value; }
        }

        /// <summary>
        /// 主机
        /// </summary>
        [ConfigurationProperty("host", DefaultValue = "", IsRequired = false)]
        public string Host
        {
            get { return (string)this["host"]; }
            set { this["host"] = value; }
        }

        /// <summary>
        /// 端口
        /// </summary>
        [ConfigurationProperty("post", DefaultValue = 0x19, IsRequired = false)]
        public int Port
        {
            get { return (int)this["post"]; }
            set { this["post"] = value; }
        }
    }

    /// <summary>
    /// 缓存依赖文件
    /// </summary>
    public class DependencyElement : ConfigurationElement
    {
        /// <summary>
        /// 后台系统目录
        /// </summary>
        [ConfigurationProperty("directory", DefaultValue = "Directory.txt", IsRequired = false)]
        public string Directory
        {
            get { return (string)this["directory"]; }
            set { this["directory"] = value; }
        }

        /// <summary>
        /// 角色对应目录
        /// </summary>
        [ConfigurationProperty("roletodirs", DefaultValue = "Roletodirs.txt", IsRequired = false)]
        public string RoleToDirs
        {
            get { return (string)this["roletodirs"]; }
            set { this["roletodirs"] = value; }
        }

        /// <summary>
        /// 角色
        /// </summary>
        [ConfigurationProperty("roles", DefaultValue = "Roles.txt", IsRequired = false)]
        public string Roles
        {
            get { return (string)this["roles"]; }
            set { this["roles"] = value; }
        }

        /// <summary>
        /// 系统字典表
        /// </summary>
        [ConfigurationProperty("dictionary", DefaultValue = "Dict.txt", IsRequired = false)]
        public string Dictionary
        {
            get { return (string)this["dictionary"]; }
            set { this["dictionary"] = value; }
        }

        /// <summary>
        /// 系统字典分类表
        /// </summary>
        [ConfigurationProperty("dictcategory", DefaultValue = "DictCategory.txt", IsRequired = false)]
        public string DictCategory
        {
            get { return (string)this["dictcategory"]; }
            set { this["dictcategory"] = value; }
        }

        /// <summary>
        /// 期数
        /// </summary>
        [ConfigurationProperty("period", DefaultValue = "Period.txt", IsRequired = false)]
        public string Period
        {
            get { return (string)this["period"]; }
            set { this["period"] = value; }
        }

        /// <summary>
        /// 数据库系统参数表tb_Parameter缓存参数
        /// </summary>
        [ConfigurationProperty("params", DefaultValue = "Params.txt", IsRequired = false)]
        public string Params
        {
            get { return (string)this["params"]; }
            set { this["params"] = value; }
        }

        /// <summary>
        /// 数据库视图
        /// </summary>
        [ConfigurationProperty("views", DefaultValue = "Views.txt", IsRequired = false)]
        public string Views
        {
            get { return (string)this["views"]; }
            set { this["views"] = value; }
        }
    }

    /// <summary>
    /// 后台系统参数常量
    /// </summary>
    public class SysParamElement : ConfigurationElement
    {
        /// <summary>
        /// 用户锁定时间
        /// </summary>
        [ConfigurationProperty("rootdirid", DefaultValue = "3", IsRequired = true)]
        public string RootDirID
        {
            get { return (string)this["rootdirid"]; }
            set { this["rootdirid"] = value; }
        }

        /// <summary>
        /// 用户锁定时间
        /// </summary>
        [ConfigurationProperty("loginlimittime", DefaultValue = "30", IsRequired = true)]
        public double LoginLimitTime
        {
            get { return (double)this["loginlimittime"]; }
            set { this["loginlimittime"] = value; }
        }

        /// <summary>
        /// 用户登录尝试次数
        /// </summary>
        [ConfigurationProperty("trylogintimes", DefaultValue = "5", IsRequired = true)]
        public int TryLoginTimes
        {
            get { return (int)this["trylogintimes"]; }
            set { this["trylogintimes"] = value; }
        }

        /// <summary>
        /// 用户设置密码强度要求，默认是3级，最低是1级，最高是5级
        /// </summary>
        [ConfigurationProperty("pwdsecuritylevel", DefaultValue = "3", IsRequired = true)]
        public int PwdSecurityLevel
        {
            get { return (int)this["pwdsecuritylevel"]; }
            set { this["pwdsecuritylevel"] = value; }
        }
    }

    /// <summary>
    /// Web页面常用参数
    /// </summary>
    public class WebParamElement : ConfigurationElement
    {
        /// <summary>
        /// 后台菜单目录根目录ID
        /// </summary>
        [ConfigurationProperty("hiddencolumns", IsRequired = false)]
        public string HiddenColumns
        {
            get { return (string)this["hiddencolumns"]; }
            set { this["hiddencolumns"] = value; }
        }
    }

    /// <summary>
    /// 数据字典表tbl_Sys_Dictionary查询,关键字 tbl_Sys_Dictionary.DictTypeName
    /// </summary>
    public class DictionaryElement : ConfigurationElement
    {

    }
    
}
