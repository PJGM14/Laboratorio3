using System;
using System.Collections;
using System.Collections.Generic;
using Estructuras.NoLinearStructures.Interface;
using Estructuras.NoLinearStructures.Node;

namespace Estructuras.NoLinearStructures.Trees
{
    public class ArbolBinario<T> : INonLinearStructure<T>, IEnumerable<T> where T : IComparable
    {
        public Nodo<T> Raiz;

        public ArbolBinario()
        {
            Raiz = null;
        }


        public void Insertar(ref Nodo<T> arbol, T value, Delegate comparer, Nodo<T> padre)
        {
            if (Raiz == null)
            {
                var nodoNuevo = new Nodo<T>(value, padre);
                Raiz = nodoNuevo;
            }
            else
            {
                if (arbol == null)
                {
                    var nodoNuevo = new Nodo<T>(value, padre);
                    arbol = nodoNuevo;
                }
                else
                {
                    if ((int) comparer.DynamicInvoke(value, arbol.Value) < 0)
                    {
                        Insertar(ref arbol.Izdo, value, comparer, arbol);
                    }
                    else if ((int) comparer.DynamicInvoke(value, arbol.Value) > 0)
                    {
                        Insertar(ref arbol.Dcho, value, comparer, arbol);
                    }
                }
            }
        }

        public T Mostrar()
        {
            throw new NotImplementedException();
        }

        public bool Buscar(Nodo<T> arbol, T value, Delegate comparer)
        {
            if (arbol == null)
            {
                return false;
            }
            else if ((int) comparer.DynamicInvoke(value, arbol.Value) == 0)
            {
                return true;
            }
            else if ((int) comparer.DynamicInvoke(value, arbol.Value) < 0)
            {
                return Buscar(arbol.Izdo, value, comparer);
            }
            else
            {
                return Buscar(arbol.Dcho, value, comparer);
            }
        }

        public T DevolverValor()
        {
            throw new NotImplementedException();
        }

        private Nodo<T> Minimo(Nodo<T> Arbol)
        {
            if (Arbol == null)
            {
                return null;
            }
            else
            {
                if (Arbol.Izdo != null)
                {
                    return Minimo(Arbol.Izdo);
                }
                else
                {
                    return Arbol;
                }
            }
        }

        private void Reemplazar(Nodo<T> Arbol, Nodo<T> NuevoNodo, Delegate comparer)
        {
            if (Arbol.Padre != null)
            {
                if (Arbol.Padre.Izdo != null)
                {
                    if ((int)comparer.DynamicInvoke(Arbol.Value, Arbol.Padre.Izdo.Value) == 0)
                    {
                        Arbol.Padre.Izdo = NuevoNodo;
                    }
                }
                else if ((int) comparer.DynamicInvoke(Arbol.Value, Arbol.Padre.Dcho.Value) == 0)
                {
                    Arbol.Padre.Dcho = NuevoNodo;
                }
            }

            if (NuevoNodo != null)
            {
                NuevoNodo.Padre = Arbol.Padre;
            }
        }

        private void EliminarNodo(Nodo<T> NodoEliminar, Delegate Comparer)
        {
            if (NodoEliminar.Izdo != null && NodoEliminar.Dcho != null)
            {
                var menor = Minimo(NodoEliminar.Dcho);
                NodoEliminar.Value = menor.Value;
                EliminarNodo(menor, Comparer);
            }
            else if (NodoEliminar.Izdo != null)
            {
                Reemplazar(NodoEliminar, NodoEliminar.Izdo, Comparer);
                NodoEliminar.Izdo = null;
                NodoEliminar.Dcho = null;
                NodoEliminar = null;
            }
            else if (NodoEliminar.Dcho != null)
            {
                Reemplazar(NodoEliminar, NodoEliminar.Dcho, Comparer);
                NodoEliminar.Izdo = null;
                NodoEliminar.Dcho = null;
                NodoEliminar = null;
            }
            else
            {
                Reemplazar(NodoEliminar, null,Comparer);
                NodoEliminar.Izdo = null;
                NodoEliminar.Dcho = null;
                NodoEliminar = null;
            }
        }

        public void Eliminar(Nodo<T> arbol, T value, Delegate comparer)
        {
            if (arbol != null && Buscar(arbol,value,comparer))
            {
                if ((int) comparer.DynamicInvoke(value, arbol.Value) < 0)
                {
                    Eliminar(arbol.Izdo, value, comparer);
                }
                else if ((int) comparer.DynamicInvoke(value, arbol.Value) > 0)
                {
                    Eliminar(arbol.Dcho, value, comparer);
                }
                else if ((int) comparer.DynamicInvoke(value, arbol.Value) == 0)
                {
                    EliminarNodo(arbol, comparer);
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}