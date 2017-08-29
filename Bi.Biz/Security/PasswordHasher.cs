using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bi.Biz.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return MD5(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            return (hashedPassword == MD5(providedPassword)) ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }

        private const string key = "6\u0017^mM*\"p:ed\u001a\acO@jPH9^Px#";
        private const string iv = "De 63A&*";
        public string EncryptDES(string code)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(code);
            string result;
            using (TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider())
            {
                tripleDESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(key);
                tripleDESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(iv);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, tripleDESCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytes, 0, bytes.Length);
                        cryptoStream.FlushFinalBlock();
                    }
                    result = Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            return result;
        }
        public string DecryptDES(string password, string verify)
        {
            string result;
            if (verify != "ZZR")
            {
                result = "";
            }
            else
            {
                byte[] array = Convert.FromBase64String(password);
                string @string;
                using (TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider())
                {
                    tripleDESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(key);
                    tripleDESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(iv);
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, tripleDESCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(array, 0, array.Length);
                            cryptoStream.FlushFinalBlock();
                            cryptoStream.Close();
                        }
                        @string = Encoding.UTF8.GetString(memoryStream.ToArray());
                    }
                }
                result = @string;
            }
            return result;
        }
        public byte[] EncryptDESBytes(byte[] code)
        {
            byte[] result;
            using (TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider())
            {
                tripleDESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(key);
                tripleDESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(iv);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, tripleDESCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(code, 0, code.Length);
                        cryptoStream.FlushFinalBlock();
                    }
                    result = memoryStream.ToArray();
                }
            }
            return result;
        }
        public byte[] DecryptDESBytes(byte[] password, string verify)
        {
            byte[] result;
            if (verify != "ZZR")
            {
                result = null;
            }
            else
            {
                byte[] array;
                using (TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider())
                {
                    tripleDESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes("6\u0017^mM*\"p:ed\u001a\acO@jPH9^Px#");
                    tripleDESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes("De 63A&*");
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, tripleDESCryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(password, 0, password.Length);
                            cryptoStream.FlushFinalBlock();
                            cryptoStream.Close();
                        }
                        array = memoryStream.ToArray();
                    }
                }
                result = array;
            }
            return result;
        }
        public string MD5(string password)
        {
            string text = "";
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes);
            for (int i = 0; i < array.GetLength(0); i++)
            {
                string text2 = array[i].ToString("x");
                if (text2.Length == 1)
                {
                    text2 = "0" + text2;
                }
                text += text2;
            }
            return text;
        }
    }
}
