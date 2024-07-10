Imports System.Threading
Imports EO
Imports EO.WebBrowser
Imports EO.WebEngine
Imports Guna.UI2.WinForms
Imports MusiCloud.Core
Imports MusiCloud.Core.YTPlayer
Imports NSoup.Safety
Imports TheArtOfDevHtmlRenderer.Core.Entities
Imports YoutubeExplode
Imports YoutubeExplode.Search
Imports YoutubeExplode.Videos

Public Class YoutubeBrowser

    Public Property Engine As EO.WebEngine.Engine = Nothing
    Public Property WebControl As EO.WinForm.WebControl = Nothing

    Public WithEvents WebView As EO.WebBrowser.WebView = Nothing
    Public Property DevForm As WebDevTools = Nothing

    Public Property FormParent As Form = Nothing

#Region " Constructor "

    Public Sub New()
        SetStyle(ControlStyles.SupportsTransparentBackColor, value:=True)
        InitializeComponent()
        Me.BackColor = Color.Transparent

        Dim CachePath As String = IO.Path.Combine(Core.GlobalInstances.AppChacheFolder, "YoutubeBrowser")

        Try
            If IO.Directory.Exists(CachePath) = False Then
                IO.Directory.CreateDirectory(CachePath)
            End If
        Catch ex As Exception

        End Try

        Engine = EO.WebEngine.Engine.Create("YoutubeMusiWeb")

        Engine.Options.CachePath = CachePath
        Engine.Options.BypassUserGestureCheck = True

        'Engine.Options.AllowProprietaryMediaFormats()


        WebView = New EO.WebBrowser.WebView() With {.Engine = Engine, .CustomUserAgent = Core.GlobalInstances.GetUserAgent}




        WebControl = New EO.WinForm.WebControl With {.WebView = WebView, .Dock = DockStyle.Fill, .BackColor = Color.FromArgb(14, 14, 14)}

        AddHandler WebView.BeforeDownload, Sub(sender As Object, e As BeforeDownloadEventArgs)
                                               e.ShowDialog = False
                                           End Sub

        AddHandler WebView.CertificateError, Sub(sender As Object, e As CertificateErrorEventArgs)
                                                 e.Continue()
                                             End Sub
        DevForm = New WebDevTools

        Panel1.Controls.Add(WebControl)

        'If Core.GlobalInstances.AppSettings.DisableGPU = True Then
        '    Dim t As New Thread(Sub()
        '                            Do While True
        '                                Try
        '                                    Console.WriteLine("asdasdasd")
        '                                    WebControl.BeginInvoke(Sub()
        '                                                               WebControl?.Refresh()
        '                                                           End Sub)
        '                                Catch ex As Exception : End Try
        '                                System.Threading.Thread.Sleep(100)
        '                            Loop
        '                        End Sub)
        '    t.Start()
        'End If

    End Sub

#End Region

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
            createParamsA.ExStyle = createParamsA.ExStyle Or WS_EX_NOACTIVATE
            Return createParamsA
        End Get
    End Property

