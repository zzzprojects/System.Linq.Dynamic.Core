using System;

namespace SldcTrimmer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                const string filterStr = "Alpha == 255";
                Console.WriteLine(filterStr);

                var filter = filterStr.ParseLambda<Color, bool>();

                var color1 = new Color { Alpha = 255 };
                var value1 = filter(color1);
                Console.WriteLine($"{color1.Alpha} -> {value1}");

                var color2 = new Color { Alpha = 128 };
                var value2 = filter(color2);
                Console.WriteLine($"{color2.Alpha} -> {value2}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetType());
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}