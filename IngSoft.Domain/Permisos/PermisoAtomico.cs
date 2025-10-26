using IngSoft.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngSoft.Domain
{
    public class PermisoAtomico : PermisoComponent
    {
        public override ICompositable AddCompositable(ICompositable compositable)
        {
            if(base.AddCompositable(compositable) is null)
            {
                PermisoAgrupamiento permisoAdmin = new PermisoAgrupamiento();
                permisoAdmin.Nombre = "Admin";
                permisoAdmin.AddCompositable(this);
                permisoAdmin.AddCompositable(compositable);
                return permisoAdmin;
            }
            else
            {
                this.Parent.AddCompositable(compositable);
                return this.Parent;
            }
        }

    }
}
