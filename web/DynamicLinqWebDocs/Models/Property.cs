using System.Collections.Generic;
using System.Xml.Serialization;

namespace DynamicLinqWebDocs.Models
{
    public class Property
    {
        public Property()
        {
            Frameworks = Frameworks.All;
        }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "isStatic")]
        public bool IsStatic { get; set; }

        [XmlAttribute(AttributeName = "valueType")]
        public string ValueType { get; set; }

        public string Description { get; set; }

        public string ValueTypeDescription { get; set; }

        public string Remarks { get; set; }

        public List<Example> Examples { get; set; }

        [XmlAttribute(AttributeName = "frameworks")]
        public Frameworks Frameworks { get; set; }

        [XmlAttribute(AttributeName = "accessors")]
        public PropertyAccessors Accessors { get; set; }

    }
}