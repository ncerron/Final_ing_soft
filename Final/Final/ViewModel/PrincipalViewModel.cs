using ConexionVM;
using Final.Commands;
using Final.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using System.Windows;

namespace Final.ViewModel
{
    public class PrincipalViewModel : INotifyPropertyChanged
    {
        int nuevoproducto = 0;
        int primerProductEliminado = 0;
        string nombre;
        string descripcion;
        decimal pv;
        decimal pc;
        string o;
        static string productoSeleccionado;

        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Producto> Productos { get; private set; }
        //public ObservableCollection<Producto> ProductosBuscar { get; private set; }
        public PrincipalViewModel()
        {
            IsEnabled = true;
            IsEnabledT = false;
            IsEnabledAgregar = true;
            IsEnabledSalir = true;
            IsEnabledEliminar = true;
            IsEnabledGuardar = false;
            IsEnabledBuscar = true;
            IsEnabledModificar = true;
            IsEnabledCancelar = false;

            //botones navegacion
            Adelante = new CommandBase(MoverAdelante, () => IsEnabled == true);
            Atras = new CommandBase(MoverAtras, () => IsEnabled == true);
            Ultimo = new CommandBase(IrUltimo, () => IsEnabled == true);
            Primero = new CommandBase(IrPrimero, () => IsEnabled == true);

            //botones de accion
            Salir = new CommandBase(SalirDelSistema, () => IsEnabledSalir == true);
            Eliminar = new CommandBase(EliminarProducto, () => IsEnabledEliminar == true);
            Agregar = new CommandBase(AgregarProducto, () => IsEnabledAgregar == true);
            Guardar = new CommandBase(GuardarProducto, () => IsEnabledGuardar == true);
            Buscar = new CommandBase(BuscarProducto, () => IsEnabledBuscar == true);
            Modificar = new CommandBase(ModificarProducto, () => IsEnabledModificar == true);
            Cancelar = new CommandBase(CancelarProducto, () => IsEnabledCancelar == true);

            Productos = new ObservableCollection<Producto>();

            //ventana BuscarProdutos
            BuscarP = new CommandBase(BuscarElProducto);
            CancelarBuscarp = new CommandBase(CancelarBuscarProducto);

            //en caso de que se haya seleccionado un producto en la ventana buscarProducto
            //se muestra el mismo en la ventana principal
            if (!string.IsNullOrEmpty(productoSeleccionado))
            {
                VerProductos();
                for (int i = 0; i < Productos.Count; i++)
                {
                    if (Productos[i].Nombre == productoSeleccionado)
                        Indice = i;
                }
            }
            else
            {
                VerProductos();
            }

        }

        int indice;
        public int Indice
        {
            get { return indice; }
            set
            {
                if (value < 0 || value > Productos.Count - 1)
                    return;

                if (value != indice)
                {
                    indice = value;
                    OnPropertyChange("Indice");
                    OnPropertyChange("ProductoSeleccionado");

                }
            }
        }

        public Producto ProductoSeleccionado
        {
            get { return Productos[Indice]; }
        }

        private void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
        }

        private void VerProductos()
        {
            Conexion cn = new Conexion();
            OleDbConnection conexion = cn.Obtener_conexion();

            //mostrar los registros con estado false, en estado true son los eliminados
            string strSQL = "SELECT * FROM productos " +
              " left join pais on productos.Idpais = pais.Idpais Where estado=false order by productos.nombre asc";

            OleDbCommand command = new OleDbCommand(strSQL, conexion);

            OleDbDataReader reader = command.ExecuteReader();
            Productos.Clear();
            while (reader.Read())
            {
                Producto pr = new Producto();
                pr.Id = (int)reader["Idproducto"];
                pr.Nombre = (string)reader["nombre"];
                pr.Descripcion = (string)reader["descripcion"];
                pr.PrecioCompra = (decimal)reader["precio_compra"];
                pr.PrecioVenta = (decimal)reader["precio_venta"];
                pr.Origen = (string)reader["pais"];
                pr.Estado = (bool)reader["estado"];

                Productos.Add(pr);
            }
            conexion.Close();
        }

        private ObservableCollection<string> v_Paises = new ObservableCollection<string>()//Combobox de Genero
        {
            "corea", "india","china","argentina"
        };
        public ObservableCollection<string> V_Paises
        {
            get { return v_Paises; }
            set
            {
                v_Paises = value;
                OnPropertyChange("Origen");
            }
        }
        private string selectedValue;
        public string SelectedValue
        {
            get { return selectedValue; }
            set
            {
                selectedValue = value;
                OnPropertyChange("Origen");
            }
        }

