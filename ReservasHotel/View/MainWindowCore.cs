﻿namespace ReservasHotel.View
{
	using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using Habitaciones.Core;
    using Habitaciones.UI;
    using Habitaciones.XML;
    using Gestión_Hotel.Core;

 
    public partial class MainWindow
    {
        public MainWindow()
        {
            this.Build();
			
            this.reservas = RegistroReservas.RecuperarXml("registro_reservas.xml");
            this.habitaciones = RegistroHabitaciones.RecuperaXml();
            //this.habitaciones = this.crearHabitaciones();
            this.ActualizaListaReservas(0);

            //Integracion CLIENTES
            this.ClienteView = new Gestión_Hotel.UI.MainWindowViewClientes();
            this.pnlClientes = this.ClienteView.BuildGUI();
            this.ClienteCore = new Gestión_Hotel.UI.MainWindowCore(this.ClienteView);

            //Integracion HABITACIONES
            this.HabitacionView = new Habitaciones.UI.MainWindowView();
            this.pnlHabitaciones = this.HabitacionView.BuildGUI();
            this.HabitacionCore = new Habitaciones.UI.MainWindowCore(this.HabitacionView);

            //Consultar clientes
            this.OpConsultarClientes.Click += (sender, e) => this.mostrarClientes();
            //Insertar clientes
            this.OpInsertarCliente.Click += (sender, e) => this.insertarCliente();

            //Consultar habitaciones
            this.OpConsultarHabitaciones.Click += (sender, e) => this.mostrarHabitaciones();
            //Insertar habitacion
            this.OpInsertarHabitacion.Click += (sender, e) => this.insertarHabitacion();

            this.OpAltaReserva.Click += (sender, e) => this.AltaReserva();
            this.FormClosed += (sender, e) => this.OnQuit();

        }

        private void mostrarClientes()
        {
            this.pnlPpal.Controls.Clear();
            this.pnlPpal.Controls.Add(this.pnlClientes);
            this.ClienteView.ResizeWindow();
        }

        private void insertarCliente()
        {
            this.ClienteCore.InsertarCliente();
        }

        private void mostrarHabitaciones()
        {
            this.pnlPpal.Controls.Clear();
            this.pnlPpal.Controls.Add(this.pnlHabitaciones);
            this.HabitacionView.ResizeWindow();
        }

        private void insertarHabitacion()
        {

            this.HabitacionCore.BuildHabitacion();
        }



		private void AltaReserva()
        {
            var habitaciones = RegistroHabitaciones.RecuperaXml();

            var dlgAltaReserva = new DlgAltaReserva(habitaciones, null);


            if (dlgAltaReserva.ShowDialog() == DialogResult.OK)
            {
               
                var reserva = new Reserva(dlgAltaReserva.habitacion, new ReservasHotel.Cliente(dlgAltaReserva.Nombre, dlgAltaReserva.Apellidos),
                    dlgAltaReserva.FechaEntrada, dlgAltaReserva.FechaSalida, dlgAltaReserva.UsaGaraje, dlgAltaReserva.Tarifa);
                this.reservas.Add(reserva);
                Console.WriteLine(reserva);
                this.ActualizaListaReservas(0);

            }
           

            return;
        }

        private void ActualizaListaReservas(int numRow)
        {
            int numRecorridos = this.reservas.Count;

            // Crea y actualiza filas
            for (int i = numRow; i < numRecorridos; ++i)
            {
                if (this.grdLista.Rows.Count <= i)
                {
                    this.grdLista.Rows.Add();
                }

                this.ActualizaFilaDeListaReservas(i);
            }

            // Eliminar filas sobrantes
            int numExtra = this.grdLista.Rows.Count - numRecorridos;
            for (; numExtra > 0; --numExtra)
            {
                this.grdLista.Rows.RemoveAt(numRecorridos);
            }

            return;
        }

        private void ActualizaFilaDeListaReservas(int rowIndex)
        {
            if (rowIndex < 0
              || rowIndex > this.grdLista.Rows.Count)
            {
                throw new System.ArgumentOutOfRangeException(
                            "fila fuera de rango: " + nameof(rowIndex));
            }

            DataGridViewRow row = this.grdLista.Rows[rowIndex];
            Reserva reserva = this.reservas[rowIndex];

            // Assign data
            row.Cells[0].Value = (rowIndex + 1).ToString().PadLeft(4, ' ');
            row.Cells[1].Value = reserva.IdReserva;
            row.Cells[2].Value = reserva.FechaEntrada;
            row.Cells[3].Value = reserva.FechaSalida;
            row.Cells[4].Value = reserva.calcularTotal();
            row.Cells[5].Value = reserva.Cliente.Nombre + " " + reserva.Cliente.Apellidos;


            // Assign tooltip text
            foreach (DataGridViewCell cell in row.Cells)
            {
                cell.ToolTipText = reserva.ToString();
            }

            return;
        }

        private void FilaSeleccionada()
        {
            int fila = System.Math.Max(0, this.grdLista.CurrentRow.Index);

            if (this.reservas.Count > fila)
            {
                this.edDetalle.Text = this.reservas[fila].ToString();
                this.edDetalle.SelectionStart = this.edDetalle.Text.Length;
                this.edDetalle.SelectionLength = 0;
            }
            else
            {
                this.edDetalle.Clear();
            }

            return;
        }



        public void modificarReserva()
        {
            DataGridViewRow fila = this.grdLista.CurrentRow;
            
                
                if (fila  != null)
                {

                    var reserva = this.reservas.getReserva(fila.Cells[1].Value.ToString());

                    if (reserva != null)
                    {
                        Console.WriteLine(reserva.Cliente.Apellidos);
                        var dlgAltaReserva = new DlgAltaReserva(habitaciones, reserva);
                        if (dlgAltaReserva.ShowDialog() == DialogResult.OK)
                        {
                            this.reservas.Remove(reserva);
                            var nuevaReserva = new Reserva(dlgAltaReserva.habitacion, new ReservasHotel.Cliente(dlgAltaReserva.Nombre, dlgAltaReserva.Apellidos),
                                dlgAltaReserva.FechaEntrada, dlgAltaReserva.FechaSalida, dlgAltaReserva.UsaGaraje, dlgAltaReserva.Tarifa);
                            this.reservas.Add(nuevaReserva);

                            actualizarReservas();

                        }

                    }


                }

            return;


        }

        public void actualizarReservas()
        {
            RegistroReservas.GuardarXml("registro_reservas.xml", this.reservas);
            this.reservas = RegistroReservas.RecuperarXml("registro_reservas.xml");
            this.ActualizaListaReservas(0);

        }

        public void eliminarReserva()
        {
            foreach (DataGridViewRow row in grdLista.SelectedRows)
            {
                if (row.Cells[0].Value != null)
                {
                    
                    Console.WriteLine(row.Cells[1].Value.ToString());
                    var reserva = this.reservas.getReserva(row.Cells[1].Value.ToString());
                    if (reserva != null)
                    {
                        this.reservas.Remove(reserva);
                    
                        actualizarReservas();

                    }
                    
                    
                }


            }
        }

        private void generarFactura()
        {
            foreach (DataGridViewRow row in grdLista.SelectedRows)
            {
                if (row.Cells[0].Value != null)
                {

                    var reserva = this.reservas.getReserva(row.Cells[1].Value.ToString());
                    if (reserva != null)
                    {

                        string message = "";
                        string caption = "";
                        // WriteAllText creates a file, writes the specified string to the file,
                        // and then closes the file.    You do NOT need to call Flush() or Close().
                        try
                        {
                            string path = Path.GetDirectoryName(Application.ExecutablePath) + "\\" + reserva.IdReserva + ".txt";
                            
                            File.WriteAllText(@path, reserva.GenerarFactura());
                            message = "Factura creada correctamente";
                            caption = "Exito";
                        }
                        catch (Exception ex)
                        {
                            message = "Ha ocurrido un problema al generar la factura";
                            caption = "Error";
                        }
                        finally
                        {
                               
                            MessageBoxButtons buttons = MessageBoxButtons.OK;
                            DialogResult result;

                            result = MessageBox.Show(message, caption, buttons);

                            if (result == System.Windows.Forms.DialogResult.OK)
                            {
                                //return false;
                            }
                        }

                       
                    }


                }


            }
           

       
        
        }



        private void OnQuit()
        {
            RegistroReservas.GuardarXml("registro_reservas.xml", this.reservas);
            this.HabitacionCore.Registro.GuardaXml();
            Application.Exit();
        }

        private void Salir()
        {

            RegistroReservas.GuardarXml("registro_reservas.xml", this.reservas);
            this.HabitacionCore.Registro.GuardaXml();
            Application.Exit();
        }


        public DlgAltaReserva DlgAltaReserva
        {
            get; private set;
        }

        public Habitaciones.UI.MainWindowView HabitacionView
        {
            get; private set;
        }

        public Habitaciones.UI.MainWindowCore HabitacionCore
        {
            get; private set;
        }

        public Gestión_Hotel.UI.MainWindowViewClientes ClienteView
        {
            get; private set;
        }

        public Gestión_Hotel.UI.MainWindowCore ClienteCore
        {
            get; private set;
        }

        private RegistroReservas reservas;
        private RegistroHabitaciones habitaciones;

    }
}
