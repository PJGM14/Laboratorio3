namespace Estructuras.LinearStructures.Interface
{
    interface ILinearDataStructure<T>
    {
        void Add(T value);
        T Delete();
        T Get();
    }
}
