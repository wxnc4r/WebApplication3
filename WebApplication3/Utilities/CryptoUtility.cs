using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;

namespace WebApplication3.Utilities
{
    public static class CryptoUtility
    {
        private static readonly byte[] Key = new byte[]
        {
            0xe7, 0xb8, 0xa5, 0x23, 0x5e, 0xee, 0x5e, 0xac,
            0x93, 0xb2, 0x36, 0xf8, 0x2a, 0x1b, 0x27, 0x0a,
            0x9e, 0xc2, 0x40, 0x9c, 0x20, 0x99, 0x5d, 0x47,
            0x64, 0x8d, 0x7e, 0xfd, 0xe5, 0x49, 0xb8, 0xee
        };

        private static readonly byte[] IV = new byte[]
        {
            0xe1, 0xee, 0x05, 0xdc, 0x0a, 0x30, 0x56, 0x90,
            0xbd, 0x69, 0x24, 0xef, 0xbd, 0x5c, 0xe5, 0xe9
        };

        public static RijndaelManaged GetRijndaelManaged()
        {
            RijndaelManaged rijndael = new RijndaelManaged
            {
                Key = Key,
                IV = IV,
                Padding = PaddingMode.PKCS7,
            };

            return rijndael;
        }
    }
}
