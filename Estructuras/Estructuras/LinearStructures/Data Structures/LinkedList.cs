using System;
using System.Collections;
using System.Collections.Generic;
using Estructuras.LinearStructures.Interface;
using Estructuras.LinearStructures.Nodes;

namespace Estructuras.LinearStructures.Data_Structures
{
    public class LinkedList <T>:ILinearDataStructure<T>, IEnumerable<T> where T : IComparable
    {
        private Nodo<T> First { get; set; }
        public int Count { get; set; }
        public bool IsEmpty { get; set; }

        public LinkedList()
        {
            First = null;
            Count = 0;
            IsEmpty = true;
        }

        public void Add(T value)
        {
            if (IsEmpty)
            {
                Count = 1;
                First = new Nodo<T>(value);
                IsEmpty = false;
            }
            else
            {
                var current = First;

                while (current.Next != null)
                {
                    current = current.Next;
                }

                var auxNodo = new Nodo<T>(value);
                current.Next = auxNodo;
                Count++;
            }
        }

        public T Delete()
        {
            throw new NotImplementedException();
        }

        public T Get()
        {
            throw new NotImplementedException();
        }

        public void Sort(Delegate comparer)
        {
            var first = First;

            var aElement = first;
            var bElement = first;

            while (bElement != null)
            {
                aElement = bElement.Next;

                while (aElement != null)
                {
                    if ((int)comparer.DynamicInvoke(aElement.Value, bElement.Value) < 0)
                    {
                        var Aux = bElement.Value;

                        bElement.Value = aElement.Value;
                        aElement.Value = Aux;
                    }
                    aElement = aElement.Next;
                }
                bElement = bElement.Next;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var node = First;

            while (node != null)
            {
                yield return node.Value;
                node = node.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}