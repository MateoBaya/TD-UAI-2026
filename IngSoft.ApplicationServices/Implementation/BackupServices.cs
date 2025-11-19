using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using IngSoft.Domain;
using IngSoft.Domain.Enums;
using IngSoft.Repository;
using IngSoft.Repository.Factory;
using IngSoft.Services;

namespace IngSoft.ApplicationServices.Implementation
{
    public class BackupServices : IBackupServices
    {
        private readonly IBackupRepository _backupRepository;
        private readonly IBitacoraRepository _bitacoraRepository;

        public BackupServices(IBackupRepository backupRepository, IBitacoraRepository bitacoraRepository)
        {
            _backupRepository = backupRepository ?? FactoryRepository.CreateBackupRepository();
            _bitacoraRepository = bitacoraRepository ?? FactoryRepository.CreateBitacoraRepository();
        }
        public void CrearBackup()
        {
            try
            {
                var carpetaBackups = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "IngSoft",
                    "Backups"
                );

                if (!Directory.Exists(carpetaBackups))
                {
                    CrearCarpetaConPermisos(carpetaBackups);
                }

                var nombreArchivo = $"IngSoft_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                var rutaCompleta = Path.Combine(carpetaBackups, nombreArchivo);

                var backup = new Backups
                {
                    Id = Guid.NewGuid(),
                    NombreArchivo = nombreArchivo,
                    RutaCompleta = rutaCompleta,
                    FechaCreacion = DateTime.Now,
                    TamanoBytes = 0,
                    UsuarioCreador = SessionManager.GetUsuario()?.UserName ?? "Sistema"
                };

                _backupRepository.RegistrarBackup(backup);

                _backupRepository.CrearBackup(carpetaBackups, nombreArchivo);

                if (File.Exists(rutaCompleta))
                {
                    var fileInfo = new FileInfo(rutaCompleta);
                    backup.TamanoBytes = fileInfo.Length;

                    _backupRepository.ActualizarTamanoBackup(backup.Id, fileInfo.Length);

                    GuardarBitacora($"Backup creado exitosamente: {nombreArchivo}", TipoEvento.Message);
                }
                else
                {
                    _backupRepository.EliminarBackup(backup.Id);
                    throw new FileNotFoundException("El archivo de backup no se creó correctamente.");
                }
            }
            catch (Exception ex)
            {
                GuardarBitacora($"Error al crear backup: {ex.Message}", TipoEvento.Error);
                throw;
            }
        }

        private void CrearCarpetaConPermisos(string ruta)
        {
            try
            {
                var dirInfo = Directory.CreateDirectory(ruta);

                var dirSecurity = dirInfo.GetAccessControl();

                var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                var accessRule = new FileSystemAccessRule(
                    everyone,
                    FileSystemRights.FullControl,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow
                );

                dirSecurity.AddAccessRule(accessRule);
                dirInfo.SetAccessControl(dirSecurity);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.CreateDirectory(ruta);
            }
        }

        public void RestaurarBackup(string rutaArchivo)
        {
            try
            {
                if (!File.Exists(rutaArchivo))
                {
                    throw new FileNotFoundException($"El archivo de backup no existe: {rutaArchivo}");
                }

                _backupRepository.RestaurarBackup(rutaArchivo);
                GuardarBitacora($"Backup restaurado exitosamente: {Path.GetFileName(rutaArchivo)}", TipoEvento.Message);
            }
            catch (Exception ex)
            {
                GuardarBitacora($"Error al restaurar backup: {ex.Message}", TipoEvento.Error);
                throw;
            }
        }

        public List<Backups> ObtenerHistorialBackups()
        {
            try
            {
                var backupsEnBD = _backupRepository.ObtenerHistorialBackups();

                var backupsValidos = backupsEnBD
                    .Where(b => File.Exists(b.RutaCompleta))
                    .ToList();

                return backupsValidos;
            }
            catch (Exception ex)
            {
                GuardarBitacora($"Error al obtener historial de backups: {ex.Message}", TipoEvento.Error);
                throw;
            }
        }

        private void GuardarBitacora(string descripcion, TipoEvento tipoEvento)
        {
            try
            {
                var usuario = SessionManager.GetUsuario();
                if (usuario != null)
                {
                    var bitacora = new Bitacora
                    {
                        Id = Guid.NewGuid(),
                        Usuario = new Usuario { IdUsuario = usuario.IdUsuario },
                        Fecha = DateTime.Now,
                        Descripcion = descripcion,
                        Origen = "GestionBackup",
                        TipoEvento = tipoEvento
                    };
                    _bitacoraRepository.GuardarBitacora(bitacora);
                }
            }
            catch
            {
                // No fallar si no se puede guardar en bitácora
            }
        }
    }
}
