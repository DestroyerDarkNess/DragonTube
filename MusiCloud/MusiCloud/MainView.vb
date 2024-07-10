Imports System.Diagnostics.Eventing.Reader
Imports System.Drawing.Printing
Imports System.IO
Imports System.IO.Packaging
Imports System.Net
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports AngleSharp.Dom
Imports AxWMPLib
Imports EO.WebBrowser
Imports FFMpegSharp
Imports FFMpegSharp.Enums
Imports Microsoft.Win32
Imports MusiCloud.Core
Imports SevenZipExtractor
Imports Vlc.DotNet.Core
Imports Vlc.DotNet.Forms
Imports YoutubeExplode
Imports YoutubeExplode.Search
Imports YoutubeExplode.Videos
Imports YoutubeExplode.Videos.Streams

Public Class MainView

#Region " Constructor "

    Public Sub New()
        Core.GlobalInstances.ExeptionManager.Initialize()
        Core.GlobalInstances.CreateCachePath()
        SetStyle(ControlStyles.UserPaint, value:=True)
        SetStyle(ControlStyles.OptimizedDoubleBuffer, value:=True)
        SetStyle(ControlStyles.SupportsTransparentBackColor, value:=True)
        InitializeComponent()
        EOWebPlayer.Show() : EOWebPlayer.Hide()

        Me.Guna2Panel1.Width += 1
        Me.Guna2Panel1.Height += 1
        Me.Guna2Panel1.BorderColor = Color.Red

        RoundBorders()
        WindowControlBox()
        CheckAds()
    End Sub

    Public Sub RoundBorders()
        If Core.GlobalInstances.AppSettings.RoundedEdges = True Then
            Me.Guna2Panel1.BorderRadius = 6
            Me.Region = System.Drawing.Region.FromHrgn(Core.Helpers.CreateRoundRectRgn(0, 0, Width + 1, Height + 1, 11, 11))
        Else
            Me.Region = Nothing
            Me.Guna2Panel1.BorderRadius = 0
        End If
    End Sub

    Public Sub WindowControlBox()
        Guna2CircleButton2.Visible = Not Core.GlobalInstances.AppSettings.WindowsControlBox
        Guna2CircleButton3.Visible = Not Core.GlobalInstances.AppSettings.WindowsControlBox
        Guna2CircleButton4.Visible = Not Core.GlobalInstances.AppSettings.WindowsControlBox
        If Core.GlobalInstances.AppSettings.WindowsControlBox = True Then
            Guna2CircleButton7.Location = New Point(750, 3)
            Me.FormBorderStyle = FormBorderStyle.Sizable
            Me.Guna2Panel1.BorderThickness = 0
        Else
            Guna2CircleButton7.Location = New Point(684, 3)
            Me.FormBorderStyle = FormBorderStyle.None
            Me.Guna2Panel1.BorderThickness = 1
        End If
    End Sub

#End Region

#Region " ControlBox "

    Private Sub Guna2CircleButton3_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton3.Click
        If Me.WindowState = FormWindowState.Normal Then
            Me.WindowState = FormWindowState.Minimized
        Else
            Me.WindowState = FormWindowState.Normal
        End If
    End Sub

    Private Sub Guna2CircleButton2_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton2.Click
        If Core.GlobalInstances.AppSettings.SystemTray = True Then
            Me.Hide()
        Else
            Me.Close()
        End If
    End Sub


    Private Sub MainView_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
        If Core.GlobalInstances.AppSettings.SystemTray = False Then
            SaveMainSetting()
            Process.GetCurrentProcess.Kill()
            Environment.Exit(0)
        Else
            Me.Hide()
        End If
    End Sub


#End Region

    Public Property EOWebPlayer As WebPlayer = New WebPlayer() With {.FormParent = Me}

    Public Property Youtube As YoutubeClient = New YoutubeClient()
    Public Property VLC_Player As VLCPlayer = Nothing

    Public WithEvents WM_Player As WMPlayer = New WMPlayer() With {.FormParent = Me}

    Public WithEvents WPF_Player As WPFPlayer = New WPFPlayer() With {.FormParent = Me}

    Public Property YTBrowser_View As New YoutubeBrowser With {.Visible = False, .FormParent = Me, .FormBorderStyle = FormBorderStyle.Sizable, .Dock = DockStyle.Fill}
    Public Property MusicHub_View As New MusicHub With {.Visible = True, .TopLevel = False, .Dock = DockStyle.Fill}
    Public Property MusicLocal_View As New LocalMusic With {.Visible = True, .TopLevel = False, .Dock = DockStyle.Fill}

    Public Property DownloadMedia_View As New DownloadedMedia With {.Visible = True, .TopLevel = False, .Dock = DockStyle.Fill}

    Public Property Config_View As New Config With {.Visible = True, .TopLevel = False, .Dock = DockStyle.Fill}

    Public Property About_View As New About With {.Visible = True, .TopLevel = False, .Dock = DockStyle.Fill}


    Private Sub MainView_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If Me.Visible = True Then
            ShowHideToolStripMenuItem.Text = "Hide"
        Else
            ShowHideToolStripMenuItem.Text = "Show"
        End If

        MediaPlayer = CreateMediaPlayer()

        AddHandler WM_Player.AxWindowsMediaPlayer1.MediaError, AddressOf AxWindowsMediaPlayer1_MediaError
        AddHandler WM_Player.AxWindowsMediaPlayer1.PlayStateChange, AddressOf AxWindowsMediaPlayer1_PlayStateChange
        AddHandler WPF_Player.Player.MediaFailed, AddressOf Player_MediaFailed
        AddHandler WPF_Player.Player.MediaOpened, AddressOf Player_MediaOpened
        AddHandler WPF_Player.Player.MediaEnded, AddressOf Player_MediaEnded
        AddHandler EOWebPlayer.YTPlayer.PositionChange, AddressOf WebPlayer_PositionChange
        AddHandler EOWebPlayer.YTPlayer.OnStateChange, AddressOf WebPlayer_OnStateChange

        AddHandler EOWebPlayer.YTPlayer.OnComplete, Sub()
                                                        Me.BeginInvoke(Sub()
                                                                           Guna2CircleButton1.Checked = True
                                                                           Guna2ProgressBar3.Visible = False
                                                                           If Guna2CheckBox1.Checked Then
                                                                               TogglePlayerWindow()
                                                                           End If
                                                                       End Sub)
                                                    End Sub

        LoadMainSetting()

        If Core.GlobalInstances.AppSettings.ExternalYT = False Then
            YTBrowser_View.FormBorderStyle = FormBorderStyle.None
            YTBrowser_View.TopLevel = False
            PanelViewContainer.Controls.Add(YTBrowser_View)
        End If


        PanelViewContainer.Controls.AddRange({MusicHub_View, MusicLocal_View, DownloadMedia_View, Config_View, About_View})

        MusicHubButton.Checked = True
        CheckVersion()

        WPF_Player.Show()
        WPF_Player.Hide()

    End Sub





    Public IsShow As Boolean = False

    Private Sub MainView_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        IsShow = True

        If Core.GlobalInstances.AppSettings.DonateCoffe = False Then
            Dim NameNotify As String = "DonateCoffe"
            Dim VLCNotify As Control = CreateButtonNotification(NameNotify, "Buy me a coffee, please!", My.Resources.icons8_bebida_para_llevar_64, New Action(Sub()
                                                                                                                                                                  RemoveNotificationByName(NameNotify)
                                                                                                                                                                  Process.Start("https://www.buymeacoffee.com/s4lsalsoft")
                                                                                                                                                                  Core.GlobalInstances.AppSettings.DonateCoffe = True
                                                                                                                                                                  Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
                                                                                                                                                              End Sub))
            Guna2HtmlToolTip1.SetToolTip(VLCNotify, "You can support this project by buying me a Coffee, <br>  If you have any features in mind, contact me!! <br> You can also join the Discord Community, Check the About section.")
            SendNotification(VLCNotify)
        End If

        If Core.GlobalInstances.AppSettings.Github = False Then
            Dim NameNotify2 As String = "Github"
            Dim VLCNotify2 As Control = CreateButtonNotification(NameNotify2, "Follow me on Github!!", My.Resources.icons8_github_30__1_, New Action(Sub()
                                                                                                                                                         RemoveNotificationByName(NameNotify2)
                                                                                                                                                         Process.Start("https://github.com/DestroyerDarkNess")
                                                                                                                                                         Core.GlobalInstances.AppSettings.Github = True
                                                                                                                                                         Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
                                                                                                                                                     End Sub))
            Guna2HtmlToolTip1.SetToolTip(VLCNotify2, "Follow me on Github!! <br>  You can find out about the changes made to the application, <br> Also if you can create an issue and notify me in the repository called DragonTube,  <br> please notify me if there are any errors or problems.")
            SendNotification(VLCNotify2)
        End If

        If Core.GlobalInstances.AppSettings.FAQ = False Then
            Dim NameNotify4 As String = "FAQ"
            Dim VLCNotify4 As Control = CreateButtonNotification(NameNotify4, "User's Guide and FAQ", My.Resources.icons8_preguntas_25, New Action(Sub()
                                                                                                                                                       RemoveNotificationByName(NameNotify4)
                                                                                                                                                       Process.Start("https://github.com/DestroyerDarkNess/DragonTube")
                                                                                                                                                       Core.GlobalInstances.AppSettings.FAQ = True
                                                                                                                                                       Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
                                                                                                                                                   End Sub))
            Guna2HtmlToolTip1.SetToolTip(VLCNotify4, "All about DragonTube and its Features With images.")
            SendNotification(VLCNotify4)
        End If


        If Core.GlobalInstances.AppSettings.VisualBasic = False Then
            Dim NameNotify3 As String = "VisualBasic"
            Dim VLCNotify3 As Control = CreateButtonNotification(NameNotify3, "Made with VB.NET", My.Resources.icons8_visual_basic_48, New Action(Sub()
                                                                                                                                                      RemoveNotificationByName(NameNotify3)
                                                                                                                                                      Process.Start("https://discord.gg/nffhPHTJpb")
                                                                                                                                                      Core.GlobalInstances.AppSettings.VisualBasic = True
                                                                                                                                                      Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
                                                                                                                                                  End Sub))
            Guna2HtmlToolTip1.SetToolTip(VLCNotify3, "This application is made with VB.NET <br>  Visual Basic DotNet is and will always be Better than C#, <br> If you think the same, enter the VB community.")
            SendNotification(VLCNotify3)
        End If


        If Core.GlobalInstances.AppSettings.Disclaimer = False Then
            Dim DisclaimerForm As New DisclaimerForm
            DisclaimerForm.ShowDialog()
        End If
    End Sub

    Private Sub CheckVersion()
        Dim Client As WebClient = New WebClient

        AddHandler Client.DownloadStringCompleted, Sub(sender As Object, e As DownloadStringCompletedEventArgs)
                                                       Try
                                                           Dim Result As String() = e.Result.Split("|")

                                                           If Result.FirstOrDefault > Core.GlobalInstances.AppVersion Then
                                                               Dim UpdateNotifyEx As String = "NewVersion"
                                                               Dim UpdateNotification As Control = CreateButtonNotification(UpdateNotifyEx, "New version available! ", My.Resources.icons8_instalando_actualizaciones_25, New Action(Sub()
                                                                                                                                                                                                                                         RemoveNotificationByName(UpdateNotifyEx)
                                                                                                                                                                                                                                         Process.Start("https://toolslib.net/downloads/viewdownload/988-dragontube/")
                                                                                                                                                                                                                                     End Sub))
                                                               Guna2HtmlToolTip1.SetToolTip(UpdateNotification, Result.LastOrDefault)
                                                               SendNotification(UpdateNotification)
                                                           End If

                                                       Catch ex As Exception : End Try
                                                   End Sub


        Client.DownloadStringAsync(New Uri("https://raw.githubusercontent.com/DestroyerDarkNess/DragonTube/main/Version"))

    End Sub




