using System;

namespace ConsoleAppCosmosDb.Entities
{
    [Serializable]
    public class EntityReading
    {
        public string Status { get; set; }
        public int Weight { get; set; }
        public EntityReadingValue Value { get; set; }
    }
}
