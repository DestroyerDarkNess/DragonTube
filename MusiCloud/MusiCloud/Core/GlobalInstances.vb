Imports System.Reflection

Namespace Core
    Public Class GlobalInstances

        Public Shared DesktopUserAgent As String = "Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.0.0 Safari/537.36"
        Public Shared MobileUserAgent As String = "Mozilla/5.0 (Linux; Android 14) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.6261.90 Mobile Safari/537.36"
        Public Shared LoginUserAgent As String = "Mozilla/5.0 (Linux; Android) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.109 Safari/537.36 CrKey/1.54.248666"

        Public Shared Function GetUserAgent() As String
            Select Case Core.GlobalInstances.AppSettings.UserAgent
                Case BrowserUserAgent.Desktop
                    Return DesktopUserAgent
                Case BrowserUserAgent.Mobile
                    Return MobileUserAgent
                Case Else
                    Return "Mozilla/5.0 (Fuchsia) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36 CrKey/1.56.500000"
            End Select
            Return String.Empty
        End Function

        Public Shared ReadOnly ID As String = Guid.NewGuid().ToString("N")
        Public Shared ReadOnly CurrentExePath As String = IO.Path.GetDirectoryName(Application.ExecutablePath)
        Public Shared ReadOnly ExeptionManager As Core.ExceptionManager = New Core.ExceptionManager
        Public Shared ReadOnly AppSettings As AppSettings = AppSettings.Load
        Public Shared ReadOnly AppVersion As String = FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion

        Public Shared ReadOnly AppName As String = "MusiCloud"
        Public Shared ReadOnly AppChacheFolder As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), AppName)

        Public Shared ReadOnly VLCDir As String = IO.Path.Combine(AppChacheFolder, "libvlc", "win-x86")

        Public Shared ffmpegPath As String = String.Empty
        Public Shared ReadOnly MyVideos As String = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)
        Public Shared ReadOnly MyMusic As String = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)

        Public Shared ReadOnly MyVideosInDragon As String = IO.Path.Combine(MyVideos, "DragonTube")
        Public Shared ReadOnly MyMusicInDragon As String = IO.Path.Combine(MyMusic, "DragonTube")

        Public Shared Sub CreateCachePath()
            Dim NewDir As New IO.DirectoryInfo(AppChacheFolder)
            If NewDir.Exists = False Then NewDir.Create()
            If IO.File.Exists(ffmpegPath) = False Then
                Dim AppFolder As String = IO.Path.GetDirectoryName(Application.ExecutablePath)
                ffmpegPath = IO.Path.Combine(AppFolder, "x86", "ffmpeg.exe")
            End If
            If IO.Directory.Exists(MyVideosInDragon) = False Then IO.Directory.CreateDirectory(MyVideosInDragon)
            If IO.Directory.Exists(MyMusicInDragon) = False Then IO.Directory.CreateDirectory(MyMusicInDragon)
        End Sub


        Public Shared ReadOnly CopyRigth As String = <![CDATA[
 ||      
//////////////////////////////// ||
          Part 1 ||
//////////////////////////////// ||
 || ||
   Disclaimer: Access to YouTube Content  
   || ||
DragonTube is a YouTube client that allows you to access and play video and audio content hosted on YouTube. We want to inform you about some important points related to the use of this application:
|| ||
1. Application Purpose: DragonTube is designed to give you a more convenient music and video streaming experience, allowing you to search, discover and enjoy content available on YouTube.
|| ||
2. Access to third-party content: By using DragonTube, you are accessing content provided by YouTube and hosted on its servers. Please note that YouTube is a platform owned by Google and is subject to its terms of service and privacy policies.
|| ||
3. User Responsibility: It is important that you understand that any content you access through DragonTube is the sole responsibility of the original creators and owners. DragonTube acts solely as a means to access this content and assumes no responsibility for its creation, editing or distribution.
|| ||
4. Compliance with YouTube Terms of Service: By using DragonTube to access YouTube content, you agree to comply with YouTube's terms of service and policies. We recommend that you review YouTube's terms of service and privacy policies to fully understand your rights and responsibilities as a user.
|| ||
//////////////////////////////// ||
          Part 2 ||
//////////////////////////////// ||
|| ||
 Disclaimer: Downloading Content and More. 
 || ||
DragonTube allows you to download content from YouTube. It is important that you understand that downloading content from YouTube may violate the platform's rules, as well as the copyright of the content owner.
|| ||
a) By using DragonTube, you agree to the following:
|| ||
1. You are responsible for your own actions and any legal consequences that may arise from downloading content from YouTube.
2. You will not download copyrighted content without the owner's permission.
3. You will not use DragonTube for illegal or commercial purposes.
4. You will not distribute or share the downloaded content with other people.
5. You understand that YouTube may terminate your account or take other action if you use DragonTube to violate its rules.
|| ||
b) DragonTube is not responsible for:
|| ||
1. Any violation of YouTube rules or copyright that may arise from downloading content.
2. Any damage or loss you may suffer as a result of using DragonTube.
3. The availability or quality of the downloaded content.
4. Any Use that the user gives to DragonTube.
|| ||
//////////////////////////////// || 
          Part 3 ||
