using System;

namespace IngSoft.Domain
{
    public class Backups : Entity
    {
        public string NombreArchivo { get; set; }
        public string RutaCompleta { get; set; }
        public DateTime FechaCreacion { get; set; }
        public long TamanoBytes { get; set; }
        public string UsuarioCreador { get; set; }
        public string TamanoFormateado
        {
            get
            {
                if (TamanoBytes < 1024)
                    return $"{TamanoBytes} B";
                else if (TamanoBytes < 1048576)
                    return $"{TamanoBytes / 1024.0:F2} KB";
                else if (TamanoBytes < 1073741824)
                    return $"{TamanoBytes / 1048576.0:F2} MB";
                else
                    return $"{TamanoBytes / 1073741824.0:F2} GB";
            }
        }
    }
}
