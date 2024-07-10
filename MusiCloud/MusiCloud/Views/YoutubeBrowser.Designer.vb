<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class YoutubeBrowser
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(YoutubeBrowser))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.LogInContextMenu1 = New MusiCloud.LogInContextMenu()
        Me.HomeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopyURLToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DownloadToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.NextToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BackToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ReloadToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ShowDevToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.TopMostToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AddToFavoritesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LogInContextMenu1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(715, 343)
        Me.Panel1.TabIndex = 1
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        '
        'LogInContextMenu1
        '
        Me.LogInContextMenu1.BackColor = System.Drawing.Color.FromArgb(CType(CType(14, Byte), Integer), CType(CType(14, Byte), Integer), CType(CType(14, Byte), Integer))
        Me.LogInContextMenu1.FontColour = System.Drawing.Color.FromArgb(CType(CType(55, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.LogInContextMenu1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.LogInContextMenu1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.HomeToolStripMenuItem, Me.CopyURLToolStripMenuItem, Me.AddToFavoritesToolStripMenuItem, Me.DownloadToolStripMenuItem, Me.ToolStripSeparator1, Me.NextToolStripMenuItem, Me.BackToolStripMenuItem, Me.ReloadToolStripMenuItem, Me.ToolStripSeparator2, Me.ShowDevToolsToolStripMenuItem, Me.ToolStripSeparator3, Me.TopMostToolStripMenuItem})
        Me.LogInContextMenu1.Name = "LogInContextMenu1"
        Me.LogInContextMenu1.ShowImageMargin = False
        Me.LogInContextMenu1.Size = New System.Drawing.Size(156, 242)
        '
        'HomeToolStripMenuItem
        '
        Me.HomeToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(CType(CType(14, Byte), Integer), CType(CType(14, Byte), Integer), CType(CType(14, Byte), Integer))
        Me.HomeToolStripMenuItem.Name = "HomeToolStripMenuItem"
        Me.HomeToolStripMenuItem.Size = New System.Drawing.Size(155, 22)
        Me.HomeToolStripMenuItem.Text = "Home"
        '
        'CopyURLToolStripMenuItem
        '
        Me.CopyURLToolStripMenuItem.Name = "CopyURLToolStripMenuItem"
        Me.CopyURLToolStripMenuItem.Size = New System.Drawing.Size(155, 22)
        Me.CopyURLToolStripMenuItem.Text = "Copy URL"
        Me.CopyURLToolStripMenuItem.Visible = False
        '
        'DownloadToolStripMenuItem
        '
        Me.DownloadToolStripMenuItem.Name = "DownloadToolStripMenuItem"
        Me.DownloadToolStripMenuItem.Size = New System.Drawing.Size(155, 22)
        Me.DownloadToolStripMenuItem.Text = "Download"
        Me.DownloadToolStripMenuItem.Visible = False
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(152, 6)
        '
        'NextToolStripMenuItem
        '
        Me.NextToolStripMenuItem.Name = "NextToolStripMenuItem"
        Me.NextToolStripMenuItem.Size = New System.Drawing.Size(155, 22)
        Me.NextToolStripMenuItem.Text = "Next"
        '
        'BackToolStripMenuItem
        '
        Me.BackToolStripMenuItem.Name = "BackToolStripMenuItem"
        Me.BackToolStripMenuItem.Size = New System.Drawing.Size(155, 22)
        Me.BackToolStripMenuItem.Text = "Back"
        '
        'ReloadToolStripMenuItem
        '
        Me.ReloadToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(CType(CType(14, Byte), Integer), CType(CType(14, Byte), Integer), CType(CType(14, Byte), Integer))
        Me.ReloadToolStripMenuItem.Name = "ReloadToolStripMenuItem"
        Me.ReloadToolStripMenuItem.Size = New System.Drawing.Size(155, 22)
        Me.ReloadToolStripMenuItem.Text = "Reload"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(152, 6)
        '
        'ShowDevToolsToolStripMenuItem
        '
        Me.ShowDevToolsToolStripMenuItem.Name = "ShowDevToolsToolStripMenuItem"
        Me.ShowDevToolsToolStripMenuItem.Size = New System.Drawing.Size(155, 22)
        Me.ShowDevToolsToolStripMenuItem.Text = "Show DevTools"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(152, 6)
        '
        'TopMostToolStripMenuItem
        '
        Me.TopMostToolStripMenuItem.Checked = True
        Me.TopMostToolStripMenuItem.CheckOnClick = True
        Me.TopMostToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.TopMostToolStripMenuItem.Name = "TopMostToolStripMenuItem"
        Me.TopMostToolStripMenuItem.Size = New System.Drawing.Size(155, 22)
        Me.TopMostToolStripMenuItem.Text = "TopMost"
        '
        'AddToFavoritesToolStripMenuItem
        '
        Me.AddToFavoritesToolStripMenuItem.Name = "AddToFavoritesToolStripMenuItem"
        Me.AddToFavoritesToolStripMenuItem.Size = New System.Drawing.Size(155, 22)
        Me.AddToFavoritesToolStripMenuItem.Text = "Add To Favorites"
        Me.AddToFavoritesToolStripMenuItem.Visible = False
        '
        'YoutubeBrowser
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(14, Byte), Integer), CType(CType(14, Byte), Integer), CType(CType(14, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(715, 343)
        Me.Controls.Add(Me.Panel1)
        Me.ForeColor = System.Drawing.Color.White
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "YoutubeBrowser"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "YoutubeBrowser"
        Me.LogInContextMenu1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents LogInContextMenu1 As LogInContextMenu
    Friend WithEvents HomeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ReloadToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents ShowDevToolsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Timer1 As Timer
    Friend WithEvents TopMostToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents NextToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BackToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CopyURLToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
    Friend WithEvents DownloadToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AddToFavoritesToolStripMenuItem As ToolStripMenuItem
End Class
