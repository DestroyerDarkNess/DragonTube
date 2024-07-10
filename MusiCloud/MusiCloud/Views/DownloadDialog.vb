Imports YoutubeExplode
Imports YoutubeExplode.Videos
Imports YoutubeExplode.Videos.Streams
Imports YoutubeExplode.Converter
Imports Guna.UI2.WinForms
Imports System.Runtime.InteropServices.ComTypes
Imports EO.Internal
Imports System.IO
Imports TagLib.Riff
Imports FFMpegSharp.FFMPEG
Imports FFMpegSharp
Imports FFMpegSharp.Enums
Imports FFMpegSharp.FFMPEG.Enums

Public Class DownloadDialog

    Public Property FormParent As MainView = Nothing

    Private Youtube As YoutubeClient = Nothing

    Private VideoData As Video = Nothing
    Private StreamManifest As StreamManifest = Nothing

    Private Sub DownloadDialog_Load(sender As Object, e As EventArgs) Handles Me.Load
        YoutubeBrowserButton.Enabled = False
    End Sub

    Public Sub LoadStreamData(ByVal videoUrl As String)
        Guna2ProgressBar2.Visible = True
        Youtube = New YoutubeClient()

        Dim video As ValueTask(Of Video) = Youtube.Videos.GetAsync(videoUrl)

        video.GetAwaiter.OnCompleted(Sub()
                                         VideoData = video.Result

                                         Guna2PictureBox1.LoadAsync(String.Format("https://i.ytimg.com/vi/{0}/maxresdefault.jpg", video.Result.Id))

                                         Label1.Text = video.Result.Title
                                         Label4.Text = video.Result.Author.ChannelTitle

                                         If video.Result.Duration Is Nothing Then
                                             Label5.Visible = False
                                         Else
                                             Label5.Text = New DateTime(video.Result.Duration.Value.Ticks).ToString("HH:mm:ss")
                                         End If

                                         Dim streamManifestEx As ValueTask(Of StreamManifest) = Youtube.Videos.Streams.GetManifestAsync(videoUrl)

                                         streamManifestEx.GetAwaiter.OnCompleted(Sub()
                                                                                     Try
                                                                                         StreamManifest = streamManifestEx.Result

                                                                                         If StreamManifest Is Nothing Then
                                                                                             FormParent.ShowMessage("StreamManifest Error! Try Againt and Check your Internet Connection!!")
                                                                                             Me.Close()
                                                                                         Else
                                                                                             GetVideoData()
                                                                                             Guna2ProgressBar2.Visible = False
                                                                                         End If
                                                                                     Catch ex As Exception
                                                                                         FormParent.ShowMessage(ex.Message, "Try Againt Please!")
                                                                                         Me.Close()
                                                                                         Me.Dispose()
                                                                                     End Try
                                                                                 End Sub)
                                     End Sub)

    End Sub

    Dim IsShow As Boolean = False


    Private Sub Guna2CircleButton2_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton2.Click
        Me.Close()
    End Sub

    Public Function GetName(ByVal NameEx As String, ByVal Extension As String)
        Dim Result As String = NameEx

        If Result.ToLower.EndsWith(Extension) = False Then
            Result += Extension
        End If

        Return Core.Helpers.CleanPath(Result)
    End Function

    Private Sub YoutubeBrowserButton_Click(sender As Object, e As EventArgs) Handles YoutubeBrowserButton.Click
        YoutubeBrowserButton.Enabled = False

        Try

            Dim Name As String = Core.Helpers.CleanPath(Label1.Text)
            Dim Index As Integer = LogInComboBox1.SelectedIndex

            Select Case LogInComboBox4.SelectedIndex
                Case 0
                    Dim FileNameEx As String = IO.Path.GetFileNameWithoutExtension(Name) & LogInComboBox1.Items(Index).ToString
                    Try
                        FileNameEx += LogInComboBox2.Items(LogInComboBox2.SelectedIndex).ToString
                    Catch ex As Exception : End Try
                    Dim FileName As String = GetName(FileNameEx.Replace(" ", "_"), ".mp4")
                    Dim FileOutput As String = IO.Path.Combine(Core.GlobalInstances.MyVideosInDragon, FileName)
                    Dim GetStreamVid As KeyValuePair(Of String, IVideoStreamInfo) = FilterVideoStream.ElementAt(Index)
                    Dim videoStreamInfo As IVideoStreamInfo = GetStreamVid.Value
                    Dim audioStreamInfo As IAudioStreamInfo = StreamManifest.GetAudioStreams().Where(Function(s) s.Container = Streams.Container.Mp4).OrderBy(Function(x) x.Bitrate.KiloBitsPerSecond).Distinct()(LogInComboBox2.SelectedIndex)

                    Dim streamInfos = New IStreamInfo() {audioStreamInfo, videoStreamInfo}

                    Dim NewProgress As New ProgressNotification With {.Name = FileName}
                    FormParent.SendNotification(NewProgress)
                    FormParent.Guna2HtmlToolTip1.SetToolTip(NewProgress, FileName)
                    NewProgress.OnLoadAction = New Action(Async Sub()

                                                              Me.Hide()

                                                              NewProgress.LogInLabel1.Font = New Font(NewProgress.LogInLabel1.Font.Name, 6, FontStyle.Regular)
                                                              NewProgress.LogInLabel1.Text = Me.Label1.Text

                                                              Dim AVCodecFixer As Boolean = Guna2CheckBox1.Checked
                                                              Dim DownloadDir As String = FileOutput
                                                              Dim wClient As New DownloadFileAsyncExtended

                                                              Try
                                                                  If IO.File.Exists(DownloadDir) Then
                                                                      IO.File.Delete(DownloadDir)
                                                                  End If
                                                              Catch ex As Exception : End Try

                                                              Dim DownloadProgress As Progress(Of Double) = New Progress(Of Double)(New Action(Of Double)(Sub(ByVal value As Double)
                                                                                                                                                              Dim Percent As Integer = value * 100
                                                                                                                                                              NewProgress.Guna2ProgressBar1.Value = Percent
                                                                                                                                                          End Sub))


                                                              Dim ConversionParam As ConversionRequest = New ConversionRequestBuilder(FileOutput).SetFFmpegPath(Core.GlobalInstances.ffmpegPath).Build

                                                              Dim DownloadMedia As ValueTask = Youtube.Videos.DownloadAsync(streamInfos, ConversionParam, DownloadProgress)

                                                              DownloadMedia.GetAwaiter.OnCompleted(Sub()
                                                                                                       Dim NewOuputPath As String = IO.Path.Combine(IO.Path.GetDirectoryName(DownloadDir), IO.Path.GetFileNameWithoutExtension(DownloadDir) & "_AV1Fixed" & IO.Path.GetExtension(DownloadDir))
                                                                                                       Dim outputFile As FileInfo = New FileInfo(NewOuputPath)
                                                                                                       Try
                                                                                                           If AVCodecFixer = True Then

                                                                                                               Dim encoder = New FFMpeg()

                                                                                                               Dim Video = VideoInfo.FromPath(DownloadDir)

                                                                                                               If Video.VideoFormat.ToLower = "av1" Then


                                                                                                                   AddHandler encoder.OnProgress, Sub(percentage)
                                                                                                                                                      Console.WriteLine("AV1 Fixer Progress {0}%", percentage)
                                                                                                                                                  End Sub

                                                                                                                   encoder.Convert(Video, outputFile, VideoType.Mp4, multithreaded:=True)

                                                                                                                   Try
                                                                                                                       If IO.File.Exists(DownloadDir) Then IO.File.Delete(DownloadDir)
                                                                                                                   Catch ex As Exception : End Try
                                                                                                               End If


                                                                                                           End If

                                                                                                           Dim NameNotifyEx As String = NewProgress.Name & "_Ex"
                                                                                                           Dim VLCNotifyEx As Control = FormParent.CreateButtonNotification(NameNotifyEx, "Download has finished!", My.Resources.icons8_emoji_de_marca_de_verificación_20, New Action(Sub()
                                                                                                                                                                                                                                                                                          Me.Close()
                                                                                                                                                                                                                                                                                          FormParent.RemoveNotificationByName(NameNotifyEx)

                                                                                                                                                                                                                                                                                      End Sub))
                                                                                                           FormParent.Guna2HtmlToolTip1.SetToolTip(VLCNotifyEx, "Download has finished : " & NewProgress.Name)
                                                                                                           FormParent.SendNotification(VLCNotifyEx)
                                                                                                           FormParent.RemoveNotificationByName(NewProgress.Name)


                                                                                                       Catch ex As Exception
                                                                                                           FormParent.RemoveNotificationByName(NewProgress.Name)
                                                                                                           If outputFile.Exists = True AndAlso outputFile.Length = 0 Then
                                                                                                               outputFile.Delete()
                                                                                                           End If
                                                                                                       End Try

                                                                                                   End Sub)

                                                          End Sub)

                    NewProgress.OnLoadAction.Invoke


                Case 1
                    Dim FileNameEx As String = IO.Path.GetFileNameWithoutExtension(Name) & LogInComboBox1.Items(Index).ToString
                    Dim FileName As String = GetName(FileNameEx.Replace(" ", "_"), ".mp3")
                    Dim FileOutput As String = IO.Path.Combine(Core.GlobalInstances.MyMusicInDragon, FileName)
                    Dim audioStreamInfo As IAudioStreamInfo = StreamManifest.GetAudioStreams().Where(Function(s) s.Container = Streams.Container.Mp4).OrderBy(Function(x) x.Bitrate.KiloBitsPerSecond).Distinct()(Index)

                    Dim streamInfos = New IStreamInfo() {audioStreamInfo}

                    Dim NewProgress As New ProgressNotification With {.Name = FileName}
                    FormParent.SendNotification(NewProgress)
                    FormParent.Guna2HtmlToolTip1.SetToolTip(NewProgress, FileName)
                    NewProgress.OnLoadAction = New Action(Async Sub()

                                                              Me.Hide()

                                                              NewProgress.LogInLabel1.Font = New Font(NewProgress.LogInLabel1.Font.Name, 6, FontStyle.Regular)
                                                              NewProgress.LogInLabel1.Text = Me.Label1.Text

                                                              Dim DownloadDir As String = FileOutput
                                                              Dim wClient As New DownloadFileAsyncExtended

                                                              Try
                                                                  If IO.File.Exists(DownloadDir) Then
                                                                      IO.File.Delete(DownloadDir)
                                                                  End If
                                                              Catch ex As Exception : End Try

                                                              Dim DownloadProgress As Progress(Of Double) = New Progress(Of Double)(New Action(Of Double)(Sub(ByVal value As Double)
                                                                                                                                                              Dim Percent As Integer = value * 100
                                                                                                                                                              NewProgress.Guna2ProgressBar1.Value = Percent
                                                                                                                                                          End Sub))


                                                              Dim ConversionParam As ConversionRequest = New ConversionRequestBuilder(FileOutput).SetFFmpegPath(Core.GlobalInstances.ffmpegPath).Build

                                                              Dim DownloadMedia As ValueTask = Youtube.Videos.DownloadAsync(streamInfos, ConversionParam, DownloadProgress)

                                                              DownloadMedia.GetAwaiter.OnCompleted(Sub()
                                                                                                       Core.Helpers.SetPicture(FileOutput, VideoData.Author.ChannelTitle, Guna2PictureBox1.Image)
                                                                                                       Dim NameNotifyEx As String = NewProgress.Name & "_Ex"
                                                                                                       Dim VLCNotifyEx As Control = FormParent.CreateButtonNotification(NameNotifyEx, "Download has finished!", My.Resources.icons8_emoji_de_marca_de_verificación_20, New Action(Sub()
                                                                                                                                                                                                                                                                                      Me.Close()
                                                                                                                                                                                                                                                                                      FormParent.RemoveNotificationByName(NameNotifyEx)
                                                                                                                                                                                                                                                                                  End Sub))
                                                                                                       FormParent.Guna2HtmlToolTip1.SetToolTip(VLCNotifyEx, NewProgress.Name)
                                                                                                       FormParent.SendNotification(VLCNotifyEx)
                                                                                                       FormParent.RemoveNotificationByName(NewProgress.Name)

                                                                                                   End Sub)

                                                          End Sub)

                    NewProgress.OnLoadAction.Invoke

            End Select
        Catch ex As Exception
            YoutubeBrowserButton.Enabled = True
        End Try
    End Sub

    Private Sub LogInComboBox4_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LogInComboBox4.SelectedIndexChanged
        If StreamManifest IsNot Nothing Then GetVideoData()
        Select Case LogInComboBox4.SelectedIndex
            Case 0 : Guna2CheckBox1.Visible = True
            Case 1 : Guna2CheckBox1.Visible = False
        End Select
    End Sub

    Private FilterVideoStream As New Dictionary(Of String, IVideoStreamInfo)



    Private Sub GetVideoData()
        LogInComboBox1.Items.Clear()
        'LogInComboBox1.Enabled = False
        Dim Index As Integer = LogInComboBox4.SelectedIndex

        Select Case Index
            Case 0

                FilterVideoStream.Clear()

                Dim videoStreamInfo As IEnumerable(Of IVideoStreamInfo) = StreamManifest.GetVideoStreams().Where(Function(s) s.Container = Streams.Container.Mp4).OrderBy(Function(x) x.VideoQuality)

                For Each VidStream As IVideoStreamInfo In videoStreamInfo

                    Dim GetVidStreamOld As IVideoStreamInfo = Nothing

                    If FilterVideoStream.TryGetValue(VidStream.VideoQuality.Label, GetVidStreamOld) = True Then

                        If VidStream.Bitrate.KiloBitsPerSecond >= GetVidStreamOld.Bitrate.KiloBitsPerSecond Then
                            If FilterVideoStream.Remove(VidStream.VideoQuality.Label) Then
                                FilterVideoStream.Add(VidStream.VideoQuality.Label, VidStream)
                            End If
                        End If

                    Else
                        FilterVideoStream.Add(VidStream.VideoQuality.Label, VidStream)
                    End If


                Next

                For Each Vid As KeyValuePair(Of String, IVideoStreamInfo) In FilterVideoStream
                    LogInComboBox1.Items.Add(Vid.Value.VideoQuality.Label & " (" & Math.Round(Vid.Value.Size.MegaBytes) & "mb)")
                Next

                LogInComboBox2.Items.Clear()
                Dim audioStreamInfo As IEnumerable(Of IAudioStreamInfo) = StreamManifest.GetAudioStreams().Where(Function(s) s.Container = Streams.Container.Mp4).OrderBy(Function(x) x.Bitrate.KiloBitsPerSecond).Distinct()

                For Each AudStream As IAudioStreamInfo In audioStreamInfo
                    LogInComboBox2.Items.Add(Math.Round(AudStream.Bitrate.KiloBitsPerSecond) & "Kbps (" & Math.Round(AudStream.Size.MegaBytes) & "mb)")
                Next

                LogInComboBox2.Visible = True
                If Not LogInComboBox2.Items.Count = 0 Then LogInComboBox2.SelectedIndex = (LogInComboBox2.Items.Count - 1)

            Case 1
                LogInComboBox2.Items.Clear()
                LogInComboBox2.Visible = False
                Dim audioStreamInfo As IEnumerable(Of IAudioStreamInfo) = StreamManifest.GetAudioStreams().Where(Function(s) s.Container = Streams.Container.Mp4).OrderBy(Function(x) x.Bitrate.KiloBitsPerSecond).Distinct()

                For Each AudStream As IAudioStreamInfo In audioStreamInfo
                    LogInComboBox1.Items.Add(Math.Round(AudStream.Bitrate.KiloBitsPerSecond) & "Kbps (" & Math.Round(AudStream.Size.MegaBytes) & "mb)")
                Next
        End Select

        If LogInComboBox1.Items.Count = 0 Then
            YoutubeBrowserButton.Enabled = False
            Label4.Text = "Stream error!! Sorry bro... Donate 1$ for me please.. I live in Venezuela"
        Else
            LogInComboBox1.SelectedIndex = 0
            YoutubeBrowserButton.Enabled = True
            Label4.Text = VideoData.Author.ChannelTitle
        End If

    End Sub


    Private Sub Guna2ToggleSwitch9_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2ToggleSwitch9.CheckedChanged
        If Guna2ToggleSwitch9.Checked = True Then
            Label7.Text = "FastPreset"
            Guna2HtmlToolTip1.SetToolTip(Guna2ToggleSwitch9, "Easily select the Preset that best suits your needs.")
        Else
            Label7.Text = "Quality"
            Guna2HtmlToolTip1.SetToolTip(Guna2ToggleSwitch9, "Select the video/audio quality you prefer.")
        End If

        If IsShow = True Then
            Core.GlobalInstances.AppSettings.DownMode = Guna2ToggleSwitch9.Checked
            Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
        End If
    End Sub

    Private Sub Guna2ProgressBar2_VisibleChanged(sender As Object, e As EventArgs) Handles Guna2ProgressBar2.VisibleChanged
        Guna2WinProgressIndicator1.Visible = Guna2ProgressBar2.Visible
    End Sub

    Private Sub Guna2WinProgressIndicator1_VisibleChanged(sender As Object, e As EventArgs) Handles Guna2WinProgressIndicator1.VisibleChanged
        If Guna2WinProgressIndicator1.Visible = True Then
            Guna2WinProgressIndicator1.Start()
        Else
            Guna2WinProgressIndicator1.Stop()
        End If
    End Sub

    Private Sub LogInComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LogInComboBox1.SelectedIndexChanged
        CheckSizeInMb()
    End Sub

    Private Sub LogInComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LogInComboBox2.SelectedIndexChanged
        CheckSizeInMb()
    End Sub

    Private Sub CheckSizeInMb()
        If StreamManifest IsNot Nothing AndAlso Not LogInComboBox1.Items.Count = 0 Then
            Try
                Select Case LogInComboBox4.SelectedIndex
                    Case 0
                        If Not LogInComboBox2.Items.Count = 0 Then
                            Dim GetStreamVid As KeyValuePair(Of String, IVideoStreamInfo) = FilterVideoStream.ElementAt(LogInComboBox1.SelectedIndex)
                            Dim videoStreamInfo As IVideoStreamInfo = GetStreamVid.Value
                            Dim audioStreamInfo As IAudioStreamInfo = StreamManifest.GetAudioStreams().Where(Function(s) s.Container = Streams.Container.Mp4).OrderBy(Function(x) x.Bitrate.KiloBitsPerSecond).Distinct()(LogInComboBox2.SelectedIndex)
                            If audioStreamInfo Is Nothing Then
                                Label2.Text = Math.Round(videoStreamInfo.Size.MegaBytes) & "mb"
                            Else
                                Label2.Text = Math.Round(videoStreamInfo.Size.MegaBytes + audioStreamInfo.Size.MegaBytes) & "mb"
                            End If
                        End If
                    Case 1
                        Dim audioStreamInfo As IAudioStreamInfo = StreamManifest.GetAudioStreams().Where(Function(s) s.Container = Streams.Container.Mp4).OrderBy(Function(x) x.Bitrate.KiloBitsPerSecond).Distinct()(LogInComboBox1.SelectedIndex)
                        If audioStreamInfo IsNot Nothing Then Label2.Text = Math.Round(audioStreamInfo.Size.MegaBytes) & "mb"
                End Select
            Catch ex As Exception : End Try
        End If
    End Sub

End Class