Imports System.IO
Imports System.Security.Policy
Imports System.Web.UI.WebControls
Imports MusiCloud.Core
Imports TagLib

Public Class DownloadedMedia

#Region " Constructor "

    Public Sub New()
        SetStyle(ControlStyles.SupportsTransparentBackColor, value:=True)
        InitializeComponent()
        Me.BackColor = Color.Transparent
    End Sub



    Private fsVideoWatch As FileSystemWatcher = Nothing
    Private fsAudioWatch As FileSystemWatcher = Nothing

    Private Sub LogRename(ByVal sender As Object, ByVal e As RenamedEventArgs)
        ListYoutube()
    End Sub

    Private Sub LogFile(ByVal sender As Object, ByVal e As FileSystemEventArgs)
        ListYoutube()
    End Sub

    Private Sub DownloadedMedia_Load(sender As Object, e As EventArgs) Handles Me.Load
        AudioCheck()
        VideoCheck()
        ListYoutube()
        Try

            If IO.Directory.Exists(Core.GlobalInstances.MyVideosInDragon) = False Then
                IO.Directory.CreateDirectory(Core.GlobalInstances.MyVideosInDragon)
            End If

            fsVideoWatch = New FileSystemWatcher With {.IncludeSubdirectories = True, .Path = Core.GlobalInstances.MyVideosInDragon, .Filter = "*.mp4", .NotifyFilter = NotifyFilters.LastAccess Or NotifyFilters.LastWrite Or NotifyFilters.FileName Or NotifyFilters.DirectoryName, .EnableRaisingEvents = True}
            fsVideoWatch.SynchronizingObject = Me

            AddHandler fsVideoWatch.Changed, New FileSystemEventHandler(AddressOf LogFile)
            AddHandler fsVideoWatch.Created, New FileSystemEventHandler(AddressOf LogFile)
            AddHandler fsVideoWatch.Deleted, New FileSystemEventHandler(AddressOf LogFile)
            AddHandler fsVideoWatch.Renamed, New RenamedEventHandler(AddressOf LogRename)


            If IO.Directory.Exists(Core.GlobalInstances.MyMusicInDragon) = False Then
                IO.Directory.CreateDirectory(Core.GlobalInstances.MyMusicInDragon)
            End If

            fsAudioWatch = New FileSystemWatcher With {.IncludeSubdirectories = True, .Path = Core.GlobalInstances.MyMusicInDragon, .Filter = "*.mp3", .NotifyFilter = NotifyFilters.LastAccess Or NotifyFilters.LastWrite Or NotifyFilters.FileName Or NotifyFilters.DirectoryName, .EnableRaisingEvents = True}
            fsAudioWatch.SynchronizingObject = Me

            AddHandler fsAudioWatch.Changed, New FileSystemEventHandler(AddressOf LogFile)
            AddHandler fsAudioWatch.Created, New FileSystemEventHandler(AddressOf LogFile)
            AddHandler fsAudioWatch.Deleted, New FileSystemEventHandler(AddressOf LogFile)
            AddHandler fsAudioWatch.Renamed, New RenamedEventHandler(AddressOf LogRename)

        Catch ex As Exception

        End Try

    End Sub

    Private Sub DownloadedMedia_Shown(sender As Object, e As EventArgs) Handles Me.Shown

    End Sub

#End Region

#Region " Scroll Manager "

    Public vScrollHelper As Guna.UI2.WinForms.Helpers.PanelScrollHelper = Nothing

    Public Sub SetScroll(ByVal VScroll As Guna.UI2.WinForms.Guna2VScrollBar)

        If vScrollHelper IsNot Nothing Then
            vScrollHelper.Dispose()
        End If

        vScrollHelper = New Guna.UI2.WinForms.Helpers.PanelScrollHelper(FlowLayoutPanel1, VScroll, True)
        VScroll.Visible = True
        vScrollHelper.UpdateScrollBar()
        'VScroll.Width = 10
        'VScroll.Location = New Point(VScroll.Location.X + 10, VScroll.Location.Y)
    End Sub

