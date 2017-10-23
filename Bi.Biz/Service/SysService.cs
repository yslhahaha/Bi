using AutoMapper;
using Bi.Domain;
using Bi.Models;
using Bi.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bi.Biz.Service
{
    public class SysService : ISysService
    {
        #region User

        public bool CreateUser(TB_ADMIN_USER user, TB_SYS_LOG log)
        {
            if (user == null)
                throw new DomainException("参数出错，请联系管理员。");

            using (OraDb104 _db = new OraDb104())
            {
                string exsitSql = "SELECT * FROM TB_ADMIN_USER WHERE USER_NAME = '" + user.USER_NAME + @"'";

                var ds = _db.ExecuteSql(exsitSql);

                if (ds.Tables[0].Rows.Count > 0) { throw new DomainException("该用户已存在。"); }

                user.SECURITYSTAMP = Guid.NewGuid().ToString();
                user.CREATEDATE = Pub.GetLocalTime();

                _db.TB_ADMIN_USER.Add(user);

                foreach (var role in user.UserRole)
                {
                    _db.TB_USER_ROLE.Add(new TB_USER_ROLE() { USER_ID = role.USER_ID, ROLE_ID = role.ROLE_ID });
                }

                _db.TB_SYS_LOG.Add(log);

                _db.SaveChanges();

                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userDto"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool UpdateUser(TB_ADMIN_USER userDto, TB_SYS_LOG log)
        {
            if (userDto == null)
                throw new ArgumentNullException("userDto");

            using (OraDb104 _db = new OraDb104())
            {
                var user = _db.TB_ADMIN_USER.Where(p => p.USER_ID == userDto.USER_ID).FirstOrDefault();

                if (user == null)
                    throw new Exception(userDto.USER_ID + " 不存在");

                if (userDto.USER_TYPE != (short)UserType.KM_USER)
                {
                    user.PASSWORD = userDto.PASSWORD;
                }
                else
                {
                    user.PASSWORD = "";
                }

                user.CN_NAME = userDto.CN_NAME;
                user.STATUS = userDto.STATUS;
                user.SECURITYSTAMP = Guid.NewGuid().ToString();

                _db.TB_USER_ROLE.RemoveRange(_db.TB_USER_ROLE.Where(it => it.USER_ID == userDto.USER_ID));

                foreach (var role in userDto.UserRole)
                {
                    _db.TB_USER_ROLE.Add(new TB_USER_ROLE() { USER_ID = role.USER_ID, ROLE_ID = role.ROLE_ID });
                }

                _db.TB_SYS_LOG.Add(log);

                _db.SaveChanges();

                return true;
            }
        }

        public bool DeleteUser(string userId, TB_SYS_LOG log)
        {
            throw new ArgumentNullException("userDto");

            using (OraDb104 _db = new OraDb104())
            {
                List<string> sqls = new List<string>();

                sqls.Add("UPDATE TB_ADMIN_USER SET STATUS = " + (short)UserStatus.DISABLE + " WHERE USER_ID = '" + userId + "'");

                sqls.Add(_db.CreateLogSql(log));

                var flag = _db.ExecuteNonQueryTran(CommandType.Text, sqls);

                if (flag) return true;

                return false;
            }
        }

        public TB_ADMIN_USER GetUserByKey(string userId)
        {
            using (OraDb104 _db = new OraDb104())
            {
                string sql = @"SELECT U.*,R.*
                                      FROM TB_ADMIN_USER U
                                      LEFT JOIN TB_USER_ROLE UR
                                        ON U.USER_ID = UR.USER_ID
                                      LEFT JOIN TB_SYS_ROLE R
                                        ON UR.ROLE_ID = R.ROLE_ID
                                     WHERE U.USER_ID = '" + userId + "'";

                var ds = _db.ExecuteSql(sql);

                if (ds.Tables[0].Rows.Count == 0) { throw new DomainException("该用户不存在。"); }

                DataTable userTable = ds.Tables[0].DefaultView.ToTable(true, "USER_ID",
                                                                                            "USER_NAME",
                                                                                            "PASSWORD",
                                                                                            "CN_NAME",
                                                                                            "STATUS",
                                                                                            "CREATEDATE",
                                                                                            "LOGINCOUNT",
                                                                                            "LASTLOGINDATE",
                                                                                            "SECURITYSTAMP",
                                                                                            "USER_TYPE");

                var user = Pub.ToObject<TB_ADMIN_USER>(ds.Tables[0])[0];

                DataTable tbRoles = ds.Tables[0].DefaultView.ToTable(true, "ROLE_ID",
                                                                                            "ROLE_NAME",
                                                                                            "ROLE_TYPE",
                                                                                            "DESCR",
                                                                                            "IS_ADMIN",
                                                                                            "STATUS");

                user.Roles = tbRoles.AsEnumerable().Where(it => it["ROLE_ID"] != null && it["ROLE_ID"].ToString() != "").Select(r => new TB_SYS_ROLE()
                {
                    ROLE_ID = r["ROLE_ID"].ToString(),
                    ROLE_NAME = r["ROLE_NAME"].ToString(),
                    ROLE_TYPE = r["ROLE_TYPE"].ToString(),
                    DESCR = r["DESCR"].ToString(),
                    IS_ADMIN = Pub.ConvertToByte(r["IS_ADMIN"]),
                    STATUS = Pub.ConvertToByte(r["STATUS"])
                }).ToList();

                return user;
            }
        }

        public TB_ADMIN_USER GetUserByLoginName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            using (OraDb104 _db = new OraDb104())
            {
                var user = _db.TB_ADMIN_USER.Where(c => c.USER_NAME == userName).FirstOrDefault();

                return user;
            }
        }

        public TB_ADMIN_USER GetUserByNameForValidate(string name, string userId)
        {
            using (OraDb104 _db = new OraDb104())
            {
                TB_ADMIN_USER user = null;

                if (userId != "")
                    user = _db.TB_ADMIN_USER.Where(d => d.USER_NAME == name && d.USER_ID != userId).FirstOrDefault();
                else
                    user = _db.TB_ADMIN_USER.Where(d => d.USER_NAME == name).FirstOrDefault();

                return user;
            }
        }

        #endregion

        #region Role

        public TB_SYS_ROLE GetRoleByKey(string id)
        {
            using (OraDb104 _db = new OraDb104())
            {
                var role = _db.TB_SYS_ROLE.Where(p => p.ROLE_ID == id).FirstOrDefault();

                if (role != null)
                    role.DirIds = _db.TB_ROLE_DIR.Where(d => d.ROLE_ID == role.ROLE_ID).Select(it => it.DIR_ID).ToList();

                return role;
            }
        }

        public IEnumerable<TB_SYS_ROLE> GetRoles()
        {
            using (OraDb104 _db = new OraDb104())
            {
                var roles = _db.TB_SYS_ROLE.Where(it => it.STATUS == (byte)RoleStatus.NORMAL).ToList();

                List<TB_SYS_ROLE> roleList = new List<TB_SYS_ROLE>();

                foreach (TB_SYS_ROLE d in roles)
                {
                    roleList.Add(d);
                }

                return roleList;
            }
        }

        public TB_SYS_ROLE GetRoleByNameForValidate(string name, string roleId)
        {
            using (OraDb104 _db = new OraDb104())
            {
                TB_SYS_ROLE user = null;

                if (roleId != "")
                    user = _db.TB_SYS_ROLE.Where(d => d.ROLE_NAME == name && d.ROLE_ID != roleId).FirstOrDefault();
                else
                    user = _db.TB_SYS_ROLE.Where(d => d.ROLE_NAME == name).FirstOrDefault();

                return user;

            }
        }

        public bool CreateRole(TB_SYS_ROLE roleDto, TB_SYS_LOG log)
        {
            if (roleDto == null)
                throw new ArgumentNullException("TB_SYS_ROLE");

            using (OraDb104 _db = new OraDb104())
            {
                TB_SYS_ROLE role = roleDto;

                foreach (string dirId in roleDto.DirIds)
                {
                    TB_ROLE_DIR rd = new TB_ROLE_DIR() { DIR_ID = dirId, ROLE_ID = role.ROLE_ID };
                    _db.TB_ROLE_DIR.Add(rd);
                }

                _db.TB_SYS_ROLE.Add(role);
                _db.TB_SYS_LOG.Add(log);

                _db.SaveChanges();

                return true;
            }
        }

        public bool UpdateRole(TB_SYS_ROLE roleDto, TB_SYS_LOG log)
        {
            if (roleDto == null)
                throw new ArgumentNullException("roleDto");

            using (OraDb104 _db = new OraDb104())
            {
                var role = _db.TB_SYS_ROLE.Where(r => r.ROLE_ID == roleDto.ROLE_ID).FirstOrDefault();

                if (role == null)
                    throw new Exception(roleDto.ROLE_NAME + " 不存在");

                var delRoleDirsQuery = _db.TB_ROLE_DIR.Where(it => it.ROLE_ID == roleDto.ROLE_ID);

                _db.TB_ROLE_DIR.RemoveRange(delRoleDirsQuery);

                role.ROLE_NAME = roleDto.ROLE_NAME;
                role.DESCR = roleDto.DESCR;
                role.STATUS = roleDto.STATUS;
                role.IS_ADMIN = roleDto.IS_ADMIN;
                role.ROLE_TYPE = roleDto.ROLE_TYPE;

                foreach (string dirId in roleDto.DirIds)
                {
                    TB_ROLE_DIR rd = new TB_ROLE_DIR() { DIR_ID = dirId, ROLE_ID = role.ROLE_ID };
                    _db.TB_ROLE_DIR.Add(rd);
                }

                _db.TB_SYS_LOG.Add(log);

                _db.SaveChanges();

                return true;
            }
        }

        public bool DeleteRole(string id, TB_SYS_LOG log)
        {
            using (OraDb104 _db = new OraDb104())
            {
                var role = _db.TB_SYS_ROLE.Where(d => d.ROLE_ID == id).FirstOrDefault();

                if (role == null) throw new DomainException("角色不存在");

                role.STATUS = (byte)RoleStatus.DISABLE;

                _db.TB_SYS_LOG.Add(log);

                _db.SaveChanges();

                return true;
            }
        }


        public bool UpdateRoleDirectory(string roleId, string[] dirs, TB_SYS_LOG log)
        {
            using (OraDb104 _db = new OraDb104())
            {
                var role = _db.TB_SYS_ROLE.Where(r => r.ROLE_ID == roleId).FirstOrDefault();

                log.MEMO1 = "Old: ";

                //foreach (TB_SYS_DIR rd in role.TB_SYS_DIR)
                //{
                //    log.MEMO1 += rd.DIR_ID + ", ";
                //}

                //role.TB_SYS_DIR.Clear();

                //IList<TB_SYS_DIR> dirList = _db.TB_SYS_DIR.Where(r => dirs.Contains(r.DIR_ID)).ToList();

                //for (int i = 0; i < dirList.Count; i++)
                //{
                //    role.TB_SYS_DIR.Add(dirList[i]);
                //}

                _db.TB_SYS_LOG.Add(log);

                return true;
            }
        }

        public List<TB_ADMIN_USER> GetUsersByROLE_ID(string roleId)
        {
            List<TB_ADMIN_USER> TB_ADMIN_USERs = new List<TB_ADMIN_USER>();
            using (OraDb104 _db = new OraDb104())
            {
                var role = _db.TB_SYS_ROLE.Where(d => d.ROLE_ID == roleId).FirstOrDefault();

                //var userList = role.TB_ADMIN_USER.ToList();
                //var users = new List<TB_ADMIN_USER>();

                //foreach (var p in userList)
                //{
                //    users.Add(p);
                //}
            }

            return TB_ADMIN_USERs;
        }

        #endregion

        #region Dir

        public IEnumerable<TB_SYS_DIR> GetUsableDirs()
        {
            using (OraDb104 _db = new OraDb104())
            {
                var usableDirs = _db.TB_SYS_DIR.Where(d => d.ENABLED == 1 && d.DELETED == (byte)DeleteStatus.NO);

                List<TB_SYS_DIR> dirs = new List<TB_SYS_DIR>();

                foreach (TB_SYS_DIR d in usableDirs)
                {
                    dirs.Add(d);
                }

                return dirs;
            }
        }

        public TB_SYS_DIR GetDirByKey(string id)
        {
            using (OraDb104 _db = new OraDb104())
            {
                var dir = _db.TB_SYS_DIR.Where(d => d.DIR_ID == id).FirstOrDefault();

                return dir;
            }
        }

        public bool CreateDir(TB_SYS_DIR dirDto, TB_SYS_LOG log)
        {
            if (dirDto == null)
                throw new ArgumentNullException("dirDTO");

            using (OraDb104 _db = new OraDb104())
            {
                TB_SYS_DIR dir = dirDto;

                _db.TB_SYS_DIR.Add(dir);
                _db.TB_SYS_LOG.Add(log);

                _db.SaveChanges();

                return true;
            }
        }

        public bool UpdateDir(TB_SYS_DIR dirDto, TB_SYS_LOG log)
        {
            if (dirDto == null)
                throw new ArgumentNullException("dirDto");

            using (OraDb104 _db = new OraDb104())
            {
                var dir = _db.TB_SYS_DIR.Where(d => d.DIR_ID == dirDto.DIR_ID).FirstOrDefault();

                if (dir == null)
                    throw new Exception(dirDto.DIR_ID + " " + "目录不存在");

                dir.DIR_NAME = dirDto.DIR_NAME;
                dir.PARENT_ID = dirDto.PARENT_ID;
                dir.MEMO = dirDto.MEMO;
                dir.ENABLED = dir.ENABLED;
                dir.SORT_NO = dirDto.SORT_NO;
                dir.DIR_TYPE = dirDto.DIR_TYPE;
                dir.IS_GROUP = dirDto.IS_GROUP;
                dir.DIR_URL = dirDto.DIR_URL;
                dir.DIR_VIEW = dirDto.DIR_VIEW;
                dir.D_LEVEL = dirDto.D_LEVEL;

                _db.TB_SYS_LOG.Add(log);

                _db.SaveChanges();

                return true;
            }
        }

        public bool DeleteDir(string id, TB_SYS_LOG log)
        {
            using (OraDb104 _db = new OraDb104())
            {
                var dir = _db.TB_SYS_DIR.Where(d => d.DIR_ID == id).FirstOrDefault();

                if (dir == null) throw new DomainException("目录不存在");

                int childDirCount = _db.TB_SYS_DIR.Where(d => d.PARENT_ID == id).Count();

                if (childDirCount > 0) throw new DomainException("所删除目录存在子目录，请删除子目录后再删除此目录");

                dir.DELETED = (byte)DeleteStatus.YES;

                _db.TB_SYS_LOG.Add(log);

                _db.SaveChanges();

                return true;
            }
        }

        public TB_SYS_DIR GetDirByName(string name, string dirId)
        {
            using (OraDb104 _db = new OraDb104())
            {
                TB_SYS_DIR dir = null;

                if (dirId != "")
                    dir = _db.TB_SYS_DIR.Where(d => d.DIR_NAME == name && d.DIR_ID != dirId).FirstOrDefault();
                else
                    dir = _db.TB_SYS_DIR.Where(d => d.DIR_NAME == name).FirstOrDefault();

                return dir;
            }
        }

        #endregion
    }
}
