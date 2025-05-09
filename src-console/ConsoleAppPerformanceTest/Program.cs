using System.Linq.Dynamic.Core;

TestDynamic();
return;

static void TestDynamic()
{
    var list = new List<Demo>();
    for (int i = 0; i < 100000; i++)
    {
        list.Add(new Demo { ID = i, Name = $"Name {i}", Description = $"Description {i}" });
    }

    var xTimeAll = System.Diagnostics.Stopwatch.StartNew();
    var query = list.AsQueryable().Select(typeof(Demo), "new { ID, Name }").ToDynamicList();
    Console.WriteLine($"Total 1st Query: {(int)xTimeAll.Elapsed.TotalMilliseconds}ms");

    xTimeAll.Restart();
    _ = query.AsQueryable().Select("ID").Cast<int>().ToList();
    Console.WriteLine($"Total 2nd Query: {(int)xTimeAll.Elapsed.TotalMilliseconds}ms");

    xTimeAll.Restart();
    _ = query.AsQueryable().Select("new { it.ID as Idee } ").ToDynamicList();
    Console.WriteLine($"Total 3rd Query: {(int)xTimeAll.Elapsed.TotalMilliseconds}ms");
}

class Demo
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}