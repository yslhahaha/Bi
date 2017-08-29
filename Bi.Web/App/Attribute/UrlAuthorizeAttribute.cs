using Bi.Web.App.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bi.Web.App.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class UrlAuthorizeAttribute : AuthorizeAttribute
    {
        private bool isAuthorize = true;

        public UrlAuthorizeAttribute() : base() { }

        public UrlAuthorizeAttribute(bool IsAuthorize)
            : base()
        {
            if (!IsAuthorize) isAuthorize = IsAuthorize;
        }

        public override async void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = AjaxError("登录超时，请重新登录系统", filterContext);
                }
                else
                {
                    filterContext.Result = new RedirectResult(
                                        string.Concat("/Auth/Login/Sign",
                                                     "?ReturnUrl=",
                                                     filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.RawUrl)));
                }
            }
            else
            {
                //TODO:权限验证
                var user = await WebHelper.GetIdentityUser();

                if (!isAuthorize) return;

                string urlPath = "";

                if (filterContext.HttpContext.Request.RawUrl.IndexOf("?") > 0)
                {
                    urlPath = filterContext.HttpContext.Request.RawUrl.Replace("?", "/").ToLower();
                }
                else
                {
                    urlPath = filterContext.HttpContext.Request.RawUrl.ToLower();
                }

                string[] path = urlPath.Split('/');
                string rightUrl = "";

                if (path.Length >= 4)
                    rightUrl = "/" + path[1] + "/" + path[2] + "/" + path[3] + "/";
                else
                    rightUrl = urlPath;

                if (string.IsNullOrEmpty(rightUrl)) return;
                if (rightUrl.ToLower().CompareTo("/") == 0) return;
                if (rightUrl.ToLower().CompareTo("/b2c") == 0) return;
                if (rightUrl.ToLower().CompareTo("/b2c/home") == 0) return;
                if (rightUrl.ToLower().CompareTo("/b2c/home/index") == 0) return;
                if (rightUrl.ToLower().IndexOf("image") >= 0) return;
                if (rightUrl.ToLower().IndexOf("/b2c/home/index") >= 0) return;

                if (!user.Rights.Values.Contains(rightUrl))
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = AjaxError("你没有权限访问该页或请重新登录再试。", filterContext);
                    }
                    else
                    {
                        filterContext.HttpContext.Response.Write("你没有权限访问该页或请重新<a href=\"/Auth/Login/Sign/\">登录</a>再试。");
                        filterContext.HttpContext.Response.End();
                    }
                }
            }
        }

        /// <summary>
        /// Ajaxes the error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="filterContext">The filter context.</param>
        /// <returns>JsonResult</returns>
        protected JsonResult AjaxError(string message, AuthorizationContext filterContext)
        {
            //If message is null or empty, then fill with generic message
            if (String.IsNullOrEmpty(message))
                message = "Something went wrong while processing your request. Please refresh the page and try again.";

            //Set the response status code to 500
            filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;

            //Needed for IIS7.0
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

            return new JsonResult
            {
                //can extend more properties 
                Data = new AjaxNoRightModel() { ErrorMessage = message },
                ContentEncoding = System.Text.Encoding.UTF8,
                JsonRequestBehavior = JsonRequestBehavior.DenyGet
            };
        }
    }

    /// <summary>
    /// AjaxExceptionModel
    /// </summary>
    public class AjaxNoRightModel
    {
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; set; }
    }
}