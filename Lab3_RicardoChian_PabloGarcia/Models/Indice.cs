using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lab3_RicardoChian_PabloGarcia.Models
{
    public class Indice : IComparable
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Debe ingresar nombre")]
        public string Nombre { get; set; }

        public Indice(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }

        public int CompareTo(object obj)
        {
            var comparer = (Indice)obj;

            return Nombre.CompareTo(comparer.Nombre);
        }

        public static Comparison<Indice> OrdenarPorNombre = delegate (Indice Ind1, Indice Ind2)
        {
            return Ind1.CompareTo(Ind2);
        };
    }
}