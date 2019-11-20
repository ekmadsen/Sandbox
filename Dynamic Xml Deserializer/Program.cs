using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;


namespace ErikTheCoder.Sandbox.Xml {
	public static class Program {
		public static void Main() {
			var directory = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program)).Location);
			var filename1 = Path.Combine(directory, "Test1.xml");
			var filename2 = Path.Combine(directory, "Test2.xml");
			var xml1 = new XmlSerializer(typeof(Response<Baz>));
			using (var stream = File.OpenRead(filename1)) {
				var response = (Response<Baz>)xml1.Deserialize(stream);
				WriteCommonProperties(response, 1);
				foreach (var baz in response.Data) {
					Console.WriteLine($"Baz ID =   {baz.Id}");
					Console.WriteLine($"Baz Name = {baz.Name}");
				}
			}
			Console.WriteLine();
			Console.WriteLine();
			var xml2 = new XmlSerializer(typeof(Response<Widget>));
			using (var stream = File.OpenRead(filename2)) {
				var response = (Response<Widget>)xml2.Deserialize(stream);
				WriteCommonProperties(response, 2);
				foreach (var widget in response.Data) {
					Console.WriteLine($"Widget ID =   {widget.Id}");
					Console.WriteLine($"Widget Cost = {widget.Cost:0.00}");
				}
			}
			Console.ReadLine();
		}


		private static void WriteCommonProperties<T>(Response<T> Response, int Number) where T : class, new() {
			Console.WriteLine($"Response {Number}");
			Console.WriteLine("==========");
			Console.WriteLine();
			Console.WriteLine($"Foo = {Response.Foo}");
			Console.WriteLine($"Bar = {Response.Bar}");
			Console.WriteLine();
		}
	}
}
