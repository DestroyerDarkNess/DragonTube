Imports System.IO
Imports EO.WebBrowser
Imports MusiCloud.Core

Public Class YoutubePlayer

	'#Region " No Windows Focus "

	'	Protected Overrides ReadOnly Property ShowWithoutActivation As Boolean
	'		Get
	'			Return True
	'		End Get
	'	End Property

	'	Private Const WS_EX_TOPMOST As Integer = &H8

	'	Private Const WS_THICKFRAME As Integer = &H40000
	'	Private Const WS_CHILD As Integer = &H40000000
	'	Private Const WS_EX_NOACTIVATE As Integer = &H8000000
	'	Private Const WS_EX_TOOLWINDOW As Integer = &H80

	'	Protected Overrides ReadOnly Property CreateParams As CreateParams
	'		Get
	'			Dim createParamsA As CreateParams = MyBase.CreateParams
	'			createParamsA.ExStyle = createParamsA.ExStyle Or WS_EX_TOPMOST Or WS_EX_NOACTIVATE Or WS_EX_TOOLWINDOW
	'			Return createParamsA
	'		End Get
	'	End Property

	'#End Region

	Public Property FormParent As Form = Nothing
	Private Sub YoutubePlayer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		Me.MinimumSize = New Size(10, 10)
	End Sub
	Dim NewControl As WebPlayer = New WebPlayer
	Private Sub YoutubePlayer_Shown(sender As Object, e As EventArgs) Handles Me.Shown

		NewControl.Show()
		NewControl.Hide()

		NewControl.YTPlayer.Load("https://www.youtube.com/watch?v=rqJDO3TWnac", YTPlayer.YTMode.Embed)
	End Sub

	Private Sub YoutubePlayer_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
		e.Cancel = True
		Dim Parent As Control = Me.FormParent
		If TypeOf Parent Is MainView Then
			DirectCast(Parent, MainView).Guna2CheckBox1.Checked = False
		End If
	End Sub

	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
		NewControl.YTPlayer.Load("https://www.youtube.com/watch?v=rqJDO3TWnac", YTPlayer.YTMode.Embed)
	End Sub

	Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
		NewControl.Visible = CheckBox1.Checked
	End Sub
End Class