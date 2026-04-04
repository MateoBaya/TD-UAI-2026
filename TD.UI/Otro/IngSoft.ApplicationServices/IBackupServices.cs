using System.Collections.Generic;
using IngSoft.Domain;

namespace IngSoft.ApplicationServices
{
    public interface IBackupServices
    {
        void CrearBackup();
        void RestaurarBackup(string rutaArchivo);
        List<Backups> ObtenerHistorialBackups();
    }
}
