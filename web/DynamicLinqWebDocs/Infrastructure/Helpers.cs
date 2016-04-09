using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace DynamicLinqWebDocs.Infrastructure
{
    public static class Helpers
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static void SetMetaDescription(this ControllerBase controller, string description)
        {
            if (description.Length > 160) throw new ArgumentOutOfRangeException();

            controller.ViewBag.MetaDescription = description;
        }

        public static void SetMetaDescription(this ControllerBase controller, string format, params string[] args )
        {
            SetMetaDescription(controller, String.Format(format, args));
        }

        public static void AddMetaKeywords(this ControllerBase controller, params string[] keywords)
        {
            var keywordsList = (List<string>)controller.ViewBag.MetaKeywords;

            if( keywordsList == null )
            {
                keywordsList = new List<string>();
                controller.ViewBag.MetaKeywords = keywordsList;
            }

            keywordsList.AddRange(keywords.Where(x => !String.IsNullOrWhiteSpace(x)));
        }
    }
}