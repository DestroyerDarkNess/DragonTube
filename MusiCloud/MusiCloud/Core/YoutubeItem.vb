Namespace Core
    Public Class YoutubeItem

        Public Url As String = String.Empty
        Public Title As String = String.Empty
        Public Author As String = String.Empty

        Public Function ID() As String
            Return Core.Helpers.GetYouTubeVideoIdFromUrl(Url)
        End Function

    End Class

End Namespace

