using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Shared.Models
{
    [Serializable]
    public class Inventory
    {
        [XmlAttribute]
        public Guid Id { get; set; }
        public int Capacity { get; set; }

        [XmlArray("Items")]
        [XmlArrayItem("Item")]
        public List<Item> Items { get; set; } = new List<Item>();

        public Inventory() { }

        public Inventory(Guid id, int capacity, List<Item>? items = null)
        {
            Id = id;
            Capacity = capacity;
            Items = items ?? new List<Item>();
        }
    }
}