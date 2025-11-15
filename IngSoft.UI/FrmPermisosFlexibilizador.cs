using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using IngSoft.ApplicationServices;
using IngSoft.ApplicationServices.Factory;
using IngSoft.ApplicationServices.Implementation;
using IngSoft.Domain;
using IngSoft.Services;

namespace IngSoft.UI
{
    internal static class FrmPermisosFlexibilizador
    {
        private static PermisoComponent permisosSistema;
        private static PermisoComponent permisosUsuario;
        private static string usuarioSeleccionadoActual;

        // Crea y retorna un TreeView con dimensiones similares al DataGridView usado en FrmUsuarioFlexiblizador
        // Si se proporciona permisoRoot, carga el TreeView con los permisos usando FlexibilizadorFormularios.LoadTreeViewFromPermisos
        internal static TreeView PermisosTreeViewCreator(string name,Point position, Size size,PermisoComponent permisoRoot = null)
        {
            var parent = FrmPermiso.ActiveForm;

            var tree = FlexibilizadorFormularios.CreateTreeView(parent,name, position, size);

            // Cargar permisos si se proporcionó la raíz; si no, intentar obtener desde sesión
            var root = permisoRoot ?? SessionManager.GetPermisos() as PermisoComponent;
            if (root != null)
            {
                FlexibilizadorFormularios.LoadTreeViewFromPermisos(tree, root);
            }

            return tree;
        }

        // Crea árbol vacío para pendientes de asignar
        internal static TreeView PermisosPendientesAsignarTreeViewCreator(Point position, Size size)
        {
            var parent = FrmPermiso.ActiveForm;
            return FlexibilizadorFormularios.CreateTreeView(parent, "treeViewPermisosPendientesAsignar", position, size);
        }

        // Crea árbol vacío para pendientes de remover
        internal static TreeView PermisosPendientesRemoverTreeViewCreator(Point position, Size size)
        {
            var parent = FrmPermiso.ActiveForm;
            return FlexibilizadorFormularios.CreateTreeView(parent, "treeViewPermisosPendientesRemover", position, size);
        }

        internal static void ListaConTodosUsuarios(Point position, Size size, Point positionArbol, Size sizeArbol)
        {
            var parent = FrmPermiso.ActiveForm;
            UsuarioServices _usuarioServices = SingleInstancesManager.Instance.ObtenerInstancia<UsuarioServices>();
            if(_usuarioServices != null)
            {
                List<Usuario> userList = _usuarioServices.ObtenerUsuarios();
                List<string> Usernames = new List<string>();
                foreach (Usuario user in userList)
                {
                    Usernames.Add(user.UserName);
                }
                // Handler para el evento SelectedIndexChanged: encuentra el Usuario seleccionado y muestra sus permisos
                EventHandler onSelected = listUsuarioChangedSelectionHandler(positionArbol, sizeArbol);
                FlexibilizadorFormularios.CreateListBox(parent, "Usuarios", position, size, Usernames, onSelected);
            }
        }

        private static EventHandler listUsuarioChangedSelectionHandler(Point position, Size size)
        {
            EventHandler onSelected = (s, e) =>
            {
                if (!(s is ListBox lb)) return;
                if (lb.SelectedItem == null) return;
                string selectedUserName = lb.SelectedItem.ToString();
                usuarioSeleccionadoActual = selectedUserName;
                AsignarPermisosVerTodosDelUsuarioSeleccionado(position, size, selectedUserName);
            };
            return onSelected;
        }

        internal static void AsignarPermisosVerTodosDelSistema(Point position, Size size)
        {
            IPermisoServices _permisoServices = ServicesFactory.CreatePermisoServices();
            permisosSistema =_permisoServices.ObtenerTodosLosPermisos();
            PermisosTreeViewCreator("treeViewPermisos",position, size, permisosSistema);
        }
        internal static TreeView AsignarPermisosVerTodosDelUsuarioSeleccionado(Point position,Size size, string usuarioSeleccionado= null)
        {
            IPermisoServices _permisosServices = ServicesFactory.CreatePermisoServices();
            if(usuarioSeleccionado != null)
            {
                permisosUsuario = _permisosServices.ObtenerPermisosPorUsuario(usuarioSeleccionado);
            }
            return PermisosTreeViewCreator("treeViewPermisosUsuario",position, size, permisosUsuario);
        }

