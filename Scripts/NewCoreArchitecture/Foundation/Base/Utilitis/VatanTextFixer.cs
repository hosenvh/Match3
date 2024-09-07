
using PandasCanPlay.BaseGame.Foundation;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PandasCanPlay.BaseGame.Foundation
{
    // NOTE: Don't take it as face value.
    public class VatanTextFixer
    {
        private readonly string fixAlgorithm;

        public VatanTextFixer(string fixAlgorithm)
        {
            this.fixAlgorithm = fixAlgorithm;
        }

        private static RijndaelManaged GetFixAlgorithm(String fixAlgorithmName)
        {
            var keyBytes = new byte[16];
            var algoBytes = Encoding.UTF8.GetBytes(fixAlgorithmName);
            Array.Copy(algoBytes, keyBytes, Math.Min(keyBytes.Length, algoBytes.Length));
            return new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }

        public string Fix(string text)
        {
            var bytes = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(FixByte(bytes, GetFixAlgorithm(fixAlgorithm)));
        }

        private static byte[] FixByte(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor()
                .TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }
    }
}
