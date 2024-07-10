Imports EO.WebEngine
Imports MusiCloud.Core

Public Class Splash

    Dim ScreenSize As Size

    Private Sub Splash_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.ScreenSize = New Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height)
        Me.Location = New Point(Me.ScreenSize.Width + MyBase.Width, 30)
        EO.WebBrowser.Runtime.AddLicense("Kb114+30EO2s3OmxGeCm3MGz8M5nzunz7fGo7vf2HaF3s7P9FOKe5ff2EL112PD9GvZ3s+X1D5+t8PT26KF+xrLUE/Go5Omzy5+v3PYEFO6ntKbC461pmaTA6bto2PD9GvZ3s/MDD+SrwPL3Gp+d2Pj26KFpqbPC3a5rp7XIzZ+v3PYEFO6ntKbC46FotcAEFOan2PgGHeR36d7SGeWawbMKFOervtrI9eBysO3XErx2s7MEFOan2PgGHeR3s7P9FOKe5ff26XXj7fQQ7azcws0X6Jzc8gQQyJ21tMbbtnCttcbcs3Wm8PoO5Kfq6doP")
        Dim ScrollCSS As String = <![CDATA[
   ::-webkit-scrollbar{
width: 5px;
}

::-webkit-scrollbar-track{
background: transparent;
}

::-webkit-scrollbar-thumb{
background-color: gray;
}

]]>.Value


        Dim BrowserOpt As BrowserOptions = New BrowserOptions()
        BrowserOpt.EnableWebSecurity = False
        BrowserOpt.EnableXSSAuditor = False
        BrowserOpt.UserStyleSheet = ScrollCSS.ToString
        BrowserOpt.AllowJavaScript = True

        EO.WebBrowser.Runtime.SetDefaultBrowserOptions(BrowserOpt)
        EO.Base.Runtime.EnableCrashReport = False
        AddHandler EO.Base.Runtime.Exception, AddressOf Runtime_Exception
    End Sub
    Private Shared Sub Runtime_Exception(ByVal sender As Object, ByVal e As EO.Base.ExceptionEventArgs)
        Console.WriteLine("Runtime Exception : " & e.ErrorException.Message)
    End Sub

    Private Sub Splash_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        ShowBar()
    End Sub

    Private Sub ShowBar()
        Dim num As Integer = 0
        While True
            Dim x As Integer = MyBase.Location.X
            Dim flag As Boolean = x > Me.ScreenSize.Width - MyBase.Width
            If Not flag Then
                Exit While
            End If
            MyBase.Location = New Point(MyBase.Location.X - 1, MyBase.Location.Y)
            Application.DoEvents()
            num -= 1
            num += 1
            If num > 2 Then
                GoTo IL_9C
            End If
        End While
        MyBase.Location = New Point((Me.ScreenSize.Width - 10) - MyBase.Width, MyBase.Location.Y)
IL_9C:
        Me.Refresh()
    End Sub

End Class