using System;
using Estructuras.NoLinearStructures.Node;

namespace Estructuras.NoLinearStructures.Interface
{
    interface INonLinearStructure<T>
    {
        void Insertar(ref Nodo<T> arbol, T value, Delegate comparer, Nodo<T> padre);

        T Mostrar();

        bool Buscar(Nodo<T> arbol, T value, Delegate comparer);

        T DevolverValor();

        void Eliminar(Nodo<T> Arbol,T value, Delegate comparer);
    }
}
