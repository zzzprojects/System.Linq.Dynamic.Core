using System;

namespace MemoryLeakTest
{
	public class Pet
	{
		public Guid Id;

		public string Name;

		public Person Owner;

		public Guid OwnerId { get { return Owner.Id; } }

		public int From;

		public int Till;
	}
}