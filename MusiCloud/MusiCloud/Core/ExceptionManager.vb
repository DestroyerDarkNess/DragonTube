Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace Core
    Public Class ExceptionManager

        Public Shared LogFileName As String = IO.Path.Combine(IO.Path.GetTempPath, "MusiCloudErrors.log")

        Public Sub Initialize()
            ExcepList = New System.Collections.Generic.List(Of String)()
            AddHandler AppDomain.CurrentDomain.ProcessExit, AddressOf CurrentDomain_ProcessExit
            AddHandler AppDomain.CurrentDomain.FirstChanceException, AddressOf FirstChanceExceptionHandler
            AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf CurrentDomain_UnhandledException

            Try
                AddHandler System.Windows.Forms.Application.ThreadException, AddressOf Application_Exception_Handler
                System.Windows.Forms.Application.SetUnhandledExceptionMode(System.Windows.Forms.UnhandledExceptionMode.CatchException, False)

                If System.IO.File.Exists(LogFileName) = True Then
                    System.IO.File.Delete(LogFileName)
                End If

            Catch
            End Try
        End Sub

        Private Sub FirstChanceExceptionHandler(ByVal sender As Object, ByVal e As System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs)
            Dim ex As Exception = CType(e.Exception, Exception)
            WriteLogError(ex)
        End Sub

        Private Sub CurrentDomain_UnhandledException(ByVal sender As Object, ByVal e As System.UnhandledExceptionEventArgs)
            Dim ex As Exception = CType(e.ExceptionObject, Exception)
            WriteLogError(ex)
        End Sub

        Private Sub Application_Exception_Handler(ByVal sender As Object, ByVal e As System.Threading.ThreadExceptionEventArgs)
            Dim ex As Exception = CType(e.Exception, Exception)
            WriteLogError(ex)
        End Sub

        Private Sub WriteLogError(ByVal Excep As Exception)
            Try
                Dim ErrorMessage As String = $"------------------------------------- {Excep.ToString()}" & Environment.NewLine & $"Message: {Excep.Message} " & Environment.NewLine & Environment.NewLine & $" {Excep.Source} " & Environment.NewLine & Environment.NewLine & "______________________________________________________________End Report."
                ExcepList.Add(ErrorMessage)
                Dim CurrentColor As ConsoleColor = Console.ForegroundColor
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine(Excep.Message)
                Console.ForegroundColor = CurrentColor
            Catch
            End Try
        End Sub

        Private Sub WriteConsoleError(ByVal Excep As Exception, ByVal Optional ID As String = "")
            Try
                Dim ErrorMessage As String = $"Error ID: {ID} ------------------------------------- {Excep.ToString()}" & Environment.NewLine & $"Message: {Excep.Message} " & Environment.NewLine & Environment.NewLine & $" {Excep.Source} " & Environment.NewLine & Environment.NewLine & "______________________________________________________________End Report."
                Dim CurrentColor As ConsoleColor = Console.ForegroundColor
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine(ErrorMessage)
                Console.ForegroundColor = CurrentColor
            Catch
            End Try
        End Sub

        Private Shared ExcepList As System.Collections.Generic.List(Of String)

        Private Sub CurrentDomain_ProcessExit(ByVal sender As Object, ByVal e As EventArgs)
            System.IO.File.WriteAllText(LogFileName, String.Join(Environment.NewLine, ExcepList))
        End Sub
    End Class
End Namespace
