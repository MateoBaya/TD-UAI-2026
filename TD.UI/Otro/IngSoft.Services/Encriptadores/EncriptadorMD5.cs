using IngSoft.Abstractions;
using System;
using System.Security.Cryptography;
using System.Text;

namespace IngSoft.Services.Encriptadores
{
    public  class EncriptadorMD5 : IEncriptadorHash
    {
        //TODO: Preguntar a los profes si deberiamos utilizar otro tipo de encriptacion por ejemplo SHA256 o Bcrypt.
        public string Encriptar(string textoPlano)
        {
            var md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(textoPlano));
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        public bool VerificarHash(string textoPlano, string hashGenerado)
        {
            string hashOfInput = Encriptar(textoPlano);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            
            if (0 == comparer.Compare(hashOfInput, hashGenerado))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
