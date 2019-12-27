using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;


namespace ErikTheCoder.Sandbox.Xml {
	public static class Program {
		public static void Main()
        {
            string directory = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program)).Location) ?? string.Empty;
			string filename1 = Path.Combine(directory, "Test1.xml");
			string filename2 = Path.Combine(directory, "Test2.xml");
			XmlSerializer xml1 = new XmlSerializer(typeof(Response<BazData>));
			using (FileStream stream = File.OpenRead(filename1)) {
				Response<BazData> response = (Response<BazData>)xml1.Deserialize(stream);
				WriteCommonProperties(response, 1);
				Console.WriteLine($"Baz Bork = {response.Data.Bork}");
				foreach (Baz baz in response.Data) {
					Console.WriteLine($"Baz ID =   {baz.Id}");
					Console.WriteLine($"Baz Name = {baz.Name}");
				}
			}
			Console.WriteLine();
			Console.WriteLine();
			XmlSerializer xml2 = new XmlSerializer(typeof(Response<WidgetData>));
			using (FileStream stream = File.OpenRead(filename2)) {
				Response<WidgetData> response = (Response<WidgetData>)xml2.Deserialize(stream);
				WriteCommonProperties(response, 2);
				Console.WriteLine($"Widget Frob = {response.Data.Frob}");
				foreach (Widget widget in response.Data) {
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
