using Bi.Biz.Security;
using Bi.Utility;
using Bi.Web.App.Facade;
using Bi.Web.Areas.Auth.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Bi.Web.Areas.Auth.Controllers
{
    public class LoginController : Controller
    {
        private BiSignInManager _signInManager;
        private BiUserManager _userManager;

        public LoginController()
        {
        }

        public LoginController(BiUserManager userManager, BiSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public BiSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<BiSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public BiUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<BiUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [AllowAnonymous]
        public ActionResult Sign(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Sign(LoginVM model, string returnUrl)
        {
            #region 验证码

            if (model == null) { model = new LoginVM(); }


            if (Session[SessionKey.Val_Code_Key] == null || Session[SessionKey.Val_Code_Key].ToString() != Request.Form["vaCode"].ToString())
            {
                ValidateCode vCode = new ValidateCode();
                string code = vCode.CreateValidateCode(4);
                Session[SessionKey.Val_Code_Key] = code;
                ModelState.AddModelError("ValError", "* 验证码错误，请重新输入。");
                return View(model);
            }

            #endregion

            string logId = Guid.NewGuid().ToString();

            //var result = await SignInHelper.PasswordSignIn(model.UserID, model.Password, model.RememberMe, logId, shouldLockout: false);
            var result = await SignInManager.PasswordSignIn(model.UserID, model.Password, model.RememberMe, logId, shouldLockout: false);
            switch (result)
            {
                case BiSignInStatus.Success:
                    {
                        return RedirectToLocal(returnUrl);
                    }
                case BiSignInStatus.LockedOut:
                    {
                        ModelState.AddModelError("UserID", "* 用户已被锁定不可用，请联系管理员。");
                        return View(model);
                    }
                case BiSignInStatus.RequiresTwoFactorAuthentication:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                case BiSignInStatus.Failure:
                    {
                        ModelState.AddModelError("UserID", "* 用户名或密码错误");
                        return View(model);
                    }
                case BiSignInStatus.Verify:
                    return RedirectToLocal("/B2C/SHOP/NEW/");
                default:
                    {
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                    }
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home", new { Area = "B2C" });
        }

        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Sign", "Login");
        }

        /// <summary>
        /// 获得验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult GetValidateCode()
        {
            ValidateCode vCode = new ValidateCode();
            string code = vCode.CreateValidateCode(4);
            Session[SessionKey.Val_Code_Key] = code;
            byte[] bytes = vCode.CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");
        }
    }
}