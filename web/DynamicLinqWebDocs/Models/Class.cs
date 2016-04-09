using System.Collections.Generic;
using System.Xml.Serialization;

namespace DynamicLinqWebDocs.Models
{
    public class Class
    {
        [XmlAttribute(AttributeName = "namespace")]
        public string Namespace { get; set; }
        
        [XmlAttribute(AttributeName="name")]
        public string Name { get; set; }

        public string Description { get; set; }

        [XmlAttribute(AttributeName = "isInterface")]
        public bool IsInterface { get; set; }

        public List<Method> Methods { get; set; }

        public List<Property> Properties { get; set; }

        public string Remarks { get; set; }
    }
}