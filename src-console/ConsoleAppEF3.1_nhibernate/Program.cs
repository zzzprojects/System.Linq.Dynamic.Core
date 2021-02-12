using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic.Core;
using log4net;
using log4net.Config;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace ConsoleAppEF3_1_nhibernate
{

	public sealed class Program
	{
		public static void Main()
		{
			var assembly = typeof(Program).Assembly;
			using (var log4NetXml = assembly.GetManifestResourceStream("ConsoleAppEF3_1_nhibernate.log4net.xml"))
				XmlConfigurator.Configure(LogManager.GetRepository(assembly), log4NetXml);

			var cfg = new Configuration();
			cfg.Configure(assembly, "ConsoleAppEF3_1_nhibernate.hibernate.cfg.xml");
			cfg.AddResource("ConsoleAppEF3_1_nhibernate.Mappings.hbm.xml", assembly);

			var sessions = cfg.BuildSessionFactory();
			new SchemaExport(cfg).Create(false, true);

			using (var session = sessions.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var properties1 = new Dictionary<string, object>
				{
					["Name"] = "First Product",
					["Description"] = "First Description"
				};
				session.Save(
					new Product
					{
						Id = Guid.NewGuid(),
						Properties = properties1
					});

				var properties2 = new
				{
					Name = "Second Product",
					Description = "Second Description"
				};
				session.Save(
					new Product
					{
						Id = Guid.NewGuid(),
						Properties = properties2
					});

				dynamic properties3 = new ExpandoObject();
				properties3.Name = "Third Product";
				properties3.Description = "Third Description";
				session.Save(
					new Product
					{
						Id = Guid.NewGuid(),
						Properties = properties3
					});

				transaction.Commit();
			}

			using (var session = sessions.OpenSession())
			using (session.BeginTransaction())
			{
				//var x = session.Query<Product>().Where(p => p.Properties["Name"] == "First Product").ToList();

				var y = session.Query<Product>()
					.Where("Properties.Name == @0", "First Product")
					.ToList();

				int i = 9;
			}
		}
	}
}
