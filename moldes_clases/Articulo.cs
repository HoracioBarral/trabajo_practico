﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moldes_clases
{
    public class Articulo
    {
        public int Id { get; set; }
        
        public string Codigo { get; set; }

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public Marca NombreMarca { get; set; }

        public Categoria TipoCategoria { set; get; }

        public List<Imagen> UrlImagen { set; get; }

        public decimal Precio { get; set; }

    }
}
