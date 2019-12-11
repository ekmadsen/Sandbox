using System;
using System.Xml.Serialization;


namespace ErikTheCoder.Sandbox.Xml {
	[Serializable()]
	[XmlRoot("Response")]
	public class Response<T> where T : class, new() {
		public string Foo { get; set; }
		public string Bar { get; set; }
		public T Data { get; set; }


		public Response() {
			Data = new T();
		}
	}
}