        // Crea botón para asignar permisos seleccionados del sistema a pendientes asignar
        internal static void BotonAsignarPermisosCreator(Point position, Size size)
        {
            var parent = FrmPermiso.ActiveForm;
            EventHandler onClick = (s, e) =>
            {
                var form = FrmPermiso.ActiveForm;
                var treeSistema = form.Controls.Find("treeViewPermisos", true).FirstOrDefault() as TreeView;
                var treePendientes = form.Controls.Find("treeViewPermisosPendientesAsignar", true).FirstOrDefault() as TreeView;
                if (treeSistema == null || treePendientes == null)
                {
                    MessageBox.Show("Asegúrese de que los árboles existen en el formulario.");
                    return;
                }

                var selected = treeSistema.SelectedNode;
                if (selected == null)
                {
                    MessageBox.Show("Seleccione un permiso del árbol de sistema para asignar.");
                    return;
                }
                else if (selected.Text == "Root")
                {
                    MessageBox.Show("No se puede asignar el nodo raíz.");
                    return;
                }
                if (selected.Tag is PermisoComponent)
                {
                    (selected.Tag as PermisoComponent).RemoveCompositable();
                }

                // Clonar nodo con tag y subnodos
                TreeNode CloneNode(TreeNode src)
                {
                    var n = new TreeNode(src.Text) { Name = src.Name, Tag = src.Tag };
                    foreach (TreeNode child in src.Nodes)
                    {
                        n.Nodes.Add(CloneNode(child));
                    }
                    return n;
                }

                var clone = CloneNode(selected);
                treePendientes.Nodes.Add(clone);
                selected.Remove();
            };
        

            FlexibilizadorFormularios.CreateButton(parent, "btnAsignarPermiso", position, size, "Agregar >>", onClick);
        }

        // Crea botón para quitar permisos seleccionados del usuario a pendientes remover
        internal static void BotonQuitarPermisosCreator(Point position, Size size)
        {
            var parent = FrmPermiso.ActiveForm;
            EventHandler onClick = (s, e) =>
            {
                var form = FrmPermiso.ActiveForm;
                var treeUsuario = form.Controls.Find("treeViewPermisosUsuario", true).FirstOrDefault() as TreeView;
                var treePendientesRem = form.Controls.Find("treeViewPermisosPendientesRemover", true).FirstOrDefault() as TreeView;
                if (treeUsuario == null || treePendientesRem == null)
                {
                    MessageBox.Show("Asegúrese de que los árboles existen en el formulario.");
                    return;
                }

                var selected = treeUsuario.SelectedNode;
                if (selected == null)
                {
                    MessageBox.Show("Seleccione un permiso del árbol del usuario para quitar.");
                    return;
                }

                TreeNode CloneNode(TreeNode src)
                {
                    var n = new TreeNode(src.Text) { Name = src.Name, Tag = src.Tag };
                    foreach (TreeNode child in src.Nodes)
                    {
                        n.Nodes.Add(CloneNode(child));
                    }
                    return n;
                }

                var clone = CloneNode(selected);
                treePendientesRem.Nodes.Add(clone);
                selected.Remove();
            };

            FlexibilizadorFormularios.CreateButton(parent, "btnQuitarPermiso", position, size, "<< Quitar", onClick);
        }

