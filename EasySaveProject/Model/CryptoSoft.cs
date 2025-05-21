using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveProject.Model
{
    internal class CryptoSoft
    {
        public static void AddEncryption(string FileName)
        {

            File.Encrypt(FileName);
        }

        // Decrypt a file.
        public static void RemoveEncryption(string FileName)
        {
            File.Decrypt(FileName);
        }
    }
}
