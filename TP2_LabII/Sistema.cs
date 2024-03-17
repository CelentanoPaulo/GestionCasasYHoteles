﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;

namespace TP2_LabII
{
    [Serializable]
    public class Sistema : IExportable
    {
        public List<Cliente> Clientes { get; private set; }
        public List<Propiedad> Propiedades { get; private set; }
        public List<Reserva> Reservas { get; private set; }
        public List<Reserva> reservasEnPropiedad { get; private set; }

        public Sistema()
        {
            Clientes = new List<Cliente>();
            Propiedades = new List<Propiedad>();
            Reservas = new List<Reserva>();
            
        }

        public void AgregarCliente(Cliente cliente)
        {
            if (cliente != null)
            {
                Clientes.Add(cliente);
            }
            else
            {
                throw new ArgumentNullException("El cliente no puede ser nulo.");
            }
        }

        public void AgregarPropiedad(Propiedad propiedad)
        {
            if (propiedad != null)
            {
                Propiedades.Add(propiedad);
            }
            else
            {
                throw new ArgumentNullException("La propiedad no puede ser nula.");
            }
        }

        public void BajaPropiedad(Propiedad propiedad)
        {
            if (propiedad == null)
            {
                throw new ArgumentNullException("La propiedad no puede ser nula.");
            }
            if (!Propiedades.Contains(propiedad))
            {
                throw new InvalidOperationException("La propiedad no existe en el sistema.");
            }
            else
            {
                Propiedades.Remove(propiedad);
            }

            List<Reserva> reservasAsociadas = new List<Reserva>();
            foreach (Reserva reserva in Reservas)
            {
                if (reserva.Propiedad == propiedad)
                {
                    reservasAsociadas.Add(reserva);
                }
            }

            foreach (Reserva reserva in reservasAsociadas)
            {
                Reservas.Remove(reserva);
            }
        }

        public Propiedad BuscarPropiedad(int codigo)
        {
            Propiedad retorno = null;

            foreach (Propiedad p in Propiedades)
            {
                if (p.Codigo.ToString() == codigo.ToString())
                {
                    retorno = p;
                }
            }
            return retorno;
        }

        public Reserva GenerarReserva(Propiedad propiedad, DateTime fechaIngreso, DateTime fechaEgreso, Cliente cliente, int cantHuespedes)
        {
            Reserva reserva = null;
            try
            {
                if (VerificarDisponibilidad(propiedad, fechaIngreso))
                {
                    reserva = new Reserva(fechaIngreso, fechaEgreso, propiedad, cliente, cantHuespedes);
                    Reservas.Add(reserva);
                }
            }
            catch (MisExcepciones ex)
            {
                throw ex;
            }

            return reserva;
        } // Genera reserva de casa

        public Reserva GenerarReserva(Hotel hotel, DateTime fechaIngreso, DateTime fechaEgreso, Cliente cliente, int numeroHabitacion, int cantHuespedes)
        {
            try
            {
                if (VerificarDisponibilidadHabitacionHotel(hotel, fechaIngreso, numeroHabitacion))
                {
                    Reserva reserva = new Reserva(fechaIngreso, fechaEgreso, hotel, cliente, numeroHabitacion, cantHuespedes);
                    Reservas.Add(reserva);

                    return reserva;
                }
                else
                {
                    throw new MisExcepciones("La habitación especificada no está disponible en este hotel para las fechas seleccionadas.");
                }
            }
            catch (MisExcepciones ex)
            {
                throw ex;
            }
        } // Genera reserva de hotel

        public Reserva BuscarReserva(string codigo)
        {
            Reserva retorno = null;

            foreach (Reserva r in Reservas)
            {
                if (r.CodigoReserva.ToString() == codigo.ToString())
                {
                    retorno = r;
                }
            }
            return retorno;
        }

        public void CancelarReserva(Reserva reserva)
        {
            if (reserva != null && Reservas.Contains(reserva))
            {
                Reservas.Remove(reserva);
            }
        }

        public bool VerificarDisponibilidad(Propiedad propiedad, DateTime fecha)
        {
            if (propiedad == null)
            {
                throw new MisExcepciones(" el error esta cuando no existe : " + nameof(propiedad));
            }

            reservasEnPropiedad = new List<Reserva>();

            foreach (Reserva reserva in Reservas)
            {
                if (reserva.Propiedad == propiedad)
                {
                    reservasEnPropiedad.Add(reserva);
                }
            }

            bool disponible = true;
            foreach (Reserva reserva in reservasEnPropiedad)
            {
                if (!(fecha < reserva.FechaIngreso || fecha >= reserva.FechaEgreso))
                {
                    disponible = false;
                    break;
                    //throw new MisExcepciones("No esta disponible en las fechas solicitadas.");
                }
                else if (disponible == false)
                {
                    MessageBox.Show("No esta disponible en las fechas solicitadas.");
                }
            }
            return disponible;
        } // Verifica disponibilidad de casas 

