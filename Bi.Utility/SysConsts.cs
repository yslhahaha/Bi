namespace Bi.Utility
{
    public static class ConnKey
    {
        public const string Conn_Cnjsb01_Key = "OraDb104Conn_CNJSB01";
        public const string Conn_Cnjsb_Key = "OraDb104Conn_CNJSB";
    }

    /// <summary>
    /// 菜单key
    /// </summary>
    public static class ViewBagKey
    {
        public const string Menu_Key = "Sys_Menu";
    }

    /// <summary>
    /// 系统Session KEY
    /// </summary>
    public static class SessionKey
    {
        /// <summary>
        /// 后台用户Session信息 KEY
        /// </summary>
        public const string SYS_USER_INFO = "Sys_User_Info";
        /// <summary>
        /// 
        /// </summary>
        public const string QUERY_FILTER_KEY = "Query_Filter";
        /// <summary>
        /// 验证码 key
        /// </summary>
        public const string Val_Code_Key = "ValidateCode";
        /// <summary>
        /// 多语系 Key
        /// </summary>
        public const string Culture_Key = "Culture";
    }

    /// <summary>
    /// 系统缓存用到的 KEY
    /// </summary>
    public static class CachingKey
    {
        /// <summary>
        /// 后台目录缓存 Key
        /// </summary>
        public const string DIRECTORY_KEY = "Sys_Directory_List";
        /// <summary>
        /// 后台角色缓存 Key
        /// </summary>
        public const string ROLES_KEY = "Sys_Role_List";
        /// <summary>
        /// 后台角色与目录缓存 Key
        /// </summary>
        public const string ROLES_DIRECTORY_KEY = "Sys_Role_Directory_List";

        /// <summary>
        /// SQL视图缓存
        /// </summary>
        public const string SQL_VIEW_KEY = "SQL_VIEW";

    }
}