#Region " Ads "

    Public NewAdPanel As PictureBox = Nothing
    Public MSG As String = "Contact me!!! :)"
    Public Property ImageAdsUrl As String = String.Empty
    Public Property AdsUrl As String = String.Empty

    Private Sub CheckAds()
        Dim Client As WebClient = New WebClient

        AddHandler Client.DownloadStringCompleted, Sub(sender As Object, e As DownloadStringCompletedEventArgs)
                                                       Try

                                                           Dim ParsedString As String = e.Result
                                                           If ParsedString.EndsWith("|") = False Then
                                                               ParsedString += "|"
                                                           End If

                                                           Dim Result As String() = ParsedString.Split("|")
                                                           ImageAdsUrl = Result.FirstOrDefault
                                                           AdsUrl = Result(1)
                                                           NewAdPanel = New PictureBox
                                                           NewAdPanel.SizeMode = PictureBoxSizeMode.StretchImage
                                                           NewAdPanel.LoadAsync(ImageAdsUrl)
                                                           NewAdPanel.Size = New Size(MusicHub_View.FlowLayoutPanel1.Width - 40, 200)
                                                           NewAdPanel.Cursor = Cursors.Hand
                                                           NewAdPanel.ErrorImage = Nothing
                                                           NewAdPanel.InitialImage = Nothing

                                                           If Result.Count >= 3 Then
                                                               Dim StrMessage As String = Result(2)

                                                               If String.IsNullOrEmpty(StrMessage) = False Then
                                                                   MSG = StrMessage
                                                                   Guna2HtmlToolTip1.SetToolTip(NewAdPanel, StrMessage)
                                                               End If
                                                           End If

                                                           AddHandler NewAdPanel.Click, Sub()
                                                                                            Try

                                                                                                If String.Equals(AdsUrl, "MyAddJS", StringComparison.OrdinalIgnoreCase) = True Then
                                                                                                    ShowADForm()
                                                                                                ElseIf IsYoutube(New Uri(AdsUrl).Host) Then
                                                                                                    YoutubeBrowserButton.Checked = True
                                                                                                    YTBrowser_View.WebView.LoadUrl(AdsUrl)
                                                                                                Else
                                                                                                    Process.Start(AdsUrl)
                                                                                                End If
                                                                                                NewAdPanel = Nothing
                                                                                            Catch ex As Exception : End Try
                                                                                        End Sub
                                                           MusicHub_View.FlowLayoutPanel1.Controls.Add(NewAdPanel)

                                                       Catch ex As Exception : End Try
                                                   End Sub

        Client.DownloadStringAsync(New Uri("https://raw.githubusercontent.com/DestroyerDarkNess/DragonTube/main/Ad"))

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

    Public Sub ShowADForm()
        Me.BeginInvoke(Sub()
                           Guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information
                           Guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
                           Guna2MessageDialog1.Show(MSG, "Email: s4lsalsoft@gmail.com")
                       End Sub)
    End Sub

#End Region

    Private Sub MusicHubButton_CheckedChanged(sender As Object, e As EventArgs) Handles MusicHubButton.CheckedChanged, MusicLocalHub.CheckedChanged, ConfigButton.CheckedChanged, AboutButton.CheckedChanged, YoutubeBrowserButton.CheckedChanged, Guna2Button2.CheckedChanged

        Dim CurrentButton As Guna.UI2.WinForms.Guna2Button = DirectCast(sender, Guna.UI2.WinForms.Guna2Button)

        If CurrentButton Is YoutubeBrowserButton Then
            YTBrowser_View.Visible = True
            YTBrowser_View.BringToFront()
            MusicHub_View.Visible = False
            MusicLocal_View.Visible = False
            Config_View.Visible = False
            About_View.Visible = False
            DownloadMedia_View.Visible = False
        ElseIf CurrentButton Is MusicHubButton Then
            MusicHub_View.Visible = True
            MusicHub_View.BringToFront()
            MusicHub_View.SetScroll(Guna2VScrollBar1)
            YTBrowser_View.Visible = False
            MusicLocal_View.Visible = False
            Config_View.Visible = False
            About_View.Visible = False
            DownloadMedia_View.Visible = False
        ElseIf CurrentButton Is MusicLocalHub Then
            MusicLocal_View.Visible = True
            MusicLocal_View.BringToFront()
            MusicLocal_View.SetScroll(Guna2VScrollBar1)
            YTBrowser_View.Visible = False
            MusicHub_View.Visible = False
            Config_View.Visible = False
            About_View.Visible = False
            DownloadMedia_View.Visible = False
        ElseIf CurrentButton Is ConfigButton Then
            Config_View.Visible = True
            Config_View.BringToFront()
            Config_View.SetScroll(Guna2VScrollBar1)
            YTBrowser_View.Visible = False
            MusicHub_View.Visible = False
            MusicLocal_View.Visible = False
            About_View.Visible = False
            DownloadMedia_View.Visible = False
        ElseIf CurrentButton Is AboutButton Then
            About_View.Visible = True
            About_View.BringToFront()
            YTBrowser_View.Visible = False
            Config_View.Visible = False
            MusicHub_View.Visible = False
            MusicLocal_View.Visible = False
            DownloadMedia_View.Visible = False
        ElseIf CurrentButton Is Guna2Button2 Then
            DownloadMedia_View.Visible = True
            DownloadMedia_View.BringToFront()
            DownloadMedia_View.SetScroll(Guna2VScrollBar1)
            About_View.Visible = False
            YTBrowser_View.Visible = False
            Config_View.Visible = False
            MusicHub_View.Visible = False
            MusicLocal_View.Visible = False
        End If

    End Sub