        private bool VerificarDisponibilidadHabitacionHotel(Hotel hotel, DateTime fecha, int numeroHabitacion)
        {
            reservasEnPropiedad = new List<Reserva>();

            foreach (Reserva reserva in Reservas)
            {
                if (reserva.Propiedad == hotel)
                {
                    reservasEnPropiedad.Add(reserva);
                }
            }
            bool disponible = true;

            foreach (Reserva reserva in reservasEnPropiedad)
            {
                if (reserva.NumeroHabitacion == numeroHabitacion)
                {
                    if (!(fecha < reserva.FechaIngreso || fecha >= reserva.FechaEgreso))
                    {
                        disponible = false;
                        break;
                        //throw new MisExcepciones("No esta disponible en las fechas solicitadas.");
                    }
                    else if(disponible == false)
                    {
                        MessageBox.Show("No esta disponible en las fechas solicitadas.");
                    }
                }
            }
            return disponible;
        }  // Verifica disponibilidad de habitaciones 

        public List<DateTime> Pintar(Propiedad propiedad) 
        {
            reservasEnPropiedad = new List<Reserva>();

            foreach (Reserva reserva in Reservas)
            {
                if (reserva.Propiedad == propiedad)
                {
                    reservasEnPropiedad.Add(reserva);
                }
            }

            List<DateTime> fechasConReservas = new List<DateTime>();

            foreach (Reserva reserva in reservasEnPropiedad)
            {
                if (propiedad is Casa)
                {
                    for (DateTime fechaReserva = reserva.FechaIngreso; fechaReserva <= reserva.FechaEgreso; fechaReserva = fechaReserva.AddDays(1))
                    {
                        fechasConReservas.Add(fechaReserva);
                    }
                }
            }
            return fechasConReservas;
        } // Pinta las fechas de las reservas de las casas

        public List<DateTime> PintarHabitacion(Hotel hotel, int numeroHabitacion)
        {
            List<Reserva> reservasEnHabitacion = Reservas
                .Where(reserva => reserva.Propiedad == hotel && reserva.NumeroHabitacion == numeroHabitacion)
                .ToList();

            List<DateTime> fechasConReservas = new List<DateTime>();

            foreach (Reserva reserva in reservasEnHabitacion)
            {
                for (DateTime fechaReserva = reserva.FechaIngreso; fechaReserva <= reserva.FechaEgreso; fechaReserva = fechaReserva.AddDays(1))
                {
                    fechasConReservas.Add(fechaReserva);
                }
            }

            return fechasConReservas;
        } // Pinta las fechas de las reservas de las habitaciones de los hoteles


        public List<Propiedad> FiltrarPropiedades(string ciudad, DateTime desde, DateTime hasta, int viajeros, string tipo, bool esFinde)
        {
            List<Propiedad> propiedadesFiltradas = new List<Propiedad>();

            foreach (Propiedad propiedad in Propiedades)
            {
                bool cumpleFiltros = true;

                if (!string.IsNullOrEmpty(ciudad) && !propiedad.Localidad.Contains(ciudad))
                    cumpleFiltros = false;

                if (desde > DateTime.Today && !VerificarDisponibilidad(propiedad, desde))
                    cumpleFiltros = false;

                if (hasta > DateTime.Today && !VerificarDisponibilidad(propiedad, hasta))
                    cumpleFiltros = false;

                if (tipo != null && !((propiedad is Casa && tipo == "Casa") || (propiedad is Hotel && tipo == "Hotel")))
                    cumpleFiltros = false;

                if (propiedad is Casa)
                {
                    Casa casa = (Casa)propiedad;

                    if (viajeros > casa.Camas)
                    {
                        cumpleFiltros = false;
                    }
                    if (casa.EsFinde != esFinde)
                    {
                        cumpleFiltros = false;
                    }

                    int cantDias = (hasta - desde).Days;

                    if (casa.MinimoDias >= cantDias)
                    {
                        cumpleFiltros = false;
                    }
                }

                if (propiedad is Hotel)
                {
                    Hotel hotel = (Hotel)propiedad;

                    int capacidadMaxima = hotel.ObtenerCapacidadMaximaHabitaciones(viajeros);

                    if (capacidadMaxima == 0 || viajeros > capacidadMaxima)
                    {
                        cumpleFiltros = false;
                    }
                }

                if (cumpleFiltros)
                    propiedadesFiltradas.Add(propiedad);
            }

            return propiedadesFiltradas;
        }

