using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml.Serialization;
using DynamicLinqWebDocs.Models;

namespace DynamicLinqWebDocs.Infrastructure.Data
{

    public class DynLINQDoc
    {
        public List<Class> Classes { get; set; }

        public List<Expression> Expressions { get; set; }

        public List<Keyword> Keywords { get; set; }
    }


    class RealDataRepo : IDataRepo 
    {
        static DynLINQDoc _doc;

        static RealDataRepo()
        {
            LoadData();
        }

        public RealDataRepo()
        {
#if DEBUG
            //reload data in debug mode so we can see xml changes without recycling IIS. 
            LoadData(); 
#endif
        }

        static void LoadData()
        {
            var serializer = new XmlSerializer(typeof(DynLINQDoc), "http://schemas.plainlogic.net/dynamiclinqdocs/2014");

            var docFolder = HostingEnvironment.MapPath(@"~/App_Data/");
            var docFolderInfo = new DirectoryInfo(docFolder);

            var docFiles = docFolderInfo.GetFiles("*.xml", SearchOption.AllDirectories);

            //var filePath = HostingEnvironment.MapPath(@"~/App_Data/DynLINQDoc.xml");

            _doc = new DynLINQDoc()
            {
                Classes = new List<Class>(),
                Expressions = new List<Expression>(),
                Keywords = new List<Keyword>()
            };

            foreach (var docFile in docFiles)
            {
                using (var file = File.Open(docFile.FullName, FileMode.Open))
                {
                    var tempDoc = (DynLINQDoc)serializer.Deserialize(file);

                    if (tempDoc.Classes != null) _doc.Classes.AddRange(tempDoc.Classes);
                    if (tempDoc.Expressions != null) _doc.Expressions.AddRange(tempDoc.Expressions);
                    if (tempDoc.Keywords != null) _doc.Keywords.AddRange(tempDoc.Keywords);
                }
            }
        }

        public IEnumerable<Class> GetClasses()
        {
            return _doc.Classes;
        }

        public Class GetClass(string className)
        {
            return _doc.Classes
                .Where(x => className.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();
        }

        public Method GetMethod(string className, string methodName, Frameworks framework, out Class @class, int overload)
        {
            @class = GetClass(className);
            if (@class == null) return null;

            if (overload < 0) return null;

            IEnumerable<Method> methodFinder = @class.Methods
                .Where(x => methodName.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase));

            if (framework == Frameworks.NotSet)
            {
                methodFinder = methodFinder.OrderByDescending(x => x.Frameworks);
            }
            else
            {
                methodFinder = methodFinder.Where(x => x.Frameworks.HasFlag(framework));
            }

            if( overload > 0 ) methodFinder = methodFinder.Skip(overload);

            return methodFinder.FirstOrDefault();
        }


        public Property GetProperty(string className, string propertyName, Frameworks framework, out Class @class)
        {
            @class = GetClass(className);
            if (@class == null) return null;

            IEnumerable<Property> propertyFinder = @class.Properties
                .Where(x => propertyName.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase));

            if (framework == Frameworks.NotSet)
            {
                propertyFinder = propertyFinder.OrderByDescending(x => x.Frameworks);
            }
            else
            {
                propertyFinder = propertyFinder.Where(x => x.Frameworks.HasFlag(framework));
            }

            return propertyFinder.FirstOrDefault();
        }

        public Expression GetExpression(string expressionName)
        {
            return _doc.Expressions
                .Where(x => expressionName.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();
        }

        public IEnumerable<Expression> GetExpressions()
        {
            return _doc.Expressions;
        }


        public IEnumerable<Keyword> GetKeywords()
        {
            return _doc.Keywords;
        }

        public Keyword GetKeyword(string keywordName)
        {
            return _doc.Keywords
               .Where(x => keywordName.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase))
               .FirstOrDefault();
        }
    }
}