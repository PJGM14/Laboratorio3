using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Web;
using Estructuras.NoLinearStructures.Interface;
using Estructuras.NoLinearStructures.Node;
using Estructuras.NoLinearStructures.Operaciones;

namespace Estructuras.NoLinearStructures.Trees
{
    public class ArbolB<T> : ArbolBusqueda<string, T> where T : ITextoTamañoFijo
    {
        //TAMAÑO TOTAL DEL ENCABEZADO
        private const int _tamañoEncabezadoBinario = 5 * OperacionesTexto.EnteroYEnterBinarioTamaño;

        //ATRIBUTOS EN EL ENCABEZADO DEL ARCHIVO
        private int _raiz;
        private int _ultimaPosicionLibre;

        //OTRAS VARIABLES PARA ACCESO AL ARCHIVO
        private FileStream _archivo = null;
        private string _archivoNombre = "";
        private IFabricaTamañoTextoFijo<T> _fabrica = null;

        public int Orden { get; private set; }
        public int Altura { get; private set; }

        public List<string> datos = new List<string>(); 
        

        public ArbolB(int orden, string nombreArchivo, IFabricaTamañoTextoFijo<T> fabrica)
        {
            //SE GUARDAN LOS PARAMETROS RECIBIDOS
            _archivoNombre = nombreArchivo;
            _fabrica = fabrica;

            //SE ABRE LA CONEXION AL ARCHIVO
            _archivo = new FileStream(_archivoNombre, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

            //SE OBTIENEN LOS VALORES DEL ENCABEZADO DEL ARCHIVO
            _raiz = OperacionesTexto.LeerEntero(_archivo, 0);
            _ultimaPosicionLibre = OperacionesTexto.LeerEntero(_archivo, 1);
            Tamaño = OperacionesTexto.LeerEntero(_archivo, 2);
            Orden = OperacionesTexto.LeerEntero(_archivo, 3);
            Altura = OperacionesTexto.LeerEntero(_archivo, 4);

            //SE CORRIGEN LOS VALORES DEL ENCABEZADO CUANDO EL ARCHIVO NO EXISTE PREVIAMENTE
            if (_ultimaPosicionLibre == OperacionesTexto.ApuntadorVacio)
            {
                _ultimaPosicionLibre = 0;
            }
            if (Tamaño == OperacionesTexto.ApuntadorVacio)
            {
                Tamaño = 0;
            }
            if (Orden == OperacionesTexto.ApuntadorVacio)
            {
                Orden = orden;
            }
            if (Altura == OperacionesTexto.ApuntadorVacio)
            {
                Altura = 1;
            }
            if (_raiz == OperacionesTexto.ApuntadorVacio)
            {
                //SE CREA LA CABEZA DEL ARBOL VACIA PARA PREVENIR ERRORES EN UN FUTURO
                NodoB<T> nodoCabeza = new Node.NodoB<T>(Orden, _ultimaPosicionLibre, OperacionesTexto.ApuntadorVacio, _fabrica);
                _ultimaPosicionLibre++;
                _raiz = nodoCabeza.Posicion;
                nodoCabeza.GuardarNodoEnDisco(_archivo, _tamañoEncabezadoBinario);
            }
            // Si el archivo existe solamente se actualizan los encabezados, sino 
            // se crea y luego se almacenan los valores iniciales 
            GuardarEncabezado();
        }

        private void GuardarEncabezado()
        {
            //SE ESCRIBE AL DISCO
            OperacionesTexto.EscribirEntero(_archivo, 0, _raiz);
            OperacionesTexto.EscribirEntero(_archivo, 1, _ultimaPosicionLibre);
            OperacionesTexto.EscribirEntero(_archivo, 2, Tamaño);
            OperacionesTexto.EscribirEntero(_archivo, 3, Orden);
            OperacionesTexto.EscribirEntero(_archivo, 4, Altura);
            _archivo.Flush();
        }

        private void AgregarRecursivo(int posicionNodoActual, string llave, T dato)
        {
            var nodoActual = NodoB<T>.LeerNodoDesdeDisco(_archivo, _tamañoEncabezadoBinario, Orden, posicionNodoActual, _fabrica);

            if (nodoActual.PosicionExactaEnNodo(llave) != -1)
            {
                throw new InvalidOperationException("La llave indicada ya está contenida en el árbol.");
            }
            if (nodoActual.EsHoja)
            {
                //SE DEBE INSERTAR EN ESTE NODO, POR LO QUE SE HACE LA LLAMADA AL MÉTODO ENCARGADO DE INSERTAR Y AJUSTAR EL ÁRBOL SI ES NECESARIO
                Subir(nodoActual, llave, dato, OperacionesTexto.ApuntadorVacio);
                GuardarEncabezado();
            }
            else
            {
                //SE HACE UNA LLAMADA RECURSIVA, BAJANDO EN EL SUBARBOL CORRESPONDIENTE SEGÚN LA POSICIÓN APROXIMADA DE LA LLAVE
                AgregarRecursivo(nodoActual.Hijos[nodoActual.PosicionAproximadaEnNodo(llave)], llave, dato);
            }
        }

        private void Subir(NodoB<T> nodoActual, string llave, T dato, int hijoDerecho)
        {
            //SI EL NODO ESTA LLENO, SE AGREGA LA INFO AL NODO Y TERMINA EL METODO 
            if (!nodoActual.Lleno)
            {
                nodoActual.AgregarDato(llave, dato, hijoDerecho);
                nodoActual.GuardarNodoEnDisco(_archivo, _tamañoEncabezadoBinario);
                return;
            }

            //SE CREA UN NUEVO NODO HERMANO
            var nuevoHermano = new NodoB<T>(Orden, _ultimaPosicionLibre, nodoActual.Padre, _fabrica);
            _ultimaPosicionLibre++;

            //DATOS A SUBIR AL PADRE LUEGO DE LA SEPARACIÓN
            string llavePorSubir = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
            T datoPorSubir = _fabrica.FabricarNulo();

            //SE LLAMA AL METODO QUE HACE LA SEPARACION 
            nodoActual.SepararNodo(llave, dato, hijoDerecho, nuevoHermano, ref llavePorSubir, ref datoPorSubir);

            //SE ACTUALIZA EL APUNTADOR A TODOS LOS HIJOS
            NodoB<T> nodoHijo = null;
            for (int i = 0; i < nuevoHermano.Hijos.Count; i++)
            {
                if (nuevoHermano.Hijos[i] != OperacionesTexto.ApuntadorVacio)
                {
                    //SE CARGA EL HIJO PARA MODIFICAR SU APUNTADOR AL PADRE
                    nodoHijo = NodoB<T>.LeerNodoDesdeDisco(_archivo, _tamañoEncabezadoBinario, Orden, nuevoHermano.Hijos[i], _fabrica);
                    nodoHijo.Padre = nuevoHermano.Posicion;
                    nodoHijo.GuardarNodoEnDisco(_archivo, _tamañoEncabezadoBinario);
                }
                else
                {
                    break;
                }
            }

            //SE EVALUA EL CASO DEL PADRE 
            if (nodoActual.Padre == OperacionesTexto.ApuntadorVacio) //SI ES LA RAIZ
            {
                //SE CREA UN NUEVO NODO RAIZ
                Node.NodoB<T> nuevaRaiz = new Node.NodoB<T>(Orden, _ultimaPosicionLibre, OperacionesTexto.ApuntadorVacio, _fabrica);
                _ultimaPosicionLibre++;
                Altura++;

                //SE AGREGA LA INFORMACION
                nuevaRaiz.Hijos[0] = nodoActual.Posicion;
                nuevaRaiz.AgregarDato(llavePorSubir, datoPorSubir, nuevoHermano.Posicion);

                //SE ACTUALIZAN LOS APUNTADORES AL PADRE
                nodoActual.Padre = nuevaRaiz.Posicion;
                nuevoHermano.Padre = nuevaRaiz.Posicion;
                _raiz = nuevaRaiz.Posicion;

                //SE GUARDAN LOS CAMBIOS
                nuevaRaiz.GuardarNodoEnDisco(_archivo, _tamañoEncabezadoBinario);
                nodoActual.GuardarNodoEnDisco(_archivo, _tamañoEncabezadoBinario);
                nuevoHermano.GuardarNodoEnDisco(_archivo, _tamañoEncabezadoBinario);
            }
            else //SI NO ES LA RAIZ
            {
                nodoActual.GuardarNodoEnDisco(_archivo, _tamañoEncabezadoBinario);
                nuevoHermano.GuardarNodoEnDisco(_archivo, _tamañoEncabezadoBinario);

                //SE CARGA EL NODO PADRE 
                var nodoPadre = Node.NodoB<T>.LeerNodoDesdeDisco(_archivo, _tamañoEncabezadoBinario, Orden, nodoActual.Padre, _fabrica);
                Subir(nodoPadre, llavePorSubir, datoPorSubir, nuevoHermano.Posicion);
            }
        }

        private NodoB<T> ObtenerRecursivo(int posicionNodoActual, string llave, out int posicion)
        {
            NodoB<T> nodoActual = Node.NodoB<T>.LeerNodoDesdeDisco(_archivo, _tamañoEncabezadoBinario, Orden, posicionNodoActual, _fabrica);
            posicion = nodoActual.PosicionExactaEnNodo(llave);
            if (posicion != -1)
            {
                return nodoActual;
            }
            else
            {
                if (nodoActual.EsHoja)
                {
                    return null;
                }
                else
                {
                    int posicionAproximada = nodoActual.PosicionAproximadaEnNodo(llave);
                    return ObtenerRecursivo(nodoActual.Hijos[posicionAproximada], llave, out posicion);
                }
            }
        }

        public override void Agregar(string llave, T dato)
        {
            try
            {
                if (llave == "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")
                {
                    throw new ArgumentOutOfRangeException("llave");
                }

                AgregarRecursivo(_raiz, llave, dato);
                Tamaño++;
            }
            catch (Exception)
            {
               
            }
        }

        public override T Obtener(string llave)
        {
            int posicion = -1;
            var nodoObtenido = ObtenerRecursivo(_raiz, llave, out posicion);
            if (nodoObtenido == null)
            {
                throw new InvalidOperationException("La llave indicada no está en el árbol.");
            }
            else
            {
                return nodoObtenido.Datos[posicion];
            }
        }

        public override bool Contiene(string llave)
        {
            int posicion = -1;
            var nodoObtenido = ObtenerRecursivo(_raiz, llave, out posicion);
            if (nodoObtenido == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void EscribirNodo(NodoB<T> nodoActual, StringBuilder texto)
        {
            for (int i = 0; i < nodoActual.Llaves.Count; i++)
            {
                if (nodoActual.Llaves[i] != "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")
                {
                    texto.AppendLine(nodoActual.Llaves[i].ToString());
                    texto.AppendLine(nodoActual.Datos[i].ToString());
                    texto.AppendLine("---------------");
                }
                else
                {
                    break;
                }
            }
        }

        public override List<string> RecorrerPreOrden()
        {
            StringBuilder texto = new StringBuilder();
            RecorrerPreOrdenRecursivo(_raiz, texto);
            return datos;
        }

        private void RecorrerPreOrdenRecursivo(int posicionActual, StringBuilder texto)
        {
            if (posicionActual == OperacionesTexto.ApuntadorVacio)
            {
                return;
            }
            var nodoActual = NodoB<T>.LeerNodoDesdeDisco(_archivo, _tamañoEncabezadoBinario, Orden, posicionActual, _fabrica);

            EscribirNodo(nodoActual, texto);
            for (int i = 0; i < nodoActual.Hijos.Count; i++)
            {             
                RecorrerPreOrdenRecursivo(nodoActual.Hijos[i], texto);                
            }

            for (int i = 0; i < nodoActual.CantidadDatos; i++)
            {
                datos.Add(nodoActual.Datos[i].ToFixedSizeString());
            }
        }

        public override string RecorrerInOrden()
        {
            StringBuilder texto = new StringBuilder();
            RecorrerInOrdenRecursivo(_raiz, texto);
            return texto.ToString();
        }

        private void RecorrerInOrdenRecursivo(int posicionActual, StringBuilder texto)
        {
            if (posicionActual == OperacionesTexto.ApuntadorVacio)
            {
                return;
            }

            var nodoActual = NodoB<T>.LeerNodoDesdeDisco(_archivo, _tamañoEncabezadoBinario, Orden, posicionActual, _fabrica);
            for (int i = 0; i < nodoActual.Hijos.Count; i++)
            {
                RecorrerInOrdenRecursivo(nodoActual.Hijos[i], texto);
                if ((i < nodoActual.Llaves.Count) && (nodoActual.Llaves[i] != ""))
                {
                    texto.AppendLine(nodoActual.Llaves[i].ToString());
                    texto.AppendLine(nodoActual.Datos[i].ToString());
                    texto.AppendLine("---------------");
                }
            }
        }

        public override string RecorrerPostOrden()
        {
            StringBuilder texto = new StringBuilder();
            RecorrerPostOrdenRecursivo(_raiz, texto);
            return texto.ToString();
        }

        private void RecorrerPostOrdenRecursivo(int posicionActual, StringBuilder texto)
        {
            if (posicionActual == OperacionesTexto.ApuntadorVacio)
            {
                return;
            }
            var nodoActual = NodoB<T>.LeerNodoDesdeDisco(_archivo, _tamañoEncabezadoBinario, Orden, posicionActual, _fabrica);
            for (int i = 0; i < nodoActual.Hijos.Count; i++)
            {
                RecorrerPreOrdenRecursivo(nodoActual.Hijos[i], texto);
            }
            EscribirNodo(nodoActual, texto);
        }

        public override int ObtenerAltura()
        {
            return Altura;
        }

        public override void Eliminar(string llave)
        {
            throw new NotImplementedException();
        }

        public override void Cerrar()
        {
            _archivo.Close();
        }
        
        public T Search(Delegate comparer, string llave)
        {
            return (T)comparer.DynamicInvoke(this, llave);
        }
    }
}
