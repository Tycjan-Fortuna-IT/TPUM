using System;
using System.Xml.Serialization;

namespace Shared.Models
{
    [Serializable]
    public class Item
    {
        [XmlAttribute]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Price { get; set; }
        public int MaintenanceCost { get; set; }

        public Item() { }

        public Item(Guid id, string name, int price, int maintenanceCost)
        {
            Id = id;
            Name = name;
            Price = price;
            MaintenanceCost = maintenanceCost;
        }
    }
}