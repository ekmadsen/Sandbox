Option Strict On

Imports ErikTheCoder.Sandbox.LeaderlessReplication
Imports ErikTheCoder.Utilities


' ReSharper disable All
Public Class QuorumNode
	Inherits NodeBase


	Public Sub New(Random As IThreadsafeRandom, Id As Integer, Name As String, RegionName As String)
		MyBase.New(Random, Id, Name, RegionName)
	End Sub


	Public Async Overrides Function ReadValueAsync(Key As String) As Task(Of String)
		Dim regionTasks = New HashSet(Of Task(Of (Value As String, ToNodeId As Integer)))
		For Each connection In Connections(RegionName)
			regionTasks.Add(Async Function()
				' Calling connection.ReadValueAsync would create an infinite loop of reads from all regional nodes to all regional nodes.
				Dim value = Await connection.GetValueAsync(Key)
				Return (value, connection.ToNode.Id)
			End Function())
		Next
		Await Task.Delay(TimeSpan.Zero)
		Return "Testing"
	End Function


	Public Async Overrides Function WriteValueAsync(Key As String, Value As String) As Task
		Await Task.Delay(0)
	End Function
End Class