#Region " Search "

    Private Sub TextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyDown
        If e.KeyData = Keys.Enter Then
            Search(TextBox1.Text)
        End If
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        Search(TextBox1.Text)
    End Sub

    Dim IsBusy As Boolean = False

    Dim cts As CancellationTokenSource = Nothing

    Private Sub Search(ByVal Text As String)

        Dim ID As String = Core.Helpers.GetYouTubeVideoIdFromUrl(Text)
        If String.IsNullOrEmpty(ID) = True Then


            If IsBusy = True Then Exit Sub Else IsBusy = True : Guna2Button1.Enabled = False

            selectedItems.Clear()
            availableItems.Clear()

            Guna2ProgressBar2.Visible = True


            If String.IsNullOrEmpty(Text) = False AndAlso YoutubeBrowserButton.Checked = True Then
                Dim Search As NavigationTask = YTBrowser_View.SearchByYoutube(Text)
                If Search IsNot Nothing Then
                    Search.OnDone(Sub()
                                      IsBusy = False
                                      Guna2Button1.Enabled = True
                                      Guna2ProgressBar2.Visible = False
                                  End Sub)
                End If
            ElseIf String.IsNullOrEmpty(Text) = False AndAlso MusicHubButton.Checked = True Then
                If cts IsNot Nothing Then
                    cts.Cancel()
                    cts = New CancellationTokenSource
                Else
                    cts = New CancellationTokenSource
                End If

                Dim SearchTask As Task(Of Boolean) = MusicHub_View.SearchByYoutube(Youtube, Text, cts.Token)
                SearchTask.GetAwaiter.OnCompleted(Sub()
                                                      IsBusy = False
                                                      Guna2Button1.Enabled = True
                                                  End Sub)

            ElseIf MusicLocalHub.Checked = True Then
                Dim SearchTask As Task(Of Boolean) = MusicLocal_View.SearchByYoutube(Text)
                SearchTask.GetAwaiter.OnCompleted(Sub()
                                                      IsBusy = False
                                                      Guna2Button1.Enabled = True
                                                  End Sub)
            ElseIf Guna2Button2.Checked = True Then
                Dim SearchTask As Task(Of Boolean) = DownloadMedia_View.SearchByYoutube(Text)
                SearchTask.GetAwaiter.OnCompleted(Sub()
                                                      IsBusy = False
                                                      Guna2Button1.Enabled = True
                                                  End Sub)
            End If


        Else
            If Guna2Button2.Checked = True Then
                DownloadMedia("https://www.youtube.com/watch?v=" & ID)
            Else
                TextBox1.Text = String.Empty
                Dim NewLocalMedia As New Core.YoutubeItem With {.Url = "https://www.youtube.com/watch?v=" & ID}
                Dim NewMedia As New MusicItem With {.LocalMedia = NewLocalMedia}
                PlayFromWeb(NewMedia, True)
            End If

        End If

    End Sub

#End Region

