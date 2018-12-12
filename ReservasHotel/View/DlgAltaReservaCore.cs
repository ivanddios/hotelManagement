﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ReservasHotel.View
{
    public partial class DlgAltaReserva
    {
        public DlgAltaReserva(List<Habitacion> hab, Reserva r)
        {
            this.reservaModificar = r;
            this.Build();
            this.habitaciones = hab;
            this.grdLista.SelectionChanged += (sender, e) => this.FilaSeleccionada();
            if (reservaModificar != null)
            {
                this.edTarifa.Value = (decimal)this.reservaModificar.TarifaDia;
                var lista = new List<Habitacion>();
                lista.Add(this.reservaModificar.Habitacion);
                ActualizaListaHabitaciones(0, lista);
            }

        }

        public void comprobarHabitacionesDisponibles()
        {
            //var fechaEntrada = this.FechaEntrada;
            //var fechaSalida = this.FechaSalida;
           /* if (validarFechas())
            {*/
                comprobarHabitaciones();
           /* }
            else
            {
                //this.fechaEntrada.Value = fechaEntrada;
                //this.fechaSalida.Value = fechaSalida;

            }*/
        }


        public void comprobarHabitaciones()
        {
            
            List<Habitacion> habOcupadas = new List<Habitacion>();
            this.reservas = RegistroReservas.RecuperarXml("registro_reservas.xml");

            var fechaEntrada = this.FechaEntrada;
            var fechaSalida = this.FechaSalida;
            foreach (Reserva r in reservas)
            {

                int fEntradaDiferenciaEntrada = DateTime.Compare(fechaEntrada, r.FechaEntrada);
                int fSalidaDiferenciaEntrada = DateTime.Compare(fechaSalida, r.FechaEntrada);

                int fEntradaDiferenciaSalida = DateTime.Compare(fechaEntrada, r.FechaSalida);
                int fSalidaDiferenciaSalida = DateTime.Compare(fechaSalida, r.FechaSalida);

                Console.WriteLine("Diferencia " + fEntradaDiferenciaEntrada + " " + fSalidaDiferenciaEntrada);
                if (fEntradaDiferenciaEntrada < 0 && fSalidaDiferenciaEntrada < 0 || fEntradaDiferenciaSalida > 0 && fSalidaDiferenciaSalida > 0)
                {
                    Console.WriteLine("Habitacion " + r.Habitacion.IdHabitacion + " disponible");
                }
                else
                {
                    Console.WriteLine("Habitacion " + r.Habitacion.ToString() + " no disponible");
                    if(this.reservaModificar != null && this.reservaModificar.Habitacion.IdHabitacion == r.Habitacion.IdHabitacion)
                    {
                        
                    }
                    else
                    {
                        habOcupadas.Add(r.Habitacion);
                    }

                    
                }

            }


            habitacionesDisponibles(habOcupadas);

        }

        private void habitacionesDisponibles(List<Habitacion> habOcupadas)
        {
            List<Habitacion> habDisponibles = new List<Habitacion>();

            foreach (Habitacion hab in this.habitaciones)
            {
                habDisponibles.Add(hab);
            }
            foreach (Habitacion h in habOcupadas)
            {
                for (int i=0; i<habDisponibles.Count;i++)
                {  
                    if(h.IdHabitacion == habDisponibles[i].IdHabitacion)
                    {
                        bool borrado = habDisponibles.Remove(habDisponibles[i]);
                    }
                }  
            }
            ActualizaListaHabitaciones(0, habDisponibles);
            //Console.WriteLine("Tabla acutalizada");
            
           


        }

        //Lista de habitaciones
        private void ActualizaListaHabitaciones(int numRow, List<Habitacion> habitacionesDisponibles)
        {
            int numRecorridos = habitacionesDisponibles.Count;

            // Crea y actualiza filas
            for (int i = numRow; i < numRecorridos; ++i)
            {
                if (this.grdLista.Rows.Count <= i)
                {
                    this.grdLista.Rows.Add();
                }

                this.ActualizaFilaDeListaHabitaciones(i, habitacionesDisponibles);
            }

            // Eliminar filas sobrantes
            int numExtra = this.grdLista.Rows.Count - numRecorridos;
            for (; numExtra > 0; --numExtra)
            {
                this.grdLista.Rows.RemoveAt(numRecorridos);
            }

            return;
        }

        private void ActualizaFilaDeListaHabitaciones(int rowIndex, List<Habitacion> habitacionesDisponibles)
        {
            if (rowIndex < 0
              || rowIndex > this.grdLista.Rows.Count)
            {
                throw new System.ArgumentOutOfRangeException(
                            "fila fuera de rango: " + nameof(rowIndex));
            }

            DataGridViewRow row = this.grdLista.Rows[rowIndex];
            Habitacion habitacion = habitacionesDisponibles[rowIndex];

            // Assign data
            //row.Cells[0].Value = (rowIndex + 1).ToString().PadLeft(4, ' ');
            row.Cells[0].Value = habitacion.IdHabitacion;
            row.Cells[1].Value = habitacion.Tipo;

            // Assign tooltip text
            foreach (DataGridViewCell cell in row.Cells)
            {
                cell.ToolTipText = habitacion.ToString();
            }
            this.FilaSeleccionada();
            return;
        }

        private void FilaSeleccionada()
        {
            //DataGridViewRow fila = this.grdLista.CurrentRow;
            foreach (DataGridViewRow row in grdLista.SelectedRows)
            {
                if (row.Cells[0].Value != null)
                {
                    var hab = new Habitacion(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString());
                    this.habitacion = hab;

                    ActualizarTarifaHabitacion(row);

                }

                


            }
        }

        private bool validarFechas()
        {
            bool fechasValidas = true;
            int compararFechas = DateTime.Compare(this.FechaEntrada, this.FechaSalida);
            
            if (compararFechas > 0)
            {

                string message = "La fecha de entrada es mayor que la fecha de salida";
                string caption = "Error";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                result = MessageBox.Show(message, caption, buttons);

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    return false;
                }
            }

            return fechasValidas;
        }

        private void ActualizarTarifaGaraje()
        {
            if (this.edGaraje.Checked == true)
            {
                this.edTarifa.Value += 2;
            }
            else
            {
                this.edTarifa.Value -= 2;
            }
            this.calcularTotal();
        }

        private void ActualizarTarifaHabitacion(DataGridViewRow row)
        {
            var precioTarifaGaraje = 0;
            if (this.edGaraje.Checked == true)
            {
                precioTarifaGaraje = 2;
            }

            if (row.Cells[1].Value.ToString() == "matrimonio")
            {
                this.edTarifa.Value = 20 + precioTarifaGaraje;
            }
            else
            {
                this.edTarifa.Value = 10 + precioTarifaGaraje;
            }
            this.calcularTotal();
        }


        public void calcularTotal()
        {
            Console.WriteLine(calcularNumDias());
            double precio = calcularNumDias() * (double)this.edTarifa.Value;
            Console.WriteLine(precio);
            precio += (precio * 0.21);
            Console.WriteLine(precio);
            this.edTotal.Text = precio.ToString();
        }

        
        private double calcularNumDias()
        {

            return (this.FechaSalida - this.FechaEntrada).Days;


        }

        public void comprobarReserva()
        {
            if (validarFechas())
            {
                this.DialogResult = DialogResult.OK;
            }

        }



    }
}