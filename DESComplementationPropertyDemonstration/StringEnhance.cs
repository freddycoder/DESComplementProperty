using System;
using System.Diagnostics;
using System.Text;

namespace DESComplementationPropertyDemonstration
{
    /// <summary>
    /// Class to encapsulate common function on base64 string and byte array.
    /// </summary>
    public class Base64StringEnhance
    {
        private readonly string _str;

        public Base64StringEnhance(string str)
        {
            PreCondition(() => Convert.FromBase64String(str), "str should be convertible to bytes from base64");

            _str = str;
        }

        private static void PreCondition(Action precondition, string message)
        {
            try
            {
                precondition();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Cannot validate precondition: {message}", e);
            }
        }

        public Base64StringEnhance(byte[] bytes)
        {
            _str = Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Base64 representation of the data
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _str;
        }

        /// <summary>
        /// Plain text representation of the data
        /// </summary>
        /// <returns></returns>
        public string ToPlainText()
        {
            return Encoding.UTF8.GetString(ToBytes());
        }

        /// <summary>
        /// Byte array of the data
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            return Convert.FromBase64String(_str);
        }

        /// <summary>
        /// Base64 string complement of the data
        /// </summary>
        /// <returns></returns>
        public string ToStringComplement()
        {
            return Convert.ToBase64String(ToBytesComplement());
        }

        /// <summary>
        /// The complement of the data
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytesComplement()
        {
            return ComplementOf(ToBytes());
        }

        /// <summary>
        /// Compute the complement of a byte array
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] ComplementOf(byte[] bytes)
        {
            var complement = new byte[bytes.Length];

            for (int i = 0; i < bytes.Length; i++)
            {
                complement[i] = (byte)~bytes[i];
            }

            return complement;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Base64StringEnhance bse)
            {
                return _str == bse._str;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _str.GetHashCode();
        }
    }
}
