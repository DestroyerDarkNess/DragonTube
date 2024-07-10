Imports Guna.UI2.WinForms
Imports MusiCloud.Core

Public Class DisclaimerForm
    Private Sub DisclaimerForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Guna2TextBox1.Text = GlobalInstances.CopyRigth.Replace("||", Environment.NewLine)
    End Sub

    Private Sub Guna2CircleButton2_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton2.Click
        Process.GetCurrentProcess.Kill()
    End Sub

    Private Sub YoutubeBrowserButton_Click(sender As Object, e As EventArgs) Handles YoutubeBrowserButton.Click
        Core.GlobalInstances.AppSettings.Disclaimer = True
        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
        Me.Close()
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        Core.GlobalInstances.AppSettings.Disclaimer = False
        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
        Process.GetCurrentProcess.Kill()
    End Sub


End Class