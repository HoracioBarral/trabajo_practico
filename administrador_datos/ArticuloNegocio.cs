﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using moldes_clases;

namespace administrador_datos
{
    public class ArticuloNegocio
    {
        public List<Articulo> listar()
        {
            List<Articulo> listaArt = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();
            datos.SetConsulta("select a.id,a.Codigo,a.Nombre,a.Descripcion,a.Precio,c.Descripcion Tipo,m.Descripcion Marca from articulos a left join categorias c on a.IdCategoria=c.Id inner join marcas m on a.IdMarca=m.Id");
            try
            {
                datos.Consulta_A_DB();
                while (datos.Lector.Read())
                {
                    Articulo art = new Articulo();
                    art.Id = (int)datos.Lector["id"];
                    art.Codigo = (string)datos.Lector["Codigo"];
                    art.Nombre = (string)datos.Lector["Nombre"];
                    art.Descripcion = (string)datos.Lector["Descripcion"];
                    art.Precio = (decimal)datos.Lector["Precio"];
                    art.TipoCategoria = new Categoria();

                    if (!(datos.Lector["Tipo"] is DBNull))
                    {
                        art.TipoCategoria.Descripcion = (string)datos.Lector["Tipo"];
                    }
                    else
                        art.TipoCategoria.Descripcion = "Generico";

                    art.NombreMarca = new Marca();
                    art.NombreMarca.Descripcion = (string)datos.Lector["Marca"];
                    List<Imagen> imagenes = new List<Imagen>();
                    imagenes = obtenerImagenes(art.Id);
                    art.UrlImagen = imagenes;
                    listaArt.Add(art);
                }
                return listaArt;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.CerrarConexion();
            }

        }

        private List<Imagen> obtenerImagenes(int id)
        {
            List<Imagen> listadoImagenes = new List<Imagen>();
            AccesoDatos datos2 = new AccesoDatos();
            datos2.SetConsulta("select i.id,i.imagenUrl,a.Id articulo from imagenes i, articulos a where a.Id=i.IdArticulo");
            try
            {
                datos2.Consulta_A_DB();
                while (datos2.Lector.Read())
                {
                    int articulo = (int)datos2.Lector["articulo"];
                    if (articulo == id)
                    {
                        Imagen imagen = new Imagen();
                        imagen.Id = (int)datos2.Lector["id"];
                        imagen.IdArticulo = articulo;
                        imagen.Url = (string)datos2.Lector["imagenUrl"];
                        listadoImagenes.Add(imagen);
                    }
                }
                return listadoImagenes;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos2.CerrarConexion();
            }
        }

        public void AgregarArticulo(Articulo nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                obtenerIdMarcaYCategoria(nuevo);
                datos.SetConsulta("insert into articulos (Codigo,Nombre,Descripcion,IdMarca,IdCategoria,Precio) values(@Codigo,@Nombre,@Descripcion,@IdMarca,@IdCategoria,@Precio)");
                datos.SetParametros("@Codigo", nuevo.Codigo);
                datos.SetParametros("@Nombre", nuevo.Nombre);
                datos.SetParametros("@Descripcion", nuevo.Descripcion);
                datos.SetParametros("@IdMarca", nuevo.NombreMarca.Id);
                datos.SetParametros("@IdCategoria", nuevo.TipoCategoria.Id);
                datos.SetParametros("@Precio", nuevo.Precio);
                datos.EjecutarAccion();
                AgregarImagenes(nuevo.UrlImagen);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.CerrarConexion();
            }

        }

        private void obtenerIdMarcaYCategoria(Articulo nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            datos.SetConsulta("select c.id IDCategoria, m.id IDMarca from categorias c, marcas m where m.Descripcion=@nombreMarca and c.Descripcion=@nombreCategoria");
            datos.SetParametros("@nombreMarca", nuevo.NombreMarca.Descripcion);
            datos.SetParametros("@nombreCategoria", nuevo.TipoCategoria.Descripcion);

            try
            {
                datos.Consulta_A_DB();
                while (datos.Lector.Read())
                {
                    nuevo.TipoCategoria.Id = (int)datos.Lector["IDCategoria"];
                    nuevo.NombreMarca.Id = (int)datos.Lector["IDMarca"];
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        private void AgregarImagenes(List<Imagen> imagenes)
        {
            int id = obetenerUltimoIdArticulos();
            foreach (Imagen aux in imagenes)
            {
                AccesoDatos datos = new AccesoDatos();
                datos.SetConsulta("insert into imagenes (IdArticulo,ImagenUrl) values(@idArticulo,@UrlImagen)");
                datos.SetParametros("@idArticulo", id);
                datos.SetParametros("@UrlImagen", aux.Url);
                try
                {
                    datos.EjecutarAccion();
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                finally
                {
                    datos.CerrarConexion();
                }
            }
            
        }

        private int obetenerUltimoIdArticulos()
        {
            AccesoDatos datos = new AccesoDatos();
            datos.SetConsulta("select id from articulos");
            int id=0;
            try
            {
                datos.Consulta_A_DB();
                while (datos.Lector.Read())
                {
                    id = (int)datos.Lector["id"];
                }
                return id;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

    }
}
