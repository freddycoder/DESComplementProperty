using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using static DESComplementationPropertyDemonstration.Base64StringEnhance;

namespace DESComplementationPropertyDemonstration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var k = new Base64StringEnhance(args[0]);
            var p = new Base64StringEnhance(args[1]);

            var iv = args.Length >= 3 ? Convert.FromBase64String(args[2]) : DES.Create()?.IV ?? throw new InvalidOperationException("DES.Create has return a null reference...");

            var file1 = args.Length >= 4 ? args[3] : "c.txt";
            var file2 = args.Length >= 5 ? args[4] : "cc.txt";

            EncryptTextToFile(p.ToString(), file1, k.ToBytes(), iv);
            EncryptTextToFile(p.ToStringComplement(), file2, k.ToBytesComplement(), iv);

            byte[] r = ComplementOf(File.ReadAllBytes(file1));
            Console.WriteLine(r == File.ReadAllBytes(file2));
        }

        // Next functions came from the source bellow, but has been modified to more fit the need of this program.
        // Source : https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.des.create?view=net-5.0

        public static void EncryptTextToFile(
            string data, string fileName, byte[] key, byte[] iv)
        {
            // Create or open the specified file.
            FileStream fStream = File.Open(fileName, FileMode.Create);

            // Create a new DES object.
            DES DESalg = DES.Create();

            // Create a CryptoStream using the FileStream
            // and the passed key and initialization vector (IV).
            CryptoStream cStream = new(fStream,
                DESalg.CreateEncryptor(key, iv),
                CryptoStreamMode.Write);

            // Create a StreamWriter using the CryptoStream.
            StreamWriter sWriter = new(cStream);

            // Write the data to the stream
            // to encrypt it.
            sWriter.Write(data);

            // Close the streams and
            // close the file.
            sWriter.Close();
            cStream.Close();
            fStream.Close();
        }

        public static string DecryptTextFromFile(
            string fileName, byte[] key, byte[] iv)
        {
            // Create or open the specified file.
            FileStream fStream = File.Open(fileName, FileMode.Open);

            // Create a new DES object.
            DES DESalg = DES.Create();

            // Create a CryptoStream using the FileStream
            // and the passed key and initialization vector (IV).
            CryptoStream cStream = new(fStream,
                DESalg.CreateDecryptor(key, iv),
                CryptoStreamMode.Read);

            // Create a StreamReader using the CryptoStream.
            StreamReader sReader = new(cStream);

            // Read the data from the stream
            // to decrypt it.
            string val = sReader.ReadToEnd();

            // Close the streams and
            // close the file.
            sReader.Close();
            cStream.Close();
            fStream.Close();

            // Return the string.
            return val;
        }

        public static byte[] EncryptTextToMemory(
            string data, byte[] key, byte[] iv)
        {
            // Create a MemoryStream.
            MemoryStream mStream = new();

            // Create a new DES object.
            DES DESalg = DES.Create();

            // Create a CryptoStream using the MemoryStream
            // and the passed key and initialization vector (IV).
            CryptoStream cStream = new(mStream,
                DESalg.CreateEncryptor(key, iv),
                CryptoStreamMode.Write);

            // Convert the passed string to a byte array.
            byte[] toEncrypt = Encoding.UTF8.GetBytes(data);

            // Write the byte array to the crypto stream and flush it.
            cStream.Write(toEncrypt, 0, toEncrypt.Length);
            cStream.FlushFinalBlock();

            // Get an array of bytes from the
            // MemoryStream that holds the
            // encrypted data.
            byte[] ret = mStream.ToArray();

            // Close the streams.
            cStream.Close();
            mStream.Close();

            // Return the encrypted buffer.
            return ret;
        }

        public static string DecryptTextFromMemory(byte[] data, byte[] key, byte[] iv)
        {
            // Create a new MemoryStream using the passed
            // array of encrypted data.
            MemoryStream msDecrypt = new(data);

            // Create a new DES object.
            DES DESalg = DES.Create();

            // Create a CryptoStream using the MemoryStream
            // and the passed key and initialization vector (IV).
            CryptoStream csDecrypt = new(msDecrypt,
                DESalg.CreateDecryptor(key, iv),
                CryptoStreamMode.Read);

            // Create buffer to hold the decrypted data.
            byte[] fromEncrypt = new byte[data.Length];

            // Read the decrypted data out of the crypto stream
            // and place it into the temporary buffer.
            csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

            //Convert the buffer into a string and return it.
            var str = Encoding.UTF8.GetString(fromEncrypt);

            return TrimEndingNullsCharacters(str);
        }

        private static string TrimEndingNullsCharacters(string str)
        {
            int nbCharNull = 0;

            for (int i = str.Length - 1; i >= 0; i--)
            {
                if (str[i] == '\0')
                {
                    nbCharNull++;
                }
            }

            return str[0..^nbCharNull];
        }
    }
}
