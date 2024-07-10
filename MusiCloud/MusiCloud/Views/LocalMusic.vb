Imports System.Threading
Imports Guna.UI2.WinForms
Imports MusiCloud.Core
Imports YoutubeExplode
Imports YoutubeExplode.Search

Public Class LocalMusic

#Region " Constructor "

    Public Sub New()
        SetStyle(ControlStyles.SupportsTransparentBackColor, value:=True)
        InitializeComponent()
        Me.BackColor = Color.Transparent
    End Sub

#End Region

    Private Sub LocalMusic_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ListYoutube()
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

    Public Async Function SearchByYoutube(ByVal Text As String) As Task(Of Boolean)
        Try
            FlowLayoutPanel1.Controls.Clear()

            If String.IsNullOrEmpty(Text) = False AndAlso Core.GlobalInstances.AppSettings.Favorites IsNot Nothing Then


                Dim foundItems As List(Of YoutubeItem) = FindYoutubeItemsByTitleOrAuthor(Core.GlobalInstances.AppSettings.Favorites, Text)


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

                For Each Item As YoutubeItem In foundItems
                    Dim NewMedia As New MusicItem With {.LocalMedia = Item}
                    NewMedia.Guna2Button2.Checked = True
                    FlowLayoutPanel1.Controls.Add(NewMedia)
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

    Public Function FindYoutubeItemsByTitleOrAuthor(items As List(Of YoutubeItem), searchTerm As String) As List(Of YoutubeItem)
        Dim foundItems As New List(Of YoutubeItem)

        Dim itemsByTitle = items.Where(Function(item) item.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList()
        foundItems.AddRange(itemsByTitle)

        Dim itemsByAuthor = items.Where(Function(item) item.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList()
        foundItems.AddRange(itemsByAuthor)

        Return foundItems
    End Function

    Public Sub ListYoutube()

        If Me.Visible = False Then
            If vScrollHelper IsNot Nothing Then
                vScrollHelper.Dispose()
            End If
        End If

        FlowLayoutPanel1.Controls.Clear()


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

        If Core.GlobalInstances.AppSettings.Favorites IsNot Nothing Then
            For Each Item As Core.YoutubeItem In Core.GlobalInstances.AppSettings.Favorites
                Dim NewMedia As New MusicItem With {.LocalMedia = Item}
                NewMedia.Guna2Button2.Checked = True
                FlowLayoutPanel1.Controls.Add(NewMedia)
            Next
        End If
    End Sub

End Class