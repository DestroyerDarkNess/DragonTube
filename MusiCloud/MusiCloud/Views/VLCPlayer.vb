Imports System.Drawing.Printing
Imports System.Runtime.InteropServices
Imports examples
Imports Vlc.DotNet.Core

Public Class VLCPlayer

    Public Property FormParent As Form = Nothing
    Public Property Media As VlcMedia = Nothing

    Public WithEvents vlcControl As Vlc.DotNet.Forms.VlcControl = Nothing

    Public Sub New(ByVal Libs As IO.DirectoryInfo, ByVal MediaOptions As String())
        SetStyle(ControlStyles.UserPaint, value:=True)
        SetStyle(ControlStyles.OptimizedDoubleBuffer, value:=True)
        SetStyle(ControlStyles.SupportsTransparentBackColor, value:=True)

        ' Esta llamada es exigida por el diseñador.
        InitializeComponent()

        Me.vlcControl = New Vlc.DotNet.Forms.VlcControl()

        Try
            Me.vlcControl.BeginInit()
            Me.vlcControl.VlcLibDirectory = Libs
            Me.vlcControl.VlcMediaplayerOptions = MediaOptions
            Me.vlcControl.EndInit()
        Catch ex As Exception
            Try
                IO.Directory.Delete(Core.GlobalInstances.VLCDir)
            Catch e As Exception : End Try
        End Try


        Me.vlcControl.Size = New Size(Guna2Panel1.Width - 1, Guna2Panel1.Height - 1)
        'Me.vlcControl.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Top
        Me.Guna2Panel1.Controls.Add(Me.vlcControl)
        Me.vlcControl.Dock = DockStyle.Fill
        UpdateSpectRatio()
    End Sub

    Private Sub VLCPlayer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
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

    Private Sub VLCPlayer_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        UpdateSpectRatio()
    End Sub

    Public Sub UpdateSpectRatio()
        If Me.vlcControl.Video IsNot Nothing Then
            Me.vlcControl.Video.AspectRatio = Me.vlcControl.Width & ":" & Me.vlcControl.Height
        End If
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
            createParamsA.ExStyle = createParamsA.ExStyle Or WS_EX_TOPMOST Or WS_EX_NOACTIVATE 'Or WS_EX_TOOLWINDOW
            Return createParamsA
        End Get
    End Property

#End Region

End Class