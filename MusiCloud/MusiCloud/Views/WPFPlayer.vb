Imports System.Runtime.InteropServices
Imports System.Windows
Imports System.Windows.Threading
Imports AxWMPLib
Imports Guna.UI2.WinForms
Imports MusiCloud.Core

Public Class WPFPlayer

    Public WithEvents Player As System.Windows.Controls.MediaElement
    Public Property FormParent As Form = Nothing

    Public Sub New()
        SetStyle(ControlStyles.UserPaint, value:=True)
        SetStyle(ControlStyles.OptimizedDoubleBuffer, value:=True)
        SetStyle(ControlStyles.SupportsTransparentBackColor, value:=True)

        ' Esta llamada es exigida por el diseñador.
        InitializeComponent()

        Player = New System.Windows.Controls.MediaElement()
        Player.LoadedBehavior = Windows.Controls.MediaState.Manual
        ElementHost1.Child = Player

    End Sub

    Private Sub WPFPlayer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.MinimumSize = New System.Drawing.Size(10, 10)

    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True

        Dim Parent As Control = Me.FormParent
        If TypeOf Parent Is MainView Then
            DirectCast(Parent, MainView).Guna2CheckBox1.Checked = False
            DirectCast(Parent, MainView).TogglePlayerWindow()
        End If
    End Sub

    Public WithEvents httpServer As HttpVideoServer = Nothing

    Public Async Sub PlayWithServer(ByVal remoteVideoUrl As String)
        If httpServer IsNot Nothing Then
            httpServer.Stop()
            httpServer.Dispose()
        End If

        Player.Stop()
        Player.Close()

        httpServer = New HttpVideoServer(remoteVideoUrl)
        Await httpServer.StartServer()

        Player.Source = New Uri(httpServer._localVideoPath)
        Player.Stretch = System.Windows.Media.Stretch.UniformToFill

        Player.Play()
    End Sub

    Public Async Sub PlayFromFile(ByVal File As String)
        If httpServer IsNot Nothing Then
            httpServer.Stop()
            httpServer.Dispose()
        End If

        Player.Stop()
        Player.Close()

        Player.Source = New Uri(File, UriKind.Absolute)
        Player.Stretch = System.Windows.Media.Stretch.UniformToFill

        Player.Play()
    End Sub

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

    Public Event PositionChange(ByVal newPosition As TimeSpan, ByVal DurationTime As TimeSpan)

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Try

            RaiseEvent PositionChange(Player.Position, Player.NaturalDuration.TimeSpan)
        Catch ex As Exception : End Try

    End Sub

#Region " Move Form "
    Dim drag As Boolean
    Dim mousex As Integer
    Dim mousey As Integer


    Private Sub Player_MouseDown(sender As Object, e As Input.MouseButtonEventArgs) Handles Player.MouseDown

        drag = True 'Sets the variable drag to true.
        mousex = Cursor.Position.X - Me.Left 'Sets variable mousex
        mousey = Cursor.Position.Y - Me.Top 'Sets variable mousey
    End Sub

    Private Sub Player_MouseUp(sender As Object, e As Input.MouseButtonEventArgs) Handles Player.MouseUp
        drag = False
    End Sub

    Private Sub Player_MouseMove(sender As Object, e As Input.MouseEventArgs) Handles Player.MouseMove
        If drag Then
            Me.Top = Cursor.Position.Y - mousey
            Me.Left = Cursor.Position.X - mousex
        End If
        Guna2ResizeBox1.Visible = Core.Helpers.MouseOverControl(Guna2ResizeBox1, Me)
    End Sub

#End Region

#Region " FullScreen "

    <DllImport("user32.dll")>
    Private Shared Function GetDoubleClickTime() As UInteger
    End Function

    Private fullscreen As Boolean = False
    Private WithEvents DoubleClickTimer As Timer = New Timer() With {.Interval = GetDoubleClickTime()}

    Private Sub Player_MouseLeftButtonUp(sender As Object, e As Input.MouseButtonEventArgs) Handles Player.MouseLeftButtonUp
        If Not DoubleClickTimer.Enabled Then
            DoubleClickTimer.Start()
        Else

            If Not fullscreen Then
                Me.WindowState = FormWindowState.Maximized
            Else
                Me.WindowState = FormWindowState.Normal
            End If

            fullscreen = Not fullscreen
        End If
    End Sub

    Private Sub DoubleClickTimer_Tick(sender As Object, e As EventArgs) Handles DoubleClickTimer.Tick
        DoubleClickTimer.Stop()
    End Sub

    Private Sub Guna2GradientPanel1_DoubleClick(sender As Object, e As EventArgs) Handles Guna2GradientPanel1.DoubleClick
        If Not fullscreen Then
            Me.WindowState = FormWindowState.Maximized
        Else
            Me.WindowState = FormWindowState.Normal
        End If

        fullscreen = Not fullscreen
    End Sub


#End Region

End Class