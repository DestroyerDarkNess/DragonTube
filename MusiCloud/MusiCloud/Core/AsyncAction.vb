Namespace Core
    Public Class AsyncAction

        Public Shared Function [Call](ByVal Action As Action) As Task(Of Object)
            Dim tcs As New TaskCompletionSource(Of Object)()

            Dim staThread As New Threading.Thread(
            Sub()
                Try
                    Dim Result = Action.DynamicInvoke()
                    tcs.SetResult(Result)
                Catch ex As Exception
                    tcs.SetException(ex)
                End Try
            End Sub)

            staThread.SetApartmentState(Threading.ApartmentState.STA)
            staThread.Start()

            Return tcs.Task
        End Function




    End Class
End Namespace

