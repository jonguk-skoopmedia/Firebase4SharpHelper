using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace FirebaseSharp.Util
{
    public static class SecureStringExtension
    {
        public static SecureString ToSecureString(this string value)
        {
            SecureString secureString;
            unsafe
            {
                int length = value.Length;
                fixed (char* secureValue = value.ToCharArray())
                {
                    secureString = new SecureString(secureValue, length);
                }
            }

            return secureString;
        }

        public static string ToStringFromSecureString(this SecureString securedString)
        {
            return Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(securedString));
        }

        public static byte[] ToBytesFromSecureString(this SecureString secureString)
        {
            return Encoding.UTF8.GetBytes(secureString.ToStringFromSecureString());
        }
    }
}
