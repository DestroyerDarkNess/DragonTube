Public Class WebDevTools
    Private Sub WebDevTools_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True

        Me.Hide()
    End Sub

End Class