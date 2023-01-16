using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Password_Safe.Infrastructure
{
    /// <summary>
    /// EncryptionService Class.
    /// </summary>
    static class EncryptionService
    {
        /// <summary>
        /// Encrypts a string with minimal security.
        /// </summary>
        /// <param name="stringToEncrypt">String to be Encrypted.</param>
        /// <returns></returns>
        public static string BasicEncrypt(string stringToEncrypt)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(stringToEncrypt);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Decrypts a string that has been encrypted with BasicEncrypt().
        /// </summary>
        /// <param name="stringToDecrypt">String to be Decrypted.</param>
        /// <returns></returns>
        public static string BasicDecrypt(string stringToDecrypt)
        {
            byte[] bytes;
            string decryptedString;
            
            try
            {
                bytes = Convert.FromBase64String(stringToDecrypt);
                decryptedString = ASCIIEncoding.ASCII.GetString(bytes);
            }
            catch(Exception)
            {
                decryptedString = "";
            }

            return decryptedString;
        }

        /// <summary>
        /// Encrypts a string using a Key.
        /// </summary>
        /// <param name="stringToEncrypt">String to be Encrypted.</param>
        /// <param name="key">Key used for Encryption.</param>
        /// <returns></returns>
        public static string SecureEncrypt(string stringToEncrypt, string key)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(stringToEncrypt);
            
            using(Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(key, new byte[]{ 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                aes.Key = rfc.GetBytes(32);
                aes.IV = rfc.GetBytes(16);

                using(MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytes, 0, bytes.Length);
                        cs.Close();
                    }
                    stringToEncrypt = Convert.ToBase64String(ms.ToArray());
                }
            }

            return stringToEncrypt;
        }

        /// <summary>
        /// Decrypts a string using the Key with which it was initially Encrypted.
        /// </summary>
        /// <param name="stringToDecrypt">String to be Decrypted.</param>
        /// <param name="key">Key used for Decryption.</param>
        /// <returns></returns>
        public static string SecureDecrypt(string stringToDecrypt, string key)
        {
            stringToDecrypt = stringToDecrypt.Replace(" ", "+");
            byte[] bytes = Convert.FromBase64String(stringToDecrypt);

            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                aes.Key = rfc.GetBytes(32);
                aes.IV = rfc.GetBytes(16);

                using(MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms,aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytes, 0, bytes.Length);
                        cs.Close();
                    }
                    stringToDecrypt = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return stringToDecrypt;
        }
    }
}
