using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Estructuras.NoLinearStructures.Interface;
using Estructuras.NoLinearStructures.Operaciones;

namespace Estructuras.NoLinearStructures.Node
{
    internal class NodoB<T> where T : ITextoTamañoFijo
    {
        public const int OrdenMinimo = 3;
        public const int OrdenMaximo = 99;
        internal int Orden { get; private set; }
        internal int Posicion { get; private set; }
        internal int Padre { get; set; }
        internal List<int> Hijos { get; set; }
        internal List<string> Llaves { get; set; }
        internal List<T> Datos { get; set; }

        internal int CantidadDatos
        {
            get
            {
                int i = 0;
                while (i < Llaves.Count && Llaves[i] != "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")
                {
                    i++;
                }
                return i;
            }
        }

        internal bool Underflow
        {
            get
            {
                return (CantidadDatos < ((Orden / 2) - 1));
            }
        }

        internal bool Lleno
        {
            get
            {
                return (CantidadDatos >= Orden - 1);
            }
        }

        internal bool EsHoja
        {
            get
            {
                bool EsHoja = true;
                for (int i = 0; i < Hijos.Count; i++)
                {
                    if (Hijos[i] != OperacionesTexto.ApuntadorVacio)
                    {
                        EsHoja = false;
                        break;
                    }
                }
                return EsHoja;
            }
        }

        internal int TamañoEnTexto
        {
            get
            {
                int tamañoEnTexto = 0;
                tamañoEnTexto += OperacionesTexto.TextoEnteroTamaño + 1; // Tamaño del indicador de Posición 
                tamañoEnTexto += OperacionesTexto.TextoEnteroTamaño + 1; // Tamaño apuntador al Padre 
                tamañoEnTexto += 2; // Separadores adicionales 
                tamañoEnTexto += (OperacionesTexto.TextoEnteroTamaño + 1) * Orden; // Tamaño Hijos 
                tamañoEnTexto += 2; // Separadores adicionales 
                tamañoEnTexto += (OperacionesTexto.TextoLlaveTamaño + 1) * (Orden - 1); // Tamaño Llaves 
                tamañoEnTexto += 2; // Separadores adicionales 
                tamañoEnTexto += (Datos[0].FixedSizeText + 1) * (Orden - 1); // Tamaño Datos 
                tamañoEnTexto += OperacionesTexto.TextoNuevaLineaTamaño; // Tamaño del Enter 
                return tamañoEnTexto;
            }
        }

        internal int TamañoEnBytes
        {
            get
            {
                return TamañoEnTexto * OperacionesTexto.BinarioCaracterTamaño;
            }
        }

        internal NodoB(int orden, int posicion, int padre, IFabricaTamañoTextoFijo<T> fabrica)
        {
            if ((orden < OrdenMinimo) || (orden > OrdenMaximo)) { throw new ArgumentOutOfRangeException("orden"); }
            if (posicion < 0) { throw new ArgumentOutOfRangeException("posicion"); }
            Orden = orden; Posicion = posicion; Padre = padre;
            LimpiarNodo(fabrica);
        }

        private int CalcularPosicionEnDisco(int tamañoEncabezado)
        {
            return tamañoEncabezado + (Posicion * TamañoEnBytes);
        }

        private string ConvertirATextoTamañoFijo()
        {
            StringBuilder datosCadena = new StringBuilder();
            datosCadena.Append(OperacionesTexto.FormatearEntero(Posicion));
            datosCadena.Append(OperacionesTexto.TextoSeparador);
            datosCadena.Append(OperacionesTexto.FormatearEntero(Padre));
            datosCadena.Append(OperacionesTexto.TextoSeparador);
            datosCadena.Append(OperacionesTexto.TextoSeparador);
            datosCadena.Append(OperacionesTexto.TextoSeparador);

            for (int i = 0; i < Hijos.Count; i++)
            {
                datosCadena.Append(OperacionesTexto.FormatearEntero(Hijos[i]));
                datosCadena.Append(OperacionesTexto.TextoSeparador);
            }

            datosCadena.Append(OperacionesTexto.TextoSeparador);
            datosCadena.Append(OperacionesTexto.TextoSeparador);

            for (int i = 0; i < Llaves.Count; i++)
            {
                datosCadena.Append(OperacionesTexto.FormatearLlave(Llaves[i]));
                datosCadena.Append(OperacionesTexto.TextoSeparador);
            }
            datosCadena.Append(OperacionesTexto.TextoSeparador);
            datosCadena.Append(OperacionesTexto.TextoSeparador);

            for (int i = 0; i < Datos.Count; i++)
            {
                datosCadena.Append(Datos[i].ToFixedSizeString().Replace(OperacionesTexto.TextoSeparador, OperacionesTexto.TextoSustitutoSeparador));
                datosCadena.Append(OperacionesTexto.TextoSeparador);
            }
            datosCadena.Append(OperacionesTexto.TextoNuevaLinea);
            return datosCadena.ToString();
        }

        private byte[] ObtenerBytes()
        {
            byte[] datosBinarios = null;
            datosBinarios = OperacionesTexto.ConvertirBinarioYTexto(ConvertirATextoTamañoFijo());
            return datosBinarios;
        }

