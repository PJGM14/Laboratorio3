using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Lab3_RicardoChian_PabloGarcia.Controllers
{
    public class ManejoArchivoController : Controller
    {
        // GET: ManejoArchivo
        public ActionResult Lector()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Lector(HttpPostedFileBase postedFile)
        {

            var FilePath = string.Empty;
            var path = Server.MapPath("~/CargaCSV/");

            try
            {
                if (postedFile != null)
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    FilePath = path + Path.GetFileName(postedFile.FileName);

                    postedFile.SaveAs(FilePath);

                    var CsvData = System.IO.File.ReadAllText(FilePath);

                    var cont = 0;

                    foreach (var fila in CsvData.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(fila))
                        {
                            if (cont != 0)
                            {
                                try
                                {
                                    var filaSecundaria = fila.Split('\r');

                                    var filaSinVacios = filaSecundaria[0];

                                    var DatosFila = filaSinVacios.Split(',');


                                    var id = "";
                                    var nombre = "";
                                    var descripcion = "";
                                    var casa = "";
                                    var precio = "";
                                    var existencia = "";


                                    var Aux = "";
                                    var ContComillas = 0;
                                    var Campo = 0;

                                    for (int i = 0; i < DatosFila.Length; i++)
                                    {
                                        var Linea = DatosFila[i];

                                        if (Linea.Contains('"'))
                                        {
                                            if (ContComillas == 0)
                                            {
                                                Aux += Linea;
                                                ContComillas = 1;
                                            }
                                            else if (ContComillas == 1)
                                            {
                                                Aux += ", " + Linea;
                                                ContComillas = 0;

                                                switch (Campo)
                                                {

                                                    case 0:
                                                        id = Aux;
                                                        Aux = "";
                                                        break;

                                                    case 1:
                                                        nombre = Aux;
                                                        Aux = "";
                                                        break;

                                                    case 2:
                                                        descripcion = Aux;
                                                        Aux = "";
                                                        break;

                                                    case 3:
                                                        casa = Aux;
                                                        Aux = "";
                                                        break;

                                                    case 4:
                                                        precio = Aux;
                                                        Aux = "";
                                                        break;

                                                    case 5:
                                                        existencia = Aux;
                                                        Aux = "";
                                                        break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (ContComillas != 0)
                                            {
                                                Aux += ", " + Linea;
                                            }
                                            else
                                            {
                                                switch (Campo)
                                                {

                                                    case 0:
                                                        id = Linea;
                                                        break;

                                                    case 1:
                                                        nombre = Linea;
                                                        break;

                                                    case 2:
                                                        descripcion = Linea;
                                                        break;

                                                    case 3:
                                                        casa = Linea;
                                                        break;

                                                    case 4:
                                                        precio = Linea;
                                                        break;

                                                    case 5:
                                                        existencia = Linea;
                                                        break;
                                                }

                                                if (Aux == "")
                                                {

                                                    Campo++;

                                                }
                                                Linea = "";
                                            }

                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    
                                }
                            }
                            cont++;
                        }
                    }
                    System.IO.File.Delete(FilePath);
                    Directory.Delete(path);
                }
                FilePath = "";
            }
            catch (Exception Error)
            {
                System.IO.File.Delete(FilePath);
                Directory.Delete(path);
                return RedirectToAction("Lector", "ManejoArchivo");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}