        #region Botones de navegacion
        public CommandBase Adelante
        {
            get; private set;
        }
        private Task MoverAdelante(object arg)
        {
            if (Indice != Productos.Count - 1)
                Indice++;
            else MessageBox.Show("No hay mas registros!");
            OnPropertyChange("ProductoSeleccionado");
            return null;
        }

        public CommandBase Atras
        {
            get; private set;
        }
        private Task MoverAtras(object arg)
        {
            if (Indice > 0)
                Indice--;
            else MessageBox.Show("Primer registro!");
            OnPropertyChange("ProductoSeleccionado");
            return null;
        }

        public CommandBase Ultimo
        {
            get; private set;
        }
        private Task IrUltimo(object arg)
        {
            if (Indice != Productos.Count - 1)
                Indice = Productos.Count - 1;
            else MessageBox.Show("Ultimo registro!");
            OnPropertyChange("ProductoSeleccionado");
            return null;
        }

        public CommandBase Primero
        {
            get; private set;
        }
        private Task IrPrimero(object arg)
        {
            if (Indice != 0)
                Indice = 0;
            else MessageBox.Show("Primer registro!");
            OnPropertyChange("ProductoSeleccionado");
            return null;
        }

        #endregion

        #region botones de accion

        public CommandBase Cancelar
        {
            get; private set;
        }

        private Task CancelarProducto(object arg)
        {
            IsEnabled = true;//botones de navegacion
            IsEnabledT = false;

            IsEnabledAgregar = true;
            IsEnabledModificar = true;
            IsEnabledEliminar = true;
            IsEnabledCancelar = false;
            IsEnabledGuardar = false;

            ProductoSeleccionado.Nombre = nombre;
            ProductoSeleccionado.Descripcion = descripcion;
            ProductoSeleccionado.PrecioCompra = pc;
            ProductoSeleccionado.PrecioVenta = pv;
            ProductoSeleccionado.Origen = o;
            return null;
        }

