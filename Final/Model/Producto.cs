using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.Model
{
    public class Producto : INotifyPropertyChanged
    {

       bool estado;
        public bool Estado
        {
            get { return estado; }
            set
            {
                if (estado != value)
                {
                    estado = value;
                    OnPropertyChange("Estado");
                }
            }
        }
        
        int id;
        public int Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChange("Id");
                }
            }
        }

        string nombre;
        public string Nombre
        {
            get { return nombre; }
            set
            {
                if (nombre != value)
                {
                    nombre = value;
                    OnPropertyChange("Nombre");
                }
            }
        }

        string descripcion;
        public string Descripcion
        {
            get { return descripcion; }
            set
            {
                if (descripcion != value)
                {
                    descripcion = value;
                    OnPropertyChange("Descripcion");
                }
            }
        }

       
       decimal precio_compra;
        public decimal PrecioCompra
        {
            get { return precio_compra; }
            set
            {
                if (precio_compra != value)
                {
                    precio_compra = value;
                    OnPropertyChange("PrecioCompra");
                }

            }
        }

        decimal precio_venta;
        public decimal PrecioVenta
        {
            get { return precio_venta; }
            set
            {
                if (precio_venta != value)
                {
                    precio_venta = value;
                    OnPropertyChange("PrecioVenta");
                }
            }
        }

        string origen;
        public string Origen
        {
            get { return origen; }
            set
            {
                if (origen != value)
                {
                    origen = value;
                    OnPropertyChange("Origen");
                }
            }
        }



    public event PropertyChangedEventHandler PropertyChanged;
       
        /// <summary>
        /// Metodo que lanza el evento PropertyChanged
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }



    }

}
