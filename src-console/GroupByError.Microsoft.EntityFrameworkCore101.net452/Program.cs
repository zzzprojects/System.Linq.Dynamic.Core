using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace GroupByError
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new MyDbContext())
            {
                //In memory list.
                var list = AddElementsToList();

                //Database
                bool deleted = db.Database.EnsureDeleted();
                Console.WriteLine("Database is deleted = {0}", deleted);
                if (db.Database.EnsureCreated())
                {
                    list.ForEach(e => db.Add(e));
                    db.SaveChanges();
                }

                //In memory tests.
                //var resultList1 = ElementsList.GroupBy(el => new { el.Attribute1, el.Attribute2 }).ToArray();         //CORRECT
                //var resultList2 = ElementsList.AsQueryable().GroupBy("new(Attribute1, Attribute2)").ToDynamicArray(); //CORRECT

                //Database tests.
                var resultDb1 = db.Element.GroupBy(el => new { el.Attribute1, el.Attribute2 }).ToArray(); //CORRECT
                var resultDb2 = db.Element.GroupBy("new(Attribute1, Attribute2)").ToDynamicArray();       //CORRECT for "Microsoft.EntityFrameworkCore" version="1.1.0"

                var test = db.Element.ToListAsync();

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

