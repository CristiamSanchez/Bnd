<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="SistFactAdmin.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Sistema Facturacion</title>
     <!-- Bootstrap CSS -->
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <link href="Styles/sweetalert.css" rel="stylesheet" />
    <script src="Scripts/sweetalert.min.js"></script>
       <!-- Favicon -->

      <style>
        #cuerpo{            
                background-image: linear-gradient(
                to bottom,
                rgba(0, 255, 0, 0.5),
                rgba(0, 0, 255, 0.5)
              ),url('../Img/bnd1.png');
                background-size: cover; /* La imagen cubrirá toda la pantalla */
                background-position: center; /* La imagen se centrará */
                background-attachment: fixed; /* La imagen se mantendrá fija */           
        }
    </style>
</head>
<body id="cuerpo" class="bg-light">
    <form id="form1" runat="server">
        <div class="container">
            <div class="row">
                <div class="col-sm-9 col-md-7 col-lg-5 mx-auto">
                    <div class="card border-0 bg-success shadow rounded-3 my-5">
                        <div id="loginU" runat="server" class="card-body p-4 p-sm-5">
                            
                            <div class="col-md-6 col-lg-12 d-flex justify-content-center align-items-center">
                            <img src="../Img/logo.png" alt="login form" id="imgLogo" class="img-fluid m-2" style="border-radius: 1rem 0 0 1rem; max-width: 30%;" />
                            <h1 class="text-center mb-5 fw-bold text-white">Pantalla Ingreso</h1>
                            </div>

                            <div class="form-floating mb-3">
                                <asp:TextBox ID="usuarioV" runat="server"  min="3" max="15" CssClass="form-control" placeholder="Usuario"></asp:TextBox>
                                <label for="usuarioV">Usuario</label>
                            </div>
                            <div class="form-floating mb-3">
                                <asp:TextBox ID="contras" runat="server"  min="4" max="20" TextMode="Password" CssClass="form-control" placeholder="Contraseña"></asp:TextBox>
                                <label for="contras">Contraseña</label>
                            </div>
                            <div class="d-grid">
                                <asp:Button ID="login" runat="server" CssClass="btn btn-warning btn-login text-uppercase fw-bold text-white" Text="Ingresar" OnClick="Btn_Ingresar" />
                            </div>
                            
                            <div class="pt-1 mb-4">
                                 <asp:LinkButton ID="btnMostrar" runat="server" OnClick="btnMostrar_Click" CssClass="badge rounded-pill m-1 text-white">No recuerda contraseña?</asp:LinkButton>
                            </div>

                        </div>

                            <!-- Recuperar contraseña -->
                            <div id="recuperaContraseña" runat="server" visible="false" class="card-body p-4 p-sm-5">
                                <h5 class="fw-bold mb-2 pb-3 text-white text-center" style="letter-spacing: 1px;">Recuperar contraseña</h5>
                                <div class="form-outline mb-3">
                                  <input type="text" id="correoB" class="form-control form-control-lg" min="4" max="30" onkeypress="return evitarEspacios2(event)" runat="server" />
                                  <label class="form-label text-white" for="form2Example27">Ingrese su correo:</label>
                                </div>

                                <div class="pt-1 mb-4">
                                  <asp:Button class="btn btn-warning btn-lg btn-block text-white m-1" id="btnCambiarC" runat="server" OnClick="btnCambiarC_Click" Text="Cambiar Contraseña" />
                                  <asp:Button class="btn btn-danger btn-lg btn-block text-white m-1" id="btnCancelar" runat="server" OnClick="btnCancelar_Click" Text="Cancelar" />
                                </div>
                            </div>

                             <!-- Cambiar contraseña -->
                            <div id="CompruebaActC" runat="server" visible="false" class="card-body p-4 p-sm-5">
                                <h5 class="fw-normal mb-3 pb-3 text-white" style="letter-spacing: 1px;">Cambiar contraseña</h5>
                                <div class="form-outline mb-4">
                                  <input type="text" id="contra1" class="form-control form-control-lg" min="4" max="20" onkeypress="return evitarEspacios2(event)" runat="server" />
                                  <label class="form-label text-white" for="form2Example27">Ingrese su contraseña:</label>
                                </div>
                                <div class="form-outline mb-4">
                                  <input type="text" id="contra2" class="form-control form-control-lg" min="4" max="20" onkeypress="return evitarEspacios2(event)" runat="server" />
                                  <label class="form-label text-white" for="form2Example27">Confirme su contraseña:</label>
                                </div>

                                <div class="flex-column gap-3 flex-wrap align-content-center justify-content-center">
                                  <asp:Button class="btn btn-warning btn-lg btn-block text-white  m-1" id="btnActualizaC" runat="server" OnClick="btnActualizaC_Click" Text="Cambiar Contraseña" />
                                  <asp:Button class="btn btn-danger btn-lg btn-block text-white m-1" id="btnCancelarA" runat="server" OnClick="btnCancelarA_Click" Text="Cancelar" />
                                </div>
                            </div>

                    </div>
                </div>
            </div>
        </div>
    </form>
    <script>
    function evitarEspacios(e) {
        if (e.which === 32) {
            e.preventDefault();
            return false;
        }
    }
    function evitarEspacios2(e) {
        if (e.which === 32) {
            e.preventDefault();
            return false;
        }
    }
 
   /* window.onbeforeunload = DesactivarBoton;*/
    </script>
</body>
</html>
