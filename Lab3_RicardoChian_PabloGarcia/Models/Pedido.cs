using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Estructuras.NoLinearStructures.Trees;

namespace Lab3_RicardoChian_PabloGarcia.Models
{
    public class Pedido
    {
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Debe ingresar nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Dirección")]
        [DataType(DataType.Text)]
        public string Direccion { get; set; }

        [Display(Name = "NIT")]
        public long Nit { get; set; }

        public List<Medicina> Medicinas { get; set; }

        [Display(Name = "Total")]
        [DataType(DataType.Currency)]
        public double Total { get; set; }

        public Pedido(string nombre, string direccion, string nit, List<Medicina> listado)
        {
            Nombre = nombre;
            Direccion = direccion;
            Nit = long.Parse(nit);
            Medicinas = listado;
            Total = CalcularTotal();

        }

        private double CalcularTotal()
        {
            var total = 0.00;

            foreach (var Item in Medicinas)
            {
                total += Item.Precio;
            }

            return total;
        }

        private bool BuscarMedicinas(ArbolBinario<Indice> indice, List<string> nombresMedicina)
        {
            var existen = true;

            foreach (var item in nombresMedicina)
            {
                var medicina = new Indice(0, item);
                if (indice.Buscar(indice.Raiz, medicina, Indice.OrdenarPorNombre) == false)
                {
                    existen = false;
                }
            }
            return existen;
        }
    }
}