        // Crea botón para guardar todos los cambios pendientes: primero remover, luego asignar
        internal static void BotonGuardarCambiosPermisosCreator(Point position, Size size)
        {
            var parent = FrmPermiso.ActiveForm;
            EventHandler onClick = (s, e) =>
            {
                if (string.IsNullOrEmpty(usuarioSeleccionadoActual))
                {
                    MessageBox.Show("No hay usuario seleccionado.");
                    return;
                }

                var form = FrmPermiso.ActiveForm;
                var treeRem = form.Controls.Find("treeViewPermisosPendientesRemover", true).FirstOrDefault() as TreeView;
                var treeAssign = form.Controls.Find("treeViewPermisosPendientesAsignar", true).FirstOrDefault() as TreeView;

                if (treeRem == null || treeAssign == null)
                {
                    MessageBox.Show("Asegúrese de que los árboles de pendientes existen en el formulario.");
                    return;
                }

                IPermisoServices permisoServices = ServicesFactory.CreatePermisoServices();

                try
                {
                    // Procesar removals
                    foreach (TreeNode node in treeRem.Nodes.Cast<TreeNode>().ToList())
                    {
                        if (node.Tag is PermisoComponent permiso)
                        {
                            permisosUsuario.RemoveCompositable(permiso);
                            permisoServices.EliminarPermisoDeUsuario(permiso, usuarioSeleccionadoActual);
                        }
                    }

                    // Procesar assigns
                    foreach (TreeNode node in treeAssign.Nodes.Cast<TreeNode>().ToList())
                    {
                        if (node.Tag is PermisoComponent permiso)
                        {
                            // Validación: recorrer permisosUsuario usando Ejecutar y comprobar si ya existe un permiso con el mismo nombre
                            bool yaAsignado = false;
                            if (permisosUsuario != null)
                            {
                                permisosUsuario.Ejecutar(pc =>
                                {
                                    if(pc.Operacion(permiso.Nombre))
                                    {
                                        yaAsignado = true;
                                    }
                                });
                            }


                            if (yaAsignado)
                            {
                                // Si ya está asignado, saltar o lanzar excepción según la política (aquí lanzamos excepción para notificar)
                                throw new Exception($"El permiso '{permiso.Nombre}' ya está asignado al usuario '{usuarioSeleccionadoActual}'.");
                            }
                            permisosUsuario.AddCompositable(permiso);
                            permisoServices.AsignarPermisoEnUsuario(permiso, usuarioSeleccionadoActual);
                        }
                    }

                    // Limpiar pendientes
                    treeRem.Nodes.Clear();
                    treeAssign.Nodes.Clear();
                    
                    TreeView treeVerTodos = form.Controls.Find("treeViewPermisos", true).FirstOrDefault() as TreeView;
                    TreeView treeUsuarios = form.Controls.Find("treeViewPermisosUsuario", true).FirstOrDefault() as TreeView;

                    UsuarioServices _usuarioServices = SingleInstancesManager.Instance.ObtenerInstancia<UsuarioServices>();
                    List<Usuario> userList = _usuarioServices.ObtenerUsuarios();


                    // Actualizar vistas
                    AsignarPermisosVerTodosDelSistema(treeVerTodos.Location, treeVerTodos.Size);
                    AsignarPermisosVerTodosDelUsuarioSeleccionado(treeUsuarios.Location, treeUsuarios.Size,usuarioSeleccionadoActual);

                    MessageBox.Show("Cambios guardados correctamente.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar cambios: {ex.Message}");
                }
            };

            FlexibilizadorFormularios.CreateButton(parent, "btnGuardarCambiosPermisos", position, size, "Guardar Cambios", onClick);
        }

