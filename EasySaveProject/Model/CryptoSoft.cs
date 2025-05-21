using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveProject.Model
{
    public enum CryptoResult
        {
            Success,
            FileNotFound,
            InvalidPath,
            Error
        }

    internal class CryptoSoft
    {
        public CryptoResult EncryptFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return CryptoResult.InvalidPath;

            if (!File.Exists(filePath))
                return CryptoResult.FileNotFound;

            try
            {
                File.Encrypt(filePath);
                return CryptoResult.Success;
            }
            catch
            {
                return CryptoResult.Error;
            }
        }

        public CryptoResult DecryptFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return CryptoResult.InvalidPath;

            if (!File.Exists(filePath))
                return CryptoResult.FileNotFound;

            try
            {
                File.Decrypt(filePath);
                return CryptoResult.Success;
            }
            catch
            {
                return CryptoResult.Error;
            }
        }
    }
}
