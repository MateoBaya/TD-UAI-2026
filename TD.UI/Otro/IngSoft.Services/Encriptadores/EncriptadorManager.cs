using IngSoft.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngSoft.Services.Encriptadores
{
    public class EncriptadorExperto
    {
        private List<IEncriptador> mEncriptadores;
        public EncriptadorExperto()
        {
            mEncriptadores = new List<IEncriptador>();
            AddAllEncriptadores();
        }
        private void AddAllEncriptadores()
        {
            mEncriptadores.Add(new EncriptadorMD5());
        }
        public string EncriptadorOnlyHash(string textoPlano)
        {
            return EncriptadorSecuencial(textoPlano,mEncriptadores.Where(e => e is IEncriptadorHash).ToList());
        }
        public string EncriptadorSecuencial(string textoPlano)
        {
            return EncriptadorSecuencial(textoPlano,mEncriptadores);
        }
        internal string EncriptadorSecuencial(string textoPlano, List<IEncriptador> pEncriptadores)
        {
            string textoEncriptado = textoPlano;
            List<IEncriptador> encriptadores= pEncriptadores;


            if (encriptadores == null || encriptadores.Count == 0)
            {
                throw new ArgumentException("La lista de encriptadores no puede ser nula o vacía.");
            }
            if (string.IsNullOrEmpty(textoPlano))
            {
                throw new ArgumentException("El texto plano no puede ser nulo o vacío.");
            }
            foreach (IEncriptador encriptador in encriptadores)
            {
                textoEncriptado = encriptador.Encriptar(textoEncriptado);
            }
            return textoEncriptado;
        }
        public bool DesencriptadorSecuencial(string textoEncriptado, string textoPlano="")
        {
            return DesencriptadorSecuencial(textoEncriptado,mEncriptadores,textoPlano);
        }
        public bool DesencriptadorOnlyHash(string textoEncriptado, string textoPlano)
        {
            return DesencriptadorSecuencial(textoEncriptado,mEncriptadores.Where(e => e is IEncriptadorHash).ToList(),textoPlano);
        }
        internal bool DesencriptadorSecuencial(string textoEncriptado, List<IEncriptador> encriptadores, string textoPlano = "")
        {
            string mTextoPlano = textoPlano;
            string mTextoEncriptado = textoEncriptado;
            mTextoPlano = EncriptadorSecuencial(mTextoPlano, encriptadores);
            return mTextoPlano.Equals(mTextoEncriptado);
        }

        //private string EncriptarKey(string textoPlano, IEncriptadorKey encriptadorKey)
        //{
        //    return encriptadorKey.Encriptar(textoPlano);
        //}

        //private string DesencriptarKey(string textoEncriptado, IEncriptadorKey encriptadorKey)
        //{
        //    return encriptadorKey.Desencriptar(textoEncriptado);
        //}

        //private string EncriptarHash(string textoPlano, IEncriptadorHash encriptadorHash)
        //{
        //    return encriptadorHash.Encriptar(textoPlano);
        //}

        //private bool VerificarHash(string textoPlano,string textoHasheado, IEncriptadorHash encriptadorHash)
        //{
        //    return encriptadorHash.VerificarHash(textoPlano, textoHasheado);
        //}
    }
}
