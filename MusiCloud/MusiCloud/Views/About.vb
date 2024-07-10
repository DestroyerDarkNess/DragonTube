Imports MusiCloud.Core

Public Class About

#Region " Constructor "

    Public Sub New()
        SetStyle(ControlStyles.SupportsTransparentBackColor, value:=True)
        InitializeComponent()
        Me.BackColor = Color.Transparent
    End Sub

#End Region

    Private Sub About_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        GdiPlusLabel2.Text = "v" & GlobalInstances.AppVersion

    End Sub


    Private Sub guna2Panel5_Click(sender As Object, e As EventArgs) Handles guna2Panel5.Click
        Process.Start("https://www.paypal.com/paypalme/SalvadorKrilewski")
    End Sub

    Private Sub guna2Panel6_Click(sender As Object, e As EventArgs) Handles guna2Panel6.Click
        Process.Start("https://www.buymeacoffee.com/s4lsalsoft")
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        Process.Start(TextBox2.Text)
    End Sub

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click
        Process.Start(TextBox3.Text)
    End Sub

    Private Sub Guna2Button3_Click(sender As Object, e As EventArgs) Handles Guna2Button3.Click
        Process.Start(TextBox4.Text)
    End Sub

    Private Sub Guna2Button4_Click(sender As Object, e As EventArgs) Handles Guna2Button4.Click
        Process.Start(TextBox5.Text)
    End Sub

    Private Sub GdiPlusLabel4_Click(sender As Object, e As EventArgs) Handles GdiPlusLabel4.Click
        Process.Start("https://github.com/Pentesting-28")
    End Sub

    Private Sub Guna2Button5_Click(sender As Object, e As EventArgs) Handles Guna2Button5.Click
        Clipboard.SetText("0x269977B5c1F592014B9b69F0A5A65B62C4E0C30e")
    End Sub


End Class