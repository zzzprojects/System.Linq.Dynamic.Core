using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace GroupByError
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new MyDbContext("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TestEF;Integrated Security=True;MultipleActiveResultSets=True"))
            {
                //In memory list.
                var ElementsList = AddElementsToList();

                //Database
                db.Database.Delete();
                if (db.Database.CreateIfNotExists())
                {
                    ElementsList.ForEach(e => db.Elements.Add(e));
                    db.SaveChanges();
                }

                //In memory tests.
                var resultList1 = ElementsList.GroupBy(el => new { el.Attribute1, el.Attribute2 }).ToArray();         //CORRECT
                var resultList2 = ElementsList.AsQueryable().GroupBy("new(Attribute1, Attribute2)").ToDynamicArray(); //CORRECT

                //Database tests 1.
                var resultDb1a = db.Elements.GroupBy(el => new { el.Attribute1 }).ToArray(); //CORRECT
                var resultDb2a = db.Elements.GroupBy("Attribute1").ToDynamicArray();         //CORRECT

                //Database tests 2.
                var resultDb1b = db.Elements.GroupBy(el => new { el.Attribute1, el.Attribute2 }).ToArray(); //CORRECT
                var resultDb2b = db.Elements.GroupBy("new(Attribute1, Attribute2)").ToDynamicArray();       //WRONG

                int x = 0;
            }
        }

        private static List<Element> AddElementsToList()
        {
            var elementList = new List<Element>();

            for (int i = 0; i < 100; i++)
            {
                var element = new Element();

                if (i < 25)
                {
                    element.Attribute1 = 10;
                    element.Attribute2 = 10;
                }
                else if (i < 50)
                {
                    element.Attribute1 = 20;
                    element.Attribute2 = 20;
                }
                else if (i < 75)
                {
                    element.Attribute1 = 30;
                    element.Attribute2 = 30;
                }
                else
                {
                    element.Attribute1 = 40;
                    element.Attribute2 = 40;
                }

                elementList.Add(element);
            }

            return elementList;
        }
    }
}

