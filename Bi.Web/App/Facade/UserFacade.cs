using Bi.Biz.Security;
using Bi.Biz.Service;
using Bi.Domain;
using Bi.Utility;
using Bi.Web.App.Caching;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Bi.Web.App.Facade
{
    public class UserFacade
    {
        public ISysService SysService { get; set; }

        public UserFacade(ISysService service = null) { SysService = service; }

        internal void BuildSubmitData(FormCollection form, TB_ADMIN_USER model)
        {
            model.STATUS = Pub.ConvertToByte(form["rbIsEnable"]);

            if (model.USER_TYPE != (short)UserType.KM_USER)
            {
                if (model.PASSWORD == "") { throw new Exception("[创建密码]不能为空"); }
                if (model.ComparePwd == "") { throw new Exception("[确认密码]不能为空"); }
                if (model.USER_NAME == "") { throw new Exception("[用户名称]不能为空"); }

                if (model.PASSWORD != model.ComparePwd) { throw new Exception("两次输入的密码的不一致，请重新输入。"); }

                model.PASSWORD = new PasswordHasher().MD5(model.PASSWORD);
            }

            model.UserRole = new List<TB_USER_ROLE>();

            if (form["selectRoleIds"].ToString().Length <= 0) { throw new Exception("请为用户选择角色。"); }

            foreach (string roleId in form["selectRoleIds"].Substring(0, form["selectRoleIds"].Length - 1).Split(','))
            {
                model.UserRole.Add(
                    new TB_USER_ROLE()
                    {
                        USER_ID = model.USER_ID,
                        ROLE_ID = roleId
                    });
            }
        }

        internal void BuildNewUser(FormCollection form, TB_ADMIN_USER model)
        {
            model.USER_ID = Guid.NewGuid().ToString();
            model.USER_TYPE = Pub.ConvertToByte(form["rbUserType"].ToString());

            BuildSubmitData(form, model);
        }

        internal void BuildUpdateUser(FormCollection form, TB_ADMIN_USER model)
        {
            BuildSubmitData(form, model);
        }

        internal async Task<string> BuildPwdData(FormCollection form, TB_ADMIN_USER userDto)
        {
            if (userDto.PASSWORD != userDto.ComparePwd) throw new Exception("设置两次新密码不一致。");

            var identityUser = await WebHelper.GetIdentityUser();
            if (!identityUser.IsAdmin)
            {
                if (userDto.PASSWORD == "" || userDto.ComparePwd == ""
                                || userDto.OldPwd == "") throw new Exception("* 为填写项，请完成相关信息再提交。");


                var user = SysService.GetUserByKey(identityUser.Id);

                if (user.PASSWORD != new PasswordHasher().MD5(userDto.OldPwd)) { throw new Exception("旧密码错误。"); }
                userDto.OldPwd = new PasswordHasher().MD5(userDto.OldPwd);

            }
            else
            {
                if (userDto.PASSWORD == "" || userDto.ComparePwd == "") throw new Exception("* 为填写项，请完成相关信息再提交。");
            }

            userDto.PASSWORD = new PasswordHasher().MD5(userDto.PASSWORD);


            return "";
        }

        public TB_ADMIN_USER FillEditPage(string id)
        {
            var user = SysService.GetUserByKey(id);

            return user;
        }
    }
}