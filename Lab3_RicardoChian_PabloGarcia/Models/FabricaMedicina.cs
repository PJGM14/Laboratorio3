using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Estructuras.NoLinearStructures.Interface;

namespace Lab3_RicardoChian_PabloGarcia.Models
{
    public class FabricaMedicina : IFabricaTamañoTextoFijo<Medicina>
    {
        public Medicina Fabricar(string textoTamañoFijo)
        {
            var datos = textoTamañoFijo.Split('-');
            var medicamento = new Medicina(datos[0], datos[1], datos[2], datos[3], datos[4], datos[5]);
            return medicamento;
        }

        public Medicina FabricarNulo()
        {
            throw new NotImplementedException();
        }
    }
}