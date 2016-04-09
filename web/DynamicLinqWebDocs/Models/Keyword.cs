using System.Collections.Generic;
using System.Xml.Serialization;

namespace DynamicLinqWebDocs.Models
{
    public class Keyword
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Remarks { get; set; }

        public List<Example> Examples { get; set; }
    }
}