        public CommandBase Salir
        {
            get; private set;
        }
        private Task SalirDelSistema(object arg)
        {
            string messageBoxText = "  ¿Desea salir?";
            string caption = "Mensaje";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    App.Current.Shutdown();
                    break;
                case MessageBoxResult.No:
                    break;
            }
            return null;
        }

        public CommandBase Eliminar
        {
            get; private set;
        }
        private Task EliminarProducto(object arg)
        {
            OnPropertyChange("ProductoSeleccionado");
            if (Productos.Count == 1)
            {
                MessageBox.Show("No hay registros para eliminar", "Eliminar Registro");
            }
            else
            {
                if (MessageBox.Show("¿Seguro que desea eliminar el producto seleccionado?",
                        "Confirmar eliminacion de registro", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Conexion cn = new Conexion();
                    OleDbConnection conexion = cn.Obtener_conexion();
                    using (var cmd = new OleDbCommand("UPDATE productos " + "SET estado = true WHERE Idproducto = @Idproducto", conexion))
                    {
                        cmd.Parameters.AddWithValue("@Idproducto", ProductoSeleccionado.Id);
                        cmd.ExecuteNonQuery();
                    }
                    cn.Cerrar_conexion(conexion);

                    if (Indice != 0)
                    {
                        Indice--;
                    }
                    else//para primer registro
                    {
                        Indice++;
                        primerProductEliminado = 1;
                    }
                    VerProductos();
                }
            }
            return null;
        }

        public CommandBase Modificar
        {
            get; private set;
        }

        private Task ModificarProducto(object arg)
        {
            OnPropertyChange("ProductoSeleccionado");
            HabilitarTextBox();
            HabilitarBotonesNavegacion();
            HabilitarBotonesAccion();
            GuardaDatosDelRegistroSeleccionado();
            return null;
        }

        public CommandBase Agregar
        {
            get; private set;
        }
        private Task AgregarProducto(object arg)
        {
            OnPropertyChange("ProductoSeleccionado");
            if (Indice == Productos.Count - 1)
            {
                Indice--;
            }
            else
            {
                Indice++;
            }
            if (primerProductEliminado == 1)
                Indice = 0;

            GuardaDatosDelRegistroSeleccionado();

            nuevoproducto = -1;
            ProductoSeleccionado.Nombre = "";
            ProductoSeleccionado.Descripcion = "";
            ProductoSeleccionado.PrecioCompra = 0;
            ProductoSeleccionado.PrecioVenta = 0;
            ProductoSeleccionado.Estado = false;

            HabilitarBotonesAccion();
            HabilitarTextBox();
            HabilitarBotonesNavegacion();
            return null;
        }

        public CommandBase Guardar
        {
            get; private set;
        }

        private Task GuardarProducto(object arg)
        {
            OnPropertyChange("ProductoSeleccionado");
            if (nuevoproducto == -1)
            {
                if (ProductoSeleccionado.Nombre != "" && ProductoSeleccionado.Descripcion != "")
                {
                    if (ProductoSeleccionado.PrecioCompra != 0 && ProductoSeleccionado.PrecioVenta != 0)
                    {
                        Conexion cn = new Conexion();
                        OleDbConnection conexion = cn.Obtener_conexion();

                        //se busca id de pais del producto ingresado
                        int consulta = Id_pais(conexion);

                        //ingreso de los datos ingresados a la base de datos
                        string a = string.Format("INSERT INTO productos(nombre, descripcion, precio_compra, precio_venta, Idpais, estado)" +
                        " VALUES('{0}','{1}','{2}','{3}','{4}', false)", ProductoSeleccionado.Nombre, ProductoSeleccionado.Descripcion, Convert.ToDecimal(ProductoSeleccionado.PrecioCompra),
                        Convert.ToDecimal(ProductoSeleccionado.PrecioVenta), consulta);

                        cn.Sql_command(conexion, a);
                        cn.Cerrar_conexion(conexion);
                        MessageBox.Show("Registro Guardado!");
                        IsEnabledT = false;
                        HabilitarBotonesNavegacion();
                        IsEnabledAgregar = true;
                        IsEnabledModificar = true;
                        IsEnabledEliminar = true;
                        IsEnabledCancelar = false;
                        Indice--;
                        VerProductos();
                    }
                    else MessageBox.Show("Ingrese un precio");
                }
                else MessageBox.Show("Existen campos vacios");
            }
            else
            {
                if (ProductoSeleccionado.Nombre != "" && ProductoSeleccionado.Descripcion != "")
                {
                    if (ProductoSeleccionado.PrecioCompra != 0 && ProductoSeleccionado.PrecioVenta != 0)
                    {
                        Conexion cn = new Conexion();
                        OleDbConnection conexion = cn.Obtener_conexion();

                        //se busca id de pais del producto ingresado
                        int consulta = Id_pais(conexion);

                        using (var cmd = new OleDbCommand("UPDATE productos SET nombre = @nombre, descripcion = @descripcion, " +
                            "precio_compra= @precio_compra,precio_venta=@precio_venta, Idpais=@Idpais WHERE Idproducto = @Idproducto", conexion))
                        {
                            cmd.Parameters.AddWithValue("@nombre", ProductoSeleccionado.Nombre);
                            cmd.Parameters.AddWithValue("@descripcion", ProductoSeleccionado.Descripcion);
                            cmd.Parameters.AddWithValue("@precio_compra", Convert.ToDouble(ProductoSeleccionado.PrecioCompra));
                            cmd.Parameters.AddWithValue("@precio_venta", Convert.ToDouble(ProductoSeleccionado.PrecioVenta));
                            cmd.Parameters.AddWithValue("@Idpais", consulta);
                            cmd.Parameters.AddWithValue("@Idroducto", ProductoSeleccionado.Id);
                            cmd.ExecuteNonQuery();
                        }
                        cn.Cerrar_conexion(conexion);
                        MessageBox.Show("Registro Modificado!");
                        IsEnabledT = false;
                        HabilitarBotonesNavegacion();
                        IsEnabledAgregar = true;
                        IsEnabledModificar = true;
                        IsEnabledEliminar = true;
                        IsEnabledGuardar = false;
                        IsEnabledCancelar = false;
                        VerProductos();
                    }
                    else MessageBox.Show("Ingrese un precio");
                }
                else MessageBox.Show("Existen campos vacios");
            }
            return null;
        }


        public CommandBase Buscar
        {
            get; private set;
        }
        private Task BuscarProducto(object arg)
        {
            View.Window2 ventana_bucar = new View.Window2();
            Application.Current.Windows[0].Close();
            ventana_bucar.Show();

            //mostrar datos en datagrid
            Conexion cn = new Conexion();
            OleDbConnection conexion = cn.Obtener_conexion();

            OleDbDataAdapter da = new OleDbDataAdapter("SELECT productos.nombre as Nombre,  productos.descripcion as Descripcion, " +
                "productos.precio_compra as PrecioDeCompra, productos.precio_venta as PrecioDeVenta, pais.pais as Pais FROM productos" +
                " left join pais on productos.Idpais = pais.Idpais order by productos.nombre asc", conexion);

            DataSet ds = new DataSet();
            da.Fill(ds);
            cn.Cerrar_conexion(conexion);
            return null;
        }

        //busca idpais segun el txtbox origen del producto
        private int Id_pais(OleDbConnection conexion)
        {
            string comando = string.Format("SELECT distinct Idpais FROM pais WHERE pais = " +
                "(SELECT pais FROM pais where pais.pais LIKE '%{0}%')", ProductoSeleccionado.Origen);
            OleDbCommand command = new OleDbCommand(comando, conexion);
            int consulta = Convert.ToInt32(command.ExecuteScalar());
            return consulta;
        }

        #endregion


        #region ventana BuscarProductos

        //MessageBox.Show(productoSeleccionado.ToString());

        string buscoProducto;
        public string BuscoProducto
        {
            get { return buscoProducto; }
            set
            {
                if (buscoProducto != value)
                {
                    buscoProducto = value;
                    OnPropertyChange("BuscoProducto");
                }
            }
        }

        public CommandBase CancelarBuscarp
        {
            get; private set;
        }


        //boton salir de ventana buscar
        private Task CancelarBuscarProducto(object arg)
        {   //guardo el nombre del producto seleccionado
            if (SeleccionGrid != -1)
            {
                int indice = Convert.ToInt16(SeleccionGrid);
                productoSeleccionado = Productos[indice].Nombre.ToString();
            }

            View.Window1 principal = new View.Window1();
            Application.Current.Windows[0].Close();
            principal.Show();
            return null;
        }

        public CommandBase BuscarP
        {
            get; private set;
        }

        private Task BuscarElProducto(object arg)
        {
            if (BuscoProducto == null)
            {
                MessageBox.Show("Ingrese un producto a buscar");
            }
            else
            {
                Conexion cn = new Conexion();
                OleDbConnection conexion = cn.Obtener_conexion();

                //mostrar los registros con estado false, en estado true son los eliminados
                string strSQL = "SELECT * FROM productos " +
                  " left join pais on productos.Idpais = pais.Idpais Where productos.nombre like " + "'%" + BuscoProducto + "%'  order by productos.nombre asc";

                OleDbCommand command = new OleDbCommand(strSQL, conexion);

                OleDbDataReader reader = command.ExecuteReader();
                Productos.Clear();
                while (reader.Read())
                {
                    Producto pr = new Producto();
                    pr.Nombre = (string)reader["nombre"];
                    pr.Descripcion = (string)reader["descripcion"];
                    pr.PrecioCompra = (decimal)reader["precio_compra"];
                    pr.PrecioVenta = (decimal)reader["precio_venta"];
                    pr.Origen = (string)reader["pais"];

                    Productos.Add(pr);
                }
                conexion.Close();
            }
            return null;
        }

        private int seleccionGrid;
        public int SeleccionGrid
        {
            get { return seleccionGrid; }
            set
            {
                seleccionGrid = value;
                OnPropertyChange("SeleccionGrid");
            }
        }


        private Task ProductoSeleccionadoGrid()
        {
            Conexion cn = new Conexion();
            OleDbConnection conexion = cn.Obtener_conexion();

            //mostrar los registros con estado false, en estado true son los eliminados
            string strSQL = "SELECT * FROM productos " +
              " left join pais on productos.Idpais = pais.Idpais Where productos.nombre like " + "'%" + SeleccionGrid + "%' && estado=false order by productos.nombre asc";

            OleDbCommand command = new OleDbCommand(strSQL, conexion);
            OleDbDataReader reader = command.ExecuteReader();
            Productos.Clear();
            while (reader.Read())
            {
                Producto pr = new Producto();
                pr.Nombre = (string)reader["nombre"];
                pr.Descripcion = (string)reader["descripcion"];
                pr.PrecioCompra = (decimal)reader["precio_compra"];
                pr.PrecioVenta = (decimal)reader["precio_venta"];
                pr.Origen = (string)reader["pais"];

                Productos.Add(pr);
            }
            conexion.Close();

            return null;
        }

        #endregion


        //guarda los datos del registro, en caso de que el usario cancele la accion
        private void GuardaDatosDelRegistroSeleccionado()
        {
            nombre = ProductoSeleccionado.Nombre;
            descripcion = ProductoSeleccionado.Descripcion;
            pc = ProductoSeleccionado.PrecioCompra;
            pv = ProductoSeleccionado.PrecioVenta;
            o = ProductoSeleccionado.Origen;
        }

        #region habilitacion de Botones y TexBox

        // modificar propiedad enabled de TextBox
        private bool _isEnabledT;
        public bool IsEnabledT
        {
            get { return _isEnabledT; }
            set
            {
                if (_isEnabledT == value)
                {
                    return;
                }

                _isEnabledT = value;
                OnPropertyChange("IsEnabledT");
            }
        }

        private void HabilitarTextBox()
        {
            if (IsEnabledT)
            {
                IsEnabledT = false;
            }
            else
            {
                IsEnabledT = true;
            }
        }

        //botones de navegacion
        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled == value)
                {
                    return;
                }

                _isEnabled = value;
                OnPropertyChange("IsEnabled");
                OnPropertyChange("IsEnabledT");
            }
        }

        private void HabilitarBotonesNavegacion()
        {
            if (IsEnabledT)
                IsEnabled = false;
            else
                IsEnabled = true;
        }

        //botones de accion
        private void HabilitarBotonesAccion()
        {
            IsEnabledAgregar = false;
            IsEnabledCancelar = true;
            IsEnabledModificar = false;
            IsEnabledEliminar = false;
            IsEnabledGuardar = true;
        }

        private bool _isEnabledAgregar;
        public bool IsEnabledAgregar
        {
            get { return _isEnabledAgregar; }
            set
            {
                if (_isEnabledAgregar == value)
                {
                    return;
                }
                _isEnabledAgregar = value;
                OnPropertyChange("IsEnabledAgregar");
            }
        }

        private bool _isEnabledAdelante;
        public bool IsEnabledAdelante
        {
            get { return _isEnabledAdelante; }
            set
            {
                if (_isEnabledAdelante == value)
                {
                    return;
                }
                _isEnabledAdelante = value;
                OnPropertyChange("IsEnabledAdelante");
            }
        }

        private bool _isEnabledSalir;
        public bool IsEnabledSalir
        {
            get { return _isEnabledSalir; }
            set
            {
                if (_isEnabledSalir == value)
                {
                    return;
                }
                _isEnabledSalir = value;
                OnPropertyChange("IsEnabledSalir");
            }
        }

        private bool _isEnabledEliminar;
        public bool IsEnabledEliminar
        {
            get { return _isEnabledEliminar; }
            set
            {
                if (_isEnabledEliminar == value)
                {
                    return;
                }
                _isEnabledEliminar = value;
                OnPropertyChange("IsEnabledEliminar");
            }
        }

        private bool _isEnabledGuardar;
        public bool IsEnabledGuardar
        {
            get { return _isEnabledGuardar; }
            set
            {
                if (_isEnabledGuardar == value)
                {
                    return;
                }
                _isEnabledGuardar = value;
                OnPropertyChange("IsEnabledGuardar");
            }
        }

        private bool _isEnabledBuscar;
        public bool IsEnabledBuscar
        {
            get { return _isEnabledBuscar; }
            set
            {
                if (_isEnabledBuscar == value)
                {
                    return;
                }
                _isEnabledBuscar = value;
                OnPropertyChange("IsEnabledBuscar");
            }
        }

        private bool _isEnabledModificar;
        public bool IsEnabledModificar
        {
            get { return _isEnabledModificar; }
            set
            {
                if (_isEnabledModificar == value)
                {
                    return;
                }
                _isEnabledModificar = value;
                OnPropertyChange("IsEnabledModificar");
            }
        }

        private bool _isEnabledCancelar;
        public bool IsEnabledCancelar
        {
            get { return _isEnabledCancelar; }
            set
            {
                if (_isEnabledCancelar == value)
                {
                    return;
                }
                _isEnabledCancelar = value;
                OnPropertyChange("IsEnabledCancelar");
            }
        }

        #endregion
    }


}
