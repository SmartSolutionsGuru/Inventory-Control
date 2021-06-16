using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SmartSolutions.Util.EncryptionUtils
{
    public class Encryption
    {
        //const string PASSWORD = "P@SSW0RD";
        const string PASSWORD = "N2167gx76hC7oCQtd9sBOnDf5CM8g51b";

        /// <summary>
        /// Set the HashID Salt
        /// </summary>
        public static string HashIDSalt = "SI-App-003-SALT";


        /// <summary>
        /// Set the HashID minimum length
        /// </summary>
        public static int HashIDMinLength = 36;


        /// <summary>
        /// Set the HashID alphabet
        /// </summary>
        public static string HashIDAlphabet = "abcdefghijklmnopqrstuvwxyz1234567890";

        public static string Encrypt(EncryptionType encryption, string text, string key = "")
        {
            string result = string.Empty;
            try
            {
                Func<string, string, string> Method = null;
                switch (encryption)
                {
                    case EncryptionType.AES:
                        Method = encrypt_AES;
                        break;

                    case EncryptionType.AES_V2:
                        Method = encrypt_AES_v2;
                        break;

                    case EncryptionType.AES_V3:
                        Method = encrypt_AES_v3;
                        break;

                    case EncryptionType.HashId:
                        Method = Encode_Hash;
                        break;

                    case EncryptionType.BCrypt:
                        Method = Encode_BCrypt;
                        break;
                    case EncryptionType.MD5:
                        Method = Encode_MD5;
                        break;
                }

                if (!string.IsNullOrEmpty(text))
                    result = Method?.Invoke(text, key);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return result;
        }

        public static string Decrypt(EncryptionType encryption, string text, string key = "")
        {
            string result = null;
            try
            {
                Func<string, string, string> Method = null;
                switch (encryption)
                {
                    case EncryptionType.AES:
                        Method = decrypt_AES;
                        break;

                    case EncryptionType.AES_V2:
                        Method = decrypt_AES_v2;
                        break;

                    case EncryptionType.AES_V3:
                        Method = decrypt_AES_v3;
                        break;

                    case EncryptionType.HashId:
                        Method = Decode_Hash;
                        break;

                    case EncryptionType.BCrypt:
                        Method = Decode_BCrypt;
                        throw new NotSupportedException("BCrypt can't be Decrypted");

                    case EncryptionType.MD5:
                        throw new NotSupportedException("MD5 can't be Decrypted");

                }

                if (!string.IsNullOrEmpty(text))
                    result = Method?.Invoke(text, key);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return result;
        }

        #region MD5
        private static string Encode_MD5(string input, string salt = "")
        {
            string retVal = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(input))
                {

                    using (MD5 md5Hash = MD5.Create())
                    {
                        //var inputBytes = Encoding.UTF8.GetBytes(input);
                        //byte[] byteArray = md5Hash.ComputeHash(inputBytes);
                        byte[] byteArray = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                        StringBuilder sBuilder = new StringBuilder();
                        for (int i = 0; i < byteArray.Length; i++)
                        {
                            sBuilder.Append(byteArray[i].ToString("x2"));
                        }
                        retVal = sBuilder.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        #endregion

        #region AES ENCRYPTION
        private static string encrypt_AES(string text, string password = "")
        {
            string result = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(password)) password = PASSWORD;
                // Get the bytes of the string
                byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(text);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Hash the password with SHA256
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                byte[] encryptedBytes = null;

                // Set your salt here, change it to meet your flavor:
                // The salt bytes must be at least 8 bytes.
                byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

                using (MemoryStream ms = new MemoryStream())
                {
                    using (RijndaelManaged AES = new RijndaelManaged())
                    {
                        AES.KeySize = 256;
                        AES.BlockSize = 128;

                        var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                        AES.Key = key.GetBytes(AES.KeySize / 8);
                        AES.IV = key.GetBytes(AES.BlockSize / 8);

                        AES.Mode = CipherMode.CBC;

                        using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                            cs.Close();
                        }
                        encryptedBytes = ms.ToArray();
                    }
                }

                result = Convert.ToBase64String(encryptedBytes);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return result;
        }
        private static string decrypt_AES(string text, string password = "")
        {
            string result = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(password)) password = PASSWORD;
                // Get the bytes of the string
                byte[] bytesToBeDecrypted = Convert.FromBase64String(text);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);


                byte[] decryptedBytes = null;

                // Set your salt here, change it to meet your flavor:
                // The salt bytes must be at least 8 bytes.
                byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

                using (MemoryStream ms = new MemoryStream())
                {
                    using (RijndaelManaged AES = new RijndaelManaged())
                    {
                        AES.KeySize = 256;
                        AES.BlockSize = 128;

                        var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                        AES.Key = key.GetBytes(AES.KeySize / 8);
                        AES.IV = key.GetBytes(AES.BlockSize / 8);

                        AES.Mode = CipherMode.CBC;

                        using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                            cs.Close();
                        }
                        decryptedBytes = ms.ToArray();
                    }
                }

                result = Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return result;
        }


        #endregion

        #region AES ENCRYPTION V2
        private static string encrypt_AES_v2(string Input, string key = "")
        {
            if (string.IsNullOrEmpty(key)) key = PASSWORD;
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Encoding.UTF8.GetBytes(Input);
                    cs.Write(xXml, 0, xXml.Length);
                }

                xBuff = ms.ToArray();
            }

            string Output = Convert.ToBase64String(xBuff);
            return Output;
        }

        private static string decrypt_AES_v2(string Input, string key = "")
        {
            if (string.IsNullOrEmpty(key)) key = PASSWORD;
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var decrypt = aes.CreateDecryptor();
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Convert.FromBase64String(Input);
                    cs.Write(xXml, 0, xXml.Length);
                }

                xBuff = ms.ToArray();
            }

            string Output = Encoding.UTF8.GetString(xBuff);
            return Output;
        }
        #endregion

        #region AES ENCRYPTION V3
        private static string encrypt_AES_v3(string Input, string key = "")
        {
            //if (string.IsNullOrEmpty(key)) key = PASSWORD;
            if (string.IsNullOrEmpty(key)) key = "lkirwf897+22#bbtrm8814z5qq=12457";
            string iv = "741952hheeyy66#cs!9hjv887mx11234";
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.None;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);

            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Encoding.UTF8.GetBytes(Input);
                    cs.Write(xXml, 0, xXml.Length);
                }

                xBuff = ms.ToArray();
            }

            string Output = Convert.ToBase64String(xBuff);
            return Output;
        }

        private static string decrypt_AES_v3(string Input, string key = "")
        {
            //if (string.IsNullOrEmpty(key)) key = PASSWORD;
            if (string.IsNullOrEmpty(key)) key = "lkirwf897+22#bbtrm8814z5qq=12457";
            string iv = "741952hheeyy66#cs!9hjv887mx11234";
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.None;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);

            var decrypt = aes.CreateDecryptor();
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Convert.FromBase64String(Input);
                    cs.Write(xXml, 0, xXml.Length);
                }

                xBuff = ms.ToArray();
            }

            string Output = Encoding.UTF8.GetString(xBuff);
            Output = Output.Replace("\0", "");
            return Output;
        }
        #endregion

        #region HASH ID
        private static string Encode_Hash(string decodeID, string password)
        {
            string strEncodeID = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(password)) password = HashIDSalt;

                int id = 0;
                int.TryParse(decodeID, out id);
                if (id != 0)
                {
                    // HashIDs for encoding
                    var hashids = new HashidsNet.Hashids(password, HashIDMinLength, HashIDAlphabet);
                    strEncodeID = hashids.Encode(id);
                }
            }
            catch (Exception ex)
            {
                strEncodeID = null;
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return strEncodeID;
        }

        private static string Decode_Hash(string encodeID, string password)
        {
            string decodeID = "0";
            try
            {
                if (string.IsNullOrEmpty(password)) password = HashIDSalt;

                // HashIDs for decoding
                var hashids = new HashidsNet.Hashids(password, HashIDMinLength, HashIDAlphabet);
                var arrDecodedIDs = hashids.Decode(encodeID);
                if (arrDecodedIDs != null && arrDecodedIDs.Length > 0)
                    decodeID = arrDecodedIDs[0].ToString();
            }
            catch (Exception ex)
            {
                decodeID = "0";
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return decodeID;
        }
        #endregion

        #region BCrypt
        private static string Encode_BCrypt(string input, string salt = "")
        {
            string retVal = null;
            if (string.IsNullOrEmpty(salt))
                retVal = BCrypt.Net.BCrypt.HashPassword(input);
            else
                retVal = BCrypt.Net.BCrypt.HashPassword(input, salt);
            return retVal;
        }

        /// <summary>
        /// Verify the given username and Password
        /// by the user and get the roles of that person
        /// </summary>
        /// <param name="input"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        private static string Decode_BCrypt(string input, string salt = "")
        {
            bool retVal = false;
            if (string.IsNullOrEmpty(salt))
            {
                salt = HashIDAlphabet;
                retVal = BCrypt.Net.BCrypt.Verify(input, salt);
            }
            //else
            //    retVal = BCrypt.Net.BCrypt.Verify(input, salt);
            return retVal.ToString();
        }
        #endregion

        public static string Hash(string EncriptionCode)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(EncriptionCode);

            var sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(bytes);

            // return HexStringFromBytes(hashBytes);
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        public enum EncryptionType
        {
            AES,
            AES_V2,
            AES_V3,
            HashId,
            BCrypt,
            MD5
        }
    }
}
