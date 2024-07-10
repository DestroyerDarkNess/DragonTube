
Imports EO.WebBrowser
Imports Guna.UI2.WinForms
Imports MusiCloud.Core
Imports YoutubeExplode.Videos


Public Class WebPlayer

    Public WithEvents YTPlayer As YTPlayer
    Public Property FormParent As Form = Nothing

    Public Sub New()
        SetStyle(ControlStyles.UserPaint, value:=True)
        SetStyle(ControlStyles.OptimizedDoubleBuffer, value:=True)
        SetStyle(ControlStyles.SupportsTransparentBackColor, value:=True)
        SetStyle(ControlStyles.ResizeRedraw, value:=True)
        ' Esta llamada es exigida por el diseñador.
        InitializeComponent()
        Me.DoubleBuffered = True
        Me.ResizeRedraw = True
        YTPlayer = New YTPlayer()
        Panel1.Controls.Add(YTPlayer.WebControl)
    End Sub


    Private Sub WebPlayer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AddHandler YTPlayer.WebView.BeforeContextMenu, AddressOf WebView1_BeforeContextMenu
    End Sub

    Private Sub WebView1_BeforeContextMenu(sender As Object, e As BeforeContextMenuEventArgs)
        e.Menu.Items.Clear()
        Dim IsMouseHover As Boolean = Core.Helpers.MouseOverControl(YTPlayer.WebControl, Panel1)
        If IsMouseHover = True Then
            LogInContextMenu1.Show()
            LogInContextMenu1.Location = Cursor.Position
        End If
    End Sub


    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True

        Dim Parent As Control = Me.FormParent
        If TypeOf Parent Is MainView Then
            DirectCast(Parent, MainView).Guna2CheckBox1.Checked = False
            DirectCast(Parent, MainView).TogglePlayerWindow()
        End If
    End Sub

    Private Sub WebPlayer_TextChanged(sender As Object, e As EventArgs) Handles Me.TextChanged
        If TypeOf FormParent Is MainView Then
            DirectCast(FormParent, MainView).Label1.Text = Me.Text

            Dim ID As String = Core.Helpers.GetYouTubeVideoIdFromUrl(YTPlayer.WebView.Url)
            If String.IsNullOrEmpty(ID) = False Then
                CheckVideo(ID)
            End If
        End If
    End Sub

    Public Sub CheckVideo(ByVal VideoID As String)
        If Core.GlobalInstances.AppSettings.Favorites IsNot Nothing Then
            Dim itemByUrl = Core.GlobalInstances.AppSettings.Favorites.FirstOrDefault(Function(item) item.ID.Equals(VideoID, StringComparison.OrdinalIgnoreCase))

            If itemByUrl IsNot Nothing Then
                ReloadToolStripMenuItem.Text = "Remove to Favorites"
            Else
                ReloadToolStripMenuItem.Text = "Add to Favorites"
            End If
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
            createParamsA.ExStyle = createParamsA.ExStyle Or WS_EX_TOPMOST Or WS_EX_NOACTIVATE Or WS_EX_TOOLWINDOW
            Return createParamsA
        End Get
    End Property

    Private Sub HomeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HomeToolStripMenuItem.Click
        Dim ID As String = Core.Helpers.GetYouTubeVideoIdFromUrl(YTPlayer.BaseUrl)
        If String.IsNullOrEmpty(ID) = False Then
            Dim VideoURL As String = "https://www.youtube.com/watch?v=" & ID
            Clipboard.SetText(VideoURL)
        End If
    End Sub

    Private Sub ReloadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReloadToolStripMenuItem.Click
        Dim ID As String = Core.Helpers.GetYouTubeVideoIdFromUrl(YTPlayer.BaseUrl)
        If String.IsNullOrEmpty(ID) = False Then
            Dim itemByID = Core.GlobalInstances.AppSettings.Favorites.FirstOrDefault(Function(item) item.ID.Equals(ID, StringComparison.OrdinalIgnoreCase))

            If itemByID IsNot Nothing Then
                Core.GlobalInstances.AppSettings.Favorites?.Remove(itemByID)
                ReloadToolStripMenuItem.Text = "Add to Favorites"
            Else
                Dim VideoURL As String = "https://www.youtube.com/watch?v=" & ID
                Dim CurrentMedia = New Core.YoutubeItem With {.Url = VideoURL, .Title = Me.Text, .Author = "DragonTube"}
                Core.GlobalInstances.AppSettings.Favorites?.Add(CurrentMedia)
                ReloadToolStripMenuItem.Text = "Remove to Favorites"
            End If
            Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
            Dim Parent As Control = FormParent
            If TypeOf Parent Is MainView Then
                DirectCast(Parent, MainView).ReloadFavorites()
            End If
        End If
    End Sub

    Private Sub ShowDevToolsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowDevToolsToolStripMenuItem.Click
        If YTPlayer.DevForm.Visible = True Then
            ShowDevToolsToolStripMenuItem.Text = "Show Dev Tools"
            YTPlayer.DevForm.Hide()
        Else
            ShowDevToolsToolStripMenuItem.Text = "Hide Dev Tools"
            YTPlayer.DevForm.Show()
            YTPlayer.WebView.ShowDevTools(YTPlayer.DevForm.Handle)
        End If
    End Sub

#End Region

#Region " Move Form "
    Dim drag As Boolean
    Dim mousex As Integer
    Dim mousey As Integer

    Private Sub YTPlayer_MouseDown(Key As Keys) Handles YTPlayer.MouseDown
        If Key = Keys.LButton Then
            drag = True 'Sets the variable drag to true.
            mousex = Cursor.Position.X - Me.Left 'Sets variable mousex
            mousey = Cursor.Position.Y - Me.Top 'Sets variable mousey
        End If
    End Sub

    Private Sub YTPlayer_MouseMove(XYPoint As Drawing.Point) Handles YTPlayer.MouseMove
        If drag Then
            Me.Top = Cursor.Position.Y - mousey
            Me.Left = Cursor.Position.X - mousex
        End If

        'Console.WriteLine("Mouse: " & Core.Helpers.MouseIsOverControl(Guna2ResizeBox1))
        'Guna2ResizeBox1.Visible = Core.Helpers.MouseOverControl(Guna2ResizeBox1, Me)
    End Sub

    Private Sub YTPlayer_MouseUp(Key As Keys) Handles YTPlayer.MouseUp
        If Key = Keys.LButton Then
            drag = False
        End If
    End Sub

    Private Sub CloseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CloseToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub DownloadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DownloadToolStripMenuItem.Click
        If Not YTPlayer.BaseUrl = String.Empty Then
            Dim Parent As Control = Me.FormParent
            If TypeOf Parent Is MainView Then
                DirectCast(Parent, MainView).DownloadMedia(YTPlayer.BaseUrl)
            End If
        End If
    End Sub


#End Region

End Class