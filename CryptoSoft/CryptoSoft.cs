using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Encryption
{
    public class CryptoSoft
    {
        public double EncryptFile(string filePath)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                File.Encrypt(filePath);
                stopwatch.Stop();
                return stopwatch.Elapsed.TotalMilliseconds;
            }
            catch
            {
                return -1;
            }
        }

        public int DecryptFile(string filePath)
        {
            try
            {
                File.Decrypt(filePath);
                return 0;
            }
            catch
            {
                return -1;
            }
        }
    }
}