#End Region

    Public Async Function SearchByYoutube(ByVal Text As String) As Task(Of Boolean)
        Try
            FlowLayoutPanel1.Controls.Clear()

            If String.IsNullOrEmpty(Text) = False AndAlso Core.GlobalInstances.AppSettings.Favorites IsNot Nothing Then

                Dim Search As List(Of FileInfo) = New List(Of FileInfo)


                If VideoToolStripMenuItem1.Checked Then
                    Search.AddRange(GetVideos)
                End If

                If MusicToolStripMenuItem.Checked Then
                    Search.AddRange(GetAudios)
                End If

                Dim foundItems As List(Of FileInfo) = FindYoutubeItemsByTitleOrAuthor(Search, Text)


                For Each Item As FileInfo In foundItems
                    Try
                        Dim NewMedia As New LocalMediaEx With {.TargetMedia = Item}
                        NewMedia.Label3.Text = Item.Name
                        Using songTag = TagLib.File.Create(Item.FullName)
                            NewMedia.Label1.Text = songTag.Tag.Publisher
                        End Using
                        If String.IsNullOrEmpty(NewMedia.Label1.Text) = True Then
                            NewMedia.Label1.Visible = False
                        End If
                        FlowLayoutPanel1.Controls.Add(NewMedia)
                    Catch ex As Exception : End Try
                Next

            Else
                ListYoutube()
            End If

            Dim Parent As Control = Me.ParentForm
            If TypeOf Parent Is MainView Then
                DirectCast(Parent, MainView).BeginInvoke(Sub()
                                                             DirectCast(Parent, MainView).Guna2ProgressBar2.Visible = False
                                                         End Sub)
            End If
        Catch ex As Exception : End Try

        Return True
    End Function

    Public Function FindYoutubeItemsByTitleOrAuthor(items As List(Of FileInfo), searchTerm As String) As List(Of FileInfo)
        Dim foundItems As New List(Of FileInfo)

        Dim itemsByTitle = items.Where(Function(item) item.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList()
        foundItems.AddRange(itemsByTitle)

        'Dim itemsByAuthor = items.Where(Function(item) item..Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList()
        'foundItems.AddRange(itemsByAuthor)

        Return foundItems
    End Function

    Private Function GetVideos() As List(Of FileInfo)
        Dim Videos As List(Of FileInfo) = FileDirSearcher.GetFiles(dirPath:=Core.GlobalInstances.MyVideosInDragon,
                                                                       searchOption:=SearchOption.AllDirectories,
                                                                       fileNamePatterns:={"*"},
                                                                       fileExtPatterns:={"*.mp4"},
                                                                       ignoreCase:=True,
                                                                       throwOnError:=False).ToList
        Return Videos
    End Function

    Private Function GetAudios() As List(Of FileInfo)
        Dim Audios As List(Of FileInfo) = FileDirSearcher.GetFiles(dirPath:=Core.GlobalInstances.MyMusicInDragon,
                                                                         searchOption:=SearchOption.AllDirectories,
                                                                         fileNamePatterns:={"*"},
                                                                         fileExtPatterns:={"*.mp3"},
                                                                         ignoreCase:=True,
                                                                         throwOnError:=False).ToList
        Return Audios
    End Function

    Dim IsCheck As Boolean = False

    Public Sub ListYoutube()
        Try
            If Me.Visible = False Then
                If vScrollHelper IsNot Nothing Then
                    vScrollHelper.Dispose()
                End If
            End If

            FlowLayoutPanel1.Controls.Clear()

            Dim Search As List(Of FileInfo) = New List(Of FileInfo)

            If VideoToolStripMenuItem1.Checked Then
                Search.AddRange(GetVideos)
            End If

            If MusicToolStripMenuItem.Checked Then
                Search.AddRange(GetAudios)
            End If

            For Each Item As FileInfo In Search
                Try
                    Dim NewMedia As New LocalMediaEx With {.TargetMedia = Item}
                    NewMedia.Label3.Text = Item.Name
                    Using songTag = TagLib.File.Create(Item.FullName)
                        NewMedia.Label1.Text = songTag.Tag.Publisher
                    End Using
                    If String.IsNullOrEmpty(NewMedia.Label1.Text) = True Then
                        NewMedia.Label1.Visible = False
                    End If
                    FlowLayoutPanel1.Controls.Add(NewMedia)
                Catch ex As Exception : End Try
            Next
        Catch ex As Exception : End Try
    End Sub

    Private Sub AudioCheck()
        MusicToolStripMenuItem.Checked = Core.GlobalInstances.AppSettings.AudioList

        If MusicToolStripMenuItem.Checked Then
            MusicToolStripMenuItem.Text = "🗹 Audio"
        Else
            MusicToolStripMenuItem.Text = "⬜ Audio"
        End If
    End Sub

    Private Sub VideoCheck()
        VideoToolStripMenuItem1.Checked = Core.GlobalInstances.AppSettings.VideoList

        If VideoToolStripMenuItem1.Checked Then
            VideoToolStripMenuItem1.Text = "🗹 Video"
        Else
            VideoToolStripMenuItem1.Text = "⬜ Video"
        End If
    End Sub

    Private Sub MusicToolStripMenuItem_CheckedChanged(sender As Object, e As EventArgs) Handles MusicToolStripMenuItem.CheckedChanged
        Core.GlobalInstances.AppSettings.AudioList = MusicToolStripMenuItem.Checked
        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
        AudioCheck()
    End Sub

    Private Sub VideoToolStripMenuItem1_CheckedChanged(sender As Object, e As EventArgs) Handles VideoToolStripMenuItem1.CheckedChanged
        Core.GlobalInstances.AppSettings.VideoList = VideoToolStripMenuItem1.Checked
        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
        VideoCheck()
    End Sub

    Private Sub ReloadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReloadToolStripMenuItem.Click
        ListYoutube()
    End Sub

End Class