        public void ExportarClientes(List<Cliente> clientes, string rutaArchivo)
        {
            if (File.Exists(rutaArchivo))
                File.Delete(rutaArchivo);

            FileStream fs = new FileStream(rutaArchivo, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);

            sw.WriteLine($"DNI;Nombre;Apellido");

            foreach (Cliente cliente in clientes)
            {
                sw.WriteLine($"{cliente.Dni};{cliente.Nombre};{cliente.Apellido}");
            }

            sw.Close();
            fs.Close();
        }

        public void ExportarReservas(List<Reserva> reservas)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Archivo de texto|*.txt|Archivo CSV|*.csv";
            saveFileDialog.Title = "Guardar reservas en archivo de texto";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string rutaArchivo = saveFileDialog.FileName;

                if (File.Exists(rutaArchivo))
                    File.Delete(rutaArchivo);

                FileStream fs = new FileStream(rutaArchivo, FileMode.CreateNew, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine("codigo;Nombre Usuario;Apellido Usuario;Fecha Ingreso;Fecha Egreso;Costo total");

                foreach (Reserva reserva in reservas)
                {
                    sw.WriteLine($"{reserva.CodigoReserva};{reserva.Cliente.Nombre};{reserva.Cliente.Apellido};{reserva.FechaIngreso};{reserva.FechaEgreso};{reserva.CostoTotal}");
                }

                sw.Close();
                fs.Close();
            }
        }

        public void ExportarPropiedades()
        {
            string rutaArchivo = Application.StartupPath + "\\propiedades.txt";

            if (File.Exists(rutaArchivo))
                File.Delete(rutaArchivo);

            FileStream fs = new FileStream(rutaArchivo, FileMode.CreateNew, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            foreach (Propiedad propiedad in Propiedades)
            {
                if (propiedad is Hotel hotel)
                {
                    // Si es un Hotel
                    sw.WriteLine($"Hotel;{hotel.Nombre};{hotel.Provincia};{hotel.Localidad};{hotel.Ubicacion};{hotel.PrecioBase};{hotel.Estrellas}");
                }
                else if (propiedad is Casa casa)
                {
                    // Si es una Casa
                    sw.WriteLine($"Casa;{casa.Nombre};{casa.Provincia};{casa.Localidad};{casa.Ubicacion};{casa.PrecioBase};{casa.Camas};{casa.MinimoDias};{casa.EsFinde}");
                }
            }
            sw.Close();
            fs.Close();
        }

        public void ImportarPropiedades()
        {
            string rutaArchivo = Application.StartupPath + "\\propiedades.txt";

            if (File.Exists(rutaArchivo))
            {
                FileStream fs = new FileStream(rutaArchivo, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                string renglon;
                string[] datos;
                while (!sr.EndOfStream)
                {
                    renglon = sr.ReadLine();
                    datos = renglon.Split(';');
                    if (datos.Length >= 4)
                    {
                        if (datos[0] == "Hotel")
                        {
                            AgregarPropiedad(new Hotel(datos[1], datos[2], datos[3], datos[4], Convert.ToDouble(datos[5]), Convert.ToInt32(datos[6])));
                        }
                        else if (datos[0] == "Casa")
                        {
                            AgregarPropiedad(new Casa(datos[1], datos[2], datos[3], datos[4], Convert.ToDouble(datos[5]), Convert.ToInt32(datos[6]), Convert.ToInt32(datos[7]), Convert.ToBoolean(datos[8])));
                        }
                    }
                }
                sr.Close();
                fs.Close();
            }
        }

        public void ExportarCalendario(string propiedad, string path)
        {
            string rutaArchivo = path;

            if (File.Exists(rutaArchivo))
                File.Delete(rutaArchivo);

            FileStream fs = new FileStream(rutaArchivo, FileMode.CreateNew, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);

            string NombrePropiedad = propiedad;

            sw.WriteLine($"Codigo Reserva;Fecha ingreso;Fecha Egreso");

            foreach(Reserva r in Reservas)
            {            
                if(NombrePropiedad == r.Propiedad.Nombre)
                {
                    sw.WriteLine($"{r.CodigoReserva};{r.FechaIngreso};{r.FechaEgreso}");
                }
            }

            sw.Close();
            fs.Close();
        }

        public void ImportarCalendario(string path)
        {
            string rutaArchivo = path;
            string[] datos;

            FileStream fs = new FileStream(rutaArchivo, FileMode.Open, FileAccess.ReadWrite);
            StreamReader sr = new StreamReader(fs);

            sr.ReadLine();
            string renglon = sr.ReadLine();
            while(renglon != null)
            {
                datos = renglon.Split(';');
                string codigo = datos[0];
                Reserva Importar = BuscarReserva(codigo);
                Importar.FechaIngreso = Convert.ToDateTime(datos[1]);
                 Importar.FechaEgreso = Convert.ToDateTime(datos[2]);
                renglon = sr.ReadLine();
            }
            sr.Close();
            fs.Close();
        } 

    }

}
