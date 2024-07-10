Imports System.Runtime.InteropServices

Namespace Core

    Public Class CursorChanger
        Private Const IDC_ARROW As Integer = 32512
        Private Const IDC_HAND As Integer = 32649

        <DllImport("user32.dll")>
        Private Shared Function LoadCursor(ByVal hInstance As IntPtr, ByVal lpCursorName As Integer) As IntPtr
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
        Private Shared Function LoadCursorFromFile(lpFileName As String) As IntPtr
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Private Shared Function SetCursor(hCursor As IntPtr) As Boolean
        End Function

        Public Sub SetCustomCursor(cursorFilePath As String)
            Dim cursorHandle As IntPtr = LoadCursorFromFile(cursorFilePath)

            If cursorHandle <> IntPtr.Zero Then
                SetCursor(cursorHandle)
            End If
        End Sub

        Public Sub RestoreDefaultCursor()
            SetCursor(LoadCursor(0, IDC_ARROW))
        End Sub

    End Class

End Namespace