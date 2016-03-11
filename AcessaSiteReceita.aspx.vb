Imports System.IO
Imports System.Net
Imports System.Web
Imports mshtml
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports PesquisaReceita

Partial Class AcessaSiteReceita
    Inherits System.Web.UI.Page

    Private _pesquisaCNPJ As New PesquisaCNPJ


    Public Sub AtualizaCaptcha()
        Try
            Dim imageData As String = _pesquisaCNPJ.GetCaptcha()
            ImageCaptcha.ImageUrl = imageData
            'É necessário salvar o cookie da conexão que gera o captcha para que seja usado na requisição dos dados
            Session("ReceitaCookie") = _pesquisaCNPJ.CookieReceita
            swich_PesquisaVisible(True)

        Catch ex As Exception
            swich_PesquisaVisible(False)
            Throw New Exception("Não foi possivel estabelecer comunicação com os servidores da receita federal. Tente novamente mais tarde.", ex)

        End Try

    End Sub

    Private Sub swich_PesquisaVisible(v As Boolean)
        PanelPesquisaCNPJReceita.Visible = v
        ibCNPJ.Visible = Not v
    End Sub

    Protected Sub ibGetDadosReceita_Click(sender As Object, e As ImageClickEventArgs)
        Try
            Dim CNPJ As String = TextBoxCNPJ.Text
            Dim Captcha As String = TextBoxCaptcha.Text
            Dim dados As String = _pesquisaCNPJ.Consulta(CNPJ, Captcha, Session("ReceitaCookie"))
            PreencherDadosReceita(dados)

            TextBoxCaptcha.Text = ""


        Catch ex As Exception
            'mensagem de erro
            AtualizaCaptcha()
        End Try

    End Sub

    'Preence os dados recuperados do site da receita nos respectivos textboxes
    Private Sub PreencherDadosReceita(ByVal dados As String)

        For Each col As PesquisaCNPJ.Coluna In [Enum].GetValues(GetType(PesquisaCNPJ.Coluna)).Cast(Of PesquisaCNPJ.Coluna)()
            TextBoxDados.Text &= [Enum].GetName(GetType(PesquisaCNPJ.Coluna), col) & ": " & _pesquisaCNPJ.RecuperaColunaValor(dados, col) & Environment.NewLine
        Next

    End Sub

End Class




      