using System.Diagnostics;
using System.IO;

namespace Encryption
{
    public class CryptoSoft
    {
        private static readonly Mutex CryptoMutex = new Mutex(false, "Global\\CryptoSoftEncryptionMutex");

        public double EncryptFile(string filePath)
        {
            bool hasHandle = false;
            try
            {
                hasHandle = CryptoMutex.WaitOne(TimeSpan.FromSeconds(5), false);

                if (!hasHandle)
                    throw new TimeoutException("CryptoSoft is already running.");

                var stopwatch = Stopwatch.StartNew();
                File.Encrypt(filePath);
                stopwatch.Stop();

                return stopwatch.Elapsed.TotalMilliseconds;
            }
            catch
            {
                return -1;
            }
            finally
            {
                if (hasHandle)
                    CryptoMutex.ReleaseMutex();
            }
        }

        public int DecryptFile(string filePath)
        {
            bool hasHandle = false;
            try
            {
                hasHandle = CryptoMutex.WaitOne(TimeSpan.FromSeconds(5), false);

                if (!hasHandle)
                    throw new TimeoutException("CryptoSoft is already running.");

                File.Decrypt(filePath);
                return 0;
            }
            catch
            {
                return -1;
            }
            finally
            {
                if (hasHandle)
                    CryptoMutex.ReleaseMutex();
            }
        }
    }
}