using System.Collections.Generic;

namespace DynamicLinqWebDocs.ViewModels
{
    public class Class
    {
        public string Name { get; set; }

        public string Namespace { get; set; }

        public string Description { get; set; }

        public string Remarks { get; set; }

        public bool IsInterface { get; set; }

        public IList<Models.Method> Methods { get; set; }

        public IList<Models.Property> Properties { get; set; }
    }
}