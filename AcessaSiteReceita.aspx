<%@ Page Language="VB" AutoEventWireup="false" Async="true" CodeFile="AcessaSiteReceita.aspx.vb" Inherits="AcessaSiteReceita" %>

<%@ Import Namespace="System.Threading" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>CNPJ - Receita Federal</title>
    <script src="https://code.jquery.com/jquery-2.2.1.min.js"></script>
    <script src="scripts/ValidateCNPJ.js"></script>
    <script src="scripts/jquery.maskedinput.js"></script>
    <script src="scripts/main.js"></script>
    <link href="styles/main.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <section>
            <asp:ScriptManager runat="server" ID="ScriptManadiver">
            </asp:ScriptManager>
            <div id="CNPJ">
            <asp:Label ID="lblTipoDocCadastro" runat="server" Text="CNPJ:"></asp:Label>
            <br />
            <asp:TextBox ID="TextBoxCNPJ" runat="server" Width="140px" MaxLength="18" class="cnpj" invalidMessage="Este CNPJ não é válido."></asp:TextBox>
            <asp:ImageButton style="float:right;" ID="ibCNPJ" OnClick="AtualizaCaptcha" runat="server" CausesValidation="False" ImageAlign="TextTop" ImageUrl="~/imagens/cnpj.png" ToolTip="Pesquisar dados da empresa na Receita Federal" />
            </div>
            <asp:Panel ID="PanelPesquisaCNPJReceita" runat="server" Visible="false">

                <div id="Captcha">

                    <asp:Image CssClass="imgCaptcha" ID="ImageCaptcha" runat="server" Height="50px" Width="180px" />
                    <br />
                    <asp:TextBox CssClass="textCaptcha" ID="TextBoxCaptcha" runat="server" MaxLength="15" Width="100%"></asp:TextBox>
                    <div id="getCNPJCommands">
                        <asp:ImageButton CssClass="ibRefreshCaptcha" ID="ibRefreshCaptcha" OnClick="AtualizaCaptcha" runat="server" CausesValidation="False" ImageAlign="TextTop" ImageUrl="~/imagens/refresh.png" ToolTip="Atualizar Captcha" />

                        <asp:ImageButton CssClass="ibGetDadosReceita" ID="ibGetDadosReceita" OnClick="ibGetDadosReceita_Click" runat="server" CausesValidation="False" ImageAlign="TextTop" ImageUrl="~/imagens/Extract object Mini.png" ToolTip="Pesquisar" />

                    </div>
                </div>
            </asp:Panel>

        </section>
        <br />

        <asp:TextBox ID="TextBoxDados" runat="server" Height="348px" TextMode="MultiLine" Width="455px"></asp:TextBox>
    </form>
</body>
</html>