#Region " Player "

    Public WithEvents MediaPlayer As Vlc.DotNet.Core.VlcMediaPlayer = Nothing
    Public Property mediaOptions As String() = New String() {"--no-video-title", "--no-stats", "--quiet", "--ignore-config", "--no-sub-autodetect-file"} ' "--input-repeat=10000"
    Private LastMedia As MusicItem = Nothing
    Private LastLocalMedia As LocalMediaEx = Nothing
    Private ctsPlay As CancellationTokenSource = Nothing

    Private IsFilePlaying As Boolean = False

    Public Sub PlayFromFile(ByVal MusicI As LocalMediaEx)

        If MusicI.VideoCodec = "av1" AndAlso Core.GlobalInstances.AppSettings.SupportAVCodec = True Then

            IsFilePlaying = True
            LastLocalMedia = MusicI

            TogglePlayerWindow()

            Try
                MediaPlayer.SetMedia(MusicI.TargetMedia)
                MediaPlayer.Play()
            Catch ex As Exception
                Throw New Exception("To Play AV1 Codec download VLC engine.")
            End Try

        Else

            IsFilePlaying = True
            LastLocalMedia = MusicI

            TogglePlayerWindow()

            If MusicI.TargetMedia IsNot Nothing AndAlso File.Exists(MusicI.TargetMedia.FullName) Then
                If Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.VLC Then
                    MediaPlayer.SetMedia(MusicI.TargetMedia)
                    MediaPlayer.Play()
                ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WMP Then
                    WM_Player.AxWindowsMediaPlayer1.URL = MusicI.TargetMedia.FullName
                    WM_Player.AxWindowsMediaPlayer1.uiMode = "none"
                    WM_Player.AxWindowsMediaPlayer1.stretchToFit = True
                    WM_Player.AxWindowsMediaPlayer1.settings.autoStart = True
                    'WM_Player.AxWindowsMediaPlayer1.settings.setMode("loop", True)
                    'WM_Player.AxWindowsMediaPlayer1.Ctlcontrols.play()
                ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WPF Then
                    WPF_Player.PlayFromFile(MusicI.TargetMedia.FullName)
                Else
                    WPF_Player.PlayFromFile(MusicI.TargetMedia.FullName)
                End If

                Guna2CircleButton1.Checked = True
                Label1.Text = MusicI.TargetMedia.Name '& "  By " & Video.Author.ChannelTitle

                If Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.VLC Then
                    VLC_Player.Text = MusicI.TargetMedia.Name
                    ' VLC_Player.Show()
                    VLC_Player.Guna2GradientPanel1.Visible = True
                ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WMP Then
                    WM_Player.Text = MusicI.TargetMedia.Name
                    '  WM_Player.Show()
                ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WPF Then
                    WPF_Player.Text = MusicI.TargetMedia.Name
                Else
                    WPF_Player.Text = MusicI.TargetMedia.Name
                    ' WPF_Player.Show()
                End If
            End If
        End If

    End Sub

    Public Sub PlayFromWeb(ByVal MusicI As MusicItem, Optional DisposedEnd As Boolean = False)
        'GC.Collect()
        If IsFilePlaying = True Then
            IsFilePlaying = False
            LastLocalMedia = Nothing
            TogglePlayerWindow()
        End If

        LastMedia = MusicI

        If Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.VLC Then
            VLC_Player.Guna2GradientPanel1.Visible = True
        ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WMP Then
            WM_Player.Guna2GradientPanel1.Visible = True
        ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WPF Then
            WPF_Player.Guna2GradientPanel1.Visible = True
        ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine >= 3 Then
            EOWebPlayer.YTPlayer.WebView.LoadUrl("")
        End If

        If ctsPlay IsNot Nothing Then
            ctsPlay.Cancel()
            ctsPlay = New CancellationTokenSource
        Else
            ctsPlay = New CancellationTokenSource
        End If

        Me.BeginInvoke(Sub()
                           Label1.Text = "Loading Media..."
                           Guna2ProgressBar3.Visible = True
                       End Sub)



        Try
            If MusicI.Media IsNot Nothing Then

                If TypeOf MusicI.Media Is VideoSearchResult Then
                    Dim Video As VideoSearchResult = DirectCast(MusicI.Media, VideoSearchResult)

                    If Core.GlobalInstances.AppSettings.MediaPlayerEngine >= 3 Then
                        Dim BaseMode As YTPlayer.YTMode = YTPlayer.YTMode.Embed

                        If Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.YT_Embed Then
                            BaseMode = YTPlayer.YTMode.Embed
                        ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.YT_Native Then
                            BaseMode = YTPlayer.YTMode.Natural
                        End If

                        EOWebPlayer.YTPlayer.WebView.StopLoad()
                        EOWebPlayer.CheckVideo(Video.Id)
                        Dim LoadPage As NavigationTask = EOWebPlayer.YTPlayer.Load(Video.Url, BaseMode)

                    Else
                        Dim SMTask As ValueTask(Of StreamManifest) = Youtube.Videos.Streams.GetManifestAsync(Video.Id, ctsPlay.Token)

                        SMTask.GetAwaiter.OnCompleted(Sub()
                                                          Try
                                                              Dim StreamManifest As StreamManifest = SMTask.Result
                                                              Dim StreamUrl As String = GetStreamByParameters(StreamManifest)

                                                              If String.IsNullOrEmpty(StreamUrl) = True Then
                                                                  Label1.Text = "Error, Server Error!!..."
                                                                  Guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error
                                                                  Guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
                                                                  Guna2ProgressBar3.Visible = False
                                                                  Guna2MessageDialog1.Show("Could not get information from the server.", "Server Error")
                                                                  Exit Sub
                                                              End If

                                                              If Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.VLC Then
                                                                  MediaPlayer.SetMedia(New Uri(StreamUrl))
                                                                  MediaPlayer.Play()
                                                              ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WMP Then
                                                                  WM_Player.AxWindowsMediaPlayer1.URL = StreamUrl
                                                                  WM_Player.AxWindowsMediaPlayer1.uiMode = "none"
                                                                  WM_Player.AxWindowsMediaPlayer1.stretchToFit = True
                                                                  WM_Player.AxWindowsMediaPlayer1.settings.autoStart = True
                                                                  'WM_Player.AxWindowsMediaPlayer1.settings.setMode("loop", True)
                                                                  'WM_Player.AxWindowsMediaPlayer1.Ctlcontrols.play()
                                                              ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WPF Then
                                                                  WPF_Player.PlayWithServer(StreamUrl)
                                                              End If

                                                              Guna2CircleButton1.Checked = True
                                                              Label1.Text = Video.Title & "  By " & Video.Author.ChannelTitle


                                                              If Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.VLC Then
                                                                  VLC_Player.Text = Video.Title
                                                                  ' VLC_Player.Show()
                                                                  VLC_Player.Guna2GradientPanel1.Visible = True
                                                              ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WMP Then
                                                                  WM_Player.Text = Video.Title
                                                                  '  WM_Player.Show()
                                                              ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WPF Then
                                                                  WPF_Player.Text = Video.Title
                                                                  ' WPF_Player.Show()
                                                              End If
                                                              TogglePlayerWindow()

                                                              'Guna2Panel1.BackgroundImage = Core.Helpers.ApplyBlurToImage(MusicI.Guna2PictureBox1.Image)

                                                          Catch ex As Exception
                                                              Label1.Text = "Error!!..."
                                                              Guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error
                                                              Guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
                                                              Guna2ProgressBar3.Visible = False
                                                              Guna2MessageDialog1.Show(ex.Message, "Server Error")
                                                          End Try
                                                      End Sub)
                    End If

                End If

            ElseIf MusicI.LocalMedia IsNot Nothing Then


                If Core.GlobalInstances.AppSettings.MediaPlayerEngine >= 3 Then
                    Dim BaseMode As YTPlayer.YTMode = YTPlayer.YTMode.Embed

                    If Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.YT_Embed Then
                        BaseMode = YTPlayer.YTMode.Embed
                    ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.YT_Native Then
                        BaseMode = YTPlayer.YTMode.Natural
                    End If

                    EOWebPlayer.YTPlayer.WebView.StopLoad()
                    EOWebPlayer.CheckVideo(MusicI.LocalMedia.ID)
                    Dim LoadPage As NavigationTask = EOWebPlayer.YTPlayer.Load(MusicI.LocalMedia.Url, BaseMode)

                Else

                    Dim SMTask As ValueTask(Of StreamManifest) = Youtube.Videos.Streams.GetManifestAsync(MusicI.LocalMedia.Url, ctsPlay.Token)

                    SMTask.GetAwaiter.OnCompleted(Sub()
                                                      Try
                                                          Dim StreamManifest As StreamManifest = SMTask.Result
                                                          Dim StreamUrl As String = GetStreamByParameters(StreamManifest)

                                                          If String.IsNullOrEmpty(StreamUrl) = True Then
                                                              Label1.Text = "Error, Server Error!!..."
                                                              Guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error
                                                              Guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
                                                              Guna2ProgressBar3.Visible = False
                                                              Guna2MessageDialog1.Show("Could not get information from the server.", "Server Error")
                                                              Exit Sub
                                                          End If


                                                          If Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.VLC Then
                                                              MediaPlayer.SetMedia(New Uri(StreamUrl))
                                                              MediaPlayer.Play()
                                                          ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WMP Then
                                                              WM_Player.AxWindowsMediaPlayer1.URL = StreamUrl
                                                              WM_Player.AxWindowsMediaPlayer1.uiMode = "none"
                                                              WM_Player.AxWindowsMediaPlayer1.stretchToFit = True
                                                              WM_Player.AxWindowsMediaPlayer1.settings.autoStart = True
                                                              'WM_Player.AxWindowsMediaPlayer1.settings.setMode("loop", True)
                                                              'WM_Player.AxWindowsMediaPlayer1.Ctlcontrols.play()
                                                          ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WPF Then
                                                              WPF_Player.PlayWithServer(StreamUrl)
                                                          End If

                                                          Guna2CircleButton1.Checked = True
                                                          Label1.Text = MusicI.LocalMedia.Title & "  By " & MusicI.LocalMedia.Author

                                                          If Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.VLC Then
                                                              VLC_Player.Text = MusicI.LocalMedia.Title
                                                              '  VLC_Player.Show()
                                                              VLC_Player.Guna2GradientPanel1.Visible = True
                                                          ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WMP Then
                                                              WM_Player.Text = MusicI.LocalMedia.Title
                                                              '   WM_Player.Show()
                                                          ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WPF Then
                                                              WPF_Player.Text = MusicI.LocalMedia.Title
                                                              ' WPF_Player.Show()
                                                          End If
                                                          TogglePlayerWindow()

                                                          'Guna2Panel1.BackgroundImage = Core.Helpers.ApplyBlurToImage(MusicI.Guna2PictureBox1.Image)

                                                      Catch ex As Exception
                                                          Label1.Text = "Error!!..."
                                                          Guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error
                                                          Guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
                                                          Guna2ProgressBar3.Visible = False
                                                          Guna2MessageDialog1.Show(ex.Message, "Server Error")
                                                      End Try
                                                  End Sub)
                End If


            End If

        Catch ex As Exception : End Try

        If DisposedEnd = True Then
            MusicI.Dispose()
        End If

    End Sub


    Private Function GetStreamByParameters(ByVal StreamManifest As StreamManifest) As String
        Dim StreamUrl As String = String.Empty

        Try
            Dim streamInfos As IEnumerable(Of Object)

            If Core.GlobalInstances.AppSettings.Quality = Core.VideoQuality.None Then
                streamInfos = StreamManifest.GetAudioOnlyStreams.OrderBy(Function(stream) stream.Bitrate.BitsPerSecond)
            Else
                streamInfos = StreamManifest.GetMuxedStreams.OrderBy(Function(stream) stream.VideoQuality.ToString())
            End If

            Dim count As Integer = streamInfos.Count()

            If count > 0 Then
                Dim index As Integer = 0
                Dim Selected As VideoBitrate = If(Core.GlobalInstances.AppSettings.Quality = Core.VideoQuality.None, Core.GlobalInstances.AppSettings.BitRate, Core.GlobalInstances.AppSettings.Quality)

                Select Case Selected
                    Case Core.VideoBitrate.High
                        index = If(count >= 2, count - 1, 0)
                    Case Core.VideoBitrate.Medium
                        If count = 3 Then
                            index = 1
                        ElseIf count = 4 Then
                            index = count - 2
                        ElseIf count >= 5 Then
                            index = count - 3
                        End If
                    Case Core.VideoBitrate.Low
                        index = 0
                End Select

                Dim ItemSelected As Object = streamInfos.ElementAt(index)

                If TypeOf ItemSelected Is MuxedStreamInfo Then
                    Dim MuxedStream As MuxedStreamInfo = DirectCast(ItemSelected, MuxedStreamInfo)
                    StreamUrl = MuxedStream.Url
                ElseIf TypeOf ItemSelected Is AudioOnlyStreamInfo Then
                    Dim AudioOnlyStream As AudioOnlyStreamInfo = DirectCast(ItemSelected, AudioOnlyStreamInfo)
                    StreamUrl = AudioOnlyStream.Url
                End If

            End If
        Catch ex As Exception : End Try

        Return StreamUrl
    End Function

    Private Function CreateMediaPlayer() As Vlc.DotNet.Core.VlcMediaPlayer
        Try
            Dim libDirectory As DirectoryInfo = New DirectoryInfo(Core.GlobalInstances.VLCDir)

            If libDirectory.Exists = False Then

                If Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.VLC Then
                    Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WPF
                    Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
                    Config_View.LoadConfig()
                End If

                Dim NameNotify As String = "VLCDownloader"
                Dim VLCNotify As Control = CreateButtonNotification(NameNotify, "Download VLC Engine Now!", My.Resources.icons8_vlc_20, New Action(
                                                                    Sub()
                                                                        RemoveNotificationByName(NameNotify)

                                                                        Dim ProgressName As String = "VLC.Downloader"
                                                                        Dim NewProgress As New ProgressNotification With {.Name = ProgressName}
                                                                        SendNotification(NewProgress)

                                                                        NewProgress.OnLoadAction = New Action(Async Sub()

                                                                                                                  NewProgress.LogInLabel1.Text = "Downloading VLC Engine..."
                                                                                                                  Dim VLC_Url As String = "https://github.com/DestroyerDarkNess/DragonTube/raw/main/libvlc.7z"
                                                                                                                  Dim VLC_ZipDir As String = IO.Path.Combine(IO.Path.GetTempPath, "libvlc.7z")
                                                                                                                  Dim wClient As New DownloadFileAsyncExtended

                                                                                                                  Try
                                                                                                                      If IO.File.Exists(VLC_ZipDir) Then
                                                                                                                          IO.File.Delete(VLC_ZipDir)
                                                                                                                      End If
                                                                                                                  Catch ex As Exception : End Try

                                                                                                                  AddHandler wClient.DownloadProgressChanged, Sub(ByVal sender As Object, ByVal e As FileDownloadProgressChangedEventArgs)
                                                                                                                                                                  NewProgress.Guna2ProgressBar1.Value = e.ProgressPercentage
                                                                                                                                                              End Sub

                                                                                                                  AddHandler wClient.DownloadCompleted, Async Sub(ByVal sender As Object, ByVal e As FileDownloadCompletedEventArgs)
                                                                                                                                                            If e.ErrorMessage IsNot Nothing Then '// Was there an error.

                                                                                                                                                                Dim NameNotifyEx As String = "VLCDownloaderEx"
                                                                                                                                                                Dim VLCNotifyEx As Control = CreateButtonNotification(NameNotifyEx, "Error downloading VLC Engine!", My.Resources.icons8_vlc_20, New Action(Sub()
                                                                                                                                                                                                                                                                                                                RemoveNotificationByName(NameNotifyEx)
                                                                                                                                                                                                                                                                                                            End Sub))
                                                                                                                                                                Guna2HtmlToolTip1.SetToolTip(VLCNotifyEx, e.ErrorMessage.Message)
                                                                                                                                                                RemoveNotificationByName(ProgressName)
                                                                                                                                                            ElseIf e.Cancelled Then '// The user cancelled the download.
                                                                                                                                                                If wClient.IsBusy = False Then wClient.ResumeAsync()
                                                                                                                                                            Else
                                                                                                                                                                Core.Helpers.Sleep(1)
                                                                                                                                                                wClient.CloseStream()
                                                                                                                                                                Core.Helpers.Sleep(1)
                                                                                                                                                                If IO.File.Exists(VLC_ZipDir) Then
                                                                                                                                                                    Try
                                                                                                                                                                        NewProgress.Guna2ProgressBar1.Style = ProgressBarStyle.Marquee
                                                                                                                                                                        NewProgress.LogInLabel1.Text = "Installing, do not close the application..."
                                                                                                                                                                        Dim AsyncThread As Thread = New Thread(Sub()
                                                                                                                                                                                                                   Using archiveFile As ArchiveFile = New ArchiveFile(VLC_ZipDir)
                                                                                                                                                                                                                       archiveFile.Extract(Core.GlobalInstances.AppChacheFolder, True)
                                                                                                                                                                                                                   End Using
                                                                                                                                                                                                                   Me.BeginInvoke(Sub()
                                                                                                                                                                                                                                      Dim NameNotifyEx As String = "ResetVLCEX"
                                                                                                                                                                                                                                      Dim VLCNotifyEx As Control = CreateButtonNotification(NameNotifyEx, "Reset the application!", My.Resources.icons8_cita_recurrente_20, New Action(Sub()
                                                                                                                                                                                                                                                                                                                                                                                           RemoveNotificationByName(NameNotifyEx)
                                                                                                                                                                                                                                                                                                                                                                                           Process.Start(Application.ExecutablePath)
                                                                                                                                                                                                                                                                                                                                                                                           Environment.Exit(0)
                                                                                                                                                                                                                                                                                                                                                                                       End Sub))
                                                                                                                                                                                                                                      Guna2HtmlToolTip1.SetToolTip(VLCNotifyEx, "Restart the application, After restart you will see the VLC option available in settings.")

                                                                                                                                                                                                                                      Core.Helpers.Sleep(5)

                                                                                                                                                                                                                                      Me.RemoveNotificationByName(ProgressName)
                                                                                                                                                                                                                                      Me.SendNotification(VLCNotifyEx)
                                                                                                                                                                                                                                  End Sub)
                                                                                                                                                                                                               End Sub)
                                                                                                                                                                        AsyncThread.Start()
                                                                                                                                                                    Catch ex As Exception
                                                                                                                                                                        Dim NameNotifyEx As String = "VLCDownloaderEx"
                                                                                                                                                                        Dim VLCNotifyEx As Control = CreateButtonNotification(NameNotifyEx, "Error extracting VLC Engine!", My.Resources.icons8_vlc_20, New Action(Sub()
                                                                                                                                                                                                                                                                                                                       RemoveNotificationByName(NameNotifyEx)
                                                                                                                                                                                                                                                                                                                   End Sub))
                                                                                                                                                                        Guna2HtmlToolTip1.SetToolTip(VLCNotifyEx, ex.Message)
                                                                                                                                                                    End Try
                                                                                                                                                                Else
                                                                                                                                                                    RemoveNotificationByName(ProgressName)
                                                                                                                                                                End If


                                                                                                                                                            End If
                                                                                                                                                        End Sub

                                                                                                                  wClient.SynchronizingObject = Me
                                                                                                                  wClient.ProgressUpdateFrequency = DownloadFileAsyncExtended.UpdateFrequency.Second

                                                                                                                  wClient.DowloadFileAsync(VLC_Url, VLC_ZipDir)
                                                                                                              End Sub)

                                                                        NewProgress.OnLoadAction.Invoke
                                                                    End Sub))
                Guna2HtmlToolTip1.SetToolTip(VLCNotify, "It is not necessary, DragonTube already comes with +3 Player Engines, <br>
                                                         but if you have problems playing videos, <br> then you can download VLC engine.")
                SendNotification(VLCNotify)
            End If

            VLC_Player = New VLCPlayer(libDirectory, mediaOptions) With {.FormParent = Me}

            Return VLC_Player.vlcControl.VlcMediaPlayer
        Catch ex As Exception
            'Guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error
            'Guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
            'Guna2MessageDialog1.Show(ex.Message, "CreateMediaPlayer Error")
            Return Nothing
        End Try
    End Function

    Public Function ShowMessage(ByVal Message As String, Optional ByVal title As String = "DragonTube") As DialogResult
        Guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information
        Guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
        Return Guna2MessageDialog1.Show(Message, title)
    End Function

    Private Sub Guna2ProgressBar1_MouseDown(sender As Object, e As MouseEventArgs) Handles Guna2ProgressBar1.MouseDown
        Try
            Dim newPosition = e.X / Guna2ProgressBar1.Width

            If Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.VLC Then
                Dim duration = MediaPlayer.Length / 1000
                Dim newPositionSeconds = newPosition * duration
                MediaPlayer.Time = CULng(newPositionSeconds * 1000)
            ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WMP Then
                Dim newPositionEx As Double = newPosition * WM_Player.AxWindowsMediaPlayer1.currentMedia.duration
                WM_Player.AxWindowsMediaPlayer1.Ctlcontrols.currentPosition = newPositionEx
            ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WPF Then
                Dim newPositionSeconds As Double = newPosition * WPF_Player.Player.NaturalDuration.TimeSpan.TotalSeconds
                WPF_Player.Player.Position = TimeSpan.FromSeconds(newPositionSeconds)
            ElseIf IsFilePlaying = True Then
                Dim newPositionSeconds As Double = newPosition * WPF_Player.Player.NaturalDuration.TimeSpan.TotalSeconds
                WPF_Player.Player.Position = TimeSpan.FromSeconds(newPositionSeconds)
            ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine >= 3 Then
                Dim newPositionSeconds As Double = newPosition * EOWebPlayer.YTPlayer.GetDurationTime.TotalSeconds
                EOWebPlayer.YTPlayer.SetCurrentTime(newPositionSeconds)
            End If

            Guna2ProgressBar1.Value = CInt(newPosition * 100)
        Catch ex As Exception : End Try
    End Sub


    Private Sub Guna2CircleButton1_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton1.Click
        ChangeMediaState()
    End Sub

    Public Sub ChangeMediaState()
        Try
            If Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.VLC Then
                If Guna2CircleButton1.Checked = False Then
                    MediaPlayer.Pause()
                Else
                    MediaPlayer.Play()
                End If
            ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WMP Then
                If Guna2CircleButton1.Checked = False Then
                    WM_Player.AxWindowsMediaPlayer1.Ctlcontrols.pause()
                Else
                    WM_Player.AxWindowsMediaPlayer1.Ctlcontrols.play()
                End If
            ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WPF Then
                If Guna2CircleButton1.Checked = False Then
                    WPF_Player.Player.Pause()
                Else
                    WPF_Player.Player.Play()
                End If
            ElseIf IsFilePlaying = True Then
                If Guna2CircleButton1.Checked = False Then
                    WPF_Player.Player.Pause()
                Else
                    WPF_Player.Player.Play()
                End If
            ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine >= 3 Then
                If Guna2CircleButton1.Checked = False Then
                    EOWebPlayer.YTPlayer.Pause()
                Else
                    EOWebPlayer.YTPlayer.Play()
                End If
            End If
        Catch ex As Exception : End Try
    End Sub

#End Region

#Region " VLC Events "

    Private Sub MediaPlayer_PositionChanged(sender As Object, e As VlcMediaPlayerPositionChangedEventArgs) Handles MediaPlayer.PositionChanged
        Try
            If Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.VLC Then
                Dim CurrentPos As Integer = CInt(Math.Floor(e.NewPosition * 100))
                Guna2ProgressBar1.Value = CurrentPos

                Dim currentTimeInSeconds As Integer = CInt(MediaPlayer.Time / 1000)

                Me.BeginInvoke(Sub()
                                   Dim formattedTime As String = Core.Helpers.FormattedTimeToString(currentTimeInSeconds)

                                   If LastMedia IsNot Nothing AndAlso String.IsNullOrEmpty(LastMedia.Label2.Text) = False Then
                                       formattedTime += " / " & LastMedia.Label2.Text
                                   ElseIf VLC_Player.Media IsNot Nothing Then
                                       formattedTime += " / " & Core.Helpers.FormattedTimeToString(VLC_Player.Media.Duration.TotalSeconds)
                                   End If
                                   Label2.Text = formattedTime
                               End Sub)
            End If
        Catch ex As Exception : End Try
    End Sub

    Private Sub MediaPlayer_EncounteredError(sender As Object, e As VlcMediaPlayerEncounteredErrorEventArgs) Handles MediaPlayer.EncounteredError
        Me.BeginInvoke(Sub()
                           Dim Message As String = "An error occurred"
                           Guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error
                           Guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
                           Guna2MessageDialog1.Show(Message, "Player Error")
                       End Sub)
    End Sub

    Private Sub MediaPlayer_EndReached(sender As Object, e As VlcMediaPlayerEndReachedEventArgs) Handles MediaPlayer.EndReached
        Me.BeginInvoke(Sub()
                           If PlayMode = Core.PlayMode.Loop Then
                               MediaPlayer.Stop()
                               MediaPlayer.Time = CULng(0 * 1000)
                               MediaPlayer.Play()
                           Else
                               StartNewSelection()
                           End If
                       End Sub)
    End Sub

    Private Sub MediaPlayer_Playing(sender As Object, e As VlcMediaPlayerPlayingEventArgs) Handles MediaPlayer.Playing
        Me.BeginInvoke(Sub()
                           VLC_Player.Guna2GradientPanel1.Visible = False
                           Guna2ProgressBar3.Visible = False
                       End Sub)
        Try
            If VLC_Player IsNot Nothing Then
                VLC_Player.Media = MediaPlayer.GetMedia()
                VLC_Player.Media.Parse()
            End If
        Catch ex As Exception : End Try

    End Sub

#End Region

#Region " Windows Media Player "

    Private Sub AxWindowsMediaPlayer1_PlayStateChange(sender As Object, e As _WMPOCXEvents_PlayStateChangeEvent)
        If e.newState = 3 Then
            Me.BeginInvoke(Sub()
                               WM_Player.Guna2GradientPanel1.Visible = False
                               Guna2ProgressBar3.Visible = False
                           End Sub)
        ElseIf e.newState = 8 Then
            If PlayMode = Core.PlayMode.Loop Then
                WM_Player.AxWindowsMediaPlayer1.Ctlcontrols.currentPosition = 0
                WM_Player.AxWindowsMediaPlayer1.Ctlcontrols.play()
            Else
                WM_Player.Timer1.Stop()
                StartNewSelection()
            End If
        ElseIf e.newState = 1 Then
            If PlayMode = Core.PlayMode.Loop Then
                WM_Player.AxWindowsMediaPlayer1.Ctlcontrols.currentPosition = 0
                WM_Player.AxWindowsMediaPlayer1.Ctlcontrols.play()
            End If
        End If
    End Sub

    Private Sub WM_Player_PositionChange(newPosition As Double, CurrentTime As Double) Handles WM_Player.PositionChange
        Try
            If Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WMP Then
                Dim CurrentPos As Integer = 100 * newPosition / CurrentTime
                Guna2ProgressBar1.Value = CurrentPos

                Dim currentTimeInSeconds As Integer = CInt(newPosition)

                Me.BeginInvoke(Sub()

                                   Dim formattedTime As String = Core.Helpers.FormattedTimeToString(currentTimeInSeconds)

                                   If LastMedia IsNot Nothing AndAlso String.IsNullOrEmpty(LastMedia.Label2.Text) = False Then
                                       formattedTime += " / " & LastMedia.Label2.Text
                                   Else
                                       formattedTime += " / " & Core.Helpers.FormattedTimeToString(CurrentTime)
                                   End If


                                   Label2.Text = formattedTime
                               End Sub)
            End If
        Catch ex As Exception : End Try
    End Sub

    Private Sub AxWindowsMediaPlayer1_MediaError(sender As Object, e As _WMPOCXEvents_MediaErrorEvent)
        Me.BeginInvoke(Sub()
                           Dim Message As String = "An error occurred"

                           If IsFilePlaying = True AndAlso LastLocalMedia IsNot Nothing AndAlso LastLocalMedia.VideoCodec = "av1" Then
                               Message = "Enable 'Support AV1 codec' in the app settings!"
                           End If

                           Guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error
                           Guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
                           Guna2MessageDialog1.Show(Message, "Player Error")
                       End Sub)
    End Sub

#End Region

#Region " WebPlayer Events "

    Private Sub WebPlayer_OnStateChange(ByVal IsOnPlay As Boolean)
        Guna2CircleButton1.Checked = IsOnPlay
    End Sub

    Private Sub WebPlayer_PositionChange(ByVal newPosition As TimeSpan, ByVal DurationTime As TimeSpan)
        Try

            Dim progressPercent As Double = (newPosition.TotalSeconds / DurationTime.TotalSeconds) * 100

            Guna2ProgressBar1.Value = Convert.ToInt32(progressPercent)

            Dim currentTimeInSeconds As Integer = CInt(newPosition.TotalSeconds)

            Dim DurationTimeInSeconds As Integer = CInt(DurationTime.TotalSeconds)

            Dim formattedTime As String = Core.Helpers.FormattedTimeToString(currentTimeInSeconds)

            formattedTime += " / " & Core.Helpers.FormattedTimeToString(DurationTimeInSeconds)

            Label2.Text = formattedTime
        Catch ex As Exception : End Try
    End Sub


#End Region

#Region " WPF Player Events "

    Private Sub WPF_Player_PositionChange(newPosition As TimeSpan, DurationTime As TimeSpan) Handles WPF_Player.PositionChange
        Try
            If Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WPF OrElse IsFilePlaying = True Then
                Dim progressPercent As Double = (newPosition.TotalSeconds / DurationTime.TotalSeconds) * 100

                Guna2ProgressBar1.Value = Convert.ToInt32(progressPercent)

                Dim currentTimeInSeconds As Integer = CInt(newPosition.TotalSeconds)

                Dim formattedTime As String = Core.Helpers.FormattedTimeToString(currentTimeInSeconds)

                If LastMedia IsNot Nothing AndAlso String.IsNullOrEmpty(LastMedia.Label2.Text) = False Then
                    formattedTime += " / " & LastMedia.Label2.Text
                Else
                    formattedTime += " / " & Core.Helpers.FormattedTimeToString(DurationTime.TotalSeconds)
                End If

                Label2.Text = formattedTime
            End If
        Catch ex As Exception : End Try
    End Sub


    Private Sub Player_MediaFailed(sender As Object, e As Windows.ExceptionRoutedEventArgs)
        Me.BeginInvoke(Sub()
                           Dim Message As String = e.ErrorException.Message

                           If IsFilePlaying = True AndAlso LastLocalMedia IsNot Nothing AndAlso LastLocalMedia.VideoCodec = "av1" Then
                               Message = "Enable 'Support AV1 codec' in the app settings!"
                           End If

                           Guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Error
                           Guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK
                           Guna2MessageDialog1.Show(Message, "Player Error")
                       End Sub)
    End Sub

    Private Sub Player_MediaEnded(sender As Object, e As Windows.RoutedEventArgs)
        If PlayMode = Core.PlayMode.Loop Then
            WPF_Player.Player.Position = TimeSpan.FromMilliseconds(1.0)
            WPF_Player.Player.Play()
        Else
            WPF_Player.Timer1.Stop()
            StartNewSelection()
        End If
    End Sub

    Private Sub Player_MediaOpened(sender As Object, e As Windows.RoutedEventArgs)
        Me.BeginInvoke(Sub()
                           WPF_Player.Guna2GradientPanel1.Visible = False
                           Guna2ProgressBar3.Visible = False
                           WPF_Player.Timer1.Start()
                       End Sub)
    End Sub


#End Region

#Region " VideoWindow "

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function SetParent(hWndChild As IntPtr, hWndNewParent As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll")>
    Private Shared Function ShowWindow(hWnd As IntPtr, nCmdShow As Integer) As Boolean
    End Function

    Private Const SW_HIDE As Integer = 0 ' Oculta la ventana
    Private Const SW_SHOW As Integer = 5 ' Muestra la ventana

    Private Sub GdiPlusLabel1_Click(sender As Object, e As EventArgs) Handles GdiPlusLabel1.Click
        Guna2CheckBox1.Checked = Not Guna2CheckBox1.Checked
        TogglePlayerWindow()
    End Sub

    Private Sub Guna2CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2CheckBox1.CheckedChanged
        Dim MouseOver As Boolean = Core.Helpers.MouseOverControl(Guna2CheckBox1, Guna2Panel2)

        If MouseOver Then
            TogglePlayerWindow()
        End If
    End Sub


    Public Function TogglePlayerWindow()

        If IsShow = False Then
            Return False
        End If

        Dim TargetForm As Form = Nothing

        If Core.GlobalInstances.AppSettings.SupportAVCodec Then
            If IsFilePlaying = True AndAlso LastLocalMedia IsNot Nothing AndAlso LastLocalMedia.VideoCodec = "av1" Then
                WM_Player.Hide() : WM_Player.AxWindowsMediaPlayer1.Ctlcontrols.stop() : WM_Player.Timer1.Stop() : WM_Player.AxWindowsMediaPlayer1.currentPlaylist.clear()
                WPF_Player.Hide() : WPF_Player.Player.Stop() : WPF_Player.Timer1.Stop()
                EOWebPlayer.Hide() : EOWebPlayer.YTPlayer.Pause()
                TargetForm = VLC_Player
            Else
                If VLC_Player IsNot Nothing Then VLC_Player.Hide() : MediaPlayer?.Stop()
            End If
        End If

        If TargetForm Is Nothing Then
            If Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.VLC Then
                WM_Player.Hide() : WM_Player.AxWindowsMediaPlayer1.Ctlcontrols.stop() : WM_Player.Timer1.Stop() : WM_Player.AxWindowsMediaPlayer1.currentPlaylist.clear()
                WPF_Player.Hide() : WPF_Player.Player.Stop() : WPF_Player.Timer1.Stop()
                EOWebPlayer.Hide() : EOWebPlayer.YTPlayer.Pause()
                TargetForm = VLC_Player
            ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WMP Then
                If VLC_Player IsNot Nothing Then VLC_Player.Hide() : MediaPlayer?.Stop()
                WPF_Player.Hide() : WPF_Player.Player.Stop() : WPF_Player.Timer1.Stop()
                EOWebPlayer.Hide() : EOWebPlayer.YTPlayer.Pause()
                TargetForm = WM_Player
            ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WPF Then
                WM_Player.Hide() : WM_Player.AxWindowsMediaPlayer1.Ctlcontrols.stop() : WM_Player.Timer1.Stop() : WM_Player.AxWindowsMediaPlayer1.currentPlaylist.clear()
                If VLC_Player IsNot Nothing Then VLC_Player.Hide() : MediaPlayer?.Stop()
                EOWebPlayer.Hide() : EOWebPlayer.YTPlayer.Pause()
                TargetForm = WPF_Player
            ElseIf IsFilePlaying = True Then
                WM_Player.Hide() : WM_Player.AxWindowsMediaPlayer1.Ctlcontrols.stop() : WM_Player.Timer1.Stop() : WM_Player.AxWindowsMediaPlayer1.currentPlaylist.clear()
                If VLC_Player IsNot Nothing Then VLC_Player.Hide() : MediaPlayer?.Stop()
                EOWebPlayer.Hide() : EOWebPlayer.YTPlayer.Pause()
                TargetForm = WPF_Player
            ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine >= 3 Then
                WM_Player.Hide() : WM_Player.AxWindowsMediaPlayer1.Ctlcontrols.stop() : WM_Player.Timer1.Stop() : WM_Player.AxWindowsMediaPlayer1.currentPlaylist.clear()
                If VLC_Player IsNot Nothing Then VLC_Player.Hide() : MediaPlayer?.Stop()
                WPF_Player.Hide() : WPF_Player.Player.Stop() : WPF_Player.Timer1.Stop()
                TargetForm = EOWebPlayer
            End If
        End If

        If TargetForm Is Nothing Then
            Return False
        End If

        If Guna2CheckBox1.Checked = True Then
            If TargetForm.Visible = False Then

                ShowWindow(TargetForm.Handle, SW_SHOW)

                Dim MyWindowLocation As String = Core.GlobalInstances.AppSettings.VideoLocation

                If String.IsNullOrEmpty(MyWindowLocation) Then
                    Core.Helpers.PositionFormInBottomRightCorner(TargetForm)
                Else
                    Try
                        Dim SplitPos As String() = MyWindowLocation.Split(",")
                        Dim WindowPos As Point = New Point(SplitPos.First, SplitPos.Last)
                        TargetForm.Location = WindowPos
                    Catch ex As Exception
                        Core.GlobalInstances.AppSettings.VideoLocation = ""
                        Core.Helpers.PositionFormInBottomRightCorner(TargetForm)
                    End Try
                End If

                Dim MyWindowSize As String = Core.GlobalInstances.AppSettings.VideoSize

                If String.IsNullOrEmpty(MyWindowSize) = False Then
                    Try
                        Dim SplitPos As String() = MyWindowSize.Split(",")
                        TargetForm.Width = SplitPos.First
                        TargetForm.Height = SplitPos.Last
                    Catch ex As Exception
                        Core.GlobalInstances.AppSettings.VideoSize = ""
                    End Try
                End If

            Else
                Core.GlobalInstances.AppSettings.VideoLocation = String.Format("{0},{1}", TargetForm.Location.X, TargetForm.Location.Y)
                Core.GlobalInstances.AppSettings.VideoSize = String.Format("{0},{1}", TargetForm.Width, TargetForm.Height)
            End If
        Else
            If TargetForm.Visible = True Then
                Core.GlobalInstances.AppSettings.VideoLocation = String.Format("{0},{1}", TargetForm.Location.X, TargetForm.Location.Y)
                Core.GlobalInstances.AppSettings.VideoSize = String.Format("{0},{1}", TargetForm.Width, TargetForm.Height)
                ShowWindow(TargetForm.Handle, SW_HIDE)
            End If
        End If

        If Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.VLC Then
            VLC_Player.UpdateSpectRatio()
        End If

        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
        Return True
    End Function

    Private Sub NotifyIcon1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        If Me.Visible = True Then
            Me.Hide()
        Else
            Me.Show()
        End If
    End Sub

    Private Sub ShowHideToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowHideToolStripMenuItem.Click
        If Me.Visible = True Then
            Me.Hide()
        Else
            ShowWindow(Me.Handle, SW_SHOW)
            Me.Show()
        End If
    End Sub

    Private Sub Guna2TrackBar1_Scroll(sender As Object, e As ScrollEventArgs) Handles Guna2TrackBar1.Scroll

        If Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.VLC Then
            MediaPlayer.Audio.Volume = Guna2TrackBar1.Value
        ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WMP Then
            WM_Player.AxWindowsMediaPlayer1.settings.volume = Guna2TrackBar1.Value
        ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WPF Then
            Dim Value As Double = (Guna2TrackBar1.Value * 1.0) / Guna2TrackBar1.Maximum
            WPF_Player.Player.Volume = Value
        ElseIf IsFilePlaying = True Then
            Dim Value As Double = (Guna2TrackBar1.Value * 1.0) / Guna2TrackBar1.Maximum
            WPF_Player.Player.Volume = Value
        ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine >= 3 Then
            Dim Value As Double = (Guna2TrackBar1.Value * 1.0) / Guna2TrackBar1.Maximum
            EOWebPlayer.YTPlayer.SetVolume(Value)
        End If

        Guna2HtmlToolTip1.SetToolTip(Guna2TrackBar1, "Volume: " & Guna2TrackBar1.Value & "%")
    End Sub

    Private Sub Guna2CircleButton5_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2CircleButton5.CheckedChanged

        If Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.VLC Then
            MediaPlayer?.Audio.ToggleMute()
        ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WMP Then
            If WM_Player IsNot Nothing Then WM_Player.AxWindowsMediaPlayer1.settings.mute = Not Guna2CircleButton5.Checked
        ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine = Core.MediaPlayer.WPF Then
            WPF_Player.Player.IsMuted = Not Guna2CircleButton5.Checked
        ElseIf IsFilePlaying = True Then
            WPF_Player.Player.IsMuted = Not Guna2CircleButton5.Checked
        ElseIf Core.GlobalInstances.AppSettings.MediaPlayerEngine >= 3 Then
            EOWebPlayer.YTPlayer.SetMute(Not Guna2CircleButton5.Checked)
        End If
    End Sub

#End Region

#Region " Settings "
    Private Sub MainView_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        SaveMainSetting()
        If Me.Visible = True Then
            ShowHideToolStripMenuItem.Text = "Hide"
        Else
            ShowHideToolStripMenuItem.Text = "Show"
        End If
    End Sub

    Private Function SaveMainSetting() As Boolean
        Try
            Core.GlobalInstances.AppSettings.Volume = Guna2TrackBar1.Value
            Core.GlobalInstances.AppSettings.VideoWindow = Guna2CheckBox1.Checked
            Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private _Loop As Image = Core.Helpers.BToImage(Core.GlobalInstances.LoopCode)
    Private _Next As Image = Core.Helpers.BToImage(Core.GlobalInstances.NextCode)
    Private _Random As Image = Core.Helpers.BToImage(Core.GlobalInstances.RandomCode)

    Private Sub LoadMainSetting()
        Try
            ToggleNotifications()
            PlayMode = Core.GlobalInstances.AppSettings.PlayerMode

            If PlayMode = Core.PlayMode.Loop Then
                PlayMode = Core.PlayMode.Loop
                Guna2CircleButton6.Image = _Loop
                Guna2HtmlToolTip1.SetToolTip(Guna2CircleButton6, "Loop Mode : Repeat the same media on a loop.")
            ElseIf PlayMode = Core.PlayMode.Next Then
                PlayMode = Core.PlayMode.Next
                Guna2CircleButton6.Image = _Next
                Guna2HtmlToolTip1.SetToolTip(Guna2CircleButton6, "Next Mode : Switch to the next media on the list.")
            ElseIf PlayMode = Core.PlayMode.Random Then
                PlayMode = Core.PlayMode.Random
                Guna2CircleButton6.Image = _Random
                Guna2HtmlToolTip1.SetToolTip(Guna2CircleButton6, "Random Mode : Switch to another media in the list randomly.")
            End If

            Guna2TrackBar1.Value = Core.GlobalInstances.AppSettings.Volume
            Guna2CheckBox1.Checked = Core.GlobalInstances.AppSettings.VideoWindow
            If MediaPlayer IsNot Nothing Then
                MediaPlayer.Audio.Volume = Guna2TrackBar1.Value
            End If
            WM_Player.AxWindowsMediaPlayer1.settings.volume = Guna2TrackBar1.Value
            Dim Value As Double = (Guna2TrackBar1.Value * 1.0) / Guna2TrackBar1.Maximum
            WPF_Player.Player.Volume = Value
        Catch ex As Exception : End Try
    End Sub

    Public Sub ToggleNotifications()
        Guna2CircleButton7.Visible = Core.GlobalInstances.AppSettings.Notifications
        Guna2GradientPanel1.Visible = Core.GlobalInstances.AppSettings.Notifications
    End Sub


#End Region

#Region " ReloadFavorites "

    Public Sub ReloadFavorites()
        MusicLocal_View.ListYoutube()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        SaveMainSetting()
        Environment.Exit(0)
    End Sub

    Private Sub PlayPauseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PlayPauseToolStripMenuItem.Click
        Guna2CircleButton1.Checked = Not Guna2CircleButton1.Checked
        ChangeMediaState()
    End Sub

    Private Sub ToggleMuteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToggleMuteToolStripMenuItem.Click
        Guna2CircleButton5.Checked = Not Guna2CircleButton5.Checked
    End Sub

#End Region

#Region " Play Mode "

    Private PlayMode As Core.PlayMode = Core.PlayMode.Loop

    Private Sub Guna2CircleButton6_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton6.Click
        If PlayMode = Core.PlayMode.Loop Then
            PlayMode = Core.PlayMode.Next
            Guna2CircleButton6.Image = _Next
            Guna2HtmlToolTip1.SetToolTip(Guna2CircleButton6, "Next Mode : Switch to the next media on the list.")
        ElseIf PlayMode = Core.PlayMode.Next Then
            PlayMode = Core.PlayMode.Random
            Guna2CircleButton6.Image = _Random
            Guna2HtmlToolTip1.SetToolTip(Guna2CircleButton6, "Random Mode : Switch to another media in the list randomly.")
        ElseIf PlayMode = Core.PlayMode.Random Then
            PlayMode = Core.PlayMode.Loop
            Guna2CircleButton6.Image = _Loop
            Guna2HtmlToolTip1.SetToolTip(Guna2CircleButton6, "Loop Mode : Repeat the same media on a loop.")
        End If
        Core.GlobalInstances.AppSettings.PlayerMode = PlayMode
        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
    End Sub

#End Region

#Region " MediaSelection "

    Private random As New Random()
    Private selectedItems As New List(Of Control)()
    Private availableItems As New List(Of Control)()

    Private Sub StartNewSelection()
        Try
            WM_Player.Timer1.Stop()
            WPF_Player.Timer1.Stop()
            Label1.Text = "Selecting Next Media using [" & PlayMode.ToString & " mode] , Please Wait..."

            If IsFilePlaying = True Then
                If LastLocalMedia IsNot Nothing Then
                    If TypeOf LastLocalMedia.Parent Is FlowLayoutPanel Then

                        Dim FLP As FlowLayoutPanel = DirectCast(LastLocalMedia.Parent, FlowLayoutPanel)
                        Dim selectedIndex As Integer = FLP.Controls.IndexOf(LastLocalMedia)

                        If PlayMode = Core.PlayMode.Next Then

                            Dim nextIndex As Integer = (selectedIndex + 1) Mod FLP.Controls.Count
                            Dim nextItem As Control = FLP.Controls(nextIndex)
                            If TypeOf nextItem Is LocalMediaEx Then
                                PlayFromFile(nextItem)
                            End If

                        ElseIf PlayMode = Core.PlayMode.Random Then

                            Dim randomItem As Control = GetRandomItem(LastMedia, FLP)
                            If TypeOf randomItem Is LocalMediaEx Then
                                PlayFromFile(randomItem)
                            End If

                        End If

                    End If
                End If
            Else
                If LastMedia IsNot Nothing Then


                    If TypeOf LastMedia.Parent Is FlowLayoutPanel Then

                        Dim FLP As FlowLayoutPanel = DirectCast(LastMedia.Parent, FlowLayoutPanel)
                        Dim selectedIndex As Integer = FLP.Controls.IndexOf(LastMedia)

                        If PlayMode = Core.PlayMode.Next Then

                            Dim nextIndex As Integer = (selectedIndex + 1) Mod FLP.Controls.Count
                            Dim nextItem As Control = FLP.Controls(nextIndex)
                            If TypeOf nextItem Is MusicItem Then
                                PlayFromWeb(nextItem)
                            End If
                        ElseIf PlayMode = Core.PlayMode.Random Then

                            Dim randomItem As Control = GetRandomItem(LastMedia, FLP)
                            If TypeOf randomItem Is MusicItem Then
                                PlayFromWeb(randomItem)
                            End If

                        End If

                    End If

                End If
            End If


        Catch ex As Exception
            Label1.Text = "Error: " & ex.Message
        End Try
    End Sub


    Private Function GetRandomItem(ByVal selectedItem As Control, ByVal FlowLayoutPanel1 As FlowLayoutPanel) As Control
        If availableItems.Count = 0 Then
            availableItems.Clear()
            For Each item As Control In FlowLayoutPanel1.Controls
                availableItems.Add(item)
            Next

            If selectedItem IsNot Nothing AndAlso availableItems.Contains(selectedItem) Then
                availableItems.Remove(selectedItem)
            End If
        End If

        Dim randomIndex As Integer = random.Next(0, availableItems.Count)
        Dim randomItem As Control = availableItems(randomIndex)
        selectedItems.Add(randomItem)
        availableItems.RemoveAt(randomIndex)

        Return randomItem
    End Function



#End Region

#Region " Notifications "

    Private Sub Guna2CircleButton7_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton7.Click
        TogglePanelNotification()
    End Sub

    Private Sub TogglePanelNotification()
        If Guna2GradientPanel1.Height = 0 Then
            Guna2GradientPanel1.Height = 222
            Timer1.Start()
        Else
            Guna2GradientPanel1.Height = 0
            Timer1.Stop()
        End If
        Guna2GradientPanel1.Visible = (Guna2GradientPanel1.Height <> 0)
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If Core.Helpers.GetAsyncKeyState(Keys.LButton) <> 0 AndAlso Not (Core.Helpers.MouseOverControl(Guna2CircleButton7, Me) OrElse Core.Helpers.MouseOverControl(Guna2GradientPanel1, Me)) Then
            Guna2GradientPanel1.Height = 0
            Guna2GradientPanel1.Visible = False
            Timer1.Stop()
        End If
    End Sub

    Public Function CreateButtonNotification(ByVal NameEx As String, ByVal Text As String, ByVal Icon As Image, ByVal ActionSub As Action) As Control
        Dim NotificationControl As New Guna.UI2.WinForms.Guna2Button With {.Name = NameEx}
        NotificationControl.BorderRadius = 2
        NotificationControl.BorderThickness = 1
        NotificationControl.FillColor = System.Drawing.Color.FromArgb(24, 24, 24)
        NotificationControl.BorderColor = Color.FromArgb(34, 34, 34)
        NotificationControl.Font = New System.Drawing.Font("Segoe UI Light", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        NotificationControl.ForeColor = System.Drawing.Color.White
        If Icon IsNot Nothing Then NotificationControl.Image = Icon
        NotificationControl.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left
        NotificationControl.ImageSize = New System.Drawing.Size(15, 15)
        NotificationControl.Size = New System.Drawing.Size(148, 20)
        NotificationControl.Text = Text
        NotificationControl.UseTransparentBackground = True
        AddHandler NotificationControl.Click, AddressOf ActionSub.Invoke
        Return NotificationControl
    End Function

    Public Sub SendNotification(ByVal Notification As Control)
        Notification.Size = New System.Drawing.Size(FlowLayoutPanel1.Width - 15, Notification.Height)
        FlowLayoutPanel1.Controls.Add(Notification)
        PaintNumber()
    End Sub

    Public Sub RemoveNotificationByName(ByVal Name As String)
        Dim controlBuscado As Control = FlowLayoutPanel1.Controls.Find(Name, False).FirstOrDefault()
        If controlBuscado IsNot Nothing Then FlowLayoutPanel1.Controls.Remove(controlBuscado) : controlBuscado.Dispose()
        PaintNumber()
    End Sub

    Private Sub PaintNumber()
        If FlowLayoutPanel1.Controls.Count = 0 Then
            Guna2NotificationPaint1.Visible = False
        Else
            Guna2NotificationPaint1.Text = FlowLayoutPanel1.Controls.Count
            Guna2NotificationPaint1.Visible = True
        End If
    End Sub

    Private Sub Guna2ProgressBar1_ValueChanged(sender As Object, e As EventArgs) Handles Guna2ProgressBar1.ValueChanged
        Try
            If IsFilePlaying = False AndAlso Core.GlobalInstances.AppSettings.MediaPlayerEngine >= 3 Then
                Dim PValue As Integer = Guna2ProgressBar1.Value
                If PValue = 100 Then
                    If PlayMode = Core.PlayMode.Loop Then
                        EOWebPlayer.YTPlayer.SetCurrentTime(0.0)
                        EOWebPlayer.YTPlayer.Play()
                    ElseIf PlayMode = Core.PlayMode.Next Then
                        If YoutubeBrowserButton.Checked = False Then
                            EOWebPlayer.YTPlayer.Pause()
                            StartNewSelection()
                        End If
                    ElseIf PlayMode = Core.PlayMode.Random Then
                        If EOWebPlayer.YTPlayer.GetAutoPlay = False Then
                            EOWebPlayer.YTPlayer.Pause()
                            StartNewSelection()
                        End If
                    End If
                End If

            End If
        Catch ex As Exception : End Try
    End Sub

    Private Sub Label2_TextChanged(sender As Object, e As EventArgs) Handles Label2.TextChanged
        If Core.GlobalInstances.AppSettings.VolumeSync = True Then
            Dim Value As Double = (Guna2TrackBar1.Value * 1.0) / Guna2TrackBar1.Maximum
            EOWebPlayer.YTPlayer.SetVolume(Value)
        End If
    End Sub

    Private Sub Label1_TextChanged(sender As Object, e As EventArgs) Handles Label1.TextChanged
        Guna2HtmlToolTip1.SetToolTip(Label1, Label1.Text)
    End Sub

#End Region

#Region " Download "

    Public Sub DownloadMedia(ByVal Url As String)

        Dim ID As String = Core.Helpers.GetYouTubeVideoIdFromUrl(Url)
        If String.IsNullOrEmpty(Url) = False Then
            Console.WriteLine("Getting: " & Url)
            Dim NewDownDialog As DownloadDialog = New DownloadDialog With {.FormParent = Me}
            NewDownDialog.LoadStreamData(Url)
            NewDownDialog.Show()

        End If
    End Sub

#End Region

End Class