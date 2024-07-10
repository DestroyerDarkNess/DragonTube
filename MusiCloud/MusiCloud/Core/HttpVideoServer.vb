Imports System.IO
Imports System.Net
Imports System.Threading.Tasks

Namespace Core

    Public Class HttpVideoServer
        Implements IDisposable

        Private ReadOnly _remoteVideoUrl As String
        Private ReadOnly _port As Integer = 8080
        Public ReadOnly _localVideoPath As String = $"http://localhost:{_port}/local_video.mp4"
        Private ReadOnly _listener As HttpListener

        Public Sub New(remoteVideoUrl As String)
            _remoteVideoUrl = remoteVideoUrl
            _listener = New HttpListener()
            _listener.Prefixes.Add($"http://localhost:{_port}/")
        End Sub

        Public Async Function StartServer() As Task
            _listener.Start()
            Await Task.Run(AddressOf HandleRequests)
        End Function

        Public Sub [Stop]()
            If _listener IsNot Nothing AndAlso _listener.IsListening Then
                _listener.Stop()
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            _listener.Close()
        End Sub

        Private Async Sub HandleRequests()
            While _listener.IsListening
                Try
                    Dim context = Await _listener.GetContextAsync()
                    Dim response As HttpListenerResponse = context.Response

                    Using stream As Stream = response.OutputStream
                        Using client As New WebClient()
                            Dim videoStream = Await client.OpenReadTaskAsync(New Uri(_remoteVideoUrl))
                            'response.ContentLength64 = videoStream.Length
                            'response.ContentType = "video/mp4"
                            response.Headers.Add(HttpResponseHeader.CacheControl, "no-cache, no-store, must-revalidate")
                            response.Headers.Add(HttpResponseHeader.Pragma, "no-cache")
                            response.Headers.Add(HttpResponseHeader.Expires, "0")
                            Await videoStream.CopyToAsync(response.OutputStream)
                        End Using
                    End Using

                    response.Close()
                Catch ex As Exception
                    Exit While
                End Try
            End While
        End Sub
    End Class


End Namespace

