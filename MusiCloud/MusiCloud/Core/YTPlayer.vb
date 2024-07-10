Imports System.Drawing.Imaging
Imports System.Runtime.Remoting
Imports System.Threading
Imports System.Web.ModelBinding
Imports System.Web.UI.WebControls
Imports System.Windows.Media.Animation
Imports AngleSharp.Dom
Imports EO.WebBrowser
Imports EO.WebEngine
Imports EO.WinForm
Imports NSoup
Imports NSoup.Select

Namespace Core
    Public Class YTPlayer
        Implements IDisposable

        Public Enum YTMode
            Embed = 0
            Natural = 1
        End Enum

#Region " Constructor "

        Public Sub New()

            Dim CachePath As String = IO.Path.Combine(Core.GlobalInstances.AppChacheFolder, "Chromium")

            Try
                If IO.Directory.Exists(CachePath) = False Then
                    IO.Directory.CreateDirectory(CachePath)
                End If
            Catch ex As Exception

            End Try

            Engine = EO.WebEngine.Engine.Create("MusiWeb")

            Engine.Options.CachePath = CachePath
            Engine.Options.BypassUserGestureCheck = True
            Engine.Options.AllowProprietaryMediaFormats()

            WebView = New EO.WebBrowser.WebView() With {.Engine = Engine, .CustomUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.3"}
            WebControl = New EO.WinForm.WebControl With {.WebView = WebView, .Dock = DockStyle.Fill, .BackColor = Color.FromArgb(14, 14, 14), .Visible = True}

            AddHandler WebView.BeforeDownload, Sub(sender As Object, e As BeforeDownloadEventArgs)
                                                   e.ShowDialog = False
                                               End Sub

            AddHandler WebView.CertificateError, Sub(sender As Object, e As CertificateErrorEventArgs)
                                                     e.Continue()
                                                 End Sub

            AddHandler WebView.RequestPermissions, Sub(sender As Object, e As RequestPermissionEventArgs)
                                                       e.Allow()
                                                   End Sub

            DevForm = New WebDevTools

        End Sub

        Public Sub ShowDev()
            DevForm.Show()
            WebView.ShowDevTools(DevForm.Handle)
        End Sub

        Public Sub HideDev()
            DevForm.Hide()
        End Sub

#End Region

#Region " Properties "

        Public Property DevForm As WebDevTools = Nothing

        Public Property PlayerForm As Form = Nothing

        Public Property BaseUrl As String = String.Empty

        Public Property BaseMode As YTMode = YTMode.Embed

        Public Property IsValid As Boolean = False

        Public Property Engine As EO.WebEngine.Engine = Nothing

        Public WithEvents WebView As EO.WebBrowser.WebView = Nothing
        Public Property WebControl As EO.WinForm.WebControl = Nothing

#End Region

#Region " Methods "

        Public Function Load(ByVal Url As String, Optional ByVal PlayerMode As YTMode = YTMode.Embed) As NavigationTask
            BaseMode = PlayerMode
            BaseUrl = Url
            Engine.Options.DisableGPU = Core.GlobalInstances.AppSettings.DisableGPU

            If WebControl.Parent IsNot Nothing Then
                WebControl.Parent.BeginInvoke(Sub()
                                                  WebControl.Visible = False
                                              End Sub)
            End If


            Dim ID As String = Core.Helpers.GetYouTubeVideoIdFromUrl(Url)
            If String.IsNullOrWhiteSpace(ID) = False Then
                IsValid = True
                Select Case BaseMode
                    Case YTMode.Embed
                        BaseUrl = "https://www.youtube.com/embed/" & ID & "?autoplay=1&loop=1&controls=1&rel=0&showinfo=0&modestbranding=1"
                    Case YTMode.Natural
                        If Url.ToLower.Contains("list") = True Then
                            BaseUrl = Url
                        Else
                            BaseUrl = "https://www.youtube.com/watch?v=" & ID
                        End If
                End Select
            Else
                BaseUrl = Url
                IsValid = False
            End If
            If BaseMode = YTMode.Embed Then
                Return WebView.LoadHtml(IFrameEmbed.Replace("$URL$", BaseUrl))
            Else
                Return WebView.LoadUrl(BaseUrl)
            End If
        End Function

#End Region

#Region " Disposed "

        Private disposedValue As Boolean

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then

                End If

                disposedValue = True
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' No cambie este código. Coloque el código de limpieza en el método "Dispose(disposing As Boolean)".
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub


#End Region

#Region " Events "

        Private Sub WebViewHost_UrlChanged(sender As Object, e As EventArgs) Handles WebView.UrlChanged
            Dim ID As String = Core.Helpers.GetYouTubeVideoIdFromUrl(WebView.Url)
            If ID = String.Empty Then
                If String.IsNullOrEmpty(BaseUrl) = True Then
                    WebView.StopLoad()
                Else
                    WebView.LoadUrl(BaseUrl).OnDone(Sub()
                                                        ExecutePaths()
                                                    End Sub)
                End If
            Else
                ExecutePaths()
            End If
        End Sub

        Private Sub ExecutePaths()
            Try

                Dim t As New Thread(Sub()
                                        Dim Intent As Integer = 0
                                        For i As Integer = 0 To 2
                                            If Intent >= 1 Then
                                                Exit For
                                            End If
                                            Try
                                                Dim DOM = WebView.GetDOMWindow
                                                If DOM IsNot Nothing Then
                                                    Dim Document As DOM.Document = DOM.document
                                                    If Document IsNot Nothing Then
                                                        If BaseMode = YTMode.Natural Then
                                                            Dim PlayerElement As DOM.Element = Document.getElementById("ytd-player")

                                                            If PlayerElement IsNot Nothing Then
                                                                WebView.EvalScript(FixedVideo)
                                                                WebView.EvalScript(CleanYT)
                                                                Play()
                                                                Intent += 1
                                                            End If
                                                        ElseIf BaseMode = YTMode.Embed Then
                                                            Intent += 1
                                                        End If

                                                        If TypeOf WebControl.Parent.Parent Is Form Then
                                                            Dim Title As String = WebView.EvalScript("document.title").ToString
                                                            If String.IsNullOrEmpty(Title) = False Then

                                                                WebControl.Parent.Parent.BeginInvoke(Sub()
                                                                                                         Try
                                                                                                             WebControl.Parent.Parent.Text = Title.Replace("- YouTube", "").Replace(", Please Wait...", "")
                                                                                                         Catch ex As Exception
                                                                                                             Intent = 0
                                                                                                         End Try
                                                                                                     End Sub)
                                                                If BaseMode = YTMode.Embed Then
                                                                    Try
                                                                        If WebControl.Parent IsNot Nothing Then
                                                                            WebControl.Parent.BeginInvoke(Sub()
                                                                                                              WebControl.Visible = True
                                                                                                          End Sub)

                                                                        End If
                                                                    Catch ex As Exception : End Try
                                                                End If
                                                            End If
                                                        End If

                                                    End If
                                                End If

                                                System.Threading.Thread.Sleep(1000)
                                                i -= 1
                                            Catch ex As Exception : End Try
                                        Next
                                        HandleOnTimeUpdate()
                                        WebView.EvalScript(AdsBlock)
                                        RaiseEvent OnComplete()
                                    End Sub)
                t.Start()

            Catch ex As Exception : End Try
        End Sub

        Public Event OnComplete()

        Private Sub WebViewHost_LoadCompleted(sender As Object, e As LoadCompletedEventArgs) Handles WebView.LoadCompleted
            If BaseMode = YTMode.Embed Then
                WebView.EvalScript(DeleteYoutubeLogo)
            End If
        End Sub

        Private Sub WebView_FullScreenModeChanged(sender As Object, e As FullscreenModeChangedArgs) Handles WebView.FullScreenModeChanged
            'If BaseMode = YTMode.Natural Then
            '    If e.Fullscreen = True Then
            '        If PlayerForm IsNot Nothing Then
            '            PlayerForm.Close()
            '            PlayerForm.Dispose()
            '        End If
            '        PlayerForm = New Form With {.Size = New Size(800, 600)}
            '        e.HWndOwner = PlayerForm.Handle
            '        e.Parent = PlayerForm
            '        PlayerForm.Show()
            '    Else
            '        PlayerForm.Hide()
            '    End If
            'End If
        End Sub

        Private Sub WebViewHost_NewWindow(sender As Object, e As NewWindowEventArgs) Handles WebView.NewWindow
            e.Accepted = False

            Dim ParsedUrl As Uri = New Uri(e.TargetUrl)

            If IsYoutube(ParsedUrl.Host) Then
                Me.Load(e.TargetUrl, BaseMode)
            End If

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

#End Region

#Region " JS Patch "

        Private IFrameEmbed As String = <![CDATA[
<!DOCTYPE html>
<html>
<body>
<style>
* {padding:0;margin:0;box-sizing:border-box;}

#video {
  position: relative;
  padding-bottom: 56.25%; /* 16:9 */
  height: 0;
}

#video iframe {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
}
</style>
<div id="video">
  <iframe width="100%" height="100%" src="$URL$" frameborder="0" allowfullscreen="allowfullscreen"></iframe>
</div>
</body>
</html>
]]>.Value

        Private DeleteYoutubeLogo As String = <![CDATA[
    var elementos = document.querySelectorAll('.ytp-youtube-button');
    
    elementos.forEach(function(elemento) {
        elemento.parentNode.removeChild(elemento);
    });

    var miniButton = document.querySelector('button.ytp-miniplayer-button.ytp-button');
if (miniButton) {
    miniButton.outerHTML = ''; // Eliminar el botón
}

]]>.Value

        Private FixedVideo As String = <![CDATA[
// Función para aplicar estilos a un elemento seleccionado
function applyStyles(selector, styles) {
    var element = document.querySelector(selector);
    if (element) {
        var styleElement = document.createElement('style');
        styleElement.innerHTML = styles;
        document.head.appendChild(styleElement);
    }
}

// Obtener el elemento de video original
var video = document.getElementById('ytd-player');

// Crear un contenedor para el video a pantalla completa
var fullscreenContainer = document.createElement('div');
fullscreenContainer.id = 'fullscreen-video';
fullscreenContainer.appendChild(video);
document.body.appendChild(fullscreenContainer);

// Aplicar estilos al contenedor de pantalla completa
applyStyles('#fullscreen-video', `
    #fullscreen-video {
        position: fixed !important;
        top: 0 !important;
        left: 0 !important;
        width: 100% !important;
        height: 100% !important;
        z-index: 9999 !important; /* Asegúrate de que esté por encima de otros elementos */
        background-color: black !important; /* Opcional, para el fondo del video */
    }
`);

// Aplicar estilos al video para ajustarse al contenedor de pantalla completa
applyStyles('video.video-stream.html5-main-video', `
    video.video-stream.html5-main-video {
        position: fixed !important;
        top: 0 !important;
        left: 0 !important;
        width: 100% !important;
        height: 100% !important;
        z-index: 9999 !important; /* Asegúrate de que esté por encima de otros elementos */
        background-color: transparent !important; /* Opcional, para el fondo del video */
    }
`);

// Aplicar estilos a los controles del video para que ocupen todo el ancho
applyStyles('div.ytp-chrome-bottom', `
    div.ytp-chrome-bottom {
        width: 100% !important;
    }
`);

// Ocultar la barra de desplazamiento en el cuerpo del documento
document.documentElement.style.overflow = 'hidden';
document.body.scroll = 'no'; // Esto puede que no sea necesario en todos los navegadores

// Encontrar y hacer clic en el botón de tamaño para maximizar el video
var resizeButton = document.querySelector('button.ytp-size-button.ytp-button');
if (resizeButton) {
    resizeButton.click();
    setTimeout(1000); // tiempo de espera para dar click antes de eliminar el boton
    resizeButton.outerHTML = ''; // Eliminar el botón después de hacer clic
}

// Encontrar y eliminar el botón de mini reproductor
var miniButton = document.querySelector('button.ytp-miniplayer-button.ytp-button');
if (miniButton) {
    miniButton.outerHTML = ''; // Eliminar el botón
}

 eoapi.extInvoke("OnShow");

]]>.Value

        Private CleanYT As String = <![CDATA[
document.getElementsByTagName('html')[0].setAttribute('dark','true');
document.getElementById("info").setAttribute("style","visibility: hidden");
document.getElementById("meta").setAttribute("style","display: none");
document.getElementById("comments").setAttribute("style","display: none");
document.getElementById("secondary").setAttribute("style","visibility: hidden");
document.getElementsByClassName("hideSurrounding--1_wpe")[0].setAttribute("class", "main--27G0M hideSurrounding--1_wpe hideSurroundingActive--18vmE hideSurroundingDark--3vdYx");
]]>.Value


        Private PlayPauseButton As String = <![CDATA[
    var PlayPauseButton = document.querySelector('button.ytp-play-button.ytp-button');
if (PlayPauseButton) {
    PlayPauseButton.click();
}
]]>.Value

        Public Sub TogglePlayState()
            WebView.EvalScript(PlayPauseButton)
        End Sub

        Public Sub Play()

            Dim PlayJS As String = <![CDATA[
     var video = document.querySelector('video');
if (video) {
      video.play();
}
]]>.Value

            WebView.EvalScript(PlayJS)

        End Sub


        Public Sub Pause()

            Dim PauseJS As String = <![CDATA[
     var video = document.querySelector('video');
if (video) {
      video.pause();
}
]]>.Value

            WebView.EvalScript(PauseJS)

        End Sub

        Public Sub SetVolume(ByVal Vol As Double)

            Dim PauseJS As String = <![CDATA[
     var video = document.querySelector('video');
if (video) {
      video.volume = $vol$;
}
]]>.Value

            WebView.EvalScript(PauseJS.Replace("$vol$", Vol))

        End Sub

        Public Sub SetMute(ByVal Vol As Boolean)
            Dim Value As String = If(Vol, "true", "false")

            Dim PauseJS As String = <![CDATA[
     var video = document.querySelector('video');
if (video) {
      video.muted = $val$;
}
]]>.Value

            WebView.EvalScript(PauseJS.Replace("$val$", Value))

        End Sub

        Public Function GetCurrentTime() As TimeSpan
            Try
                Dim CurrentTime As Double = Math.Round(WebView.EvalScript("document.querySelector('video').currentTime;"))
                Dim CurrentTimeSpan As TimeSpan = TimeSpan.FromSeconds(CurrentTime)
                Return CurrentTimeSpan
            Catch ex As Exception : End Try
            Return TimeSpan.FromSeconds(0)
        End Function

        Public Function GetDurationTime() As TimeSpan
            Try
                Dim DurationTime As Double = Math.Round(WebView.EvalScript("document.querySelector('video').duration;"))
                Dim DurationTimeSpan As TimeSpan = TimeSpan.FromSeconds(DurationTime)
                Return DurationTimeSpan
            Catch ex As Exception : End Try
            Return TimeSpan.FromSeconds(0)
        End Function

        Public Function GetAutoPlay() As Boolean
            Try
                Dim Value As String = WebView.EvalScript("document.querySelector('div.ytp-autonav-toggle-button').getAttribute('aria-checked');").ToString
                Return CBool(Value)
            Catch ex As Exception : End Try
            Return False
        End Function

        Public Sub SetCurrentTime(ByVal Seconds As Double)
            Dim PauseJS As String = <![CDATA[
     var video = document.querySelector('video');
if (video) {
      video.currentTime = $val$;
}
]]>.Value

            WebView.EvalScript(PauseJS.Replace("$val$", Seconds))
        End Sub

        Public Event PositionChange(ByVal newPosition As TimeSpan, ByVal DurationTime As TimeSpan)
        Public Event OnStateChange(ByVal IsOnPlay As Boolean)
        Public Event MouseDown(ByVal Key As Keys)
        Public Event MouseUp(ByVal Key As Keys)
        Public Event MouseMove(ByVal XYPoint As Point)

        Public Sub HandleOnTimeUpdate()

            Dim HookerJS As String = <![CDATA[
     var video = document.querySelector('video');
if (video) {

     video.ontimeupdate = (event) => {  
          var video = document.querySelector('video');
          if (video) {
            eoapi.extInvoke("OnTimeUpdate", [video.currentTime, video.duration]);
             var miniButton = document.querySelector('button.branding-img-container.ytp-button');
                if (miniButton) {
                    if (miniButton.outerHTML != '') {  miniButton.outerHTML = ''; }
                }
          }
     };

     video.onplay = (event) => { eoapi.extInvoke("OnPlay"); };

     video.onpause = (event) => { eoapi.extInvoke("OnPause"); };

     video.onmousedown = (e) => { 
     eoapi.extInvoke("MouseDown", [e.button]);

     };

     video.onmouseup = (e) => { 
      eoapi.extInvoke("MouseUp", [e.button]);
     };

     video.onmousemove = (e) => { 
      eoapi.extInvoke("OnMouseMove", [e.offsetX, e.offsetY]);
     };

     video.onplaying = (event) => { eoapi.extInvoke("OnPlaying"); };

}
]]>.Value

            WebView.EvalScript(HookerJS)

        End Sub

        Private Sub WebView_JSExtInvoke(sender As Object, e As JSExtInvokeArgs) Handles WebView.JSExtInvoke
            Select Case e.FunctionName
                Case "OnTimeUpdate"
                    Try
                        Dim CurrentTime As Double = Math.Round(e.Arguments(0))
                        Dim Duration As Double = Math.Round(e.Arguments(1))

                        Dim CurrentTimeSpan As TimeSpan = TimeSpan.FromSeconds(CDbl((New Decimal(CurrentTime))))

                        Dim DurationTimeSpan As TimeSpan = TimeSpan.FromSeconds(CDbl((New Decimal(Duration))))

                        'Dim CurrentTimeSpan As TimeSpan = TimeSpan.FromSeconds(CurrentTime)
                        'Dim DurationTimeSpan As TimeSpan = TimeSpan.FromSeconds(Duration)
                        RaiseEvent PositionChange(CurrentTimeSpan, DurationTimeSpan)
                    Catch ex As Exception : Console.WriteLine("OnTimeUpdate: " & ex.Message) : End Try
                Case "OnPlay"
                    Try
                        RaiseEvent OnStateChange(True)
                    Catch ex As Exception : End Try
                Case "OnPause"
                    Try
                        RaiseEvent OnStateChange(False)
                    Catch ex As Exception : End Try
                Case "MouseDown"
                    Try
                        Select Case e.Arguments(0)
                            Case 0
                                RaiseEvent MouseDown(Keys.LButton)
                            Case 1
                                RaiseEvent MouseDown(Keys.MButton)
                            Case 2
                                RaiseEvent MouseDown(Keys.RButton)
                        End Select
                    Catch ex As Exception : End Try
                Case "MouseUp"
                    Try
                        Select Case e.Arguments(0)
                            Case 0
                                RaiseEvent MouseUp(Keys.LButton)
                            Case 1
                                RaiseEvent MouseUp(Keys.MButton)
                            Case 2
                                RaiseEvent MouseUp(Keys.RButton)
                        End Select
                    Catch ex As Exception : End Try
                Case "OnMouseMove"
                    Try
                        Dim NewPoint As Point = New Point(e.Arguments(0), e.Arguments(1))
                        RaiseEvent MouseMove(NewPoint)
                    Catch ex As Exception : End Try
                Case "OnPlaying"
                    Try
                        If WebControl.Parent IsNot Nothing Then
                            WebControl.Parent.BeginInvoke(Sub()
                                                              WebControl.Visible = True
                                                          End Sub)
                        End If

                        Dim Title As String = WebView.EvalScript("document.title").ToString
                        If String.IsNullOrEmpty(Title) = False Then
                            WebControl.Parent.Parent.BeginInvoke(Sub()
                                                                     Try
                                                                         WebControl.Parent.Parent.Text = Title.Replace("- YouTube", "").Replace(", Please Wait...", "")
                                                                     Catch ex As Exception : End Try
                                                                 End Sub)
                        End If

                    Catch ex As Exception : End Try
                Case "OnShow"
                    Try
                        If WebControl.Parent IsNot Nothing Then
                            WebControl.Parent.BeginInvoke(Sub()
                                                              WebControl.Visible = True
                                                          End Sub)

                        End If
                    Catch ex As Exception : End Try
            End Select
        End Sub

        Private AdsBlock As String = <![CDATA[

setInterval(function() {
// Check if still on correct page



 if (true === true) {
    

// Check if a Skip Ad button is already available (before starting the ad skipping loop)
SKIP_AD_BUTTON_ELEMENTS_0 = document.getElementsByClassName("ytp-ad-skip-button-modern ytp-button")
SKIP_AD_BUTTON_ELEMENT_0 = SKIP_AD_BUTTON_ELEMENTS_0[0]
if (SKIP_AD_BUTTON_ELEMENT_0 === null || SKIP_AD_BUTTON_ELEMENT_0 === undefined) {
//console.debug("Did not click on Skip Ad button - Button not found")
} else {
SKIP_AD_BUTTON_ELEMENT_0.click()
}

// Check if a Skip Ad button (non modern one) is already available (before starting the ad skipping loop)
SKIP_AD_BUTTON_ELEMENTS_1 = document.getElementsByClassName("ytp-ad-skip-button ytp-button")
SKIP_AD_BUTTON_ELEMENT_1 = SKIP_AD_BUTTON_ELEMENTS_1[0]
if (SKIP_AD_BUTTON_ELEMENT_1 === null || SKIP_AD_BUTTON_ELEMENT_1 === undefined) {
//console.debug("Did not click on Skip Ad button - Button not found")
} else {
SKIP_AD_BUTTON_ELEMENT_1.click()
}



// Check if there's an ad banner (on the right side)
AD_SLOT_2 = document.getElementsByClassName("style-scope ytd-in-feed-ad-layout-renderer")[0]
if (AD_SLOT_2 !== null && AD_SLOT_2 !== undefined && AD_SLOT_2.remove !== null && AD_SLOT_2.remove !== undefined) {
AD_SLOT_2.remove()
}

AD_SLOT_3 = document.getElementsByClassName("style-scope ytd-player-legacy-desktop-watch-ads-renderer")[0]
if (AD_SLOT_3 !== null && AD_SLOT_3 !== undefined) {
AD_SLOT_3.remove()
}

CURRENT_VAL = 0
STOP_LOOP = false
// Ad skipping loop
SELF_LOOP = setInterval(function() {
if (STOP_LOOP === false) {
//console.debug("Loop2")
CURRENT_VAL += 1
if (CURRENT_VAL === 50) {
//console.debug("Exiting ad skipping loop - Limit reached")
STOP_LOOP = true
clearInterval(SELF_LOOP)
}
PROGRESS_BAR_ELEMENT = document.getElementsByClassName('ytp-play-progress ytp-swatch-background-color')[0]
if (typeof PROGRESS_BAR_ELEMENT === "object" && PROGRESS_BAR_ELEMENT !== null) {
CHECKED_ELEM_COLOR = window.getComputedStyle(PROGRESS_BAR_ELEMENT).backgroundColor 
if (CHECKED_ELEM_COLOR !== "rgb(255, 204, 0)") {
if (CHECKED_ELEM_COLOR === "rgb(255, 0, 0)") {
//console.debug("Not in an ad")
STOP_LOOP = true
clearInterval(SELF_LOOP)
} else {
//console.debug("Did not ad skip - No ad detected")
}
} else {
//console.debug("In an ad")
// First, go to the end of the ad (it's considered a video)
VID_ELEM = document.querySelectorAll("video")[0]
if (VID_ELEM !== null && VID_ELEM !== undefined) {
if (VID_ELEM.currentTime !== null && VID_ELEM.currentTime !== undefined && VID_ELEM.duration !== null && VID_ELEM.duration !== undefined && VID_ELEM.currentTime !== NaN && VID_ELEM.duration !== NaN) {
VID_ELEM.currentTime = VID_ELEM.duration
}
} 
// Now, simulate a click on the Skip Ad button (if it's found, sometimes going to the end of the ad doesn't prompt the Skip Ad button and just instantly puts the video or next ad)
SKIP_AD_BUTTON_ELEMENTS = document.getElementsByClassName("ytp-ad-skip-button-modern ytp-button")
SKIP_AD_BUTTON_ELEMENT = SKIP_AD_BUTTON_ELEMENTS[0]
if (SKIP_AD_BUTTON_ELEMENT === null || SKIP_AD_BUTTON_ELEMENT === undefined) {
//console.debug("Did not click on Skip Ad button - Button not found")
} else {
SKIP_AD_BUTTON_ELEMENT.click()
}

// Incase it's not modern
SKIP_AD_BUTTON_ELEMENTS_2 = document.getElementsByClassName("ytp-ad-skip-button ytp-button")
SKIP_AD_BUTTON_ELEMENT_2 = SKIP_AD_BUTTON_ELEMENTS_2[0]
if (SKIP_AD_BUTTON_ELEMENT_2 === null || SKIP_AD_BUTTON_ELEMENT_2 === undefined) {
//console.debug("Did not click on Skip Ad button - Button not found")
} else {
SKIP_AD_BUTTON_ELEMENT_2.click()
}






}
}
}
}
, 0)
}
}
, 0)

]]>.Value


#End Region

    End Class

End Namespace

