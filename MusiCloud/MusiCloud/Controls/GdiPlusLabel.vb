Imports System.Drawing
Imports System.Windows.Forms

Public Class GdiPlusLabel
    Inherits Label

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Using brush As New SolidBrush(Me.ForeColor)
            Using format As New StringFormat()
                format.Alignment = StringAlignment.Near
                format.LineAlignment = StringAlignment.Center
                e.Graphics.DrawString(Me.Text, Me.Font, brush, New RectangleF(Me.ClientRectangle.Location, Me.ClientSize), format)
            End Using
        End Using
    End Sub
End Class
