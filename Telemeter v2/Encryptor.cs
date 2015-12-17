using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Telemeter_v2
{
    public class Encryptor
    {
        string key;

        public Encryptor()
        {
            key = "uqbB5unAJyAEJDSqdSqK";
        }

        public string encrypt(string input)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            Byte[] plainText = Encoding.Unicode.GetBytes(input);
            Byte[] salt = Encoding.ASCII.GetBytes(key.Length.ToString());
            PasswordDeriveBytes secretKey = new PasswordDeriveBytes(key, salt);
            ICryptoTransform encryptor = rijndaelCipher.CreateEncryptor(secretKey.GetBytes(16), secretKey.GetBytes(16));
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

            cryptoStream.Write(plainText, 0, plainText.Length);
            cryptoStream.FlushFinalBlock();

            Byte[] cipherBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            return Convert.ToBase64String(cipherBytes);
        }

        public string decrypt(string input)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            Byte[] encryptedData = Convert.FromBase64String(input);
            Byte[] salt = Encoding.ASCII.GetBytes(key.Length.ToString());
            PasswordDeriveBytes secretKey = new PasswordDeriveBytes(key, salt);
            ICryptoTransform decryptor = rijndaelCipher.CreateDecryptor(secretKey.GetBytes(16), secretKey.GetBytes(16));
            MemoryStream memoryStream = new MemoryStream(encryptedData);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            Byte[] plainText = new Byte[encryptedData.Length];

            int decryptedCount = cryptoStream.Read(plainText, 0, plainText.Length);
            memoryStream.Close();
            cryptoStream.Close();

            return Encoding.Unicode.GetString(plainText, 0, decryptedCount);
        }
    }
}
