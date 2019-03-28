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
            var medicamento = new Medicina(datos[0].Trim(), datos[1].Trim(), datos[2].Trim(), datos[3].Trim(), datos[4].Trim(), datos[5].Trim());
            return medicamento;
        }

        public Medicina FabricarNulo()
        {
            return new Medicina();
        }
    }
}