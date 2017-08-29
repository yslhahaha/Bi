using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace Bi.Web.App.Caching
{
    public class BaseCaching
    {
        #region Caching

        /// <summary>
        /// 建立缓存，方式：文件依赖
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cachingKey"></param>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        public void CreateCaching<T>(string cachingKey, IList<T> data, string fileName)
        {
            string dicPath = System.Web.Hosting.HostingEnvironment.MapPath("~/CachingFiles/");

            if (!Directory.Exists(dicPath))
                Directory.CreateDirectory(dicPath);

            string filePath = dicPath + fileName;

            if (!File.Exists(filePath))
                File.Create(filePath);

            System.Web.Caching.CacheDependency dep = new System.Web.Caching.CacheDependency(filePath);

            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(
                cachingKey,
                data,
                dep,
                System.Web.Caching.Cache.NoAbsoluteExpiration, //从不过期
                System.Web.Caching.Cache.NoSlidingExpiration, //禁用可调过期
                System.Web.Caching.CacheItemPriority.Default,
                null);
        }

        /// <summary>
        /// 建立缓存，方式：文件依赖
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cachingKey"></param>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        public void CreateCaching(string cachingKey, DataSet sysViews, string fileName)
        {
            string dicPath = System.Web.Hosting.HostingEnvironment.MapPath("~/CachingFiles/");

            if (!Directory.Exists(dicPath))
                Directory.CreateDirectory(dicPath);

            string filePath = dicPath + fileName;

            if (!File.Exists(filePath))
                File.Create(filePath);


            System.Web.Caching.CacheDependency dep = new System.Web.Caching.CacheDependency(filePath);

            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(
                cachingKey,
                sysViews,
                dep,
                System.Web.Caching.Cache.NoAbsoluteExpiration, //从不过期
                System.Web.Caching.Cache.NoSlidingExpiration, //禁用可调过期
                System.Web.Caching.CacheItemPriority.Default,
                null);
        }

        #endregion

        ///<summary>
        ///获取当前应用程序指定CacheKey的Cache对象值
        ///</summary>
        ///<param name="CacheKey">索引键值</param>
        ///<returns>返回缓存对象</returns>
        public static object GetCache(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;

            return objCache[CacheKey];
        }

        ///<summary>
        ///设置以缓存依赖的方式缓存数据
        ///</summary>
        ///<param name="CacheKey">索引键值</param>
        ///<param name="objObject">缓存对象</param>
        ///<param name="cacheDepen">依赖对象</param>
        public static void SetCache(string CacheKey, object objObject, System.Web.Caching.CacheDependency dep)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(
                CacheKey,
                objObject,
                dep,
                System.Web.Caching.Cache.NoAbsoluteExpiration, //从不过期
                System.Web.Caching.Cache.NoSlidingExpiration, //禁用可调过期
                System.Web.Caching.CacheItemPriority.Default,
                null);
        }
    }
}