#End Region

    Private Sub YoutubeBrowser_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TopMostToolStripMenuItem.Checked = Core.GlobalInstances.AppSettings.TopMostYT

        CheckTop()
        WebView.ZoomFactor = 0.8
        WebView.CustomUserAgent = Core.GlobalInstances.GetUserAgent
        WebView.LoadUrl("https://www.youtube.com/")
    End Sub

    Private Sub YoutubeBrowser_Shown(sender As Object, e As EventArgs) Handles Me.Shown

    End Sub

    Private Sub HomeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HomeToolStripMenuItem.Click
        WebView.LoadUrl("https://www.youtube.com/")
    End Sub

    Private Sub ReloadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReloadToolStripMenuItem.Click
        WebView.Reload()
    End Sub

    Private Sub WebView1_BeforeContextMenu(sender As Object, e As BeforeContextMenuEventArgs) Handles WebView.BeforeContextMenu

        If e.MenuInfo.SourceFlags = ContextMenuSourceFlags.Page Or ContextMenuSourceFlags.Link Then

            Dim URL As String = e.MenuInfo.LinkUrl

            If String.IsNullOrEmpty(URL) = False Then
                CopyURLToolStripMenuItem.Tag = e.MenuInfo.LinkUrl
                CopyURLToolStripMenuItem.Visible = True
            Else
                CopyURLToolStripMenuItem.Visible = False
            End If

            Dim ID As String = Core.Helpers.GetYouTubeVideoIdFromUrl(URL)

            If String.IsNullOrEmpty(ID) = False Then
                Dim YTLink As String = "https://www.youtube.com/watch?v=" & ID
                DownloadToolStripMenuItem.Tag = YTLink
                AddToFavoritesToolStripMenuItem.Tag = YTLink

                AddToFavoritesToolStripMenuItem.Visible = True

                Dim itemByID = Core.GlobalInstances.AppSettings.Favorites.FirstOrDefault(Function(item) item.ID.Equals(ID, StringComparison.OrdinalIgnoreCase))

                If itemByID Is Nothing Then
                    AddToFavoritesToolStripMenuItem.Text = "Add to Favorites"
                Else
                    AddToFavoritesToolStripMenuItem.Text = "Remove to Favorites"
                End If

                DownloadToolStripMenuItem.Visible = True
            Else
                DownloadToolStripMenuItem.Visible = False
                AddToFavoritesToolStripMenuItem.Visible = False
            End If

        Else
            CopyURLToolStripMenuItem.Visible = False
        End If

        e.Menu.Items.Clear()
        Dim IsMouseHover As Boolean = Core.Helpers.MouseOverControl(WebControl, Me)
        If IsMouseHover = True Then
            LogInContextMenu1.Show()
            LogInContextMenu1.Location = Cursor.Position
        End If
    End Sub

    Dim ReloadEx As String = String.Empty

    Private Sub WebView1_UrlChanged(sender As Object, e As EventArgs) Handles WebView.UrlChanged
        Try
            Dim BaseUrl As String = WebView.Url
            Dim UrlUri As New Uri(BaseUrl)
            Dim IsYT As Boolean = IsYoutube(UrlUri.Host)

            If IsYT Then



                Dim Video As String = Core.Helpers.GetYouTubeVideoIdFromUrl(WebView.Url.ToLower.Replace("m.youtube", "www.youtube"))

                If Video = String.Empty Then

                    If Not WebView.CustomUserAgent = Core.GlobalInstances.GetUserAgent Then
                        WebView.CustomUserAgent = Core.GlobalInstances.GetUserAgent
                        WebView.LoadUrl(WebView.Url)
                    Else
                        WebView.EvalScript(DarkYT)
                        Dim Parent As Control = Me.FormParent

                        If TypeOf Parent Is MainView Then
                            Dim JSParsed As String = AdJS.Replace("$IMA$", DirectCast(Parent, MainView).ImageAdsUrl).Replace("MyAddJS", DirectCast(Parent, MainView).AdsUrl)
                            WebView.EvalScript(JSParsed)
                        End If
                    End If
                Else

                    If Core.GlobalInstances.AppSettings.NaturalYT = False Then
                        WebView.StopLoad()
                        WebView.GoBack()

                        Dim Parent As Control = Me.FormParent

                        If TypeOf Parent Is MainView Then

                            Dim NewLocalMedia As New Core.YoutubeItem With {.Url = BaseUrl} ' "https://www.youtube.com/watch?v=" & Video
                            Dim NewMedia As New MusicItem With {.LocalMedia = NewLocalMedia}
                            DirectCast(Parent, MainView).PlayFromWeb(NewMedia, True)

                        End If
                    Else
                        'If WebView.CustomUserAgent = Core.GlobalInstances.MobileUserAgent Then
                        '    WebView.EvalScript(ScrollFix)
                        'End If
                    End If

                End If
            ElseIf String.Equals(UrlUri.Host, "accounts.google.com", StringComparison.OrdinalIgnoreCase) Then
                If Not WebView.CustomUserAgent = Core.GlobalInstances.LoginUserAgent Then
                    WebView.CustomUserAgent = Core.GlobalInstances.LoginUserAgent
                    WebView.LoadUrl(WebView.Url).OnDone(Sub()
                                                            WebView.Reload(True)
                                                        End Sub)

                Else
                    Console.WriteLine("Google Url: " & WebView.Url)
                    Console.WriteLine("UserAgent: " & WebView.CustomUserAgent)
                End If
            Else
                Console.WriteLine("Host: " & UrlUri.Host)
            End If

        Catch ex As Exception : End Try
    End Sub

    Private Sub WebView_FullScreenModeChanged(sender As Object, e As FullscreenModeChangedArgs) Handles WebView.FullScreenModeChanged
        Try
            If Core.GlobalInstances.AppSettings.MaximizedInApp = True Then
                If e.Fullscreen = True Then
                    e.HWndOwner = Me.Handle
                    e.Parent = Me
                    Panel1.Visible = False
                Else
                    Panel1.Visible = True
                End If
            Else
                Panel1.Visible = True
            End If
        Catch ex As Exception : End Try
    End Sub


    Private Function IsYoutube(ByVal Host As String) As Boolean
        If String.Equals(Host, "www.youtube.com", StringComparison.OrdinalIgnoreCase) Then
            Return True
        ElseIf String.Equals(Host, "m.youtube.com", StringComparison.OrdinalIgnoreCase) Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Sub WebView1_LoadCompleted(sender As Object, e As LoadCompletedEventArgs) Handles WebView.LoadCompleted
        WebView.EvalScript(DarkYT)
        Dim Parent As Control = Me.FormParent

        If TypeOf Parent Is MainView Then
            Dim JSParsed As String = AdJS.Replace("$IMA$", DirectCast(Parent, MainView).ImageAdsUrl).Replace("MyAddJS", DirectCast(Parent, MainView).AdsUrl)
            WebView.EvalScript(JSParsed)
        End If
    End Sub

    Public Function SearchByYoutube(ByVal Text As String) As NavigationTask
        Try
            Dim Url As String = "https://www.youtube.com/results?search_query=" & Text.Replace(" ", "+")

            If String.IsNullOrWhiteSpace(Text) = True Then
                Url = "https://www.youtube.com/"
            End If

            Return WebView.LoadUrl(Url)
        Catch ex As Exception : End Try
        Return Nothing
    End Function


    Private Sub WebView_NewWindow(sender As Object, e As NewWindowEventArgs) Handles WebView.NewWindow
        Dim ParsedUrl As Uri = New Uri(e.TargetUrl)

        If String.Equals(e.TargetUrl.Replace("https://www.youtube.com/", "").Replace("https://m.youtube.com/", ""), "MyAddJS", StringComparison.OrdinalIgnoreCase) Then
            Dim Parent As Control = Me.FormParent

            If TypeOf Parent Is MainView Then
                DirectCast(Parent, MainView).ShowADForm()
            End If
        ElseIf IsYoutube(ParsedUrl.Host) Then
            WebView.LoadUrl(e.TargetUrl)
        End If

        e.Accepted = False
    End Sub

    Private Sub ShowDevToolsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowDevToolsToolStripMenuItem.Click
        If DevForm.Visible = True Then
            ShowDevToolsToolStripMenuItem.Text = "Show Dev Tools"
            DevForm.Hide()
        Else
            ShowDevToolsToolStripMenuItem.Text = "Hide Dev Tools"
            DevForm.Show()
            WebView.ShowDevTools(DevForm.Handle)
        End If
    End Sub

    Private Sub WebView_LoadFailed(sender As Object, e As LoadFailedEventArgs) Handles WebView.LoadFailed
        Dim ErrorMessage As String = e.ErrorMessage
        Dim ErrorCode As String = e.ErrorCode
        Console.WriteLine("Msg: " & ErrorMessage & "  Code: " & ErrorCode)
    End Sub

    Private Sub WebView_RequestPermissions(sender As Object, e As RequestPermissionEventArgs) Handles WebView.RequestPermissions
        'If e.Permissions = Permissions.Microphone OrElse e.Permissions = Permissions.WebCam Then
        e.Allow()
        'End If
    End Sub



    Private DarkYT As String = <![CDATA[
document.getElementsByTagName('html')[0].setAttribute('dark','true');
localStorage.setItem('DarkMode', 'True');
]]>.Value


    Private ScrollFix As String = <![CDATA[
const styleElement = document.createElement('style');
styleElement.textContent = `
body::-webkit-scrollbar {
  width: 15px !important; /* Ancho del scrollbar en 15 píxeles */
}
`;
document.head.appendChild(styleElement);
]]>.Value

    Private AdJS As String = <![CDATA[
function redirectOnImageClick(imgElement, url) {
  imgElement.addEventListener('click', () => {
    window.open(url, '_blank');
  });

  imgElement.style.cursor = 'pointer';
}

function FuckAD(div) {
  

  if (div) {
    const img = document.createElement('img');

    img.src = '$IMA$'; 
    img.style.position = 'absolute';
    img.style.top = '0';
    img.style.left = '0';
    img.style.width = '100%';
    img.style.height = '100%';
    img.style.border = 'none';

    redirectOnImageClick(img, 'MyAddJS'); 

    div.appendChild(img);
  }
}


function myTimer() {
 const div = document.querySelector('div.ytd-banner-promo-renderer-background.style-scope.ytd-banner-promo-renderer');

   if (!div) {
    div = document.querySelector('div.style-scope.ytd-statement-banner-renderer');
   }

   if (div) {
     FuckAD(div);
    // clearInterval(myVar);
   }
}


let myVar = setInterval(myTimer, 1000);


]]>.Value


    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
        Dim Parent As Control = Me.FormParent
        If TypeOf Parent Is MainView Then
            Me.Hide()
            DirectCast(Parent, MainView).MusicHubButton.Checked = True
        End If
    End Sub

    Private Sub TopMostToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TopMostToolStripMenuItem.Click
        Core.GlobalInstances.AppSettings.TopMostYT = TopMostToolStripMenuItem.Checked
        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
    End Sub

    Private Sub TopMostToolStripMenuItem_CheckedChanged(sender As Object, e As EventArgs) Handles TopMostToolStripMenuItem.CheckedChanged
        CheckTop()
    End Sub
    Private Sub CheckTop()
        If TopMostToolStripMenuItem.Checked = True Then
            TopMostToolStripMenuItem.Text = "🗹 TopMost"
        Else
            TopMostToolStripMenuItem.Text = "⬜ TopMost"
        End If
        Me.TopMost = TopMostToolStripMenuItem.Checked
    End Sub

    Private Sub NextToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NextToolStripMenuItem.Click
        If WebView.CanGoForward Then WebView?.GoForward()
    End Sub

    Private Sub BackToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BackToolStripMenuItem.Click
        If WebView.CanGoBack Then WebView?.GoBack()
    End Sub

    Private Sub CopyURLToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyURLToolStripMenuItem.Click
        Clipboard.SetText(CopyURLToolStripMenuItem.Tag)
    End Sub

    Private Sub DownloadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DownloadToolStripMenuItem.Click
        Dim Parent As Control = Me.FormParent

        If TypeOf Parent Is MainView Then

            DirectCast(Parent, MainView).DownloadMedia(DownloadToolStripMenuItem.Tag)

        End If
    End Sub

    Private Sub AddToFavoritesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddToFavoritesToolStripMenuItem.Click
        Dim ID As String = Core.Helpers.GetYouTubeVideoIdFromUrl(AddToFavoritesToolStripMenuItem.Tag)
        If String.IsNullOrEmpty(ID) = False Then
            Dim itemByID = Core.GlobalInstances.AppSettings.Favorites.FirstOrDefault(Function(item) item.ID.Equals(ID, StringComparison.OrdinalIgnoreCase))

            If itemByID IsNot Nothing Then
                Core.GlobalInstances.AppSettings.Favorites?.Remove(itemByID)
                AddToFavoritesToolStripMenuItem.Text = "Add to Favorites"
            Else
                Dim VideoURL As String = "https://www.youtube.com/watch?v=" & ID
                Dim CurrentMedia = New Core.YoutubeItem With {.Url = VideoURL, .Author = "DragonTube"}
                Core.GlobalInstances.AppSettings.Favorites?.Add(CurrentMedia)
                AddToFavoritesToolStripMenuItem.Text = "Remove to Favorites"
            End If
            Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
            Dim Parent As Control = Me.FormParent
            If TypeOf Parent Is MainView Then
                DirectCast(Parent, MainView).ReloadFavorites()
            End If
        End If
    End Sub


End Class