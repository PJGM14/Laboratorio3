using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras.NoLinearStructures.Interface
{
    public interface IFabricaTamañoTextoFijo<T> where T : ITextoTamañoFijo
    {
        T Fabricar(string textoTamañoFijo);
        T FabricarNulo();
    }
}
