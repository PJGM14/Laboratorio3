namespace Estructuras.LinearStructures.Nodes
{
    class Nodo<T>
    {
        public T Value { get; set; }
        public Nodo<T> Next { get; set; }

        public Nodo(T value)
        {
            this.Value = value;
            Next = null;
        }
    }
}
