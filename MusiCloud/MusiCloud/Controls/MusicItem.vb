Imports System.IO
Imports System.Net
Imports AngleSharp.Io
Imports EO.WebBrowser
Imports MusiCloud.Core
Imports YoutubeExplode
Imports YoutubeExplode.Search
Imports YoutubeExplode.Videos
Imports YoutubeExplode.Videos.Streams

Public Class MusicItem

    Public Property Media As Search.ISearchResult = Nothing
    Public Property LocalMedia As Core.YoutubeItem = Nothing

    Private Sub MusicItem_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Media IsNot Nothing Then

            If TypeOf Media Is VideoSearchResult Then
                Dim video As VideoSearchResult = DirectCast(Media, VideoSearchResult)

                Label3.Text = video.Title
                Label1.Text = video.Author.ChannelTitle
                If video.Duration Is Nothing Then
                    Label2.Text = ""
                Else
                    Label2.Text = New DateTime(video.Duration.Value.Ticks).ToString("HH:mm:ss")
                End If

                If Core.GlobalInstances.AppSettings.Favorites IsNot Nothing Then
                    Dim itemByID = Core.GlobalInstances.AppSettings.Favorites.FirstOrDefault(Function(item) item.ID.Equals(video.Id, StringComparison.OrdinalIgnoreCase))

                    If itemByID IsNot Nothing Then
                        Guna2Button2.Checked = True
                    End If
                End If

                Guna2PictureBox1.LoadAsync(String.Format("http://img.youtube.com/vi/{0}/0.jpg", video.Id))

                ' Guna2PictureBox1.LoadAsync(String.Format("https://i.ytimg.com/vi/{0}/maxresdefault.jpg", video.Id))

            ElseIf TypeOf Media Is PlaylistSearchResult Then


            End If

        ElseIf LocalMedia IsNot Nothing Then

            Label3.Text = LocalMedia.Title
            Label1.Text = LocalMedia.Author
            Label2.Text = ""

            Dim ID As String = Core.Helpers.GetYouTubeVideoIdFromUrl(LocalMedia.Url)
            If String.IsNullOrEmpty(ID) = False Then

                Guna2PictureBox1.LoadAsync(String.Format("http://img.youtube.com/vi/{0}/0.jpg", ID))
                '  Guna2PictureBox1.LoadAsync(String.Format("https://i.ytimg.com/vi/{0}/maxresdefault.jpg", ID))

                If String.IsNullOrEmpty(LocalMedia.Title) = True Then
                    Dim Youtube = New YoutubeClient()

                    Dim video As ValueTask(Of Video) = Youtube.Videos.GetAsync(LocalMedia.Url)

                    video.GetAwaiter.OnCompleted(Sub()

                                                     Dim itemByID As YoutubeItem = Core.GlobalInstances.AppSettings.Favorites.FirstOrDefault(Function(item) item.ID.Equals(ID, StringComparison.OrdinalIgnoreCase))

                                                     If itemByID IsNot Nothing Then
                                                         itemByID.Title = video.Result.Title
                                                         itemByID.Author = video.Result.Author.ChannelTitle

                                                         Core.AppSettings.Save(Core.GlobalInstances.AppSettings)

                                                         Label3.Text = video.Result.Title
                                                         Label1.Text = video.Result.Author.ChannelTitle

                                                         If video.Result.Duration Is Nothing Then
                                                             Label2.Text = ""
                                                         Else
                                                             Label2.Text = New DateTime(video.Result.Duration.Value.Ticks).ToString("HH:mm:ss")
                                                         End If


                                                     End If


                                                 End Sub)
                End If


            End If



        End If
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        Dim Parent As Control = Me.ParentForm.ParentForm
        If TypeOf Parent Is MainView Then
            DirectCast(Parent, MainView).PlayFromWeb(Me)
        End If
    End Sub

    Dim CurrentMedia As Core.YoutubeItem = Nothing

    Private Sub Guna2Button2_Click(sender As Object, e As EventArgs) Handles Guna2Button2.Click

        If CurrentMedia Is Nothing Then
            If Media IsNot Nothing Then
                If TypeOf Media Is VideoSearchResult Then
                    Dim video As VideoSearchResult = DirectCast(Media, VideoSearchResult)

                    CurrentMedia = New Core.YoutubeItem With {.Url = video.Url, .Title = video.Title, .Author = video.Author.ChannelTitle}

                ElseIf TypeOf Media Is PlaylistSearchResult Then

                End If

            ElseIf LocalMedia IsNot Nothing Then
                CurrentMedia = LocalMedia
            End If
        End If

        If Guna2Button2.Checked = True Then
            Core.GlobalInstances.AppSettings.Favorites?.Add(CurrentMedia)
        Else
            Dim itemByID = Core.GlobalInstances.AppSettings.Favorites.FirstOrDefault(Function(item) item.ID.Equals(CurrentMedia.ID, StringComparison.OrdinalIgnoreCase))

            If itemByID IsNot Nothing Then
                Core.GlobalInstances.AppSettings.Favorites?.Remove(CurrentMedia)
            End If
        End If

        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)

        Dim Parent As Control = Me.ParentForm.ParentForm
        If TypeOf Parent Is MainView Then
            DirectCast(Parent, MainView).ReloadFavorites()
        End If
    End Sub

    Private Sub UrlToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UrlToolStripMenuItem.Click
        If Media IsNot Nothing Then
            Clipboard.SetText(Media.Url)
        ElseIf LocalMedia IsNot Nothing Then
            Clipboard.SetText(LocalMedia.Url)
        End If
    End Sub

    Private Sub IDToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IDToolStripMenuItem.Click
        If Media IsNot Nothing Then
            Dim ID As String = Core.Helpers.GetYouTubeVideoIdFromUrl(Media.Url)
            If String.IsNullOrWhiteSpace(ID) = False Then
                Clipboard.SetText(ID)
            End If
        ElseIf LocalMedia IsNot Nothing Then
            Dim ID As String = Core.Helpers.GetYouTubeVideoIdFromUrl(LocalMedia.Url)
            If String.IsNullOrWhiteSpace(ID) = False Then
                Clipboard.SetText(ID)
            End If
        End If
    End Sub

    Private Sub Guna2Button3_Click(sender As Object, e As EventArgs) Handles Guna2Button3.Click
        Dim Parent As Control = Me.ParentForm.ParentForm
        If TypeOf Parent Is MainView Then
            Dim UrlTarget As String = String.Empty
            If Media IsNot Nothing Then
                UrlTarget = Media.Url
            ElseIf LocalMedia IsNot Nothing Then
                UrlTarget = LocalMedia.Url
            End If
            DirectCast(Parent, MainView).DownloadMedia(UrlTarget)
        End If
    End Sub
End Class
