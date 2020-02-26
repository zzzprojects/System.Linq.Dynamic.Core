using System;
using System.Collections.Generic;

namespace ConsoleAppCosmosDb.Entities
{
    [Serializable]
    public class EntityData
    {
        public IList<EntityReading> Readings { get; set; }
    }
}
