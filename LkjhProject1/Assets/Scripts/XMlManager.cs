using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class XMlManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		CreateXml();
		
	}

	void CreateXml()
	{
		
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "utf-8", "yes"));

		XmlNode root = xmlDocument.CreateNode(XmlNodeType.Element, "Languages", string.Empty);
		xmlDocument.AppendChild(root);

		XmlNode child = xmlDocument.CreateNode(XmlNodeType.Element, "English", string.Empty);
		root.AppendChild(child);

		XmlElement name = xmlDocument.CreateElement("Name");
		name.InnerText = "GameStart";
		child.AppendChild(name);

		xmlDocument.Save("./Assets/Resources/Language.xml");
	}
	
}
