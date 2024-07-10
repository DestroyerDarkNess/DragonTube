Imports MusiCloud.My
Imports SettingsProviderNet
Imports System.ComponentModel
Imports System.IO
Imports System.Windows.Documents
Imports YoutubeExplode.Videos

Namespace Core

    Public Enum VideoQuality
        High = 0
        Medium = 1
        Low = 2
        None = 3
    End Enum

    Public Enum VideoBitrate
        High = 0
        Medium = 1
        Low = 2
    End Enum

    Public Enum MediaPlayer
        VLC = 0
        WMP = 1
        WPF = 2
        YT_Embed = 3
        YT_Native = 4
    End Enum

    Public Enum BrowserUserAgent
        Desktop = 0
        Mobile = 1
    End Enum

    Public Enum PlayMode As UInteger
        [Loop] = 0
        [Next] = 1
        Random = 2
    End Enum

    Public Class AppSettings

        <DefaultValue(False)>
        <Description("DonateCoffe")>
        Public Property DonateCoffe As Boolean

        <DefaultValue(False)>
        <Description("Github")>
        Public Property Github As Boolean

        <DefaultValue(False)>
        <Description("FAQ")>
        Public Property FAQ As Boolean

        <DefaultValue(False)>
        <Description("VisualBasic")>
        Public Property VisualBasic As Boolean


        <DefaultValue(40)>
        <DisplayName("Video Search Limit")>
        Public Property VideoLimit As Integer

        <DefaultValue(100)>
        <DisplayName("Volume")>
        Public Property Volume As Integer

        <DefaultValue(False)>
        <Description("Video Window")>
        Public Property VideoWindow As Boolean

        <DefaultValue("")>
        <DisplayName("Video Window Location")>
        Public Property VideoLocation As String

        <DefaultValue("")>
        <DisplayName("Video Window Size")>
        Public Property VideoSize As String

        Public Property Favorites As List(Of Core.YoutubeItem)

        <DefaultValue(2)>
        <DisplayName("VideoEngine")>
        Public Property MediaPlayerEngine As MediaPlayer = MediaPlayer.WPF

        <DefaultValue(0)>
        <DisplayName("UserAgent")>
        Public Property UserAgent As BrowserUserAgent = BrowserUserAgent.Desktop

        Public Property BitRate As VideoBitrate = VideoBitrate.High

        Public Property Quality As VideoQuality = VideoQuality.High

        Public Property PlayerMode As PlayMode = PlayMode.Loop

        <DefaultValue(True)>
        <Description("Notifications")>
        Public Property Notifications As Boolean

        <DefaultValue(True)>
        <Description("DisableGPU")>
        Public Property DisableGPU As Boolean

        <DefaultValue(False)>
        <Description("VolumeSync")>
        Public Property VolumeSync As Boolean

        <DefaultValue(False)>
        <Description("ExternalYT")>
        Public Property ExternalYT As Boolean

        <DefaultValue(True)>
        <Description("SystemTray")>
        Public Property SystemTray As Boolean

        <DefaultValue(False)>
        <Description("DownMode")>
        Public Property DownMode As Boolean

        <DefaultValue(True)>
        <Description("VideoList")>
        Public Property VideoList As Boolean

        <DefaultValue(True)>
        <Description("AudioList")>
        Public Property AudioList As Boolean

        <DefaultValue(True)>
        <Description("RoundedEdges")>
        Public Property RoundedEdges As Boolean

        <DefaultValue(False)>
        <Description("WindowsControlBox")>
        Public Property WindowsControlBox As Boolean

        <DefaultValue(False)>
        <Description("NaturalYT")>
        Public Property NaturalYT As Boolean

        <DefaultValue(False)>
        <Description("TopMostYT")>
        Public Property TopMostYT As Boolean

        <DefaultValue(False)>
        <Description("Disclaimer")>
        Public Property Disclaimer As Boolean

        <DefaultValue(False)>
        <Description("MaximizedInApp")>
        Public Property MaximizedInApp As Boolean

        <DefaultValue(False)>
        <Description("SupportAVCodec")>
        Public Property SupportAVCodec As Boolean

        Public Shared Function Load() As AppSettings
            Dim settingsProvider = GetProvider()
            Return settingsProvider.GetSettings(Of AppSettings)
        End Function

        Public Shared Sub Save(ByVal AppSettings As AppSettings)
            If AppSettings.Favorites IsNot Nothing Then
                If Not AppSettings.Favorites.Count = 0 Then
                    Dim encontrados As New Dictionary(Of String, YoutubeItem)

                    For Each video As YoutubeItem In AppSettings.Favorites
                        If Not encontrados.ContainsKey(video.ID) Then
                            encontrados.Add(video.ID, video)
                        End If
                    Next

                    AppSettings.Favorites.Clear()
                    AppSettings.Favorites.AddRange(encontrados.Values.ToList)

                End If
            End If

            Dim settingsProvider = GetProvider()
            settingsProvider.SaveSettings(AppSettings)
        End Sub

        Public Shared Function GetProvider() As SettingsProvider
            Dim Storage As RoamingAppDataStorage = New RoamingAppDataStorage("MusiCloud")
            Return New SettingsProvider(Storage)
        End Function

    End Class

End Namespace

