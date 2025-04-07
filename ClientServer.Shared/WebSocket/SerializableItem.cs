namespace Server.Presentation
{
    [Serializable]
    public class SerializableItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int MaintenanceCost { get; set; }

        public SerializableItem()
        {
            Id = Guid.NewGuid();
            Name = string.Empty;
            Price = 0;
            MaintenanceCost = 0;
        }

        public SerializableItem(Guid id, string name, int price, int maintenanceCost)
        {
            Id = id;
            Name = name;
            Price = price;
            MaintenanceCost = maintenanceCost;
        }
    }
}
