using System;

namespace NullableDemo
{
	public class Pet
	{
		public Guid Id;

		public string Name;

		public Person Owner;

		public Guid? OwnerId => Owner.Id;

	    public int From;

		public int Till;
	}
}
