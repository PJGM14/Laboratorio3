using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lab3_RicardoChian_PabloGarcia.Models
{
    public class Medicina : IComparable
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Casa médica")]
        public string Casa { get; set; }

        [Display(Name = "Precio")]
        [DataType(DataType.Currency)]
        public double Precio { get; set; }

        [Display(Name = "Existecias")]
        public int Existencia { get; set; }

        public Medicina(string id, string nombre, string descripcion, string casa, string precio, string existencia)
        {
            Id = Int32.Parse(id);
            Nombre = nombre;
            Descripcion = descripcion;
            Casa = casa;

            var aux = precio.Substring(1);

            Precio = Double.Parse(aux);
            //Precio = double.Parse(precio, CultureInfo.InvariantCulture);
            Existencia = Int32.Parse(existencia);
        }

        public int CompareTo(object obj)
        {
            var comparer = (Medicina)obj;

            return Nombre.CompareTo(comparer.Nombre);
        }

        public static Comparison<Medicina> OrdenarPorNombre = delegate (Medicina Med1, Medicina Med2)
        {
            return Med1.CompareTo(Med2);
        };
    }
}