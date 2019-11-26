using System.Collections.Generic;
using System.Xml.Serialization;


namespace ErikTheCoder.Sandbox.Xml {
	public class BazData : List<Baz> {
		[XmlAttribute]
		public string Bork { get; set; }
	}
}
