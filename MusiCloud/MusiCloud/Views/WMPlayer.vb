Imports System.Runtime.InteropServices
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports AxWMPLib
Imports WMPLib
Public Class WMPlayer

#Region " No Windows Focus "

	Protected Overrides ReadOnly Property ShowWithoutActivation As Boolean
		Get
			Return True
		End Get
	End Property

	Private Const WS_EX_TOPMOST As Integer = &H8

	Private Const WS_THICKFRAME As Integer = &H40000
	Private Const WS_CHILD As Integer = &H40000000
	Private Const WS_EX_NOACTIVATE As Integer = &H8000000
	Private Const WS_EX_TOOLWINDOW As Integer = &H80

	Protected Overrides ReadOnly Property CreateParams As CreateParams
		Get
			Dim createParamsA As CreateParams = MyBase.CreateParams
			createParamsA.ExStyle = createParamsA.ExStyle Or WS_EX_TOPMOST Or WS_EX_NOACTIVATE Or WS_EX_TOOLWINDOW
			Return createParamsA
		End Get
	End Property

#End Region
	Public Property FormParent As Form = Nothing
	Private Sub WMPlayer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		Me.MinimumSize = New Size(10, 10)
	End Sub
	Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
		e.Cancel = True
		Dim Parent As Control = Me.FormParent
		If TypeOf Parent Is MainView Then
			DirectCast(Parent, MainView).Guna2CheckBox1.Checked = False
			DirectCast(Parent, MainView).TogglePlayerWindow()
		End If
	End Sub
	Private Sub WMPlayer_Shown(sender As Object, e As EventArgs) Handles Me.Shown
	End Sub
	Public Const WM_NCLBUTTONDOWN As Integer = &HA1
	Public Const HT_CAPTION As Integer = &H2
	<DllImportAttribute("user32.dll")>
	Public Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
	End Function
	<DllImportAttribute("user32.dll")>
	Public Shared Function ReleaseCapture() As Boolean
	End Function
	Private Sub Form1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown, Guna2GradientPanel1.MouseDown
		If e.Button = Windows.Forms.MouseButtons.Left Then
			ReleaseCapture()
			SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0)
		End If
	End Sub


#Region " Move Form "
	Dim drag As Boolean
	Dim mousex As Integer
	Dim mousey As Integer

	Private Sub AxWindowsMediaPlayer1_MouseMoveEvent(sender As Object, e As _WMPOCXEvents_MouseMoveEvent) Handles AxWindowsMediaPlayer1.MouseMoveEvent
		If drag Then
			Me.Top = Cursor.Position.Y - mousey
			Me.Left = Cursor.Position.X - mousex
		End If

		Guna2ResizeBox1.Visible = Core.Helpers.MouseOverControl(Guna2ResizeBox1, Me)
	End Sub

	Private Sub AxWindowsMediaPlayer1_MouseUpEvent(sender As Object, e As _WMPOCXEvents_MouseUpEvent) Handles AxWindowsMediaPlayer1.MouseUpEvent
		drag = False
	End Sub

	Private Sub AxWindowsMediaPlayer1_MouseDownEvent(sender As Object, e As _WMPOCXEvents_MouseDownEvent) Handles AxWindowsMediaPlayer1.MouseDownEvent
		drag = True 'Sets the variable drag to true.
		mousex = Cursor.Position.X - Me.Left 'Sets variable mousex
		mousey = Cursor.Position.Y - Me.Top 'Sets variable mousey
	End Sub

#End Region


	Public Event PositionChange(ByVal newPosition As Double, ByVal DurationTime As Double)

	Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
		Try
			RaiseEvent PositionChange(AxWindowsMediaPlayer1.Ctlcontrols.currentPosition, AxWindowsMediaPlayer1.Ctlcontrols.currentItem.duration)
		Catch ex As Exception : End Try
	End Sub

	Private Sub AxWindowsMediaPlayer1_OpenStateChange(sender As Object, e As _WMPOCXEvents_OpenStateChangeEvent) Handles AxWindowsMediaPlayer1.OpenStateChange
		If AxWindowsMediaPlayer1.openState = WMPLib.WMPOpenState.wmposMediaOpen Then
			Timer1.Start()
		End If
	End Sub


End Class