Module Program

	Public Sub Main
		Run().Wait()
	End Sub


	Private Async Function Run() As Task

		Dim regionTasks = New HashSet(Of Task(Of (Value As String, ToNodeId As Integer)))
		For index As Integer = 0 To 9
			regionTasks.Add(Async Function()
				Dim localIndex = index ' Avoid capturing only the final value of index.  C# is immune from this problem.
				Dim value = Await GetValue(localIndex)
				Return (value, localIndex)
			End Function())
		Next
		Await Task.WhenAll(regionTasks)
		For Each task In regionTasks
			Dim data = Await task
			Console.WriteLine($"Value = {data.Value}, ToNodeId = {data.ToNodeId}.")
		Next 
	End Function


	Private Async Function GetValue(ToNodeId As Integer) As Task(Of String)
		Await Task.Delay(TimeSpan.FromMilliseconds(100))
		Return $"{ToNodeId} Blah"
	End Function

End Module
