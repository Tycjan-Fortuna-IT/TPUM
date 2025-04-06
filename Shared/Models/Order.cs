using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Shared.Models
{
    [Serializable]
    public class Order
    {
        [XmlAttribute]
        public Guid Id { get; set; }
        public Hero? Buyer { get; set; }

        [XmlArray("ItemsToBuy")]
        [XmlArrayItem("Item")]
        public List<Item> ItemsToBuy { get; set; } = new List<Item>();

        public Order() { }

        public Order(Guid id, Hero? buyer, List<Item>? itemsToBuy = null)
        {
            Id = id;
            Buyer = buyer;
            ItemsToBuy = itemsToBuy ?? new List<Item>();
        }
    }
}