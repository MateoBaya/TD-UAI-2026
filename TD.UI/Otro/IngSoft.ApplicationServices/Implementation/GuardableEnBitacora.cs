using System;
using IngSoft.Domain;
using IngSoft.Domain.Enums;

namespace IngSoft.ApplicationServices.Implementation
{
    public abstract class GuardableEnBitacora
    {
        internal static Action<Usuario, string, string, TipoEvento> _registrarEnBitacora;

        public void SetRegistradoBitacora(Action<Usuario, string, string, TipoEvento> registrarEnBitacora)
        {
            _registrarEnBitacora = registrarEnBitacora;
        }

    }
}
