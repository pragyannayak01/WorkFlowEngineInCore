using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlowEngine.Repository.Account
{
    public class CommonFunction
    {
        #region Member Variables
        static string strDecodeMatch = "X+HIcwPJE/4=";
        //static readonly Byte[] hashedBytes;

        public static UTF8Encoding Encoder { get; set; } = new UTF8Encoding();
        public static MD5CryptoServiceProvider Md5Hasher { get; set; } = new MD5CryptoServiceProvider();
        #endregion
        public static string GetDecodedData(string strDataToDecode)
        {
            string[] decodeArray = strDataToDecode.Split(' ');
            strDataToDecode = null;
            for (int i = 0; i < decodeArray.Length; i++)
            {
                if (decodeArray[i].ToString() == strDecodeMatch)
                {
                    strDataToDecode += "&";
                    strDataToDecode += " ";
                }
                else
                {
                    strDataToDecode += decodeArray[i].ToString();
                    strDataToDecode += " ";
                }
            }
            return strDataToDecode;
        }
        //*******************Summery****************************
        //Function Name             : EncryptData(),DecryptData()
        //Purpose                   : Get the GlobalLinkId by logined user
        //InPut Parameters Name     :    toEncrypt,  cipherString
        //InPut Parameters DataType : string
        //OutPut Parameters Name    : None
        //OutPut Parameters DataType: None
        //Retun  Value              : encodeData,decodedData
        //Retun Datatype            : string
        //Created By                : Dilip Kumar Tripathy
        //Created Date              : 09-JAN-2012
        //*****************************************************
        #region Encrypt & Decrypt method

        public static string EncryptData(string toEncrypt)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            string key = "!@#$%^&*()_+~`";
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();
            AesCryptoServiceProvider oTripleDESCryptoServiceProvider = new AesCryptoServiceProvider();
            oTripleDESCryptoServiceProvider.Key = keyArray;
            oTripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
            oTripleDESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = oTripleDESCryptoServiceProvider.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            oTripleDESCryptoServiceProvider.Clear();
            string encodeData = Convert.ToBase64String(resultArray, 0, resultArray.Length);
            return encodeData;
        }
        public static string DecryptData(string cipherString)
        {
            if (cipherString != "")
            {
                cipherString = cipherString.Replace(" ", "+");
                byte[] keyArray;
                byte[] toEncryptArray = Convert.FromBase64String(cipherString);
                string key = "!@#$%^&*()_+~`";
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
                AesCryptoServiceProvider tdes = new AesCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                tdes.Clear();
                string decodedData = UTF8Encoding.UTF8.GetString(resultArray);
                return decodedData;
            }
            else
            {
                return "";
            }

        }
        public static string Encrypt(string clearText)
        {
            try
            {
                string EncryptionKey = "MAKV2SPBNI99212";
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return clearText;
        }
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        #endregion

    }

}
