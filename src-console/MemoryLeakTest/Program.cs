using System;

namespace MemoryLeakTest
{
	class Program
	{
		static void Main(string[] args)
		{
            Console.WriteLine("press key to run");
		    Console.ReadKey();

			Test t = new Test();
			t.Join();
		}
	}
}