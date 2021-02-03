using System;
using System.Collections.Generic;

namespace ConsoleAppEF3_1_nhibernate
{
	class Product
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IDictionary<string, object> Properties { get; set; }
	}
}
