using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WeCourseFaster.Service
{
    public class Encrypt
    {

        static string gas(string data, string key0, string iv0)
        {
            key0 = Regex.Replace(key0, @"/(^\s+)|(\s+$)/g", "");

            var encrypted = doEncrypt(data, key0, iv0);
            return encrypted;
        }

        public static string encryptAES(string data, string _p1)
        {

            var encrypted = gas(rds(64) + data, _p1, rds(16));
            return encrypted;
        }

        static string chars = "ABCDEFGHJKMNPQRSTWXYZabcdefhijkmnprstwxyz2345678";
        static int chars_len = chars.Length;

        static string rds(int len)
        {
            var retStr = "";
            for (int i = 0; i < len; i++)
            {
                retStr += chars[(int)Math.Floor(new Random().NextDouble() * chars_len)];
            }
            return retStr;
        }
        public static string doEncrypt(string toEncrypt, string key, string iv)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] ivArray = UTF8Encoding.UTF8.GetBytes(iv);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.IV = ivArray;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string GetMiddleStr(string oldStr, string preStr, string nextStr)
        {
            string tempStr = oldStr.Substring(oldStr.IndexOf(preStr) + preStr.Length);
            tempStr = tempStr.Substring(0, tempStr.IndexOf(nextStr));
            return tempStr;
        }
    }
}
