using System.Collections.Generic;
using System.Xml.Serialization;


namespace ErikTheCoder.Sandbox.Xml {
	public class WidgetData : List<Widget> {
		[XmlAttribute]
		public string Frob { get; set; }
	}
}