//////////////////////////////// || 
|| ||
 Disclaimer: FAQ - End.
 || ||
We recommend that you check YouTube's rules and content copyright before downloading.
|| ||
If you have questions or concerns about using DragonTube, please contact the developer.
|| ||
By using DragonTube, you agree to this Disclaimer.
|| ||
Please remember that DragonTube is a third-party application and is not affiliated with or endorsed by YouTube or Google. If you have any questions or concerns about content you find through DragonTube, we recommend contacting the YouTube support team directly.
|| ||
Thank you for using DragonTube. Enjoy your music and video playing experience!
|| ||
        ]]>.Value



        Public Shared ReadOnly LoopCode As String = <![CDATA[
            iVBORw0KGgoAAAANSUhEUgAAAB4AAAAeCAYAAAA7MK6iAAAACXBIWXMAAAsTAAALEwEAmpwYAAABB0lEQVR4nO3VPUrEQByG8RSuVuq6YCGeYAsPYrXgISwEbb2CtZ0XsbFT1g/YwkrxCoIWaqMWPxkykUUwJJmYxjz1vPNk/sy8ybKenprgDFtZ18j5xAnWuxYXPOMQi1WCS9jHNd60wx22y6SbuG1JVk0sP2khvccEy4mjforTG5QFDuakw7rCH+IPHGNUJXATQ5Om0rjPKcZ1Aq9RXHu8SeAC551Ke/4C+eW96lq6Gi/uS53QODyRRPFOFF9WWTyKJRDKQIJ0iIco3itbOIg1F+rumwbClXjSQjr7tTpDkcdCb5sZNsq+sk1xaMJpGG/pT2JOvoBdPKaMujFYwxHeOxW3+Zx6/idfK8KdxerXVWUAAAAASUVORK5CYII=
        ]]>.Value

        Public Shared ReadOnly NextCode As String = <![CDATA[
          iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAACXBIWXMAAAsTAAALEwEAmpwYAAAAhUlEQVR4nO3UQQrCQAxA0Vl4CQteRJRurUfyOB7JI3Su4NonsyhCsYU6Mwu1f5mEfEhCQlhZAs6IPieimxPkNB/o5wRFCH8n2OGC+yi+xyFbEF65Blc8RvEyggG0uFUTJLD5XoFaI1JrydhOnGk60WMJwWJ+WxBrv+suFeQ0x2lSsBLe8ATbIXOuQKODlAAAAABJRU5ErkJggg==
 ]]>.Value

        Public Shared ReadOnly RandomCode As String = <![CDATA[
      iVBORw0KGgoAAAANSUhEUgAAABkAAAAZCAYAAADE6YVjAAAACXBIWXMAAAsTAAALEwEAmpwYAAAA9UlEQVR4nO3Vr0sEYRDG8QVBDjTYzAe2AzEY/QGn1WSwWQWTRuX+DrEaNZoOthqNmoyCP+7ime8jCxteF+8473bggk9ahtn9vu/OPDNZ9q+K0MyiAeijHQ3axif2JklO9YVrLP6St4tnDCvvFDda+svpVpGjU4mv4AOHWChj7al/GdbQw3ISu8RNrTXBLc7K5wbe0aq1u7CB16I2OMX9TB8cA8pxjC62oiD7eMJ6CCABPeIgEtDCER4iIXlRC7yE1ASbSXedhHQX7nCe+OStVp+McPxFbY4Pm11+aoCrEVN4p/TNbFN4in3Sm2ifzPtmbIYC5kLfpFyVdwMcGlIAAAAASUVORK5CYII=
  ]]>.Value

    End Class

End Namespace

