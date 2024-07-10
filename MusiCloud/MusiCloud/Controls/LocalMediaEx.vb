Imports System.IO
Imports FFMpegSharp

Public Class LocalMediaEx

    Public Property TargetMedia As FileInfo = Nothing

    Public Property VideoCodec As String = String.Empty

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        Dim Parent As Control = Me.ParentForm.ParentForm
        If TypeOf Parent Is MainView Then
            DirectCast(Parent, MainView).PlayFromFile(Me)
        End If
    End Sub

    Private Sub ReloadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReloadToolStripMenuItem.Click
        Try
            If IO.File.Exists(TargetMedia.FullName) Then IO.File.Delete(TargetMedia.FullName)
        Catch ex As Exception : End Try
    End Sub

    Private Sub LocalMediaEx_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            Dim Video As VideoInfo = New VideoInfo(TargetMedia)
            VideoCodec = Video.VideoFormat.ToLower
        Catch ex As Exception : End Try
    End Sub

End Class