        private void LimpiarNodo(IFabricaTamañoTextoFijo<T> fabrica)
        {
            Hijos = new List<int>();

            for (int i = 0; i < Orden; i++)
            {
                Hijos.Add(OperacionesTexto.ApuntadorVacio);
            }

            Llaves = new List<string>();

            for (int i = 0; i < Orden - 1; i++)
            {
                Llaves.Add("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            }

            Datos = new List<T>();

            for (int i = 0; i < Orden - 1; i++)
            {
                Datos.Add(fabrica.FabricarNulo());
            }
        }

        internal static NodoB<T> LeerNodoDesdeDisco(FileStream archivo, int tamañoEncabezado, int orden, int posicion, IFabricaTamañoTextoFijo<T> fabrica)
        {
            if (archivo == null)
            {
                throw new ArgumentNullException("archivo");
            }
            if (tamañoEncabezado < 0)
            {
                throw new ArgumentOutOfRangeException("tamañoEncabezado");
            }
            if ((orden < OrdenMinimo) || (orden > OrdenMaximo))
            {
                throw new ArgumentOutOfRangeException("orden");
            }
            if (posicion < 0)
            {
                throw new ArgumentOutOfRangeException("posicion");
            }
            if (fabrica == null)
            {
                throw new ArgumentNullException("fabrica");
            }
            // Se crea un nodo nulo para poder acceder a las 
            // propiedades de tamaño calculadas sobre la instancia 

            // el dato de la instancia del nodo 
            NodoB<T> nuevoNodo = new NodoB<T>(orden, posicion, 0, fabrica);

            // Se crea un buffer donde se almacenarán los bytes leidos 
            byte[] datosBinario = new byte[nuevoNodo.TamañoEnBytes];

            // Variables a ser utilizadas luego de que el archivo sea leido 
            string datosCadena = "";
            string[] datosSeparados = null;
            int PosicionEnDatosCadena = 1;

            // Se ubica la posición donde deberá estar el nodo y se lee desde el archivo 
            archivo.Seek(nuevoNodo.CalcularPosicionEnDisco(tamañoEncabezado), SeekOrigin.Begin);
            archivo.Read(datosBinario, 0, nuevoNodo.TamañoEnBytes);

            // Se convierten los bytes leidos del archivo a una cadena 
            datosCadena = OperacionesTexto.ConvertirBinarioYTexto(datosBinario);

            // Se quitan los saltos de línea y se separa en secciones 
            datosCadena = datosCadena.Replace(OperacionesTexto.TextoNuevaLinea, "");
            datosCadena = datosCadena.Replace("".PadRight(3, OperacionesTexto.TextoSeparador), OperacionesTexto.TextoSeparador.ToString());
            datosSeparados = datosCadena.Split(OperacionesTexto.TextoSeparador);

            // Se obtiene la posición del Padre 
            nuevoNodo.Padre = Convert.ToInt32(datosSeparados[PosicionEnDatosCadena]);
            PosicionEnDatosCadena++;

            // Se asignan al nodo vacío los hijos desde la cadena separada 
            for (int i = 0; i < nuevoNodo.Hijos.Count; i++)
            {
                nuevoNodo.Hijos[i] = Convert.ToInt32(datosSeparados[PosicionEnDatosCadena]);
                PosicionEnDatosCadena++;
            }

            // Se asignan al nodo vacío las llaves desde la cadena separada 
            for (int i = 0; i < nuevoNodo.Llaves.Count; i++)
            {
                nuevoNodo.Llaves[i] = datosSeparados[PosicionEnDatosCadena];
                PosicionEnDatosCadena++;
            }

            // Se asignan al nodo vacío los datos la cadena separada 
            for (int i = 0; i < nuevoNodo.Datos.Count; i++)
            {
                datosSeparados[PosicionEnDatosCadena] = datosSeparados[PosicionEnDatosCadena].Replace(OperacionesTexto.TextoSustitutoSeparador, OperacionesTexto.TextoSeparador);
                nuevoNodo.Datos[i] = fabrica.Fabricar(datosSeparados[PosicionEnDatosCadena]);
                PosicionEnDatosCadena++;
            }

            // Se retorna el nodo luego de agregar toda la información 
            return nuevoNodo;
        }

        internal void GuardarNodoEnDisco(FileStream archivo, int tamañoEncabezado)
        {
            // Se ubica la posición donde se debe escribir 
            archivo.Seek(CalcularPosicionEnDisco(tamañoEncabezado), SeekOrigin.Begin);
            // Se escribe al archivo y se fuerza a vaciar el buffer 
            archivo.Write(ObtenerBytes(), 0, TamañoEnBytes);
            archivo.Flush();
        }

        internal void LimpiarNodoEnDisco(FileStream archivo, int tamañoEncabezado, IFabricaTamañoTextoFijo<T> fabrica)
        {
            // Se limpia el contenido del nodo 
            LimpiarNodo(fabrica);
            // Se guarda en disco el objeto que ha sido limpiado 
            GuardarNodoEnDisco(archivo, tamañoEncabezado);
        }

        internal int PosicionAproximadaEnNodo(string llave)
        {
            int posicion = Llaves.Count;
            int llaveBuscar = GetNumericString(llave);

            for (int i = 0; i < Llaves.Count; i++)
            {
                int llaveArbol = GetNumericString(Llaves[i]);

                if (llaveArbol > llaveBuscar || (Llaves[i] == "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"))
                {
                    posicion = i;
                    break;
                }
            }

            return posicion;
        }

        internal int GetNumericString(string llave)
        {
            var chars = llave.ToCharArray();
            int result = 0;

            for (int i = 0; i < chars.Length; i++)
            {
                result += (int)chars[i];
            }

            return result;
        }

        internal int PosicionExactaEnNodo(string llave)
        {
            int posicion = -1;

            for (int i = 0; i < Llaves.Count; i++)
            {
                string temp = Llaves[i];

                if (llave.Trim() == temp.Trim())
                {
                    posicion = i;
                    break;
                }
            }
            return posicion;
        }

        internal void AgregarDato(string llave, T dato, int hijoDerecho)
        {
            AgregarDato(llave, dato, hijoDerecho, true);
        }

        internal void AgregarDato(string llave, T dato, int hijoDerecho, bool ValidarLleno)
        {
            if (Lleno && ValidarLleno)
            {
                throw new IndexOutOfRangeException("El nodo está lleno, ya no puede insertar más datos");
            }
            if (llave == "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")
            {
                throw new ArgumentOutOfRangeException("llave");
            }
            // Se ubica la posición para insertar, en el punto 
            // donde se encuentre el primer registro mayor que la llave 
            int posicionParaInsertar = 0;

            posicionParaInsertar = PosicionAproximadaEnNodo(llave);

            // Corrimiento de hijos 
            for (int i = Hijos.Count - 1; i > posicionParaInsertar + 1; i--)
            {
                Hijos[i] = Hijos[i - 1];
            }

            Hijos[posicionParaInsertar + 1] = hijoDerecho;

            // Corrimiento de llaves 
            for (int i = Llaves.Count - 1; i > posicionParaInsertar; i--)
            {
                Llaves[i] = Llaves[i - 1];
                Datos[i] = Datos[i - 1];
            }
            Llaves[posicionParaInsertar] = OperacionesTexto.FormatearLlave(llave);
            Datos[posicionParaInsertar] = dato;
        }

        internal void AgregarDato(string llave, T dato)
        {
            AgregarDato(llave, dato, OperacionesTexto.ApuntadorVacio);
        }

        internal void EliminarDato(string llave)
        {
            if (!EsHoja)
            {
                throw new Exception("Solo pueden eliminarse llaves en nodos hoja");
            }

            // Se ubica la posición para eliminar, en el punto 
            // donde se encuentre el registro igual a la llave 
            int posicionParaEliminar = -1;
            posicionParaEliminar = PosicionExactaEnNodo(llave);

            // La llave no está contenida en el nodo  
            if (posicionParaEliminar == -1)
            {
                throw new ArgumentException("No puede eliminarse ya que no existe la llave en el nodo");
            }

            // Corrimiento de llaves y datos 
            for (int i = Llaves.Count - 1; i > posicionParaEliminar; i--)
            {
                Llaves[i - 1] = Llaves[i];
                Datos[i - 1] = Datos[i];
            }
            Llaves[Llaves.Count - 1] = "";
        }

        internal void SepararNodo(string llave, T dato, int hijoDerecho, NodoB<T> nuevoNodo, ref string llavePorSubir, ref T datoPorSubir)
        {
            if (!Lleno)
            {
                throw new Exception("Uno nodo solo puede separarse si está lleno");
            }
            // Incrementar el tamaño de las listas en una posición 
            Llaves.Add("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            Datos.Add(dato);
            Hijos.Add(OperacionesTexto.ApuntadorVacio);

            // Agregar los nuevos elementos en orden 
            AgregarDato(llave, dato, hijoDerecho, false);

            // Obtener los valores a subir 
            int mitad = (Orden / 2);
            llavePorSubir = OperacionesTexto.FormatearLlave(Llaves[mitad]);
            datoPorSubir = Datos[mitad];
            Llaves[mitad] = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

            // Llenar las llaves y datos que pasan al nuevo nodo 
            int j = 0;
            for (int i = mitad + 1; i < Llaves.Count; i++)
            {
                nuevoNodo.Llaves[j] = Llaves[i];
                nuevoNodo.Datos[j] = Datos[i];
                Llaves[i] = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
                j++;
            }

            // Llenar los hijos que pasan al nuevo nodo 
            j = 0;
            for (int i = mitad + 1; i < Hijos.Count; i++)
            {
                nuevoNodo.Hijos[j] = Hijos[i];
                Hijos[i] = OperacionesTexto.ApuntadorVacio;
                j++;
            }

            Llaves.RemoveAt(Llaves.Count - 1);
            Datos.RemoveAt(Datos.Count - 1);
            Hijos.RemoveAt(Hijos.Count - 1);
        }
    }
}