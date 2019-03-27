using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras.NoLinearStructures.Node
{
    public class Nodo<T>
    {
        public T Value { get; set; }
        public Nodo<T> Izdo;
        public Nodo<T> Dcho;
        public Nodo<T> Padre;

        public Nodo(T value, Nodo<T> padre)
        {
            this.Value = value;
            Izdo = null;
            Dcho = null;
            Padre = padre;
        }
    }
}
