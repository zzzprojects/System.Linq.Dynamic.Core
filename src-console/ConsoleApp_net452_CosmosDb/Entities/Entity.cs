using System;

namespace ConsoleAppCosmosDb.Entities
{
    [Serializable]
    public class Entity
    {
        public Guid Id { get; set; }
        public EntityData Data { get; set; }
    }
}
