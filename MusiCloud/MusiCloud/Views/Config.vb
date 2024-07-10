Imports Guna.UI2.WinForms

Public Class Config

#Region " Constructor "

    Public Sub New()
        SetStyle(ControlStyles.SupportsTransparentBackColor, value:=True)
        InitializeComponent()
        Me.BackColor = Color.Transparent
    End Sub

#End Region

    Private Sub Config_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadConfig()
    End Sub

    Public Sub LoadConfig()
        LogInNumeric1.Value = Core.GlobalInstances.AppSettings.VideoLimit
        LogInComboBox1.SelectedIndex = Core.GlobalInstances.AppSettings.MediaPlayerEngine
        LogInComboBox2.SelectedIndex = Core.GlobalInstances.AppSettings.BitRate
        LogInComboBox3.SelectedIndex = Core.GlobalInstances.AppSettings.Quality
        LogInComboBox4.SelectedIndex = Core.GlobalInstances.AppSettings.UserAgent
        Guna2ToggleSwitch1.Checked = Core.GlobalInstances.AppSettings.Notifications
        Guna2ToggleSwitch2.Checked = Not Core.GlobalInstances.AppSettings.DisableGPU
        Guna2ToggleSwitch3.Checked = Core.GlobalInstances.AppSettings.VolumeSync
        Guna2ToggleSwitch4.Checked = Core.GlobalInstances.AppSettings.ExternalYT
        Guna2ToggleSwitch5.Checked = Core.GlobalInstances.AppSettings.SystemTray
        Guna2ToggleSwitch6.Checked = Core.GlobalInstances.AppSettings.RoundedEdges
        Guna2ToggleSwitch7.Checked = Core.GlobalInstances.AppSettings.WindowsControlBox
        Guna2ToggleSwitch8.Checked = Core.GlobalInstances.AppSettings.NaturalYT
        Guna2ToggleSwitch9.Checked = Core.GlobalInstances.AppSettings.MaximizedInApp
        Guna2ToggleSwitch10.Checked = Core.GlobalInstances.AppSettings.SupportAVCodec
        LogInNumeric1.Invalidate()
    End Sub

    Dim IsShow As Boolean = False

    Private Sub Config_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        IsShow = True
    End Sub

    Private Sub LogInNumeric1_TextChanged(sender As Object, e As EventArgs) Handles LogInNumeric1.TextChanged
        If IsShow = False Then Return
        Core.GlobalInstances.AppSettings.VideoLimit = LogInNumeric1.Value
        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
    End Sub

    Private Sub LogInComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LogInComboBox1.SelectedIndexChanged
        If IsShow = False Then Return
        Dim SelectedIndex As Integer = LogInComboBox1.SelectedIndex
        If SelectedIndex = 0 Then
            If TypeOf Me.ParentForm Is MainView Then
                If DirectCast(Me.ParentForm, MainView).MediaPlayer Is Nothing Then
                    LogInComboBox1.SelectedIndex = Core.GlobalInstances.AppSettings.MediaPlayerEngine
                    DirectCast(Me.ParentForm, MainView).ShowMessage("Please download the necessary libraries to use VLC.", "Additional download required.")
                Else
                    Core.GlobalInstances.AppSettings.MediaPlayerEngine = LogInComboBox1.SelectedIndex
                End If
            End If
        Else
                Core.GlobalInstances.AppSettings.MediaPlayerEngine = LogInComboBox1.SelectedIndex
        End If

        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
        If TypeOf Me.ParentForm Is MainView Then
            Dim Current As Boolean = DirectCast(Me.ParentForm, MainView).Guna2CheckBox1.Checked
            DirectCast(Me.ParentForm, MainView).Guna2CheckBox1.Checked = False
            DirectCast(Me.ParentForm, MainView).Guna2CheckBox1.Checked = Current
        End If
    End Sub

    Private Sub LogInComboBox3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LogInComboBox3.SelectedIndexChanged
        If IsShow = False Then Return
        Core.GlobalInstances.AppSettings.Quality = LogInComboBox3.SelectedIndex
        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
    End Sub

    Private Sub LogInComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LogInComboBox2.SelectedIndexChanged
        If IsShow = False Then Return
        Core.GlobalInstances.AppSettings.BitRate = LogInComboBox2.SelectedIndex
        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
    End Sub

    Private Sub Guna2ToggleSwitch1_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2ToggleSwitch1.CheckedChanged
        If IsShow = False Then Return
        Core.GlobalInstances.AppSettings.Notifications = Guna2ToggleSwitch1.Checked
        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
        If TypeOf Me.ParentForm Is MainView Then
            DirectCast(Me.ParentForm, MainView).ToggleNotifications()
        End If
    End Sub

    Private Sub LogInComboBox4_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LogInComboBox4.SelectedIndexChanged
        If IsShow = False Then Return
        Core.GlobalInstances.AppSettings.UserAgent = LogInComboBox4.SelectedIndex
        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
    End Sub

    Private Sub Guna2ToggleSwitch2_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2ToggleSwitch2.CheckedChanged
        If IsShow = False Then Return
        Core.GlobalInstances.AppSettings.DisableGPU = Not Guna2ToggleSwitch2.Checked
        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
        If TypeOf Me.ParentForm Is MainView Then
            Dim Message As DialogResult = DirectCast(Me.ParentForm, MainView).ShowMessage("The application will restart.")
            If Message = DialogResult.OK Then
                Process.Start(Application.ExecutablePath)
                Environment.Exit(0)
            End If
        End If
    End Sub


    Private Sub Guna2ToggleSwitch3_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2ToggleSwitch3.CheckedChanged
        If IsShow = False Then Return
        Core.GlobalInstances.AppSettings.VolumeSync = Guna2ToggleSwitch3.Checked
        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
    End Sub


    Private Sub Guna2ToggleSwitch4_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2ToggleSwitch4.CheckedChanged
        If IsShow = False Then Return
        Core.GlobalInstances.AppSettings.ExternalYT = Guna2ToggleSwitch4.Checked
        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
        If TypeOf Me.ParentForm Is MainView Then
            Dim Message As DialogResult = DirectCast(Me.ParentForm, MainView).ShowMessage("The application will restart.")
            If Message = DialogResult.OK Then
                Process.Start(Application.ExecutablePath)
                Environment.Exit(0)
            End If
        End If
    End Sub


    Private Sub Guna2ToggleSwitch5_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2ToggleSwitch5.CheckedChanged
        If IsShow = False Then Return
        Core.GlobalInstances.AppSettings.SystemTray = Guna2ToggleSwitch5.Checked
        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
    End Sub

    Private Sub Guna2ToggleSwitch6_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2ToggleSwitch6.CheckedChanged
        If IsShow = False Then Return
        Core.GlobalInstances.AppSettings.RoundedEdges = Guna2ToggleSwitch6.Checked
        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
        If TypeOf Me.ParentForm Is MainView Then
            DirectCast(Me.ParentForm, MainView).RoundBorders()
        End If
    End Sub

    Private Sub Guna2ToggleSwitch7_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2ToggleSwitch7.CheckedChanged
        If IsShow = False Then Return
        Core.GlobalInstances.AppSettings.WindowsControlBox = Guna2ToggleSwitch7.Checked
        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
        If TypeOf Me.ParentForm Is MainView Then
            DirectCast(Me.ParentForm, MainView).WindowControlBox()
        End If
    End Sub

    Private Sub Guna2ToggleSwitch8_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2ToggleSwitch8.CheckedChanged
        If IsShow = False Then Return
        Core.GlobalInstances.AppSettings.NaturalYT = Guna2ToggleSwitch8.Checked
        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
    End Sub


    Private Sub Guna2ToggleSwitch9_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2ToggleSwitch9.CheckedChanged
        If IsShow = False Then Return
        Core.GlobalInstances.AppSettings.MaximizedInApp = Guna2ToggleSwitch9.Checked
        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
    End Sub


    Private Sub Guna2ToggleSwitch10_CheckedChanged(sender As Object, e As EventArgs) Handles Guna2ToggleSwitch10.CheckedChanged
        If IsShow = False Then Return
        Core.GlobalInstances.AppSettings.SupportAVCodec = Guna2ToggleSwitch10.Checked
        Core.AppSettings.Save(Core.GlobalInstances.AppSettings)
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

End Class