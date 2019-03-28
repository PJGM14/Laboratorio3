using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Estructuras.NoLinearStructures.Trees;
using Lab3_RicardoChian_PabloGarcia.Models;

namespace Lab3_RicardoChian_PabloGarcia.Declaradores
{
    public class Data
    {
        private static Data _instance = null;

        public static Data Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Data();
                }
                return _instance;
            }
        }

        public List<Medicina> ListaPedido = new List<Medicina>();
        public ArbolBinario<Indice> ArbolIndice = new ArbolBinario<Indice>();
        public ArbolB<Medicina> MedicinasTree;

        public void Instanciar(string path)
        {
            MedicinasTree = new ArbolB<Medicina>(5,path,new FabricaMedicina());
        }
    }
}