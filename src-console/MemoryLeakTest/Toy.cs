using System;

namespace MemoryLeakTest
{
	public class Toy
	{
		public Guid Id;

		public int From;

		public int Till;

		public Pet Pet;

		public Guid PetId { get { return Pet.Id; } }
	}
}