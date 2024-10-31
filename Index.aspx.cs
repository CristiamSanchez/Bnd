using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;


using System.Configuration;
using System.Drawing;
using System.IO;
//using Oracle.ManagedDataAccess.Client;
//using OfficeOpenXml;
using System.Collections;
using System.Net;
//using Devart.Common;
using static System.Net.WebRequestMethods;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace SistFactAdmin
{
    public partial class Index : System.Web.UI.Page
    {
        private static bool eventoEjecutado = false;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void ManejarTiempo(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Thread.Sleep(1000); // Simulación de trabajo
            }
        }

        protected bool ValidaC()
        {
            bool valido = true;
            try
            {
                using (SqlConnection conexion =  Forms.CNN.GetConnection())
                {
                    conexion.Open();
                    using (SqlCommand comando = new SqlCommand("[sfi].SFVALIDAC", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Add("@p_Contraseña", SqlDbType.VarChar, 20).Value = contras.Text.Trim();
                        comando.Parameters.Add("@p_Respuesta", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                        comando.ExecuteNonQuery();
                        string resultado = comando.Parameters["@p_Respuesta"].Value.ToString();
                        bool contieneLetra = resultado.Contains("OK") || resultado.Contains("SI");
                        if (contieneLetra)
                        { valido = false; }
                        else
                        { valido = true; }
                    }
                    conexion.Close();
                }
            }
            catch (Exception ex)
            {
              Console.WriteLine("Error: " + ex.Message);
            }
            return valido;
        }

        protected void Btn_Ingresar(object sender, EventArgs e)
        {
            string esUsuarioAutenticado = "";
            System.Threading.Thread.Sleep(3000);
            try
            {
                if (usuarioV.Text.Length > 4 && contras.Text.Length > 4)
                {
                    if (ValidaC())
                    {
                        using (SqlConnection conexion = Forms.CNN.GetConnection())
                        {
                            conexion.Open();
                            using (SqlCommand comando = new SqlCommand("[sfi].SFAUTENTICAU", conexion))
                            {
                                comando.CommandType = CommandType.StoredProcedure;
                                comando.Parameters.Add("@p_NombreUsuario", SqlDbType.VarChar, 15).Value = usuarioV.Text.Trim();
                                comando.Parameters.Add("@p_Contraseña", SqlDbType.VarChar, 20).Value = contras.Text.Trim();
                                comando.Parameters.Add("@p_Respuesta", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                                comando.ExecuteNonQuery();

                                string resultado = comando.Parameters["@p_Respuesta"].Value.ToString();
                                bool contieneLetra = resultado.Contains("U") || resultado.Contains("P") || resultado.Contains("A");

                                if (contieneLetra)
                                {
                                    esUsuarioAutenticado = Convert.ToString(comando.Parameters["@p_Respuesta"].Value.ToString());
                                }
                                else
                                {
                                    ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", "swal('Warning!','Usuario o Contraseña Incorrecta','warning')", true);
                                }
                            }
                            conexion.Close();
                        }

                        if (!string.IsNullOrEmpty(esUsuarioAutenticado))
                        {
                            // Iniciar el hilo con el token de cancelación
                            Thread tiempoThread = new Thread(() => ManejarTiempo(_cts.Token));
                            tiempoThread.Start();

                            // Autenticado correctamente
                            Session["NombUsu"] = usuarioV.Text.Trim();
                            Session["Tipo"] = esUsuarioAutenticado;
                            Session.Timeout = 30;
                            eventoEjecutado = true; // Marcar que el evento se ha ejecutado

                            // Redirigir al usuario
                            Response.Redirect(DevuelveCadena() + ".aspx", false);
                            Context.ApplicationInstance.CompleteRequest(); // Completa la solicitud sin abortar el hilo

                            _cts.Cancel();
                        }
                        else
                        {
                            // No autenticado correctamente
                            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", "swal('Warning!','Nombre de usuario o contraseña incorrectos!','warning')", true);
                        }
                    }
                    else
                    {
                        CompruebaActC.Visible = true;
                        loginU.Visible = false;
                    }
                }
                else
                {
                    //Page.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire({ icon: 'error', title: 'Error', text: 'Llenar todos los campos!' });", true);
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", "swal('Warning!','Llenar todos los campos','warning')", true);
                }
                limpiaCarpeta();
            }
            catch (Exception ex)
            {
                 Console.WriteLine("Error: " + ex.Message);
            }

        }

        private void limpiaCarpeta()
        {
            try
            {
                DirectoryInfo tempDir = new DirectoryInfo(base.Server.MapPath("~/ArchivosPDF/"));
                if (!tempDir.Exists)
                {
                    return;
                }
                DateTime currentDate = DateTime.Now;
                FileInfo[] files = tempDir.GetFiles();
                foreach (FileInfo file in files)
                {
                    if (currentDate.Subtract(file.CreationTime).Days > 1)
                    {
                        file.Delete();
                    }
                }
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message.ToString()); }
        }

        protected void btnMostrar_Click(object sender, EventArgs e)
        {
            loginU.Visible = false;
            recuperaContraseña.Visible = true;
        }

        protected void btnCambiarC_Click(object sender, EventArgs e)
        {
            try
            {
                if (correoB.Value.Length > 8)
                {
                    using (SqlConnection conexion = Forms.CNN.GetConnection())
                    {
                        conexion.Open();
                        using (SqlCommand comando = new SqlCommand("[sfi].[SFRECUPERAC]", conexion))
                        {
                            comando.CommandType = CommandType.StoredProcedure;
                            comando.Parameters.Add("@p_correoB", SqlDbType.VarChar, 30).Value = correoB.Value.Trim();
                            comando.Parameters.Add("@p_Respuesta", SqlDbType.Int, 100).Direction = ParameterDirection.Output;
                            comando.Parameters.Add("@p_Respuesta2", SqlDbType.VarChar, 130).Direction = ParameterDirection.Output;
                            comando.Parameters.Add("@p_Respuesta3", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                            comando.ExecuteNonQuery();
                            int resultado = Convert.ToInt32(comando.Parameters["@p_Respuesta"].Value);
                            string resultado2 = Convert.ToString(comando.Parameters["@p_Respuesta2"].Value);
                            string resultado3 = Convert.ToString(comando.Parameters["@p_Respuesta3"].Value);
                            if (resultado > 0)
                            {
                                Task.Run(() => EnviarCorreo(resultado2, resultado3));

                                //EnviarCorreo(resultado2, resultado3);
                                ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", "swal('Éxito!','Cambio de contraseña exitoso, revisar respuesta en correo!','success')", true);
                            }
                            else
                            {
                                ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", "swal('Advertencia!','Correo incorrecto','warning')", true);
                            }
                            loginU.Visible = true;
                            recuperaContraseña.Visible = false;
                            CompruebaActC.Visible = false;
                        }
                        conexion.Close();
                    }
                }
                else
                {
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", "swal('Advertencia!','Llenar todos los campos','warning')", true);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            loginU.Visible = true;
            recuperaContraseña.Visible = false;
        }

        public void EnviarCorreo(string nombre, string contraseñaN)
        {
            try
            {
                var message = new MailMessage();
                message.From = new MailAddress("csanchez@banadesa.hn", "Soporte TI");
                message.To.Add(new MailAddress(correoB.Value));
                message.Subject = "Recuperacion de Usuario Sistema de Facturacion";

                string body = "<body>" +
                    "<h1 style='color:White; background:green; text-align: center; font-family: cursive;'> Sistema de Entrada OP </h1>" +
                    "<h3 style='text-align: center; font-family: cursive;'> Estimado <b>" + nombre + "</b> </h3>" +
                    "<span> Se ha cambiado su Contraseña a: <b>" + contraseñaN + ".</b> </span>" +
                    "<span> Cualquier duda o consulta, no olvides ponerte en contacto con el departamento de Tecnología </span></br>" +
                    "</br></br><span> Saludos Cordiales </span>" +
                    "</body>";

                message.IsBodyHtml = true;
                message.Body = body;

                using (var client = new SmtpClient("correo.banadesa.hn", 587))
                {
                    client.Credentials = new NetworkCredential("csanchez@banadesa.hn", "Hinata*2023");
                    client.EnableSsl = true;
                    client.Send(message);
                }
            }
            catch (Exception ex)
            {
              Console.WriteLine("Error al enviar el correo electrónico: " + ex.Message);
            }
        }

        protected void btnActualizaC_Click(object sender, EventArgs e)
        {
            // Obtener los valores de los TextBoxes
            string contraseña1 = contra1.Value.Trim(); // Eliminar espacios al principio y al final
            string contraseña2 = contra2.Value.Trim(); // Eliminar espacios al principio y al final

            // Verificar si las contraseñas son iguales
            if (contraseña1 == contraseña2)
            {
                try
                {
                    using (SqlConnection conexion = Forms.CNN.GetConnection())
                    {
                        conexion.Open();
                        using (SqlCommand comando = new SqlCommand("[sfi].SFCAMBIOC", conexion))
                        {
                            comando.CommandType = CommandType.StoredProcedure;
                            comando.Parameters.Add("@p_cont", SqlDbType.VarChar, 20).Value = contraseña2;
                            comando.Parameters.Add("@p_usu", SqlDbType.VarChar, 15).Value = usuarioV.Text.Trim();
                            comando.ExecuteNonQuery();
                            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", "swal('Éxito!','Cambio de contraseña exitoso!','success')", true);
                        }
                        conexion.Close();
                    }
                    loginU.Visible = true;
                    recuperaContraseña.Visible = false;
                    CompruebaActC.Visible = false;
                }
                catch (Exception ex)
                {
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", "swal('Advertencia!','Error al cambiar contraseña: " + ex.Message + "','warning')", true);
                }
            }
            else
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", "swal('Advertencia!','Contraseñas no coinciden','warning')", true);
            }
        }

        protected void btnCancelarA_Click(object sender, EventArgs e)
        {
            loginU.Visible = true;
            recuperaContraseña.Visible = false;
            CompruebaActC.Visible = false;
        }

        protected string DevuelveCadena()
        {
            string cadena = "Model/InclusionFinanciera";
            return cadena;
        }
    }
}