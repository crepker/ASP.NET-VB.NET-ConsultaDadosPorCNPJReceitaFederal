Imports Microsoft.VisualBasic
Imports System.Net
Imports System.Drawing
Imports System.IO
Imports System.Threading
Imports System.Threading.Tasks
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Web


Namespace PesquisaReceita

    Public Class PesquisaCNPJ
        Private cookiecontainer As New CookieContainer()
        Private UrlDominio As String = "http://www.receita.fazenda.gov.br"
        Private UrlBase As String = "http://www.receita.fazenda.gov.br/pessoajuridica/cnpj/cnpjreva/"
        Private UrlGet As String = "cnpjreva_solicitacao2.asp"
        Private UrlPost As String = "Valida.asp"
        Private UrlImagemCaptcha As String = "captcha/gerarCaptcha.asp"
        Private RetornoHtml As String = ""
        Private _erro As String = ""
        Private Myproxy As New WebProxy
        Public Property CookieReceita As CookieContainer
            Set(value As CookieContainer)
                cookiecontainer = value
            End Set
            Get
                Return cookiecontainer
            End Get
        End Property

        Event GetCaptchaComplete()
        Event GetCaptchaFailed()
        Public Function GetCaptcha() As String

            Dim HtmlResponse As String = New CookieAwareWebClient(cookiecontainer).DownloadString(New Uri(UrlBase & UrlGet))

            Dim ByteCaptcha As Byte() = New CookieAwareWebClient(cookiecontainer).DownloadData(New Uri(UrlBase + UrlImagemCaptcha))

            If ByteCaptcha IsNot Nothing Then
                Return "data:image/jpeg;base64," + Convert.ToBase64String(ByteCaptcha, 0, ByteCaptcha.Length)
                RaiseEvent GetCaptchaComplete()
            Else
                Throw New Exception("Não foi possível recuperar a imagem de validação do site da Receita Federal")
                RaiseEvent GetCaptchaFailed()
            End If

        End Function

        Public Function Consulta(ByVal cnpj As String, ByVal captcha As String, ByVal cc As CookieContainer) As String
            CookieReceita = cc
            Dim request = TryCast(WebRequest.Create(UrlBase + UrlPost), HttpWebRequest)
            request.ProtocolVersion = HttpVersion.Version10
            request.CookieContainer = cookiecontainer
            request.Method = "POST"

            Dim postData As String = ""
            postData = postData & "origem=comprovante&"
            postData = postData & "cnpj=" + New Regex("[^\d]").Replace(cnpj, String.Empty) + "&"
            postData = postData & "txtTexto_captcha_serpro_gov_br=" + captcha + "&"
            postData = postData & "submit1=Consultar&"
            postData = postData & "search_type=cnpj"

            Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
            request.ContentType = "application/x-www-form-urlencoded"
            request.ContentLength = byteArray.Length

            Dim dataStream As Stream = request.GetRequestStream()
            dataStream.Write(byteArray, 0, byteArray.Length)
            dataStream.Close()

            Dim response As WebResponse = request.GetResponse()
            Dim stHtml As StreamReader = New StreamReader(response.GetResponseStream(), Encoding.GetEncoding("ISO-8859-1"))
            Dim retorno As String = stHtml.ReadToEnd()

            If retorno.Contains("Verifique se o mesmo foi digitado corretamente") Then
                Throw New System.InvalidOperationException("O número do CNPJ não foi digitado corretamente")
            ElseIf (retorno.Contains("Erro na Consulta")) Then
                Throw New System.InvalidOperationException("Os caracteres digitados não correspondem com a imagem")
            End If

            Return retorno

        End Function

        ''Metodo de leitura dos dados da pesquisa por CNPJ na receita federal
        Public Function RecuperaColunaValor(ByVal pattern As String, ByVal col As Coluna) As String
            Try


                Dim S As String = pattern.Replace("\n", "").Replace("\t", "").Replace("\r", "")

                Select Case col
                    Case Coluna.RazaoSocial

                        S = StringEntreString(S, "<!-- Início Linha NOME EMPRESARIAL -->", "<!-- Fim Linha NOME EMPRESARIAL -->")
                        S = StringEntreString(S, "<tr>", "</tr>")
                        S = StringEntreString(S, "<b>", "</b>")
                        Return S.Trim()

                    Case Coluna.NomeFantasia

                        S = StringEntreString(S, "<!-- Início Linha ESTABELECIMENTO -->", "<!-- Fim Linha ESTABELECIMENTO -->")
                        S = StringEntreString(S, "<tr>", "</tr>")
                        S = StringEntreString(S, "<b>", "</b>")
                        Return S.Trim()

                    Case Coluna.NaturezaJuridica

                        S = StringEntreString(S, "<!-- Início Linha NATUREZA JURÍDICA -->", "<!-- Fim Linha NATUREZA JURÍDICA -->")
                        S = StringEntreString(S, "<tr>", "</tr>")
                        S = StringEntreString(S, "<b>", "</b>")
                        Return S.Trim()

                    Case Coluna.AtividadeEconomicaPrimaria

                        S = StringEntreString(S, "<!-- Início Linha ATIVIDADE ECONOMICA -->", "<!-- Fim Linha ATIVIDADE ECONOMICA -->")
                        S = StringEntreString(S, "<tr>", "</tr>")
                        S = StringEntreString(S, "<b>", "</b>")
                        Return S.Trim()

                    Case Coluna.AtividadeEconomicaSecundaria

                        S = StringEntreString(S, "<!-- Início Linha ATIVIDADE ECONOMICA SECUNDARIA-->", "<!-- Fim Linha ATIVIDADE ECONOMICA SECUNDARIA -->")
                        S = StringEntreString(S, "<tr>", "</tr>")
                        S = StringEntreString(S, "<b>", "</b>")
                        Return S.Trim()

                    Case Coluna.NumeroDaInscricao

                        S = StringEntreString(S, "<!-- Início Linha NÚMERO DE INSCRIÇÃO -->", "<!-- Fim Linha NÚMERO DE INSCRIÇÃO -->")
                        S = StringEntreString(S, "<tr>", "</tr>")
                        S = StringEntreString(S, "<b>", "</b>")
                        Return S.Trim()

                    Case Coluna.MatrizFilial

                        S = StringEntreString(S, "<!-- Início Linha NÚMERO DE INSCRIÇÃO -->", "<!-- Fim Linha NÚMERO DE INSCRIÇÃO -->")
                        S = StringEntreString(S, "<tr>", "</tr>")
                        S = StringSaltaString(S, "</b>")
                        S = StringEntreString(S, "<b>", "</b>")
                        Return S.Trim()

                    Case Coluna.EnderecoLogradouro

                        S = StringEntreString(S, "<!-- Início Linha LOGRADOURO -->", "<!-- Fim Linha LOGRADOURO -->")
                        S = StringEntreString(S, "<tr>", "</tr>")
                        S = StringEntreString(S, "<b>", "</b>")
                        Return S.Trim()

                    Case Coluna.EnderecoNumero

                        S = StringEntreString(S, "<!-- Início Linha LOGRADOURO -->", "<!-- Fim Linha LOGRADOURO -->")
                        S = StringEntreString(S, "<tr>", "</tr>")
                        S = StringSaltaString(S, "</b>")
                        S = StringEntreString(S, "<b>", "</b>")
                        Return S.Trim()

                    Case Coluna.EnderecoComplemento

                        S = StringEntreString(S, "<!-- Início Linha LOGRADOURO -->", "<!-- Fim Linha LOGRADOURO -->")
                        S = StringEntreString(S, "<tr>", "</tr>")
                        S = StringSaltaString(S, "</b>")
                        S = StringSaltaString(S, "</b>")
                        S = StringEntreString(S, "<b>", "</b>")
                        Return S.Trim()

                    Case Coluna.EnderecoCEP

                        S = StringEntreString(S, "<!-- Início Linha CEP -->", "<!-- Fim Linha CEP -->")
                        S = StringEntreString(S, "<tr>", "</tr>")
                        S = StringEntreString(S, "<b>", "</b>")
                        Return S.Trim()

                    Case Coluna.EnderecoBairro

                        S = StringEntreString(S, "<!-- Início Linha CEP -->", "<!-- Fim Linha CEP -->")
                        S = StringEntreString(S, "<tr>", "</tr>")
                        S = StringSaltaString(S, "</b>")
                        S = StringEntreString(S, "<b>", "</b>")
                        Return S.Trim()

                    Case Coluna.EnderecoCidade

                        S = StringEntreString(S, "<!-- Início Linha CEP -->", "<!-- Fim Linha CEP -->")
                        S = StringEntreString(S, "<tr>", "</tr>")
                        S = StringSaltaString(S, "</b>")
                        S = StringSaltaString(S, "</b>")
                        S = StringEntreString(S, "<b>", "</b>")
                        Return S.Trim()

                    Case Coluna.EnderecoEstado

                        S = StringEntreString(S, "<!-- Início Linha CEP -->", "<!-- Fim Linha CEP -->")
                        S = StringEntreString(S, "<tr>", "</tr>")
                        S = StringSaltaString(S, "</b>")
                        S = StringSaltaString(S, "</b>")
                        S = StringSaltaString(S, "</b>")
                        S = StringEntreString(S, "<b>", "</b>")
                        Return S.Trim()

                    Case Coluna.ContatoEmail

                        S = StringEntreString(S, "<!-- Início de Linha de Contato -->", "<!-- Fim de Linha de Contato -->")
                        S = StringEntreString(S, "<tr>", "</tr>")
                        S = StringEntreString(S, "<b>", "</b>")
                        Return S.Trim()

                    Case Coluna.ContatoTelefone

                        S = StringEntreString(S, "<!-- Início de Linha de Contato -->", "<!-- Fim de Linha de Contato -->")
                        S = StringEntreString(S, "<tr>", "</tr>")
                        S = StringSaltaString(S, "</b>")
                        S = StringSaltaString(S, "</font>")
                        S = StringSaltaString(S, "</font>")
                        S = StringEntreString(S, "<b>", "</font>")
                        Return S.Trim()

                    Case Coluna.SituacaoCadastral

                        S = StringEntreString(S, "<!-- Início Linha SITUAÇÃO CADASTRAL -->", "<!-- Fim Linha SITUACAO CADASTRAL -->")
                        S = StringEntreString(S, "<tr>", "</tr>")
                        S = StringEntreString(S, "<b>", "</b>")
                        Return S.Trim()

                    Case Coluna.DataSituacaoCadastral

                        S = StringEntreString(S, "<!-- Início Linha SITUAÇÃO CADASTRAL -->", "<!-- Fim Linha SITUACAO CADASTRAL -->")
                        S = StringEntreString(S, "<tr>", "</tr>")
                        S = StringSaltaString(S, "</b>")
                        S = StringEntreString(S, "<b>", "</b>")
                        Return S.Trim()

                    Case Coluna.MotivoSituacaoCadastral

                        S = StringEntreString(S, "<!-- Início Linha MOTIVO DE SITUAÇÃO CADASTRAL -->", "<!-- Fim Linha MOTIVO DE SITUAÇÃO CADASTRAL -->")
                        S = StringEntreString(S, "<tr>", "</tr>")
                        S = StringEntreString(S, "<b>", "</b>")
                        Return S.Trim()

                    Case Else
                        Return S

                End Select
            Catch ex As Exception
                Throw New Exception("Devido à modificações no site da Receita Federal, não foi possivel pesquisar os dados desta empresa")
            End Try

        End Function

        Public Enum Coluna
            RazaoSocial = 0
            NomeFantasia
            AtividadeEconomicaPrimaria
            AtividadeEconomicaSecundaria
            NumeroDaInscricao
            MatrizFilial
            NaturezaJuridica
            SituacaoCadastral
            DataSituacaoCadastral
            MotivoSituacaoCadastral
            EnderecoLogradouro
            EnderecoNumero
            EnderecoComplemento
            EnderecoCEP
            EnderecoBairro
            EnderecoCidade
            EnderecoEstado
            ContatoEmail
            ContatoTelefone
        End Enum

        Private Function StringEntreString(ByVal Str As String, ByVal StrInicio As String, ByVal StrFinal As String) As String
            Dim Ini As Integer
            Dim Fim As Integer
            Dim Diff As Integer
            Ini = Str.IndexOf(StrInicio)
            Fim = Str.IndexOf(StrFinal)
            If Ini > 0 Then
                Ini = Ini + StrInicio.Length
            End If
            If Fim > 0 Then
                Fim = Fim + StrFinal.Length
            End If
            Diff = ((Fim - Ini) - StrFinal.Length)
            If (Fim > Ini) And (Diff > 0) Then
                Return Str.Substring(Ini, Diff)
            Else
                Return ""
            End If

        End Function

        Private Function StringSaltaString(ByVal Str As String, ByVal StrInicio As String) As String

            Dim Ini As Integer
            Ini = Str.IndexOf(StrInicio)
            If Ini > 0 Then

                Ini = Ini + StrInicio.Length
                Return Str.Substring(Ini)

            Else
                Return Str
            End If

        End Function

        Public Function StringPrimeiraLetraMaiuscula(ByVal Str As String) As String

            Dim StrResult As String = ""
            If Str.Length > 0 Then

                StrResult += Str.Substring(0, 1).ToUpper()
                StrResult += Str.Substring(1, Str.Length - 1).ToLower()
            End If
            Return StrResult
        End Function

    End Class

    Public Class CookieAwareWebClient
        Inherits System.Net.WebClient

        Private _mContainer As CookieContainer

        Public Sub New(ByVal container As CookieContainer)
            _mContainer = container
        End Sub
        Public Sub SetCookieContainer(ByVal container As CookieContainer)
            _mContainer = container
        End Sub

        Protected Overrides Function GetWebRequest(address As Uri) As WebRequest

            Dim request As WebRequest = MyBase.GetWebRequest(address)
            Dim webRequest = TryCast(request, HttpWebRequest)
            If webRequest IsNot Nothing Then
                webRequest.CookieContainer = _mContainer
                webRequest.KeepAlive = True
                webRequest.ProtocolVersion = HttpVersion.Version10
            End If
            Return request
        End Function
    End Class


End Namespace