        // Crea la UI para agregar un permiso: two textboxes and a button
        internal static void PantallaAgregarPermisoCreator(Point positionNombre, Point positionPadre, Point positionButton)
        {
            var parent = FrmPermiso.ActiveForm;
            if (parent == null) return;

            // Crear TextBoxes usando FlexibilizadorFormularios
            var txtNombre = FlexibilizadorFormularios.CreateTextBox(parent, "NombrePermiso", positionNombre);
            var txtPadre = FlexibilizadorFormularios.CreateTextBox(parent, "PermisoPadre", positionPadre);

            // Handler del botón: crea PermisoAtomico con GUID y opcional Parent
            EventHandler onClick = (s, e) =>
            {
                string nombre = txtNombre.Text?.Trim();
                string padre = txtPadre.Text?.Trim();

                if (string.IsNullOrEmpty(nombre))
                {
                    MessageBox.Show("El nombre del permiso no puede estar vacío.");
                    return;
                }

                IPermisoServices permisoServices = ServicesFactory.CreatePermisoServices();

                // Construir permiso
                var nuevo = new PermisoAtomico { Nombre = nombre };
                // asignar Id random
                nuevo.Id = Guid.NewGuid();
                
                // Si hay padre, crear agrupamiento temporal y notificar asignación para que ParentName funcione
                if (!string.IsNullOrEmpty(padre))
                {
                    var tempParent = new PermisoAgrupamiento { Nombre = padre };
                    tempParent.Add(nuevo);
                }

                try
                {
                    permisosSistema.AddCompositable(nuevo);
                    permisoServices.GuardarPermiso(nuevo);
                    MessageBox.Show($"Permiso '{nombre}' guardado correctamente.");

                    // limpiar campos
                    txtNombre.Text = string.Empty;
                    txtPadre.Text = string.Empty;

                    // actualizar árbol de sistema si existe
                    var treeSistema = parent.Controls.Find("treeViewPermisos", true).FirstOrDefault() as TreeView;
                    if (treeSistema != null)
                    {
                        CrearArbolPermisosConSelector(treeSistema.Location, treeSistema.Size,Tree_AfterSelect_FillTextboxes);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar permiso: {ex.Message}");
                }
            };

            FlexibilizadorFormularios.CreateButton(parent, "btnCrearPermiso", positionButton, new Size(140, 30), "Crear Permiso", onClick);
        }

        // Crea la UI para modificar un permiso: nombre actual, nombre nuevo, padre (opcional) y botón
        internal static void PantallaModificarPermisoCreator(Point positionNombre, Point positionNombreNuevo, Point positionPadre, Point positionButton)
        {
            var parent = FrmPermiso.ActiveForm;
            if (parent == null) return;

            var txtNombre = FlexibilizadorFormularios.CreateTextBox(parent, "NombrePermiso", positionNombre);
            var txtNombreNuevo = FlexibilizadorFormularios.CreateTextBox(parent, "NombreNuevo", positionNombreNuevo);
            var txtPadre = FlexibilizadorFormularios.CreateTextBox(parent, "PermisoPadre", positionPadre);

            EventHandler onClick = (s, e) =>
            {
                string nombre = txtNombre.Text?.Trim();
                string nombreNuevo = txtNombreNuevo.Text?.Trim();
                string nombrePadre = txtPadre.Text?.Trim();
                // padre no es necesario para modificar nombre, pero se mantiene por compatibilidad

                if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(nombreNuevo))
                {
                    MessageBox.Show("Nombre actual y Nombre nuevo no pueden estar vacíos.");
                    return;
                }

                IPermisoServices permisoServices = ServicesFactory.CreatePermisoServices();
                try
                {
                    permisoServices.ModificarPermiso(nombre, nombreNuevo,nombrePadre);
                    MessageBox.Show($"Permiso '{nombre}' modificado a '{nombreNuevo}' correctamente.");

                    txtNombre.Text = string.Empty;
                    txtNombreNuevo.Text = string.Empty;
                    txtPadre.Text = string.Empty;

                    // actualizar árbol de sistema
                    var treeSistema = parent.Controls.Find("treeViewPermisos", true).FirstOrDefault() as TreeView;
                    if (treeSistema != null)
                    {
                        CrearArbolPermisosConSelector(treeSistema.Location, treeSistema.Size, Tree_AfterSelect_FillTextboxes_Modify);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al modificar permiso: {ex.Message}");
                }
            };

            FlexibilizadorFormularios.CreateButton(parent, "btnModificarPermiso", positionButton, new Size(140, 30), "Modificar Permiso", onClick);
        }

        // Crea la UI para eliminar un permiso: nombre y botón
        internal static void PantallaEliminarPermisoCreator(Point positionNombre, Point positionButton)
        {
            var parent = FrmPermiso.ActiveForm;
            if (parent == null) return;

            var txtNombre = FlexibilizadorFormularios.CreateTextBox(parent, "NombrePermiso", positionNombre);

            EventHandler onClick = (s, e) =>
            {
                string nombre = txtNombre.Text?.Trim();
                if (string.IsNullOrEmpty(nombre))
                {
                    MessageBox.Show("El nombre del permiso no puede estar vacío.");
                    return;
                }

                IPermisoServices permisoServices = ServicesFactory.CreatePermisoServices();
                try
                {
                    permisoServices.EliminarPermiso(nombre);
                    MessageBox.Show($"Permiso '{nombre}' eliminado correctamente.");
                    txtNombre.Text = string.Empty;

                    // actualizar árbol de sistema
                    var treeSistema = parent.Controls.Find("treeViewPermisos", true).FirstOrDefault() as TreeView;
                    if (treeSistema != null)
                    {
                        CrearArbolPermisosConSelector(treeSistema.Location, treeSistema.Size,Tree_AfterSelect_FillTextboxes_Delete);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar permiso: {ex.Message}");
                }
            };

            FlexibilizadorFormularios.CreateButton(parent, "btnEliminarPermiso", positionButton, new Size(140, 30), "Eliminar Permiso", onClick);
        }

        // Crea un árbol con todos los permisos y asigna un delegado que al seleccionar nodo llena los textboxes NombrePermiso y PermisoPadre
        internal static TreeView CrearArbolPermisosConSelector(Point position, Size size, TreeViewEventHandler evento)
        {
            var parent = FrmPermiso.ActiveForm;
            if (parent == null) return null;

            // Crear/actualizar árbol
            AsignarPermisosVerTodosDelSistema(position, size);
            var tree = parent.Controls.Find("treeViewPermisos", true).FirstOrDefault() as TreeView;
            if (tree == null) return null;

            tree.AfterSelect += evento;
            return tree;
        }

        internal static void Tree_AfterSelect_FillTextboxes_Modify(object sender, TreeViewEventArgs e)
        {
            var node = e.Node;
            if (node == null) return;
            var form = FrmPermiso.ActiveForm;
            if (form == null) return;
            // Buscar textboxes
            var txtNombre = form.Controls.Find("txtNombrePermiso", true).FirstOrDefault() as TextBox;
            var txtPadre = form.Controls.Find("txtPermisoPadre", true).FirstOrDefault() as TextBox;

            if(txtNombre != null && txtPadre!= null)
            {
                txtNombre.Text = node != null ? node.Text : string.Empty;
                txtPadre.Text = node != null ? node.Parent.Text : string.Empty;
            }
        }
        internal static void Tree_AfterSelect_FillTextboxes_Delete(object sender, TreeViewEventArgs e)
        {
            var node = e.Node;
            if (node == null) return;
            var form = FrmPermiso.ActiveForm;
            if (form == null) return;
            // Buscar textbox
            var txtNombre = form.Controls.Find("txtNombrePermiso", true).FirstOrDefault() as TextBox;
            if (txtNombre != null)
            {
                txtNombre.Text = node != null ? node.Text : string.Empty;
            }
        }
        internal static void Tree_AfterSelect_FillTextboxes(object sender, TreeViewEventArgs e)
        {
            var node = e.Node;
            if (node == null) return;
            var form = FrmPermiso.ActiveForm;
            if (form == null) return;

            // Buscar textboxes
            var txtPadre = form.Controls.Find("txtPermisoPadre", true).FirstOrDefault() as TextBox;


            // ParentName: si existe parent node, mostrar su texto
            if (txtPadre != null)
            {
                txtPadre.Text = node != null ? node.Text : string.Empty;
            }
        }

    }
}
