using System.Linq.Dynamic.Core.CustomTypeProviders;

namespace System.Linq.Dynamic.Core.Tests.Entities
{
    [DynamicLinqType]
    public class Worker : BaseEmployee
    {
        public string Other { get; set; }
    }
}