using System.Security.Cryptography;
using System.Text;

namespace SPH.Utilities
{
    public class Serv_Encrip
    {
        public static string EncriptarClave(string clave)
        {
            if (string.IsNullOrEmpty(clave))
                throw new ArgumentNullException(nameof(clave));

            StringBuilder sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(clave));
                foreach (byte b in result)
                {
                    sb.Append(b.ToString("x2"));
                }
            }
            return sb.ToString();
        }

    }
}
