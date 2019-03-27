using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estructuras.NoLinearStructures.Interface
{
    public interface ITextoTamañoFijo
    {
        int FixedSizeText { get; }
        string ToFixedSizeString();
    }
}
