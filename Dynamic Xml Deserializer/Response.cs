using System;
using System.Collections.Generic;
using System.Xml.Serialization;


namespace ErikTheCoder.Sandbox.Xml {
	[Serializable()]
	[XmlRoot("Response")]
	public class Response<T> where T : class, new() {
		public string Foo { get; set; }
		public string Bar { get; set; }		
		public List<T> Data { get; set; }


		public Response() {
			Data = new List<T>();
		}
	}
}
