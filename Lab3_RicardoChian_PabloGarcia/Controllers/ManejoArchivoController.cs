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

                                        var id = DatosFila[0];

                                        var nombre = DatosFila[1];

                                        var descripcion = "";
                                        var casa = "";
                                        var precio = "";
                                        var existencia = "";

                                        if (DatosFila.Length == 6)
                                        {
                                            descripcion = DatosFila[2];
                                            casa = DatosFila[3];
                                            precio = DatosFila[4];
                                            existencia = DatosFila[5];
                                        }

                                        if (DatosFila.Length == 7)
                                        {
                                            var segundo = DatosFila[2];
                                            var Tercero = DatosFila[3];
                                            var cuarto = DatosFila[4];
                                            var quinto = DatosFila[5];
                                            var sexto = DatosFila[6];

                                            if (segundo.Contains('"'))
                                            {
                                                descripcion = segundo + Tercero;
                                                Tercero = "";
                                            }
                                            else
                                            {
                                                descripcion = segundo;
                                            }

                                            if (Tercero != "")
                                            {
                                                if (Tercero.Contains('"'))
                                                {
                                                    casa = Tercero + cuarto;
                                                    cuarto = "";
                                                }
                                            }
                                            if (cuarto != "")
                                            {
                                                casa = cuarto;
                                                precio = quinto;
                                                existencia = sexto;
                                            }
                                            else
                                            {
                                                precio = quinto;
                                                existencia = sexto;
                                            }
                                        }

                                        if (DatosFila.Length == 8)
                                        {
                                            var segundo = DatosFila[2];
                                            var Tercero = DatosFila[3];
                                            var cuarto = DatosFila[4];
                                            var quinto = DatosFila[5];
                                            var sexto = DatosFila[6];
                                            var septimo = DatosFila[7];

                                            descripcion = segundo + Tercero;
                                            casa = cuarto + quinto;
                                            precio = sexto;
                                            existencia = septimo;
                                        }

                                        //var Farmaco = new Medicina(id, nombre, descripcion, casa, precio, existencia);
                                        //Data.Instance.ListaMedicinas.Add(Farmaco);
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

                    //foreach (var item in Data.Instance.ListaMedicinas)
                    //{
                    //    var indice = new Indice(item.Id,item.Nombre);
                    //    Data.Instance.ArbolIndice.Insertar(ref Data.Instance.ArbolIndice.Raiz,indice,Indice.OrdenarPorNombre,null);
                    //}

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