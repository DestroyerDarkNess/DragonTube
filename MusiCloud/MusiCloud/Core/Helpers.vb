Imports System.Buffers.Text
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Management
Imports System.Net.Http
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions
Imports System.Web
Imports Guna.UI2.WinForms
Imports TagLib

Namespace Core
    Public Class Helpers

        Public Shared Function BToImage(ByVal B64 As String) As System.Drawing.Image
            Dim image = Nothing
            Using ms As MemoryStream = New MemoryStream(Convert.FromBase64String(B64))
                image = System.Drawing.Image.FromStream(ms)
            End Using
            Return image
        End Function

        Public Shared Sub SetPicture(ByVal fileName As String, ByVal Publisher As String, ByVal picName As System.Drawing.Image)
            Try
                Using songTag = TagLib.File.Create(fileName)

                    If picName IsNot Nothing Then
                        Dim Pic As Byte() = ImageToByteArray(picName)
                        Dim ByteVectorEx As New ByteVector(Pic)
                        Dim pics As IPicture() = New TagLib.IPicture(0) {}
                        pics(0) = New TagLib.Picture(ByteVectorEx)
                        songTag.Tag.Pictures = pics
                    End If

                    songTag.Tag.Publisher = Publisher

                    songTag.Save()
                End Using
            Catch : End Try
        End Sub

        Public Shared Function ImageToByteArray(ByVal imageIn As System.Drawing.Image) As Byte()
            Using ms = New MemoryStream()
                imageIn.Save(ms, imageIn.RawFormat)
                Return ms.ToArray()
            End Using
        End Function

        Public Shared Function CleanPath(ByVal s As String) As String
            Return String.Join("_", s.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd("."c)
        End Function

        Public Shared Function FormattedTimeToString(ByVal TimeInSeconds As Integer) As String
            Dim hours As Integer = TimeInSeconds \ 3600
            Dim minutes As Integer = (TimeInSeconds Mod 3600) \ 60
            Dim seconds As Integer = TimeInSeconds Mod 60

            Dim formattedTime As String = ""
            If hours > 0 Then
                formattedTime = $"{hours:00}:{minutes:00}:{seconds:00}"
            Else
                formattedTime = $"{minutes:00}:{seconds:00}"
            End If

            Return formattedTime
        End Function

        Public Shared Function GetYouTubeVideoIdFromUrl(ByVal url As String) As String
            Try
                Dim uri As Uri = Nothing

                If Not Uri.TryCreate(url, UriKind.Absolute, uri) Then

                    Try
                        uri = New UriBuilder("http", url).Uri
                    Catch
                        Return ""
                    End Try
                End If

                Dim host As String = uri.Host
                Dim youTubeHosts As String() = {"www.youtube.com", "youtube.com", "youtu.be", "www.youtu.be"}
                If Not youTubeHosts.Contains(host) Then Return ""
                Dim query = HttpUtility.ParseQueryString(uri.Query)

                If query.AllKeys.Contains("v") Then
                    Return Regex.Match(query("v"), "^[a-zA-Z0-9_-]{11}$").Value
                ElseIf query.AllKeys.Contains("u") Then
                    Return Regex.Match(query("u"), "/watch\?v=([a-zA-Z0-9_-]{11})").Groups(1).Value
                Else
                    Dim last = uri.Segments.Last().Replace("/", "")
                    If Regex.IsMatch(last, "^v=[a-zA-Z0-9_-]{11}$") Then Return last.Replace("v=", "")
                    Dim segments As String() = uri.Segments
                    If segments.Length > 2 AndAlso segments(segments.Length - 2) <> "v/" AndAlso segments(segments.Length - 2) <> "watch/" Then Return ""
                    Return Regex.Match(last, "^[a-zA-Z0-9_-]{11}$").Value
                End If
            Catch ex As Exception : End Try
            Return String.Empty
        End Function
        Public Shared Function StringToHex(ByVal text As String) As String
            Dim hex As String = String.Empty
            For i As Integer = 0 To text.Length - 1
                hex &= Asc(text.Substring(i, 1)).ToString("x").ToUpper
            Next
            Return hex
        End Function


        <DllImport("user32.dll")>
        Public Shared Function GetAsyncKeyState(ByVal nVirtKey As Integer) As Short
        End Function

        <DllImport("Gdi32.dll", EntryPoint:="CreateRoundRectRgn")>
        Public Shared Function CreateRoundRectRgn(ByVal nLeftRect As Integer, ByVal nTopRect As Integer, ByVal nRightRect As Integer, ByVal nBottomRect As Integer, ByVal nWidthEllipse As Integer, ByVal nHeightEllipse As Integer) As IntPtr
        End Function

        Public Shared Function GetColorAt(x As Integer, y As Integer) As Color
            Using screen As Bitmap = New Bitmap(1, 1)
                Using g As Graphics = Graphics.FromImage(screen)
                    g.CopyFromScreen(x, y, 0, 0, New Size(1, 1))
                End Using

                Return screen.GetPixel(0, 0)
            End Using
        End Function

        Public Shared Function MouseOverControl(ByVal Contrl As Control, ByVal ParentForm As Control) As Boolean
            Dim mousePosition As Point = ParentForm.PointToClient(Cursor.Position)

            If Contrl.Bounds.Contains(mousePosition) Then
                Return True
            Else
                Return False
            End If
        End Function


        Public Shared Sub PositionFormInBottomRightCorner(form As Form)
            Dim screenBounds As Rectangle = Screen.PrimaryScreen.WorkingArea
            Dim formBounds As Rectangle = form.Bounds

            Dim newX As Integer = screenBounds.Right - formBounds.Width
            Dim newY As Integer = screenBounds.Bottom - formBounds.Height

            form.Location = New Point(newX, newY)
        End Sub

#Region " Sleep "

        ' [ Sleep ]
        '
        ' // By Elektro H@cker
        '
        ' Examples :
        ' Sleep(5) : MsgBox("Test")
        ' Sleep(5, Measure.Seconds) : MsgBox("Test")

        Public Enum Measure
            Milliseconds = 1
            Seconds = 2
            Minutes = 3
            Hours = 4
        End Enum

        Public Shared Sub Sleep(ByVal Duration As Int64, Optional ByVal Measure As Measure = Measure.Seconds)

            Dim Starttime = DateTime.Now

            Select Case Measure
                Case Measure.Milliseconds : Do While (DateTime.Now - Starttime).TotalMilliseconds < Duration : Application.DoEvents() : Loop
                Case Measure.Seconds : Do While (DateTime.Now - Starttime).TotalSeconds < Duration : Application.DoEvents() : Loop
                Case Measure.Minutes : Do While (DateTime.Now - Starttime).TotalMinutes < Duration : Application.DoEvents() : Loop
                Case Measure.Hours : Do While (DateTime.Now - Starttime).TotalHours < Duration : Application.DoEvents() : Loop
                Case Else
            End Select

        End Sub

#End Region

    End Class
End Namespace

