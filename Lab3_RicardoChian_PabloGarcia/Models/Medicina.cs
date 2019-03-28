using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using Estructuras.NoLinearStructures.Interface;

namespace Lab3_RicardoChian_PabloGarcia.Models
{
    public class Medicina : IComparable, ITextoTamañoFijo
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

        //ID-NOMBRE-DESCRIPCION-CASA-PRECIO-EXISTENCIAS
        //10-  60  -     80    - 40 -  10  -      10    = 215
        private const string FormatoConst = "0000000000-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx-0000000000-0000000000";

        public Medicina()
        {
            Id = 0;
            Nombre = "";
            Descripcion = "";
            Casa = "";
            Precio = 0.00;
            Existencia = 0;
        }

        public Medicina(string id, string nombre, string descripcion, string casa, string precio, string existencia)
        {
            Id = Int32.Parse(id);
            Nombre = nombre;
            Descripcion = descripcion;
            Casa = casa;

            var aux = precio.Substring(1);
            Precio = Double.Parse(aux);

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

        public int FixedSizeText
        {
            get
            {
                return 215;
            }

        }

        public override string ToString()
        {
            return ToFixedSizeString();
        }

        public string ToFixedSizeString()
        {
            var strgBuilder = new StringBuilder();

            strgBuilder.Append(Id.ToString().PadLeft(10, '0'));
            strgBuilder.Append('-');
            strgBuilder.Append(Nombre.PadLeft(60, 'x'));
            strgBuilder.Append('-');
            strgBuilder.Append(Descripcion.PadLeft(80, 'x'));
            strgBuilder.Append('-');
            strgBuilder.Append(Casa.PadLeft(40, 'x'));
            strgBuilder.Append('-');
            strgBuilder.Append(Precio.ToString().PadLeft(10, '0'));
            strgBuilder.Append('-');
            strgBuilder.Append(Existencia.ToString().PadLeft(10, '0'));

            return strgBuilder.ToString();
        }
    }
}