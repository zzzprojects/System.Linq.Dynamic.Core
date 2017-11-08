using System;

namespace NullableDemo
{
	public class Toy
	{
		public Guid Id;

		public int From;

		public int Till;

		public Pet Pet;

		public Guid PetId => Pet.Id;
	}
}
