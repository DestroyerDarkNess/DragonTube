Imports System.Threading
Imports Guna.UI2.WinForms.Helpers
Imports YoutubeExplode
Imports YoutubeExplode.Search
Imports YoutubeExplode.Videos
Imports YoutubeExplode.Videos.Streams

Public Class MusicHub

#Region " Constructor "

    Public Sub New()
        SetStyle(ControlStyles.SupportsTransparentBackColor, value:=True)
        InitializeComponent()
        Me.BackColor = Color.Transparent
    End Sub

#End Region



    Private Sub MusicHub_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

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

    Public Async Function SearchByYoutube(ByVal Youtube As YoutubeClient, ByVal Text As String, ByVal CancelToken As CancellationToken) As Task(Of Boolean)
        Try
            FlowLayoutPanel1.Controls.Clear()
            Dim staThread As New Threading.Thread(
            Async Sub()
                Try

                    Try
                        Dim ParenteX As Control = Me.ParentForm

                        If TypeOf ParenteX Is MainView Then
                            Dim AdPicture As PictureBox = DirectCast(ParenteX, MainView).NewAdPanel
                            If AdPicture IsNot Nothing Then
                                Me.BeginInvoke(Sub()
                                                   FlowLayoutPanel1.Controls.Add(AdPicture)
                                               End Sub)
                            End If
                        End If
                    Catch ex As Exception : End Try

                    Dim AsyncSequence As IAsyncEnumerable(Of Search.ISearchResult) = Youtube.Search.GetResultsAsync(Text, CancelToken)
                    Dim iterator = AsyncSequence.GetAsyncEnumerator()
                    Try


                        Dim ID As Integer = 0

                        Do While Await iterator.MoveNextAsync()

                            If ID >= Core.GlobalInstances.AppSettings.VideoLimit Then
                                Exit Do
                            End If

                            Dim item As Search.ISearchResult = iterator.Current

                            Try
                                If TypeOf item Is VideoSearchResult Then

                                    Me.BeginInvoke(Sub()
                                                       Dim video As VideoSearchResult = DirectCast(item, VideoSearchResult)
                                                       Dim NewMedia As New MusicItem With {.Media = item, .Name = ID}
                                                       FlowLayoutPanel1.Controls.Add(NewMedia)
                                                   End Sub)

                                    ID += 1
                                ElseIf TypeOf item Is PlaylistSearchResult Then
                                    ' Not support Sorry
                                End If
                            Catch ex As Exception

                            End Try
                        Loop

                    Finally
                        iterator.DisposeAsync()
                    End Try

                    Dim Parent As Control = Me.ParentForm

                    If TypeOf Parent Is MainView Then

                        DirectCast(Parent, MainView).BeginInvoke(Sub()
                                                                     DirectCast(Parent, MainView).Guna2ProgressBar2.Visible = False
                                                                 End Sub)

                    End If

                Catch ex As Exception : End Try
            End Sub)

            staThread.Start()


        Catch ex As Exception : End Try

        Return True
    End Function


End Class