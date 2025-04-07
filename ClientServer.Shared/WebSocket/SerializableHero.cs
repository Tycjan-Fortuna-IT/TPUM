namespace ClientServer.Shared.WebSocket
{
    [Serializable]
    public class SerializableHero
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Gold { get; set; }

        public SerializableInventory Inventory { get; set; }

        public SerializableHero()
        {
            Id = Guid.NewGuid();
            Name = string.Empty;
            Gold = 0;
            Inventory = new SerializableInventory();
        }

        public SerializableHero(Guid id, string name, float gold, SerializableInventory inventory)
        {
            Id = id;
            Name = name;
            Gold = gold;
            Inventory = inventory;
        }
    }
}
