using System;
using System.Collections.Generic;
using IngSoft.Domain;

namespace IngSoft.Repository
{
    public interface IBackupRepository
    {
        void CrearBackup(string rutaDestino, string nombreArchivo);
        void RestaurarBackup(string rutaArchivo);
        List<Backups> ObtenerHistorialBackups();
        void RegistrarBackup(Backups backup);
        string ObtenerRutaBackupPorDefecto();
        void ActualizarTamanoBackup(Guid id, long tamanoBytes);
        void EliminarBackup(Guid id);
    }
}
