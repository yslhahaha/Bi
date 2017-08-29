using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bi.Config;
using System.Data;
using System.Reflection;
using System.Security.Cryptography;

namespace Bi.Utility
{
    public static class Pub
    {
        /// <summary>
        /// 根据指定类型，从数据库中获取数据，存放到List<T>集合中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_dtGet"></param>
        /// <returns></returns>
        public static List<T> ToObject<T>(DataTable _dtGet) where T : class, new()
        {
            try
            {
                List<T> _lstReturn = new List<T>();

                //获得属性集合
                T _tmpObj = new T();
                Type _type = _tmpObj.GetType();
                PropertyInfo[] _properties = _type.GetProperties();

                for (int i = 0; i < _dtGet.Rows.Count; i++)
                {
                    T _item = new T();
                    foreach (PropertyInfo _property in _properties)
                    {
                        if (!_dtGet.Columns.Contains(_property.Name))
                        { continue; }
                        object _value = _dtGet.Rows[i][_property.Name];
                        if (_value.GetType() == typeof(System.DBNull))
                        { continue; }

                        if (_value.GetType() == typeof(Int64))
                        {
                            _value = Convert.ToInt32(_value);
                        }

                        if (_value.GetType() == typeof(Double))
                        {
                            _value = Convert.ToDecimal(_value);
                        }

                        if (_value.GetType() == typeof(Int32) && (_property.PropertyType == typeof(Nullable<short>) || _property.PropertyType == typeof(short)))
                        {
                            _value = Convert.ToInt16(_value);
                        }

                        if (_value.GetType() == typeof(Int16) && (_property.PropertyType == typeof(Nullable<byte>) || _property.PropertyType == typeof(byte)))
                        {
                            _value = Convert.ToByte(_value);
                        }

                        _property.SetValue(_item, _value, null);
                    }
                    _lstReturn.Add(_item);
                }
                return _lstReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// 过滤字符串，去空格
        /// </summary>
        /// <param name="stringInput">输入的字符串</param>
        /// <returns>处理过的字符串</returns>
        public static string DataFilter(object stringInput)
        {
            if (stringInput == null)
                return "";

            string temp = string.Empty;
            temp = stringInput.ToString().Trim();
            return temp;
        }
        /// <summary>
        /// 过滤字符串，去空格
        /// </summary>
        /// <param name="strMayBeNull">输入的字符串</param>
        /// <returns>处理过的字符串</returns>
        public static string NullFilter(object strMayBeNull)
        {
            try
            {
                string tempvalue = Convert.ToString(strMayBeNull);
                tempvalue = tempvalue.Trim();
                return tempvalue;
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// 安全转换字符串为数字
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal ConvertToNum(object obj)
        {
            try
            {
                return Convert.ToDecimal(obj);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 将对象安全转换为长整型
        /// </summary>
        /// <param name="obj">输入的对象</param>
        /// <returns>长整型的数</returns>
        public static Int64 ConvertToInt64(object obj)
        {
            try
            {
                return Convert.ToInt64(obj);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 将对象安全转换为整型
        /// </summary>
        /// <param name="obj">输入的对象</param>
        /// <returns>整型的数</returns>
        public static Int32 ConvertToInt32(object obj)
        {
            try
            {
                return Convert.ToInt32(obj);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 将对象安全转换为短整型
        /// </summary>
        /// <param name="obj">输入的对象</param>
        /// <returns>短整型的数</returns>
        public static Int16 ConvertToInt16(object obj)
        {
            try
            {
                return Convert.ToInt16(obj);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 把对象转化为日期
        /// 注意如果转换失败返回0001-01-01这个日期，在.NET里是合法的，在数据库中不合法。
        /// </summary>
        /// <param name="obj">输入的对象</param>
        /// <returns>日期</returns>
        public static DateTime ConvertToDateTime(object obj)
        {
            try
            {
                return Convert.ToDateTime(obj);
            }
            catch
            {
                return Convert.ToDateTime(null);
            }
        }
        /// <summary>
        /// 将对象转化为短日期
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime ConvertToShortDateTime(object obj)
        {
            try
            {
                DateTime temp = Convert.ToDateTime(obj);
                return DateTime.Parse(temp.ToShortDateString());
            }
            catch
            {
                return Convert.ToDateTime(null);
            }
        }
        /// <summary>
        /// 把对象转化为日期
        /// 注意如果转换失败返回1900-01-01这个日期
        /// </summary>
        /// <param name="obj">输入的对象</param>
        /// <returns>日期</returns>
        public static DateTime ConvertToDateTime2(object obj)
        {
            try
            {
                return Convert.ToDateTime(obj);
            }
            catch
            {
                return Convert.ToDateTime(null);
            }
        }
        /// <summary>
        /// 把对象转化为数值
        /// </summary>
        /// <param name="obj">输入的对象</param>
        /// <returns>十进制数</returns>
        public static decimal ConvertToDecimal(object obj)
        {
            try
            {
                return Convert.ToDecimal(obj);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 上浮取整
        /// </summary>
        /// <param name="obj">输入的对象</param>
        /// <returns>十进制数</returns>
        public static decimal ConvertToCeiling(object obj)
        {
            try
            {
                return Math.Ceiling(Convert.ToDecimal(obj));

            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 将数字转换为字符串，设定小数位数
        /// </summary>
        /// <param name="number">数字</param>
        /// <param name="digit">小数位数</param>
        /// <returns>带小数部分的字符串</returns>
        public static string NumberToText(decimal number, int digit)
        {
            return number.ToString("f" + digit.ToString());
        }
        /// <summary>
        /// 把对象转化为Bool型
        /// </summary>
        /// <param name="obj">输入的对象</param>
        /// <returns>bool型值</returns>
        public static bool ConvertToBool(object obj)
        {
            try
            {
                return Convert.ToBoolean(obj);
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 转换为Byte[]
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ConvertToByteArray(object obj)
        {
            try
            {
                return (byte[])(obj);
            }
            catch
            {
                return null;
            }
        }
        public static byte ConvertToByte(object obj)
        {
            try
            {
                return Convert.ToByte(obj);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// Method to make sure that user's inputs are not malicious
        /// </summary>
        /// <param name="text">User's Input</param>
        /// <param name="maxLength">Maximum length of input</param>
        /// <returns>The cleaned up version of the input</returns>
        public static string InputText(string text, int maxLength)
        {
            text = text.Trim();
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            if (text.Length > maxLength)
                text = text.Substring(0, maxLength);
            text = Regex.Replace(text, "[\\s]{2,}", " ");	//two or more spaces
            text = Regex.Replace(text, "(<[b|B][r|R]/*>)+|(<[p|P](.|\\n)*?>)", "\n");	//<br>
            text = Regex.Replace(text, "(\\s*&[n|N][b|B][s|S][p|P];\\s*)+", " ");	//&nbsp;
            text = Regex.Replace(text, "<(.|\\n)*?>", string.Empty);	//any other tags
            text = text.Replace("'", "''");

            return text;
        }
        /// <summary>
        /// Method to make sure that user's inputs are not malicious
        /// </summary>
        /// <param name="text">User's Input</param>
        /// <returns></returns>
        public static string InputText(string text)
        {
            return InputText(text, int.MaxValue);
        }
        /// <summary>
        /// 空Guid
        /// </summary>
        /// <returns></returns>
        public static Guid GetNullGuid()
        {
            return new Guid("00000000-0000-0000-0000-000000000000");
        }
        /// <summary>
        /// 将对象转换为Guid类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Guid ConvertToGuid(object obj)
        {
            try
            {
                string temp = Convert.ToString(obj);
                return new Guid(temp);
            }
            catch
            {
                return GetNullGuid();
            }
        }
        /// <summary>
        /// 检查字符串是否是合法的数字
        /// </summary>
        /// <param name="NumString"></param>
        /// <returns></returns>
        public static bool CheckNumValid(string NumString)
        {
            try
            {
                if (!IsNumber(NumString))
                    return false;
                Convert.ToDouble(NumString);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 检查字符串是否是合法的数字
        /// </summary>
        /// <param name="NumString"></param>
        /// <returns></returns>
        public static bool IsNumber(string NumString)
        {
            return Regex.IsMatch(NumString, "^\\d+(?:\\.\\d+)?$");
        }
        /// <summary>
        /// 转换成正数
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal ConvertToPositive(object obj)
        {
            decimal v = 0;
            try
            {
                v = Convert.ToDecimal(obj);
                if (v > 0)
                    return v;
                else
                    return (-1) * v;
            }
            catch
            {
                return 0;
            }
        }

        public static DateTime GetLocalTime()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// 修改缓存依赖文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool ModifyDependencyFile(string fileName)
        {
            string fullName = System.Web.Hosting.HostingEnvironment.MapPath("~/CachingFiles/") + fileName;

            using (FileStream fs = new FileStream(fullName, FileMode.OpenOrCreate))
            {
                try
                {
                    fs.SetLength(0);

                    char[] charData = DateTime.Now.ToString().ToCharArray();
                    byte[] byteData = new byte[charData.Length];

                    Encoding.UTF8.GetEncoder().GetBytes(charData, 0, charData.Length, byteData, 0, true);

                    fs.Write(byteData, 0, byteData.Length);
                    fs.Close();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 获取图片文件流
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static byte[] GetBytesFromImage(string filename)
        {
            filename = System.Web.Hosting.HostingEnvironment.MapPath(filename);

            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            int length = (int)fs.Length;
            byte[] image = new byte[length];
            fs.Read(image, 0, length);
            fs.Close();
            return image;
        }
        /// <summary>
        /// 获取系统自定义配置
        /// </summary>
        /// <returns></returns>
        public static SysConfigSection GetSysConfig()
        {
            return (SysConfigSection)ConfigurationManager.GetSection("tiensConfiguration");
        }

        /// <summary>
        /// 检查列是否为隐藏列
        /// </summary>
        /// <param name="colName">列名</param>
        /// <param name="info">列属性</param>
        /// <returns></returns>
        public static bool IsHidenColumn(string colName)
        {
            string[] hiddenColumns = GetSysConfig().WebParam.HiddenColumns.Split('|');

            foreach (string hidCol in hiddenColumns)
            {
                if (colName.IndexOf(hidCol) >= 0) return true;
            }

            return false;

        }

        /// <summary>
        /// 转换字符数组，到数字数组
        /// </summary>
        /// <param name="stringArr"></param>
        /// <returns></returns>
        public static long[] ConverToInt64Arr(string[] stringArr)
        {
            long[] longArr = new long[stringArr.Length];

            for (int i = 0; i < stringArr.Length; i++)
            {
                longArr[i] = ConvertToInt64(stringArr[i]);
            }

            return longArr;
        }

        public static string SubString(string value, int len)
        {
            if (value.Length > len)
            {
                return value.Substring(0, len);
            }
            else return value;
        }

        public static string GetImageSrc(object imgBytes)
        {
            if (imgBytes != null && imgBytes.ToString() != "")
            {
                string imageBase64 = Convert.ToBase64String((byte[])imgBytes);
                if (!string.IsNullOrEmpty(imageBase64))
                    return string.Format("data:image/gif;base64,{0}", imageBase64);
                else
                    return "";
            }
            else
            {
                return "";
            }
        }

        ///// <summary>
        ///// 发送集团邮箱邮件
        ///// </summary>
        ///// <param name="title"></param>
        ///// <param name="msg"></param>
        ///// <returns></returns>
        //public static string SendMail(string title, string msg)
        //{
        //    if (GetSysConfig().MailParam.OnOff == "off") { return "ok"; }

        //    try
        //    {
        //        string fullName = System.Web.Hosting.HostingEnvironment.MapPath("~/CachingFiles/MailSend.txt");

        //        int mailCount = 0;

        //        string[] data = File.ReadAllLines(fullName);

        //        if (data.Count() > 1)
        //        {
        //            if (DateTime.Now > ConvertToDateTime(data[1]))
        //            {
        //                data[1] = DateTime.Now.AddDays(Convert.ToDouble((0 - Convert.ToInt16(DateTime.Now.DayOfWeek))) + 7).ToShortDateString();
        //                mailCount = 1;
        //            }
        //            else
        //            {
        //                mailCount = ConvertToInt32(data[0]) + 1;
        //            }

        //            if (mailCount < ConvertToInt32(Pub.GetSysConfig().MailParam.MailMaxSendNum) &&
        //                DateTime.Now < ConvertToDateTime(data[1]))
        //            {
        //                data[0] = mailCount.ToString();

        //                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
        //                client.Host = Pub.GetSysConfig().MailParam.Host;//使用SMTP服务器发送邮件 
        //                client.UseDefaultCredentials = true;
        //                client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
        //                //SMTP服务器需要用163邮箱的用户名和密码作认证
        //                client.Credentials = new System.Net.NetworkCredential(Pub.GetSysConfig().MailParam.User, Pub.GetSysConfig().MailParam.Password);
        //                System.Net.Mail.MailMessage Message = new System.Net.Mail.MailMessage();
        //                Message.From = new System.Net.Mail.MailAddress(Pub.GetSysConfig().MailParam.From);
        //                //这里需要注意，163似乎有规定发信人的邮箱地址必须是163的，而且发信人的邮箱用户名必须和上面SMTP服务器认证时的用户名相同                                                                
        //                //因为上面用的用户名abc作SMTP服务器认证，所以这里发信人的邮箱地址也应该写为abc@163.com
        //                string[] mails = Pub.GetSysConfig().MailParam.To.Split(',');
        //                foreach (var m in mails)
        //                {
        //                    Message.To.Add(m);//将邮件发送给
        //                }
        //                if (string.IsNullOrEmpty(title))
        //                {
        //                    Message.Subject = "紧急异常日志报告-物流系统-" + GetSysConfig().MailParam.LocationName;
        //                }
        //                else
        //                {
        //                    Message.Subject = title + "-紧急异常日志报告-物流系统-" + GetSysConfig().MailParam.LocationName;
        //                }

        //                Message.Body = msg;
        //                Message.SubjectEncoding = System.Text.Encoding.UTF8;
        //                Message.BodyEncoding = System.Text.Encoding.UTF8;
        //                Message.Priority = System.Net.Mail.MailPriority.High;
        //                Message.IsBodyHtml = true;
        //                client.Send(Message);

        //                StreamWriter sw = new StreamWriter(fullName, false);
        //                foreach (string s in data)
        //                    sw.WriteLine(s);

        //                sw.Close();//写入
        //            }
        //        }
        //        else
        //        {
        //            string[] t = { "0", DateTime.Now.ToShortDateString() };
        //            StreamWriter sw = new StreamWriter(fullName, false);
        //            foreach (string s in t)
        //                sw.WriteLine(s);

        //            sw.Close();//写入
        //        }

        //        return "ok";
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        ///// <summary>
        ///// 发送集团邮箱邮件(测试)
        ///// </summary>
        ///// <param name="title"></param>
        ///// <param name="msg"></param>
        ///// <returns></returns>
        //public static string SendMailTest(string title, string msg)
        //{
        //    if (GetSysConfig().MailParam.OnOff == "off") { return "ok"; }

        //    try
        //    {
        //        string fullName = System.Web.Hosting.HostingEnvironment.MapPath("~/CachingFiles/MailSend.txt");

        //        int mailCount = 0;

        //        string[] data = File.ReadAllLines(fullName);

        //        if (data.Count() > 1)
        //        {
        //            if (DateTime.Now > ConvertToDateTime(data[1]))
        //            {
        //                data[1] = DateTime.Now.AddDays(Convert.ToDouble((0 - Convert.ToInt16(DateTime.Now.DayOfWeek))) + 7).ToShortDateString();
        //                mailCount = 1;
        //            }
        //            else
        //            {
        //                mailCount = ConvertToInt32(data[0]) + 1;
        //            }

        //            if (mailCount < ConvertToInt32(Pub.GetSysConfig().MailParam.MailMaxSendNum) &&
        //                DateTime.Now < ConvertToDateTime(data[1]))
        //            {
        //                data[0] = mailCount.ToString();

        //                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
        //                client.Host = Pub.GetSysConfig().MailParam.Host;//使用SMTP服务器发送邮件 
        //                client.UseDefaultCredentials = true;
        //                client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
        //                //SMTP服务器需要用163邮箱的用户名和密码作认证
        //                client.Credentials = new System.Net.NetworkCredential(Pub.GetSysConfig().MailParam.User, Pub.GetSysConfig().MailParam.Password);
        //                System.Net.Mail.MailMessage Message = new System.Net.Mail.MailMessage();
        //                Message.From = new System.Net.Mail.MailAddress(Pub.GetSysConfig().MailParam.From);
        //                //这里需要注意，163似乎有规定发信人的邮箱地址必须是163的，而且发信人的邮箱用户名必须和上面SMTP服务器认证时的用户名相同                                                                
        //                //因为上面用的用户名abc作SMTP服务器认证，所以这里发信人的邮箱地址也应该写为abc@163.com
        //                string[] mails = Pub.GetSysConfig().MailParam.To.Split(',');

        //                foreach (var m in mails)
        //                {
        //                    Message.To.Add(m);//将邮件发送给
        //                }

        //                if (string.IsNullOrEmpty(title))
        //                {
        //                    Message.Subject = "测试-紧急异常日志报告-物流系统" + GetSysConfig().MailParam.LocationName;
        //                }
        //                else
        //                {
        //                    Message.Subject = title + "测试-紧急异常日志报告-物流系统" + GetSysConfig().MailParam.LocationName;
        //                }

        //                Message.Body = msg;
        //                Message.SubjectEncoding = System.Text.Encoding.UTF8;
        //                Message.BodyEncoding = System.Text.Encoding.UTF8;
        //                Message.Priority = System.Net.Mail.MailPriority.High;
        //                Message.IsBodyHtml = true;
        //                client.Send(Message);

        //                StreamWriter sw = new StreamWriter(fullName, false);
        //                foreach (string s in data)
        //                    sw.WriteLine(s);

        //                sw.Close();//写入
        //            }
        //        }
        //        else
        //        {
        //            string[] t = { "0", DateTime.Now.ToShortDateString() };
        //            StreamWriter sw = new StreamWriter(fullName, false);
        //            foreach (string s in t)
        //                sw.WriteLine(s);

        //            sw.Close();//写入
        //        }

        //        return "ok";
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        /// <summary>
        /// 读取一个文件，返回字符串，每行间用\r\n分隔开，
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetWordFromTxt(string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader srReader = new StreamReader(fileStream);

            string words = srReader.ReadToEnd();

            srReader.Close();
            fileStream.Close();

            return words;
        }
        /// <summary>
        /// 读取一个文件，返回字符串，每行间用<br>分隔开，
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetReverseOrderWordFromTxt(string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
            StreamReader srReader = new StreamReader(fileStream);

            string[] words = srReader.ReadToEnd().Replace("\r\n", "&").Split('&');

            string reverseOrderWords = "";

            for (int i = words.Length - 2; i >= 0; i--)
            {
                reverseOrderWords += words[i] + "\r\n";
            }

            srReader.Close();
            fileStream.Close();
            if (reverseOrderWords.Length > 4)
                return reverseOrderWords.Substring(0, reverseOrderWords.Length - 4);
            else
                return "";
        }

        public static string GetReverseOrderHtmlWordFromTxt(string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
            StreamReader srReader = new StreamReader(fileStream);

            string[] words = srReader.ReadToEnd().Replace("\r\n", "&").Split('&');

            string reverseOrderWords = "";

            for (int i = words.Length - 2; i >= 0; i--)
            {
                reverseOrderWords += words[i] + "<br />";
            }

            srReader.Close();
            fileStream.Close();
            if (reverseOrderWords.Length > 4)
                return reverseOrderWords.Substring(0, reverseOrderWords.Length - 4);
            else
                return "";
        }

        public static string MD5(string str)
        {
            byte[] b = Encoding.UTF8.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            return ret;
        }

        public static string Hash(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }
    }
}
